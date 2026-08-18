//**********************************************************************************
//* マスタ・テーブル（Suppliers）サンプル（Ｐ層）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：SuppliersBController
//* クラス日本語名  ：画面Ｂ（一覧・行追加／行削除・バッチ更新）
//*
//* 作成日時        ：2026/08/12
//* 作成者          ：コーディング エージェント
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/12  コーディング Ａ   新規作成
//*  2026/08/19  コーディング Ａ   Session の直列化を DTTables JSON へ変更
//**********************************************************************************

using System;
using System.Data;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using MVC_Sample.Logic.Business;
using MVC_Sample.Logic.Common;
using MVC_Sample.Models.ViewModels;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Dto;

namespace MVC_Sample.Controllers
{
    /// <summary>画面Ｂ（一覧・行追加／行削除・バッチ更新）</summary>
    /// <remarks>
    /// バッチ更新は DataRow の RowState（Added / Modified / Deleted）で CUD を振り分ける。
    /// Web は複数ポストバックに跨って編集するため、編集中の DataTable を Session に保持する。
    ///
    /// ★ ASP.NET Core の Session は string / byte[] しか持てない（net48 のようにオブジェクトを直接置けない）。
    ///   基盤の DTTables を JSON にして往復させる（Touryo.Infrastructure.Public.Dto）。
    ///   DTTables は RowState と、削除行の DataRowVersion.Original を保持する。
    /// </remarks>
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class SuppliersBController : MyBaseMVControllerCore
    {
        /// <summary>編集中 DataTable の Session キー</summary>
        private const string SessionKey = "SuppliersEditing";

        /// <summary>編集中 DataTable の表名（DataSet 内での識別子）</summary>
        private const string TableName = "Suppliers";

        #region Session への DataTable の出し入れ

        /// <summary>編集中の DataTable を Session から取り出す</summary>
        /// <returns>編集中の DataTable（無ければ null）</returns>
        private DataTable LoadEditingTable()
        {
            string json = this.HttpContext.Session.GetString(SuppliersBController.SessionKey);
            if (string.IsNullOrEmpty(json)) { return null; }

            DataTable dt = DTTables.JsonToDTTables(json).ToDataSet()
                .Tables[SuppliersBController.TableName];

            // ★ DTTables の往復では「列スキーマ」が落ちる（実測）。
            //   復元されるのは 列名・型・値・RowState までで、
            //   AutoIncrement / AutoIncrementSeed / AutoIncrementStep / PrimaryKey / AllowDBNull は
            //   既定値（False / 0 / 1 / なし / True）に戻る。
            //   B層の一覧取得が仕込んだ「IDENTITY 列の負値 仮採番」もここで失われるため、
            //   取り出すたびに掛け直す。掛け直さないと、次の［行追加］で SupplierID が
            //   DBNull のまま追加され（例外にはならない）、仮採番の約束が静かに崩れる。
            SuppliersBController.RestoreTempNumbering(dt);

            return dt;
        }

        /// <summary>編集中の DataTable を Session へ格納する</summary>
        /// <param name="dt">編集中の DataTable（null なら削除）</param>
        private void SaveEditingTable(DataTable dt)
        {
            if (dt == null)
            {
                this.HttpContext.Session.Remove(SuppliersBController.SessionKey);
                return;
            }

            DataSet ds = new DataSet();
            ds.Tables.Add(dt.Copy());

            this.HttpContext.Session.SetString(
                SuppliersBController.SessionKey,
                DTTables.DTTablesToJson(DTTables.FromDataSet(ds)));
        }

        /// <summary>IDENTITY 列（SupplierID）の負値 仮採番を掛け直す</summary>
        /// <param name="dt">Session から取り出した DataTable</param>
        /// <remarks>
        /// シードは -1 固定ではなく「既にある最小の仮採番値 - 1」にする。
        /// -1 固定にすると、往復のたびに採番が -1 に巻き戻り、
        /// 2行目以降の追加行が -1 で重複する（実測）。
        /// PrimaryKey は掛け直さない（DB 側の主キーが本体で、ここでは DataTable の
        /// クライアント側制約に過ぎない。仮採番中の行に制約を掛けても得が無い）。
        /// </remarks>
        private static void RestoreTempNumbering(DataTable dt)
        {
            if (dt == null) { return; }

            DataColumn pk = dt.Columns["SupplierID"];
            if (pk == null) { return; }

            long nextSeed = -1;

            foreach (DataRow dr in dt.Rows)
            {
                // 削除行は現在値を持たない。仮採番行（Added）が削除されることもあるが、
                // その値を使い回しても実害が無いのでここでは見ない。
                if (dr.RowState == DataRowState.Deleted) { continue; }
                if (dr["SupplierID"] == DBNull.Value) { continue; }

                long value = Convert.ToInt64(dr["SupplierID"]);
                if (value <= nextSeed) { nextSeed = value - 1; }
            }

            pk.AutoIncrement = true;
            pk.AutoIncrementSeed = nextSeed;
            pk.AutoIncrementStep = -1;
        }

        #endregion

        #region 画面表示

        /// <summary>画面の初期表示</summary>
        /// <param name="model">SuppliersViewModel</param>
        /// <returns>初期表示状態の画面</returns>
        [HttpGet]
        public IActionResult Index(SuppliersViewModel model)
        {
            // 画面を開き直したら編集内容は破棄する（Session を残さない）
            this.SaveEditingTable(null);
            return View(model);
        }

        #endregion

        #region 一覧取得

        /// <summary>Suppliers の一覧を取得する</summary>
        /// <param name="model">SuppliersViewModel</param>
        /// <returns>再描画</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectAll(SuppliersViewModel model)
        {
            // ↓B層実行：Suppliers の一覧を取得-----------------------------------------------------

            SuppliersParameterValue parameterValue = new SuppliersParameterValue(
                this.ControllerName, "-", this.ActionName, "SQL", this.UserInfo);

            SuppliersLayerB layerB = new SuppliersLayerB();
            SuppliersReturnValue returnValue = (SuppliersReturnValue)await layerB.DoBusinessLogicAsync(
                parameterValue, DbEnum.IsolationLevelEnum.User);

            // ↑B層実行：Suppliers の一覧を取得-----------------------------------------------------

            if (returnValue.ErrorFlag)
            {
                model.Message = returnValue.ErrorMessage;
            }
            else
            {
                this.SaveEditingTable(returnValue.Suppliers);
                model.Suppliers = returnValue.Suppliers;
                model.Message = "一覧を取得しました（" + returnValue.Suppliers.Rows.Count + " 件）。";
            }

            return View("Index", model);
        }

        #endregion

        #region 行追加／行削除（グリッドの編集）

        /// <summary>グリッド外の［追加］＝空行を足す（RowState は Added になる）</summary>
        /// <param name="model">SuppliersViewModel</param>
        /// <returns>再描画</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddRow(SuppliersViewModel model)
        {
            DataTable dt = this.LoadEditingTable();
            if (dt == null)
            {
                model.Message = "先に一覧を取得して下さい。";
                return View("Index", model);
            }

            // 画面の編集内容を DataTable に反映してから行を足す
            this.ReadRowsIntoTable(dt, model);

            DataRow newRow = dt.NewRow();
            // SupplierID は IDENTITY 列＝DataTable 上は負値で仮採番される
            // （B層の一覧取得で仕込み、Session から取り出すたびに LoadEditingTable で掛け直している）
            dt.Rows.Add(newRow);

            this.SaveEditingTable(dt);
            model.Suppliers = dt;
            model.Message = "行を追加しました。";
            return View("Index", model);
        }

        /// <summary>グリッド内の［削除］＝行を削除状態にする（RowState は Deleted になる）</summary>
        /// <param name="model">SuppliersViewModel</param>
        /// <param name="rowIndex">DataTable の行インデックス</param>
        /// <returns>再描画</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteRow(SuppliersViewModel model, int rowIndex)
        {
            DataTable dt = this.LoadEditingTable();
            if (dt == null)
            {
                model.Message = "先に一覧を取得して下さい。";
                return View("Index", model);
            }

            this.ReadRowsIntoTable(dt, model);

            if (0 <= rowIndex && rowIndex < dt.Rows.Count)
            {
                // ★ Rows.Remove ではなく Delete（Remove だと Deleted にならず DELETE が出ない）
                dt.Rows[rowIndex].Delete();
            }

            this.SaveEditingTable(dt);
            model.Suppliers = dt;
            model.Message = "行を削除しました（［更新］でDBに反映されます）。";
            return View("Index", model);
        }

        #endregion

        #region バッチ更新

        /// <summary>［更新］＝編集内容（CUD）を一括でDBへ反映する</summary>
        /// <param name="model">SuppliersViewModel</param>
        /// <returns>再描画</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatchUpdate(SuppliersViewModel model)
        {
            DataTable dt = this.LoadEditingTable();
            if (dt == null)
            {
                model.Message = "先に一覧を取得して下さい。";
                return View("Index", model);
            }

            // 画面の編集内容を DataTable へ読み戻す（この代入で Modified が立つ）
            this.ReadRowsIntoTable(dt, model);

            // ↓B層実行：Suppliers のバッチ更新-----------------------------------------------------

            SuppliersParameterValue parameterValue = new SuppliersParameterValue(
                this.ControllerName, "-", this.ActionName, "SQL", this.UserInfo);
            parameterValue.Suppliers = dt;

            SuppliersLayerB layerB = new SuppliersLayerB();
            SuppliersReturnValue returnValue = (SuppliersReturnValue)await layerB.DoBusinessLogicAsync(
                parameterValue, DbEnum.IsolationLevelEnum.User);

            // ↑B層実行：Suppliers のバッチ更新-----------------------------------------------------

            if (returnValue.ErrorFlag)
            {
                // 業務例外＝ロールバック済み。編集内容（RowState）はそのまま残してやり直せるようにする
                this.SaveEditingTable(dt);
                model.Suppliers = dt;
                model.Message = returnValue.ErrorMessage;
                return View("Index", model);
            }

            // 反映できたので編集状態を確定する
            dt.AcceptChanges();

            string message = "更新しました（挿入 " + returnValue.InsertCount
                + " 件／更新 " + returnValue.UpdateCount
                + " 件／削除 " + returnValue.DeleteCount + " 件）。";

            // IDENTITY の採番値は DataTable に戻らないので、反映後は一覧を取り直す
            SuppliersParameterValue reloadParameter = new SuppliersParameterValue(
                this.ControllerName, "-", "SelectAll", "SQL", this.UserInfo);

            SuppliersReturnValue reloadReturn = (SuppliersReturnValue)await (new SuppliersLayerB()).DoBusinessLogicAsync(
                reloadParameter, DbEnum.IsolationLevelEnum.User);

            this.SaveEditingTable(reloadReturn.Suppliers);
            model.Suppliers = reloadReturn.Suppliers;
            model.Message = message;

            return View("Index", model);
        }

        #endregion

        #region ユーティリティ

        /// <summary>画面のセル値を DataTable へ読み戻す</summary>
        /// <param name="dt">編集中の DataTable</param>
        /// <param name="model">SuppliersViewModel</param>
        /// <remarks>
        /// 現在値と一致するなら代入しない（無駄な Modified ＝無駄な UPDATE を作らないため）。
        /// 行の対応付けは RowIndex で行う（Deleted 行は描画されず連番がズレるため）。
        /// </remarks>
        private void ReadRowsIntoTable(DataTable dt, SuppliersViewModel model)
        {
            if (model.Rows == null) { return; }

            foreach (SupplierRowViewModel row in model.Rows)
            {
                if (row.RowIndex < 0 || dt.Rows.Count <= row.RowIndex) { continue; }

                DataRow dr = dt.Rows[row.RowIndex];
                if (dr.RowState == DataRowState.Deleted) { continue; }

                SuppliersBController.SetIfChanged(dr, "CompanyName", row.CompanyName);
                SuppliersBController.SetIfChanged(dr, "ContactName", row.ContactName);
                SuppliersBController.SetIfChanged(dr, "ContactTitle", row.ContactTitle);
                SuppliersBController.SetIfChanged(dr, "City", row.City);
                SuppliersBController.SetIfChanged(dr, "Country", row.Country);
                SuppliersBController.SetIfChanged(dr, "Phone", row.Phone);
            }
        }

        /// <summary>値が変わっているときだけ代入する</summary>
        /// <param name="dr">対象行</param>
        /// <param name="columnName">列名</param>
        /// <param name="newValue">画面の値</param>
        private static void SetIfChanged(DataRow dr, string columnName, string newValue)
        {
            string current = dr[columnName] == DBNull.Value ? "" : Convert.ToString(dr[columnName]);
            string edited = newValue ?? "";

            if (current == edited) { return; }

            // 元が DBNull の列に "" を入れると無駄な Modified になる＝空文字は DBNull へ戻す
            dr[columnName] = (edited.Length == 0) ? (object)DBNull.Value : edited;
        }

        #endregion
    }
}
