//**********************************************************************************
//* マスタ・テーブル（Suppliers）サンプル（Ｄ層）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：SuppliersDao
//* クラス日本語名  ：Suppliers の個別Dao
//*
//* 作成日時        ：2026/08/12
//* 作成者          ：コーディング エージェント
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/12  コーディング Ａ   新規作成
//**********************************************************************************

using MVC_Sample.Logic.Common;

using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Public.Db;

namespace MVC_Sample.Logic.Dao
{
    /// <summary>Suppliers の個別Dao</summary>
    public class SuppliersDao : MyBaseDao
    {
        /// <summary>コンストラクタ</summary>
        /// <param name="dam">Dam（B層が保持しているものを受け取る）</param>
        public SuppliersDao(BaseDam dam) : base(dam) { }

        /// <summary>Suppliers のデータ件数を取得する</summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <param name="returnValue">戻り値クラス</param>
        public void SelectCount(SuppliersParameterValue parameterValue, SuppliersReturnValue returnValue)
        {
            // ↓DBアクセス-----------------------------------------------------

            this.SetSqlByFile2("SuppliersCount.sql");
            returnValue.Count = (int)this.ExecSelectScalar();

            // ↑DBアクセス-----------------------------------------------------
        }
    }
}
