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

using WSIFType_sample;

using Touryo.Infrastructure.Business.RichClient.Presentation;
using Touryo.Infrastructure.Framework.RichClient.Presentation;
using Touryo.Infrastructure.Framework.Transmission;

namespace WSClientWin_sample.Suppliers
{
    /// <summary>画面Ａ（件数確認・画面遷移）</summary>
    /// <remarks>
    /// 3層リッチクライアントなので、B層は直呼びせず
    /// CallController.Invoke(サービス論理名, 引数クラス) で呼ぶ。
    /// 論理名の実体（アセンブリ・クラス）は TMInProcessDefinition.xml が解決する
    /// ＝インプロセス⇄Web サービスをコード無変更で切り替えられる。
    /// 分離レベルはサーバ側が決めるので、ここでは渡さない。
    /// </remarks>
    public partial class SuppliersFormA : SuppliersBaseForm
    {
        /// <summary>サービス論理名（TMInProcessDefinition.xml / TMProtocolDefinition2.xml で解決）</summary>
        private const string LogicalName = "suppliersWebService2"; //suppliersInProcess

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

            CallController callCtrl = new CallController(MyBaseControllerWin.UserInfo);
            SuppliersReturnValue returnValue =
                (SuppliersReturnValue)callCtrl.Invoke(SuppliersFormA.LogicalName, parameterValue);

            // ↑B層実行：Suppliers のデータ件数を取得-----------------------------------------------------

            if (returnValue.ErrorFlag)
            {
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
            using (SuppliersFormB formB = new SuppliersFormB())
            {
                formB.ShowDialog(this);
            }
        }

        #endregion
    }
}
