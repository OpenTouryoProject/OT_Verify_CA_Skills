//**********************************************************************************
//* マスタ・テーブル（Suppliers）サンプル（ビューモデル）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：SuppliersViewModel
//* クラス日本語名  ：Suppliers 画面のビューモデル
//*
//* 作成日時        ：2026/08/12
//* 作成者          ：コーディング エージェント
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/12  コーディング Ａ   新規作成
//**********************************************************************************

using System.Collections.Generic;
using System.Data;

namespace MVC_Sample.Models.ViewModels
{
    /// <summary>Suppliers 画面のビューモデル</summary>
    public class SuppliersViewModel : BaseViewModel
    {
        /// <summary>ダイアログに表示するメッセージ（空なら表示しない）</summary>
        public string Message { get; set; }

        /// <summary>一覧（バッチ更新の編集対象）</summary>
        public DataTable Suppliers { get; set; }

        /// <summary>ポストバックで戻ってくる編集後の明細</summary>
        public List<SupplierRowViewModel> Rows { get; set; }

        /// <summary>コンストラクタ</summary>
        public SuppliersViewModel()
        {
            this.Message = "";
            this.Rows = new List<SupplierRowViewModel>();
        }
    }

    /// <summary>一覧1行分（ポストバックで受け取る）</summary>
    /// <remarks>
    /// RowIndex は DataTable の行インデックス。
    /// Deleted 行はグリッドに描画しないため連番とはズレる＝必ずこの値で DataRow を引く。
    /// </remarks>
    public class SupplierRowViewModel
    {
        /// <summary>DataTable の行インデックス</summary>
        public int RowIndex { get; set; }

        /// <summary>CompanyName</summary>
        public string CompanyName { get; set; }

        /// <summary>ContactName</summary>
        public string ContactName { get; set; }

        /// <summary>ContactTitle</summary>
        public string ContactTitle { get; set; }

        /// <summary>City</summary>
        public string City { get; set; }

        /// <summary>Country</summary>
        public string Country { get; set; }

        /// <summary>Phone</summary>
        public string Phone { get; set; }
    }
}
