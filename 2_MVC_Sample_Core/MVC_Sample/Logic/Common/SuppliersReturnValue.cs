//**********************************************************************************
//* マスタ・テーブル（Suppliers）サンプル（戻り値クラス）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：SuppliersReturnValue
//* クラス日本語名  ：Suppliers 業務処理の戻り値クラス
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

namespace MVC_Sample.Logic.Common
{
    /// <summary>Suppliers 業務処理の戻り値クラス</summary>
    /// <remarks>
    /// エラー系（ErrorFlag / ErrorMessageID / ErrorMessage / ErrorInfo）は
    /// BaseReturnValue が持つので、ここには定義し直さない。
    /// </remarks>
    public class SuppliersReturnValue : MyReturnValue
    {
        /// <summary>件数（件数確認）</summary>
        public int Count { get; set; }

        /// <summary>一覧（Suppliers の全件）</summary>
        public DataTable Suppliers { get; set; }

        /// <summary>バッチ更新の反映件数（挿入／更新／削除）</summary>
        public int InsertCount { get; set; }

        /// <summary>バッチ更新の反映件数（更新）</summary>
        public int UpdateCount { get; set; }

        /// <summary>バッチ更新の反映件数（削除）</summary>
        public int DeleteCount { get; set; }
    }
}
