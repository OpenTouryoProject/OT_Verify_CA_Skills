//**********************************************************************************
//* 業務画面用の Master Page（Ｐ層）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：businessScreen
//* クラス日本語名  ：業務画面用の Master Page
//*
//* 作成日時        ：2026/08/12
//* 作成者          ：コーディング エージェント
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/12  コーディング Ａ   新規作成
//**********************************************************************************

using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;

namespace WebForms_Sample.Aspx.Common.Master
{
    /// <summary>業務画面用の Master Page</summary>
    /// <remarks>
    /// マスタ ページのコードビハインドは BaseMasterController を継承する。
    /// フッタのメイン ボタン（btnMain1〜5）のイベントは、
    /// UOC_businessScreen_btnMainN_Click として各コンテンツ画面のコードビハインドに実装する。
    /// </remarks>
    public partial class businessScreen : BaseMasterController
    {
        /// <summary>UserName</summary>
        public string UserName
        {
            get
            {
                var user = (MyUserInfo)UserInfoHandle.GetUserInformation();

                if (user == null)
                {
                    return "anonymous";
                }
                else
                {
                    return user.UserName;
                }
            }
        }
    }
}
