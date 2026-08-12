//**********************************************************************************
//* マスタ・テーブル（Suppliers）サンプル（Ｂ層）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：SuppliersLayerB
//* クラス日本語名  ：Suppliers の業務処理
//*
//* 作成日時        ：2026/08/12
//* 作成者          ：コーディング エージェント
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/12  コーディング Ａ   新規作成（件数確認）
//*  2026/08/12  コーディング Ａ   一覧取得（共通Dao）を追加
//*  2026/08/12  コーディング Ａ   バッチ更新（自動生成Dao）を追加
//**********************************************************************************

using System;
using System.Data;

using MVC_Sample.Logic.Common;
using MVC_Sample.Logic.Dao;

using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Business.Exceptions;
using Touryo.Infrastructure.Framework.Exceptions;

namespace MVC_Sample.Logic.Business
{
    /// <summary>Suppliers の業務処理</summary>
    /// <remarks>
    /// UOC_〈methodName〉 はレイトバインドで呼ばれる（引数1つ・戻り値 void）。
    /// 戻り値は this.ReturnValue で返す（メソッド冒頭で設定する＝例外時にも戻るようにするため）。
    /// トランザクションのコミット／ロールバックはフレームワークが行うので、ここには書かない。
    /// </remarks>
    public class SuppliersLayerB : MyFcBaseLogic
    {
        #region 件数確認（個別Dao）

        /// <summary>Suppliers のデータ件数を取得する</summary>
        /// <param name="parameterValue">引数クラス</param>
        private void UOC_SelectCount(SuppliersParameterValue parameterValue)
        {
            // 戻り値クラスは業務処理の前に設定する（例外時にも戻り値を返すため）
            SuppliersReturnValue returnValue = new SuppliersReturnValue();
            this.ReturnValue = returnValue;

            // ↓業務処理-----------------------------------------------------

            SuppliersDao dao = new SuppliersDao(this.GetDam());
            dao.SelectCount(parameterValue, returnValue);

            // ↑業務処理-----------------------------------------------------
        }

        #endregion

        #region 一覧取得（共通Dao）

        /// <summary>Suppliers の一覧を取得する</summary>
        /// <param name="parameterValue">引数クラス</param>
        private void UOC_SelectAll(SuppliersParameterValue parameterValue)
        {
            SuppliersReturnValue returnValue = new SuppliersReturnValue();
            this.ReturnValue = returnValue;

            // ↓業務処理-----------------------------------------------------

            // 共通Dao は SQL をプロパティで指定する（SetSqlByFile2 の直呼びは実行時エラー）
            CmnDao cmnDao = new CmnDao(this.GetDam());
            cmnDao.SQLFileName = "SuppliersSelectAll.sql";

            DataTable dt = new DataTable("Suppliers");
            cmnDao.ExecSelectFill_DT(dt);

            // ★ 追加行（Added）のために SupplierID 列を仮採番できるようにする。
            //   Fill はスキーマ（NOT NULL）も取り込むため、そのままだと dt.NewRow() の追加が
            //   NoNullAllowedException（列 'SupplierID' に nulls を使用することはできません）になる。
            //   SupplierID は IDENTITY＝実際の採番はDB側なので、DataTable 上は
            //   実データと衝突しない負値で仮採番しておく（INSERT には渡さない）。
            DataColumn pk = dt.Columns["SupplierID"];
            pk.AutoIncrement = true;
            pk.AutoIncrementSeed = -1;
            pk.AutoIncrementStep = -1;

            // 主キーを持たせておく（画面側の行特定・バッチ更新の前提）
            dt.PrimaryKey = new DataColumn[] { pk };

            returnValue.Suppliers = dt;

            // ↑業務処理-----------------------------------------------------
        }

        #endregion

        #region バッチ更新（自動生成Dao）

