//**********************************************************************************
//* マスタ・テーブル（Suppliers）サンプル（Ｐ層）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：SuppliersFormA
//* クラス日本語名  ：画面Ａ（件数確認・画面遷移）
//*
//* 作成日時        ：2026/08/12
//* 作成者          ：コーディング エージェント
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/12  コーディング Ａ   新規作成
//**********************************************************************************

using System.Windows.Forms;

using _2CSClientWin_sample.Suppliers.Business;
using _2CSClientWin_sample.Suppliers.Common;

using Touryo.Infrastructure.Business.RichClient.Presentation;
using Touryo.Infrastructure.Framework.RichClient.Presentation;
using Touryo.Infrastructure.Public.Db;

namespace _2CSClientWin_sample.Suppliers
{
    /// <summary>画面Ａ（件数確認・画面遷移）</summary>
    /// <remarks>
    /// 基底フォーム（SuppliersBaseForm）のフッタ ボタンも、フレームワークが
    /// コントロール ツリーを再帰検索して結線するので UOC_btnMainN_Click で受けられる。
    /// </remarks>
    public partial class SuppliersFormA : SuppliersBaseForm
    {
        /// <summary>コンストラクタ</summary>
        public SuppliersFormA()
        {
            this.InitializeComponent();
        }

        #region 初期処理

        /// <summary>フォームの初期処理</summary>
        protected override void UOC_FormInit()
        {
            // 画面ごとのボタン制御（キャプション変更・不要なものは非活性）
            this.SetFooterButtons("件数確認", "画面遷移", null, null, null);
        }

        #endregion

        #region イベント処理

        /// <summary>［件数確認］Suppliers のデータ件数を MessageBox で表示する</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        protected void UOC_btnMain1_Click(RcFxEventArgs rcFxEventArgs)
        {
            // ↓B層実行：Suppliers のデータ件数を取得-----------------------------------------------------

            SuppliersParameterValue parameterValue = new SuppliersParameterValue(
                this.Name, rcFxEventArgs.ControlName, "SelectCount", "SQL",
                MyBaseControllerWin.UserInfo);

            SuppliersLayerB layerB = new SuppliersLayerB();
            SuppliersReturnValue returnValue =
                (SuppliersReturnValue)layerB.DoBusinessLogic(parameterValue, DbEnum.IsolationLevelEnum.User);

            // ★ 2CS はコネクションがグローバルでコミットが手動＝呼ばないと確定しない
            SuppliersLayerB.CommitAndClose();

            // ↑B層実行：Suppliers のデータ件数を取得-----------------------------------------------------

            if (returnValue.ErrorFlag)
            {
                // 業務例外は戻り値で戻る（2CS は自動ロールバックされないので明示的に巻き戻す）
                SuppliersLayerB.RollbackAndClose();
                MessageBox.Show(returnValue.ErrorMessage, "件数確認",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("Suppliers は " + returnValue.Count + " 件のデータがあります。",
                    "件数確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>［画面遷移］画面Ｂを開く</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        protected void UOC_btnMain2_Click(RcFxEventArgs rcFxEventArgs)
        {
            // WinForms に画面遷移の仕組みは無いので、フォームを開く
            using (SuppliersFormB formB = new SuppliersFormB())
            {
                formB.ShowDialog(this);
            }
        }

        #endregion
    }
}
