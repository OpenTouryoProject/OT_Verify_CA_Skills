//**********************************************************************************
//* マスタ・テーブル（Suppliers）サンプル（Ｐ層）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：SuppliersBaseForm
//* クラス日本語名  ：業務画面の基底フォーム（フッタのメイン ボタン5つを持つ）
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

using Touryo.Infrastructure.Business.RichClient.Presentation;

namespace _2CSClientWin_sample.Suppliers
{
    /// <summary>業務画面の基底フォーム</summary>
    /// <remarks>
    /// MyBaseControllerWin を継承した基底フォームに共通レイアウト（フッタのメイン ボタン5つ）を実装し、
    /// 各画面はこの基底フォームを継承する。
    ///
    /// ★ WinForms では、基底フォームから継承したコントロールも
    ///   フレームワークがコントロール ツリーを再帰検索して結線する。
    ///   Web Forms のマスタ ページのような「マスタ名の接頭辞」は要らず、
    ///   ハンドラ名は所在に関わらず UOC_〈コントロール名〉_〈イベント名〉。
    ///   （＝各画面で UOC_btnMain1_Click … を実装する）
    /// </remarks>
    public partial class SuppliersBaseForm : MyBaseControllerWin
    {
        /// <summary>コンストラクタ</summary>
        public SuppliersBaseForm()
        {
            this.InitializeComponent();
        }

        /// <summary>フッタのメイン ボタンのキャプション・活性/非活性を設定する</summary>
        /// <param name="captions">5つ分のキャプション（null / 空なら非活性）</param>
        protected void SetFooterButtons(params string[] captions)
        {
            Button[] buttons = new Button[]
            {
                this.btnMain1, this.btnMain2, this.btnMain3, this.btnMain4, this.btnMain5
            };

            for (int i = 0; i < buttons.Length; i++)
            {
                string caption = (captions != null && i < captions.Length) ? captions[i] : null;

                if (string.IsNullOrEmpty(caption))
                {
                    buttons[i].Text = "－";
                    buttons[i].Enabled = false;
                }
                else
                {
                    buttons[i].Text = caption;
                    buttons[i].Enabled = true;
                }
            }
        }
    }
}
