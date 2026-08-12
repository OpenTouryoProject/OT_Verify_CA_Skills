//**********************************************************************************
//* マスタ・テーブル（Suppliers）サンプル（Ｐ層）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：SuppliersFormB
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

using System.Data;
using System.Windows.Forms;

using WSIFType_sample;

using Touryo.Infrastructure.Business.RichClient.Presentation;
using Touryo.Infrastructure.Framework.RichClient.Presentation;
using Touryo.Infrastructure.Framework.Transmission;

namespace WSClientWin_sample.Suppliers
{
    /// <summary>画面Ｂ（一覧・行追加／行削除・バッチ更新）</summary>
    /// <remarks>
    /// 3層なので B層は CallController.Invoke(サービス論理名, 引数クラス) で呼ぶ。
    /// トランザクション境界はサーバ側の B層なので、クライアントで
    /// CommitAndClose / RollbackAndClose は呼ばない（2CS との違い）。
    ///
    /// 一覧は DataGridView に DataTable を BindingSource 経由でバインドし、
    /// ［追加］／［削除］は通常のボタン（btn 接頭辞で結線）で行う。
    /// </remarks>
    public partial class SuppliersFormB : SuppliersBaseForm
    {
        /// <summary>サービス論理名（TMInProcessDefinition.xml / TMProtocolDefinition2.xml で解決）</summary>
        internal const string LogicalName = "suppliersWebService2"; //suppliersInProcess

        /// <summary>編集中の DataTable（RowState を保つ）</summary>
        private DataTable editingTable = null;

        /// <summary>コンストラクタ</summary>
        public SuppliersFormB()
        {
            this.InitializeComponent();
        }

        #region 初期処理

        /// <summary>フォームの初期処理</summary>
        protected override void UOC_FormInit()
        {
            this.SetFooterButtons("一覧取得", "更新", "閉じる", null, null);
        }

        #endregion

        #region フッタのメイン ボタンのイベント処理

        /// <summary>［一覧取得］Suppliers の一覧を取得してグリッドへバインドする</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        protected void UOC_btnMain1_Click(RcFxEventArgs rcFxEventArgs)
        {
            // ↓B層実行：Suppliers の一覧を取得-----------------------------------------------------

            SuppliersParameterValue parameterValue = new SuppliersParameterValue(
                this.Name, rcFxEventArgs.ControlName, "SelectAll", "SQL",
                MyBaseControllerWin.UserInfo);

            CallController callCtrl = new CallController(MyBaseControllerWin.UserInfo);
            SuppliersReturnValue returnValue =
                (SuppliersReturnValue)callCtrl.Invoke(SuppliersFormB.LogicalName, parameterValue);

            // ↑B層実行：Suppliers の一覧を取得-----------------------------------------------------

            if (returnValue.ErrorFlag)
            {
                MessageBox.Show(returnValue.ErrorMessage, "一覧取得",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.editingTable = returnValue.Suppliers;
            this.BindGrid();

            MessageBox.Show("一覧を取得しました（" + this.editingTable.Rows.Count + " 件）。",
                "一覧取得", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>［更新］YES/NO 確認のうえバッチ更新を実行する</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        protected void UOC_btnMain2_Click(RcFxEventArgs rcFxEventArgs)
        {
            if (this.editingTable == null)
            {
                MessageBox.Show("先に一覧を取得して下さい。", "更新",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // グリッドの編集中セルを確定させる
            this.dgvSuppliers.EndEdit();
            this.bsSuppliers.EndEdit();

            // YES/NO 確認ダイアログは標準の MessageBox
            DialogResult answer = MessageBox.Show("更新します。よろしいですか？", "更新",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (answer != DialogResult.Yes) { return; }

            // ↓B層実行：Suppliers のバッチ更新-----------------------------------------------------

            SuppliersParameterValue parameterValue = new SuppliersParameterValue(
                this.Name, rcFxEventArgs.ControlName, "BatchUpdate", "SQL",
                MyBaseControllerWin.UserInfo);
            parameterValue.Suppliers = this.editingTable;

            CallController callCtrl = new CallController(MyBaseControllerWin.UserInfo);
            SuppliersReturnValue returnValue =
                (SuppliersReturnValue)callCtrl.Invoke(SuppliersFormB.LogicalName, parameterValue);

            // ↑B層実行：Suppliers のバッチ更新-----------------------------------------------------

            if (returnValue.ErrorFlag)
            {
                // 業務例外はサーバ側 B層でロールバック済み（3層はクライアントで巻き戻さない）
                MessageBox.Show(returnValue.ErrorMessage, "更新",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 反映できたので編集状態を確定する
            this.editingTable.AcceptChanges();

            string message = "更新しました（挿入 " + returnValue.InsertCount
                + " 件／更新 " + returnValue.UpdateCount
                + " 件／削除 " + returnValue.DeleteCount + " 件）。";

            // IDENTITY の採番値は DataTable に戻らないので、反映後は一覧を取り直す
            SuppliersParameterValue reloadParameter = new SuppliersParameterValue(
                this.Name, rcFxEventArgs.ControlName, "SelectAll", "SQL",
                MyBaseControllerWin.UserInfo);

            SuppliersReturnValue reloadReturn =
                (SuppliersReturnValue)(new CallController(MyBaseControllerWin.UserInfo)).Invoke(
                    SuppliersFormB.LogicalName, reloadParameter);

            this.editingTable = reloadReturn.Suppliers;
            this.BindGrid();

            MessageBox.Show(message, "更新", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>［閉じる］画面を閉じる</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        protected void UOC_btnMain3_Click(RcFxEventArgs rcFxEventArgs)
        {
            this.Close();
        }

        #endregion

        #region グリッド周辺のボタンのイベント処理

        /// <summary>［行追加］グリッド外の追加ボタン＝空行を足す（RowState は Added になる）</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        protected void UOC_btnAddRow_Click(RcFxEventArgs rcFxEventArgs)
        {
            if (this.editingTable == null)
            {
                MessageBox.Show("先に一覧を取得して下さい。", "行追加",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.dgvSuppliers.EndEdit();
            this.bsSuppliers.EndEdit();

            DataRow newRow = this.editingTable.NewRow();
            // SupplierID は IDENTITY 列＝DataTable 上は負値で仮採番される（B層で設定済み）
            this.editingTable.Rows.Add(newRow);
        }

        /// <summary>［削除］選択行を削除状態にする（RowState は Deleted になる）</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        protected void UOC_btnDeleteRow_Click(RcFxEventArgs rcFxEventArgs)
        {
            if (this.editingTable == null) { return; }

            this.dgvSuppliers.EndEdit();
            this.bsSuppliers.EndEdit();

            if (this.dgvSuppliers.CurrentRow == null) { return; }

            DataRowView drv = this.dgvSuppliers.CurrentRow.DataBoundItem as DataRowView;
            if (drv == null) { return; }

            // ★ Rows.Remove ではなく Delete（Remove だと Deleted にならず DELETE が出ない）
            drv.Row.Delete();
        }

        #endregion

        #region ユーティリティ

        /// <summary>グリッドへバインドする</summary>
        private void BindGrid()
        {
            // Deleted 行は DefaultView から外れる＝グリッドには表示されない
            this.bsSuppliers.DataSource = this.editingTable;
            this.dgvSuppliers.DataSource = this.bsSuppliers;

            if (this.dgvSuppliers.Columns.Contains("SupplierID"))
            {
                // IDENTITY 列は編集させない
                this.dgvSuppliers.Columns["SupplierID"].ReadOnly = true;
            }
        }

        #endregion
    }
}