        /// <summary>Suppliers の明細をバッチ更新する（DataRowState で CUD を振り分ける）</summary>
        /// <param name="parameterValue">引数クラス</param>
        private void UOC_BatchUpdate(SuppliersParameterValue parameterValue)
        {
            SuppliersReturnValue returnValue = new SuppliersReturnValue();
            this.ReturnValue = returnValue;

            // ↓業務処理-----------------------------------------------------

            DataTable dt = parameterValue.Suppliers;
            if (dt == null)
            {
                throw new BusinessApplicationException(
                    "W0001", "更新対象がありません。先に一覧を取得して下さい。", "-");
            }

            DaoSuppliers dao = new DaoSuppliers(this.GetDam());

            // ★ 排他方式について
            //   Suppliers にはタイムスタンプ列が無い。タイムスタンプが無いときの定石は
            //   「取得時の全列（DataRowVersion.Original）を WHERE に入れる D3_Update / D4_Delete」だが、
            //   このテーブルは HomePage が ntext で、SQL Server では ntext を "=" で比較できない
            //   （Msg 402: データ型 ntext と nvarchar は equal to 演算子では互換性がありません）。
            //   生成された D3_Update / D4_Delete は全列を "=" で比較するため、このテーブルでは実行時に落ちる。
            //   → 主キーのみを WHERE に持つ S3_Update / S4_Delete を使う。
            //   この構成では更新件数0の検知は「他者が先に削除した」ことの検知に留まり、
            //   上書き（Lost Update）は検知できない。厳密な排他が要るならタイムスタンプ列の追加を検討する。

            // 同じ主キーを使い回す場合に備え、Deleted → Added の順で流す。
            // （Added を先に流すと、まだ消えていない旧行と主キーが衝突しうる）
            foreach (DataRow dr in dt.Rows)
            {
                if (dr.RowState != DataRowState.Deleted) { continue; }

                dao.ClearParametersFromHt();

                // 削除行は現在値を持たないので Original から読む
                dao.PK_SupplierID = dr["SupplierID", DataRowVersion.Original];

                int deleted = dao.S4_Delete();
                if (deleted == 0)
                {
                    // 対象行が既に存在しない（他者が先に削除した）＝リトライ可能なので業務例外
                    throw new BusinessApplicationException(
                        "W0002", "他のユーザによって削除されています。再取得してやり直して下さい。",
                        "SupplierID=" + dr["SupplierID", DataRowVersion.Original]);
                }
                returnValue.DeleteCount += deleted;
            }

            foreach (DataRow dr in dt.Rows)
            {
                switch (dr.RowState)
                {
                    case DataRowState.Added:

                        dao.ClearParametersFromHt();

                        // SupplierID は IDENTITY 列なので設定しない。
                        // 設定した列だけを INSERT する D1_Insert（動的SQL）を使う
                        // （S1_Insert は全列を INSERT するため IDENTITY 列で失敗する）。
                        dao.CompanyName  = SuppliersLayerB.ToDbValue(dr["CompanyName"]);
                        dao.ContactName  = SuppliersLayerB.ToDbValue(dr["ContactName"]);
                        dao.ContactTitle = SuppliersLayerB.ToDbValue(dr["ContactTitle"]);
                        dao.City         = SuppliersLayerB.ToDbValue(dr["City"]);
                        dao.Country      = SuppliersLayerB.ToDbValue(dr["Country"]);
                        dao.Phone        = SuppliersLayerB.ToDbValue(dr["Phone"]);

                        returnValue.InsertCount += dao.D1_Insert();

                        break;

                    case DataRowState.Modified:

                        dao.ClearParametersFromHt();

                        // WHERE は主キー（取得時の値＝Original）
                        dao.PK_SupplierID = dr["SupplierID", DataRowVersion.Original];

                        // SET 句は Set_〈列名〉_forUPD（〈列名〉に入れると WHERE 条件になる）
                        dao.Set_CompanyName_forUPD  = SuppliersLayerB.ToDbValue(dr["CompanyName"]);
                        dao.Set_ContactName_forUPD  = SuppliersLayerB.ToDbValue(dr["ContactName"]);
                        dao.Set_ContactTitle_forUPD = SuppliersLayerB.ToDbValue(dr["ContactTitle"]);
                        dao.Set_City_forUPD         = SuppliersLayerB.ToDbValue(dr["City"]);
                        dao.Set_Country_forUPD      = SuppliersLayerB.ToDbValue(dr["Country"]);
                        dao.Set_Phone_forUPD        = SuppliersLayerB.ToDbValue(dr["Phone"]);

                        int updated = dao.S3_Update();
                        if (updated == 0)
                        {
                            // 更新件数0＝対象行が既に存在しない
                            throw new BusinessApplicationException(
                                "W0002", "他のユーザによって更新されています。再取得してやり直して下さい。",
                                "SupplierID=" + dr["SupplierID", DataRowVersion.Original]);
                        }
                        returnValue.UpdateCount += updated;

                        break;
                }
            }

            // ↑業務処理-----------------------------------------------------
        }

        #endregion

        #region ユーティリティ

        /// <summary>空文字は NULL 相当（DBNull）として扱う</summary>
        /// <param name="value">列の値</param>
        /// <returns>DB へ渡す値</returns>
        /// <remarks>
        /// 自動生成Dao はプロパティに設定された列だけを SQL に載せる。
        /// null を渡すと「設定しなかった」ことになり列ごと落ちるので、DBNull を渡す。
        /// </remarks>
        private static object ToDbValue(object value)
        {
            if (value == null || value == DBNull.Value) { return DBNull.Value; }
            if (value is string && ((string)value).Length == 0) { return DBNull.Value; }
            return value;
        }

        #endregion
    }
}
