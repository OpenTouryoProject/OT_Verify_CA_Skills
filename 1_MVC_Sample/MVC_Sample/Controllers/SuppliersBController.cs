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
//**********************************************************************************

using System;
using System.Data;
using System.Threading.Tasks;
using System.Web.Mvc;

using MVC_Sample.Logic.Business;
using MVC_Sample.Logic.Common;
using MVC_Sample.Models.ViewModels;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Public.Db;

namespace MVC_Sample.Controllers
{
    /// <summary>画面Ｂ（一覧・行追加／行削除・バッチ更新）</summary>
    /// <remarks>
    /// バッチ更新は DataRow の RowState（Added / Modified / Deleted）で CUD を振り分ける。
    /// Web は複数ポストバックに跨って編集するため、編集中の DataTable を Session に保持する
    /// （RowState を保つため。StateServer なので DataTable は直列化可能である必要がある＝可）。
    /// </remarks>
    [Authorize]
    public class SuppliersBController : MyBaseMVController
    {
        /// <summary>編集中 DataTable の Session キー</summary>
        private const string SessionKey = "SuppliersEditingDataTable";

        /// <summary>編集中の DataTable（Session）</summary>
        private DataTable EditingTable
        {
            get { return this.Session[SuppliersBController.SessionKey] as DataTable; }
            set { this.Session[SuppliersBController.SessionKey] = value; }
        }

        #region 画面表示

        /// <summary>画面の初期表示</summary>
        /// <param name="model">SuppliersViewModel</param>
        /// <returns>初期表示状態の画面</returns>
        [HttpGet]
        public ActionResult Index(SuppliersViewModel model)
        {
            // 画面を開き直したら編集内容は破棄する（Session を残さない）
            this.EditingTable = null;
            return View(model);
        }

        #endregion

        #region 一覧取得

        /// <summary>Suppliers の一覧を取得する</summary>
        /// <param name="model">SuppliersViewModel</param>
        /// <returns>再描画</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectAll(SuppliersViewModel model)
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
                this.EditingTable = returnValue.Suppliers;
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
        public ActionResult AddRow(SuppliersViewModel model)
        {
            DataTable dt = this.EditingTable;
            if (dt == null)
            {
                model.Message = "先に一覧を取得して下さい。";
                return View("Index", model);
            }

            // 画面の編集内容を DataTable に反映してから行を足す
            this.ReadRowsIntoTable(dt, model);

            DataRow newRow = dt.NewRow();
            // SupplierID は IDENTITY 列＝採番されるまで値が無い
            dt.Rows.Add(newRow);

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
        public ActionResult DeleteRow(SuppliersViewModel model, int rowIndex)
        {
            DataTable dt = this.EditingTable;
            if (dt == null)
            {
                model.Message = "先に一覧を取得して下さい。";
                return View("Index", model);
            }

            this.ReadRowsIntoTable(dt, model);

            if (0 <= rowIndex && rowIndex < dt.Rows.Count)
            {
                DataRow dr = dt.Rows[rowIndex];
                if (dr.RowState == DataRowState.Added)
                {
                    // 未挿入の追加行は取り除くだけでよい
                    dr.Delete();
                }
                else
                {
                    // ★ Rows.Remove ではなく Delete（Remove だと Deleted にならず DELETE が出ない）
                    dr.Delete();
                }
            }

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
        public async Task<ActionResult> BatchUpdate(SuppliersViewModel model)
        {
            DataTable dt = this.EditingTable;
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

            this.EditingTable = reloadReturn.Suppliers;
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
