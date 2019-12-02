using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.Dmoney.Models;
using Nop.Plugin.Payments.Dmoney.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Plugin.Payments.Dmoney.Controllers
{
    public class PaymentDmoneyController : BasePaymentController
    {
        private readonly ILogger _logger;
        private readonly DmoneyPaymentSettings _dmoneyPaymentSettings;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly IDmoneyPaymentService _dmoneyPaymentService;
        private readonly ILocalizationService _localizationService;
        private readonly IDmoneyTransactionService _dmoneyTransactionService;

        public PaymentDmoneyController(ILogger logger,
            DmoneyPaymentSettings dmoneyPaymentSettings,
            ISettingService settingService,
            IOrderService orderService,
            IDmoneyPaymentService dmoneyPaymentService,
            ILocalizationService localizationService,
            IDmoneyTransactionService dmoneyTransactionService)
        {
            this._logger = logger;
            this._dmoneyPaymentSettings = dmoneyPaymentSettings;
            this._settingService = settingService;
            this._orderService = orderService;
            this._dmoneyPaymentService = dmoneyPaymentService;
            this._localizationService = localizationService;
            this._dmoneyTransactionService = dmoneyTransactionService;
        }

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

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel()
            {
                BillerCode = _dmoneyPaymentSettings.BillerCode,
                EnableLog = _dmoneyPaymentSettings.EnableLog,
                GatewayUrl = _dmoneyPaymentSettings.GatewayUrl,
                OrgCode = _dmoneyPaymentSettings.OrgCode,
                Password = _dmoneyPaymentSettings.Password,
                SecretKey = _dmoneyPaymentSettings.SecretKey,
                TransactionVerificationUrl = _dmoneyPaymentSettings.TransactionVerificationUrl
            };

            return View("~/Plugins/Payments.Dmoney/Views/PaymentDmoney/Configure.cshtml", model);
        }


        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();
            
            _dmoneyPaymentSettings.BillerCode = model.BillerCode;
            _dmoneyPaymentSettings.EnableLog = model.EnableLog;
            _dmoneyPaymentSettings.GatewayUrl = model.GatewayUrl;
            _dmoneyPaymentSettings.OrgCode = model.OrgCode;
            _dmoneyPaymentSettings.Password = model.Password;
            _dmoneyPaymentSettings.SecretKey = model.SecretKey;
            _dmoneyPaymentSettings.TransactionVerificationUrl = model.TransactionVerificationUrl;

            _settingService.SaveSetting(_dmoneyPaymentSettings);

            return Configure();
        }

        public ActionResult Approve(string transactionTrackingNo)
        {
            _dmoneyPaymentService.CheckPaymentTransactionStatus(transactionTrackingNo);
            SuccessNotification(_localizationService.GetResource("Plugins.Payments.Dmoney.ApproveMessase"));
            return RedirectToRoute("HomePage");
        }

        public ActionResult Cancel(string transactionTrackingNo)
        {
            ErrorNotification(_localizationService.GetResource("Plugins.Payments.Dmoney.CancelMessase"));
            return RedirectToRoute("HomePage");
        }

        public ActionResult Decline(string transactionTrackingNo)
        {
            ErrorNotification(_localizationService.GetResource("Plugins.Payments.Dmoney.DeclineMessase"));
            return RedirectToRoute("HomePage");
        }
    }
}
