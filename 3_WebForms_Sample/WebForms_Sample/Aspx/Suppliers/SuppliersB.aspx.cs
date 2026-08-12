//**********************************************************************************
//* マスタ・テーブル（Suppliers）サンプル（Ｐ層）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：SuppliersB
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
using System.Web.UI.WebControls;

using WebForms_Sample.Suppliers.Business;
using WebForms_Sample.Suppliers.Common;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Db;

namespace WebForms_Sample.Aspx.Suppliers
{
    /// <summary>画面Ｂ（一覧・行追加／行削除・バッチ更新）</summary>
    /// <remarks>
    /// バッチ更新は DataRow の RowState（Added / Modified / Deleted）で CUD を振り分ける。
    /// 複数ポストバックに跨って編集するので、編集中の DataTable を Session に保持する
    /// （RowState を保つため。sessionState は StateServer＝DataTable は直列化可能なので可）。
    ///
    /// 行の対応付けは SupplierID で行う（Deleted 行はグリッドから外れて index がずれるため）。
    /// 追加行の SupplierID は B層で負値の仮採番が入るので、追加行も一意に引ける。
    /// </remarks>
    public partial class SuppliersB : MyBaseController
    {
        /// <summary>編集中 DataTable の Session キー</summary>
        private const string SessionKey = "SuppliersEditingDataTable";

        /// <summary>編集中の DataTable（Session）</summary>
        private DataTable EditingTable
        {
            get { return this.Session[SuppliersB.SessionKey] as DataTable; }
            set { this.Session[SuppliersB.SessionKey] = value; }
        }

        #region ページロード処理（実装必須）

        /// <summary>初回ロード時の初期処理</summary>
        protected override void UOC_FormInit()
        {
            // 画面を開き直したら編集内容は破棄する（Session を残さない）
            this.EditingTable = null;
            this.InitFooterButtons();
        }

        /// <summary>ポストバック時の初期処理</summary>
        protected override void UOC_FormInit_PostBack()
        {
            this.InitFooterButtons();
        }

        /// <summary>フッタのメイン ボタンのキャプション・活性/非活性を画面ごとに設定する</summary>
        private void InitFooterButtons()
        {
            ((Button)this.GetMasterWebControl("btnMain1")).Text = "一覧取得";
            ((Button)this.GetMasterWebControl("btnMain2")).Text = "更新";
            ((Button)this.GetMasterWebControl("btnMain3")).Text = "戻る";

            for (int i = 4; i <= 5; i++)
            {
                Button button = (Button)this.GetMasterWebControl("btnMain" + i);
                button.Text = "－";
                button.Enabled = false;
            }
        }

        #endregion

        #region マスタ ページ上のメイン ボタンのイベント処理

        /// <summary>［一覧取得］Suppliers の一覧を取得してグリッドへバインドする</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>遷移先URL（遷移しないので空文字列）</returns>
        protected string UOC_businessScreen_btnMain1_Click(FxEventArgs fxEventArgs)
        {
            // ↓B層実行：Suppliers の一覧を取得-----------------------------------------------------

            SuppliersParameterValue parameterValue = new SuppliersParameterValue(
                this.ContentPageFileNoEx, fxEventArgs.ButtonID, "SelectAll", "SQL", this.UserInfo);

            SuppliersLayerB layerB = new SuppliersLayerB();
            SuppliersReturnValue returnValue =
                (SuppliersReturnValue)layerB.DoBusinessLogic(parameterValue, DbEnum.IsolationLevelEnum.User);

            // ↑B層実行：Suppliers の一覧を取得-----------------------------------------------------

            if (returnValue.ErrorFlag)
            {
                this.ShowOKMessageDialog(
                    returnValue.ErrorMessageID, returnValue.ErrorMessage,
                    FxEnum.IconType.Exclamation, "一覧取得");
                return string.Empty;
            }

            this.EditingTable = returnValue.Suppliers;
            this.BindGrid();

            return string.Empty;
        }

        /// <summary>［更新］YES/NO 確認ダイアログを表示する</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>遷移先URL（遷移しないので空文字列）</returns>
        /// <remarks>
        /// ★ YES/NO の後処理は「別ポストバック」で走るので、
        ///   ダイアログを出す時点で画面の編集内容を DataTable へ確定しておく。
        /// </remarks>
        protected string UOC_businessScreen_btnMain2_Click(FxEventArgs fxEventArgs)
        {
            DataTable dt = this.EditingTable;
            if (dt == null)
            {
                this.ShowOKMessageDialog(
                    "W0000", "先に一覧を取得して下さい。", FxEnum.IconType.Exclamation, "更新");
                return string.Empty;
            }

            // 次のポストバックまでローカル変数は残らないので、ここで DataTable に反映しておく
            this.ReadGridIntoTable(dt);

            this.ShowYesNoMessageDialog("Q0001", "更新します。よろしいですか？", "更新");

            return string.Empty;
        }

