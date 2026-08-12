//**********************************************************************************
//* マスタ・テーブル（Suppliers）サンプル（Ｐ層）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：SuppliersA
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

using System.Web.UI.WebControls;

using WebForms_Sample.Suppliers.Business;
using WebForms_Sample.Suppliers.Common;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Db;

namespace WebForms_Sample.Aspx.Suppliers
{
    /// <summary>画面Ａ（件数確認・画面遷移）</summary>
    /// <remarks>
    /// マスタ ページ上のボタンのハンドラ名は UOC_〈マスタ名〉_〈コントロール名〉_〈イベント名〉。
    /// 接頭辞はコンテンツ .aspx の名前ではなく、マスタ .master の名前（businessScreen）。
    /// 実装先は画面コードクラス（ここ）＝親クラス2 のカスタマイズは不要。
    /// </remarks>
    public partial class SuppliersA : MyBaseController
    {
        #region ページロード処理（実装必須）

        /// <summary>初回ロード時の初期処理</summary>
        protected override void UOC_FormInit()
        {
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
            ((Button)this.GetMasterWebControl("btnMain1")).Text = "件数確認";
            ((Button)this.GetMasterWebControl("btnMain2")).Text = "画面遷移";

            // 不要なボタンは disable にする
            for (int i = 3; i <= 5; i++)
            {
                Button button = (Button)this.GetMasterWebControl("btnMain" + i);
                button.Text = "－";
                button.Enabled = false;
            }
        }

        #endregion

        #region マスタ ページ上のメイン ボタンのイベント処理

        /// <summary>［件数確認］Suppliers のデータ件数を OK メッセージ ダイアログに表示する</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>遷移先URL（遷移しないので空文字列）</returns>
        protected string UOC_businessScreen_btnMain1_Click(FxEventArgs fxEventArgs)
        {
            // ↓B層実行：Suppliers のデータ件数を取得-----------------------------------------------------

            SuppliersParameterValue parameterValue = new SuppliersParameterValue(
                this.ContentPageFileNoEx, fxEventArgs.ButtonID, "SelectCount", "SQL", this.UserInfo);

            SuppliersLayerB layerB = new SuppliersLayerB();
            SuppliersReturnValue returnValue =
                (SuppliersReturnValue)layerB.DoBusinessLogic(parameterValue, DbEnum.IsolationLevelEnum.User);

            // ↑B層実行：Suppliers のデータ件数を取得-----------------------------------------------------

            // 業務例外は例外ではなく戻り値（ErrorFlag）で戻る＝catch しない
            if (returnValue.ErrorFlag)
            {
                this.ShowOKMessageDialog(
                    returnValue.ErrorMessageID, returnValue.ErrorMessage,
                    FxEnum.IconType.Exclamation, "件数確認");
            }
            else
            {
                this.ShowOKMessageDialog(
                    "I0001", "Suppliers は " + returnValue.Count + " 件のデータがあります。",
                    FxEnum.IconType.Information, "件数確認");
            }

            return string.Empty;
        }

        /// <summary>［画面遷移］画面Ｂへ遷移する</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>遷移先URL</returns>
        protected string UOC_businessScreen_btnMain2_Click(FxEventArgs fxEventArgs)
        {
            return "~/Aspx/Suppliers/SuppliersB.aspx";
        }

        #endregion
    }
}
