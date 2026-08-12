//**********************************************************************************
//* マスタ・テーブル（Suppliers）サンプル（引数クラス）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：SuppliersParameterValue
//* クラス日本語名  ：Suppliers 業務処理の引数クラス
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

using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Util;

namespace _2CSClientWin_sample.Suppliers.Common
{
    /// <summary>Suppliers 業務処理の引数クラス</summary>
    public class SuppliersParameterValue : MyParameterValue
    {
        /// <summary>バッチ更新対象の明細（RowState で CUD を振り分ける）</summary>
        public DataTable Suppliers { get; set; }

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        /// <param name="screenId">画面ID</param>
        /// <param name="controlId">コントロールID</param>
        /// <param name="methodName">メソッド名（B層は "UOC_" + これでレイトバインドする）</param>
        /// <param name="actionType">アクション タイプ（先頭がDBMSコード）</param>
        /// <param name="user">ユーザ情報</param>
        public SuppliersParameterValue(
            string screenId, string controlId, string methodName, string actionType, MyUserInfo user)
            : base(screenId, controlId, methodName, actionType, user)
        {
            // Baseのコンストラクタに引数を渡すために必要。
        }

        #endregion
    }
}