        /// <summary>［戻る］画面Ａへ遷移する</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>遷移先URL</returns>
        protected string UOC_businessScreen_btnMain3_Click(FxEventArgs fxEventArgs)
        {
            return "~/Aspx/Suppliers/SuppliersA.aspx";
        }

        #endregion

        #region コンテンツ ページ上のコントロールのイベント処理

        /// <summary>［行追加］グリッド外の追加ボタン＝空行を足す（RowState は Added になる）</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>遷移先URL（遷移しないので空文字列）</returns>
        protected string UOC_btnAddRow_Click(FxEventArgs fxEventArgs)
        {
            DataTable dt = this.EditingTable;
            if (dt == null)
            {
                this.ShowOKMessageDialog(
                    "W0000", "先に一覧を取得して下さい。", FxEnum.IconType.Exclamation, "行追加");
                return string.Empty;
            }

            // 画面の編集内容を DataTable に反映してから行を足す
            this.ReadGridIntoTable(dt);

            DataRow newRow = dt.NewRow();
            // SupplierID は IDENTITY 列＝DataTable 上は負値で仮採番される（B層で設定済み）
            dt.Rows.Add(newRow);

            this.BindGrid();

            return string.Empty;
        }

        /// <summary>グリッド内の［削除］＝行を削除状態にする（RowState は Deleted になる）</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <param name="e">GridViewDeleteEventArgs</param>
        /// <returns>遷移先URL（遷移しないので空文字列）</returns>
        protected string UOC_gvwSuppliers_RowDeleting(FxEventArgs fxEventArgs, EventArgs e)
        {
            DataTable dt = this.EditingTable;
            if (dt == null) { return string.Empty; }

            GridViewDeleteEventArgs de = (GridViewDeleteEventArgs)e;

            // 画面の編集内容を先に反映（削除行の特定は SupplierID で行う）
            this.ReadGridIntoTable(dt);

            object supplierId = this.GetSupplierId(de.RowIndex);
            if (supplierId != null)
            {
                DataRow dr = dt.Rows.Find(supplierId);
                if (dr != null)
                {
                    // ★ Rows.Remove ではなく Delete（Remove だと Deleted にならず DELETE が出ない）
                    dr.Delete();
                }
            }

            this.BindGrid();

            return string.Empty;
        }

        /// <summary>グリッドの行コマンド（削除は RowDeleting で受けるのでここでは何もしない）</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>遷移先URL（遷移しないので空文字列）</returns>
        protected string UOC_gvwSuppliers_RowCommand(FxEventArgs fxEventArgs)
        {
            return string.Empty;
        }

        #endregion

        #region YES/NO 確認ダイアログの後処理

        /// <summary>［YES］バッチ更新を実行する</summary>
        /// <param name="parentFxEventArgs">ダイアログを開いたボタンのイベント引数</param>
        /// <remarks>
        /// 後処理は画面コードクラスに実装する（親クラス2 の共通ハンドラには書けない）。
        /// ★ 戻り値は void。コントロールのイベント ハンドラ（string＝遷移先URL）とは違う。
        /// </remarks>
        protected override void UOC_YesNoDialog_Yes_Click(FxEventArgs parentFxEventArgs)
        {
            // 1画面に確認ダイアログが複数ある場合に備え、開いたボタンで振り分ける
            if (parentFxEventArgs.ButtonID != "btnMain2") { return; }

            DataTable dt = this.EditingTable;
            if (dt == null) { return; }

            // ↓B層実行：Suppliers のバッチ更新-----------------------------------------------------

            SuppliersParameterValue parameterValue = new SuppliersParameterValue(
                this.ContentPageFileNoEx, parentFxEventArgs.ButtonID, "BatchUpdate", "SQL", this.UserInfo);
            parameterValue.Suppliers = dt;

            SuppliersLayerB layerB = new SuppliersLayerB();
            SuppliersReturnValue returnValue =
                (SuppliersReturnValue)layerB.DoBusinessLogic(parameterValue, DbEnum.IsolationLevelEnum.User);

            // ↑B層実行：Suppliers のバッチ更新-----------------------------------------------------

            if (returnValue.ErrorFlag)
            {
                // 業務例外＝ロールバック済み。編集内容（RowState）は残してやり直せるようにする
                this.ShowOKMessageDialog(
                    returnValue.ErrorMessageID, returnValue.ErrorMessage,
                    FxEnum.IconType.Exclamation, "更新");
                return;
            }

            // 反映できたので編集状態を確定する
            dt.AcceptChanges();

            string message = "更新しました（挿入 " + returnValue.InsertCount
                + " 件／更新 " + returnValue.UpdateCount
                + " 件／削除 " + returnValue.DeleteCount + " 件）。";

            // IDENTITY の採番値は DataTable に戻らないので、反映後は一覧を取り直す
            SuppliersParameterValue reloadParameter = new SuppliersParameterValue(
                this.ContentPageFileNoEx, parentFxEventArgs.ButtonID, "SelectAll", "SQL", this.UserInfo);

            SuppliersReturnValue reloadReturn =
                (SuppliersReturnValue)(new SuppliersLayerB()).DoBusinessLogic(
                    reloadParameter, DbEnum.IsolationLevelEnum.User);

            this.EditingTable = reloadReturn.Suppliers;
            this.BindGrid();

            this.ShowOKMessageDialog("I0002", message, FxEnum.IconType.Information, "更新");
        }

