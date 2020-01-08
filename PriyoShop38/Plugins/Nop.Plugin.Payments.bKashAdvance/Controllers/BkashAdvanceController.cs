using Nop.Plugin.Payments.bKashAdvance.Models;
using Nop.Plugin.Payments.bKashAdvance.Services;
using Nop.Services.Configuration;
using Nop.Services.Payments;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Plugin.Payments.bKashAdvance.Controllers
{
    public class BkashAdvanceController : BasePaymentController
    {
        #region Field        
        private readonly ISettingService _settingService;
        private readonly BkashAdvancePaymentSettings _bkashAdvancePaymentSettings;
        private readonly IBkashAdvanceService _bkashAdvanceService;

        #endregion

        #region Ctr

        public BkashAdvanceController(ISettingService settingService,
            BkashAdvancePaymentSettings bkashAdvancePaymentSettings,
            IBkashAdvanceService bkashAdvanceService)
        {
            this._settingService = settingService;
            this._bkashAdvancePaymentSettings = bkashAdvancePaymentSettings;
            _bkashAdvanceService = bkashAdvanceService;
        }

        #endregion


        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();
            return paymentInfo;
        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();
            return warnings;
        }

        public ActionResult Configure()
        {
            var model = new ConfigurationModel()
            {
                AppKey = _bkashAdvancePaymentSettings.AppKey,
                AppSecret = _bkashAdvancePaymentSettings.AppSecret,
                Password = _bkashAdvancePaymentSettings.Password,
                Username = _bkashAdvancePaymentSettings.Username,
                AdditionalFee = _bkashAdvancePaymentSettings.AdditionalFee,
                AdditionalFeePercentage = _bkashAdvancePaymentSettings.AdditionalFeePercentage,
                Description = _bkashAdvancePaymentSettings.Description,
                UseSandbox = _bkashAdvancePaymentSettings.UseSandbox,
                EnableCapture = _bkashAdvancePaymentSettings.EnableCapture
            };
            return View("~/Plugins/Payments.bKashAdvance/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public ActionResult Configure(ConfigurationModel model)
        {
            _bkashAdvancePaymentSettings.AdditionalFee = model.AdditionalFee;
            _bkashAdvancePaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            _bkashAdvancePaymentSettings.AppKey = model.AppKey;
            _bkashAdvancePaymentSettings.AppSecret = model.AppSecret;
            _bkashAdvancePaymentSettings.Description = model.Description;
            _bkashAdvancePaymentSettings.Password = model.Password;
            _bkashAdvancePaymentSettings.Username = model.Username;
            _bkashAdvancePaymentSettings.UseSandbox = model.UseSandbox;
            _bkashAdvancePaymentSettings.EnableCapture = model.EnableCapture;

            _settingService.SaveSetting(_bkashAdvancePaymentSettings);

            return Configure();
        }

        [HttpPost]
        public ActionResult GetPayment(string id)
        {
            var result = _bkashAdvanceService.GetPayment(id);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetBalance()
        {
            var result = _bkashAdvanceService.GetBalance();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetTransaction(string id)
        {
            var result = _bkashAdvanceService.GetTransaction(id);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
