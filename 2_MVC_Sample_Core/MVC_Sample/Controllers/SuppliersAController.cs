//**********************************************************************************
//* マスタ・テーブル（Suppliers）サンプル（Ｐ層）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：SuppliersAController
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

using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MVC_Sample.Logic.Business;
using MVC_Sample.Logic.Common;
using MVC_Sample.Models.ViewModels;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Public.Db;

namespace MVC_Sample.Controllers
{
    /// <summary>画面Ａ（件数確認・画面遷移）</summary>
    /// <remarks>
    /// MVC に UOC メソッドは無い（アクションメソッドを普通に書く）。
    /// B層の振り分けは引数クラスに渡す MethodName で決まる（サンプルに倣い this.ActionName を渡す）。
    /// </remarks>
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class SuppliersAController : MyBaseMVControllerCore
    {
        /// <summary>画面の初期表示</summary>
        /// <param name="model">SuppliersViewModel</param>
        /// <returns>初期表示状態の画面</returns>
        [HttpGet]
        public IActionResult Index(SuppliersViewModel model)
        {
            return View(model);
        }

        /// <summary>Suppliers のデータ件数を取得する</summary>
        /// <param name="model">SuppliersViewModel</param>
        /// <returns>再描画</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectCount(SuppliersViewModel model)
        {
            // ↓B層実行：Suppliers のデータ件数を取得-----------------------------------------------------

            SuppliersParameterValue parameterValue = new SuppliersParameterValue(
                this.ControllerName, "-", this.ActionName, "SQL", this.UserInfo);

            SuppliersLayerB layerB = new SuppliersLayerB();
            SuppliersReturnValue returnValue = (SuppliersReturnValue)await layerB.DoBusinessLogicAsync(
                parameterValue, DbEnum.IsolationLevelEnum.User);

            // ↑B層実行：Suppliers のデータ件数を取得-----------------------------------------------------

            // 業務例外は例外ではなく戻り値（ErrorFlag）で戻る＝catch しない
            if (returnValue.ErrorFlag)
            {
                model.Message = returnValue.ErrorMessage;
            }
            else
            {
                model.Message = "Suppliers は " + returnValue.Count + " 件のデータがあります。";
            }

            return View("Index", model);
        }
    }
}