        /// <summary>［NO］何もしない</summary>
        /// <param name="parentFxEventArgs">ダイアログを開いたボタンのイベント引数</param>
        protected override void UOC_YesNoDialog_No_Click(FxEventArgs parentFxEventArgs)
        {
        }

        /// <summary>［×］何もしない</summary>
        /// <param name="parentFxEventArgs">ダイアログを開いたボタンのイベント引数</param>
        protected override void UOC_YesNoDialog_X_Click(FxEventArgs parentFxEventArgs)
        {
        }

        #endregion

        #region ユーティリティ

        /// <summary>グリッドへバインドする</summary>
        private void BindGrid()
        {
            DataTable dt = this.EditingTable;
            // Deleted 行は DefaultView から外れる＝グリッドには表示されない
            this.gvwSuppliers.DataSource = (dt == null) ? null : dt.DefaultView;
            this.gvwSuppliers.DataBind();
        }

        /// <summary>グリッドの指定行の SupplierID を取得する</summary>
        /// <param name="rowIndex">グリッドの行インデックス</param>
        /// <returns>SupplierID（取れなければ null）</returns>
        private object GetSupplierId(int rowIndex)
        {
            if (rowIndex < 0 || this.gvwSuppliers.Rows.Count <= rowIndex) { return null; }

            HiddenField hf = (HiddenField)this.gvwSuppliers.Rows[rowIndex].FindControl("hfSupplierId");
            if (hf == null || string.IsNullOrEmpty(hf.Value)) { return null; }

            return Convert.ToInt32(hf.Value);
        }

        /// <summary>グリッドのセル値を DataTable へ読み戻す</summary>
        /// <param name="dt">編集中の DataTable</param>
        /// <remarks>
        /// セル編集は自動では DataTable に入らないので、グリッドから読み戻す（この代入で Modified が立つ）。
        /// 現在値と一致するなら代入しない（無駄な Modified ＝無駄な UPDATE を作らないため）。
        /// </remarks>
        private void ReadGridIntoTable(DataTable dt)
        {
            foreach (GridViewRow row in this.gvwSuppliers.Rows)
            {
                HiddenField hf = (HiddenField)row.FindControl("hfSupplierId");
                if (hf == null || string.IsNullOrEmpty(hf.Value)) { continue; }

                DataRow dr = dt.Rows.Find(Convert.ToInt32(hf.Value));
                if (dr == null || dr.RowState == DataRowState.Deleted) { continue; }

                SuppliersB.SetIfChanged(dr, "CompanyName", SuppliersB.GetText(row, "tbCompanyName"));
                SuppliersB.SetIfChanged(dr, "ContactName", SuppliersB.GetText(row, "tbContactName"));
                SuppliersB.SetIfChanged(dr, "ContactTitle", SuppliersB.GetText(row, "tbContactTitle"));
                SuppliersB.SetIfChanged(dr, "City", SuppliersB.GetText(row, "tbCity"));
                SuppliersB.SetIfChanged(dr, "Country", SuppliersB.GetText(row, "tbCountry"));
                SuppliersB.SetIfChanged(dr, "Phone", SuppliersB.GetText(row, "tbPhone"));
            }
        }

        /// <summary>行内テキストボックスの値を取得する</summary>
        /// <param name="row">グリッドの行</param>
        /// <param name="id">コントロールID</param>
        /// <returns>値</returns>
        private static string GetText(GridViewRow row, string id)
        {
            TextBox textBox = (TextBox)row.FindControl(id);
            return (textBox == null) ? "" : textBox.Text;
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
