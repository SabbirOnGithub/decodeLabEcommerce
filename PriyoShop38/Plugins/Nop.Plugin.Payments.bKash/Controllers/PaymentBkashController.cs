using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Plugin.Payments.Bkash.Models;
using Nop.Services.Configuration;
using Nop.Services.Payments;
using Nop.Web.Framework.Controllers;
using Nop.Core;
using Nop.Services.Stores;

namespace Nop.Plugin.Payments.Bkash.Controllers
{
    public class PaymentBkashController : BasePaymentController
    {
        #region Field        
        private readonly ISettingService _settingService;
        private readonly BkashPaymentSettings _bkashPaymentSettings;

        #endregion

        #region Ctr

        public PaymentBkashController(ISettingService settingService, BkashPaymentSettings bkashPaymentSettings)
        {            
            this._settingService = settingService;
            this._bkashPaymentSettings = bkashPaymentSettings;
        }

        #endregion

        #region Payment Configuration

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel
            {
                DescriptionText = _bkashPaymentSettings.DescriptionText,
                AdditionalFee = _bkashPaymentSettings.AdditionalFee,
                AdditionalFeePercentage = _bkashPaymentSettings.AdditionalFeePercentage,
                BkashPaymentPhoneNumber = _bkashPaymentSettings.BkashPaymentPhoneNumber,
                BkashSendMoneyPhoneNumber = _bkashPaymentSettings.BkashSendMoneyPhoneNumber,
                CustomerSupportPhoneNumber = _bkashPaymentSettings.CustomerSupportPhoneNumber,
                DialingNumber = _bkashPaymentSettings.DialingNumber
            };

            return View("~/Plugins/Payments.bKash/Views/PaymentBkash/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _bkashPaymentSettings.DescriptionText = model.DescriptionText;
            _bkashPaymentSettings.AdditionalFee = model.AdditionalFee;
            _bkashPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            _bkashPaymentSettings.BkashPaymentPhoneNumber = model.BkashPaymentPhoneNumber;
            _bkashPaymentSettings.DialingNumber = model.DialingNumber;
            _bkashPaymentSettings.BkashSendMoneyPhoneNumber = model.BkashSendMoneyPhoneNumber;
            _bkashPaymentSettings.CustomerSupportPhoneNumber = model.CustomerSupportPhoneNumber;

            _settingService.SaveSetting(_bkashPaymentSettings);            

            //now clear settings cache
            _settingService.ClearCache();

            return View("~/Plugins/Payments.bKash/Views/PaymentBkash/Configure.cshtml", model);
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            var model = new PaymentInfoModel()
            {
                DescriptionText = _bkashPaymentSettings.DescriptionText
            };

            return View("~/Plugins/Payments.bKash/Views/PaymentBkash/PaymentInfo.cshtml", model);
        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();
            return warnings;
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();
            return paymentInfo;
        }

        #endregion

        //1 min
        [OutputCache(Duration = 1 * 60)]
        public ActionResult HowToPay()
        {
            var model = new BkashInfoModel()
            {
                DialNumber = _bkashPaymentSettings.DialingNumber,
                BkashPaymentPhoneNumber = _bkashPaymentSettings.BkashPaymentPhoneNumber,
                CustomerSupportPhoneNumber = _bkashPaymentSettings.CustomerSupportPhoneNumber,
                BkashSendMoneyPhoneNumber = _bkashPaymentSettings.BkashSendMoneyPhoneNumber,
            };
            
            return View("~/Plugins/Payments.bKash/Views/PaymentBkash/HowToPay.cshtml", model);
        }

        public ActionResult Settings()
        {
            var model = new ConfigurationModel
            {
                DescriptionText = _bkashPaymentSettings.DescriptionText,
                AdditionalFee = _bkashPaymentSettings.AdditionalFee,
                AdditionalFeePercentage = _bkashPaymentSettings.AdditionalFeePercentage,
                DialingNumber = _bkashPaymentSettings.DialingNumber,
                CustomerSupportPhoneNumber = _bkashPaymentSettings.CustomerSupportPhoneNumber,

                BkashPaymentPhoneNumber = _bkashPaymentSettings.BkashPaymentPhoneNumber,
                BkashSendMoneyPhoneNumber = _bkashPaymentSettings.BkashSendMoneyPhoneNumber,

            };

            return View("~/Plugins/Payments.bKash/Views/PaymentBkash/Settings.cshtml", model);
        }
        [HttpPost]
        [AdminAuthorize]
        public ActionResult Settings(ConfigurationModel model)
        {
            if (ModelState.IsValid)
            {
                //save settings
                _bkashPaymentSettings.DescriptionText = model.DescriptionText;
                _bkashPaymentSettings.AdditionalFee = model.AdditionalFee;
                _bkashPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;

                _bkashPaymentSettings.DialingNumber = model.DialingNumber;
                _bkashPaymentSettings.CustomerSupportPhoneNumber = model.CustomerSupportPhoneNumber;

                _bkashPaymentSettings.BkashSendMoneyPhoneNumber = model.BkashSendMoneyPhoneNumber;
                _bkashPaymentSettings.BkashPaymentPhoneNumber = model.BkashPaymentPhoneNumber;

                _settingService.SaveSetting(_bkashPaymentSettings);
            }
            return View("~/Plugins/Payments.bKash/Views/PaymentBkash/Settings.cshtml", model);
        }
        public ActionResult BkashPaymentsPage()
        {
            return View("~/Plugins/Payments.bKash/Views/PaymentBkash/BkashPaymentsPage.cshtml");
        }
    }
}