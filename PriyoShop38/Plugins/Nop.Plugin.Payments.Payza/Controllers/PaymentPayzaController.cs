using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Payza.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.Payza.Controllers
{
    public class PaymentPayzaController : BasePaymentController
    {
        private readonly ISettingService _settingService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly PayzaPaymentSettings _payzaPaymentSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly ILocalizationService _localizationService;

        public PaymentPayzaController(ISettingService settingService,
            IPaymentService paymentService, IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            PayzaPaymentSettings payzaPaymentSettings,
            PaymentSettings paymentSettings,
            ILocalizationService localizationService)
        {
            this._settingService = settingService;
            this._paymentService = paymentService;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._payzaPaymentSettings = payzaPaymentSettings;
            this._paymentSettings = paymentSettings;
            this._localizationService = localizationService;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel();
            model.GatewayUrl = _payzaPaymentSettings.GatewayUrl;
            model.SuccessUrl = _payzaPaymentSettings.SuccessUrl;
            model.CancelUrl = _payzaPaymentSettings.CancelUrl;
            model.MerchantAccount = _payzaPaymentSettings.MerchantAccount;

            return View("~/Plugins/Payments.Payza/Views/PaymentPayza/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _payzaPaymentSettings.GatewayUrl = model.GatewayUrl;
            _payzaPaymentSettings.SuccessUrl = model.SuccessUrl;
            _payzaPaymentSettings.CancelUrl = model.CancelUrl;
            _payzaPaymentSettings.MerchantAccount = model.MerchantAccount;
            _settingService.SaveSetting(_payzaPaymentSettings);

            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            var model = new PaymentInfoModel();

            return Content(_localizationService.GetResource("Plugins.Payments.Payza.RedirectionTip"));
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

        [ValidateInput(false)]
        public ActionResult IPNHandler(FormCollection form)
        {
 
            var sb = new StringBuilder();
            sb.AppendLine("Response: Payza");
           
            
            foreach (var key in form.AllKeys)
            {
                sb.AppendLine(key+":" + form[key]);
            }
            var orderId = 0;
            bool getorderId= int.TryParse(form["apc_1"], out orderId);
            Order order=null;
            if (getorderId)
            {
                order= _orderService.GetOrderById(orderId);
            }
            if (order != null)
            {
                //order note
                order.OrderNotes.Add(new OrderNote
                {
                    Note = sb.ToString(),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _orderService.UpdateOrder(order);
                decimal totalAmount = 0;
                var status = form["ap_status"];
                bool getTotalAmount = decimal.TryParse(form["ap_totalamount"], out totalAmount);
                
                if(status!=null && getTotalAmount && (totalAmount==order.OrderTotal) && _orderProcessingService.CanMarkOrderAsPaid(order))
                {
                    _orderProcessingService.MarkOrderAsPaid(order);
                }
            }
            
            return null;

        }
       
        public ActionResult Payza()
        {
            return View("~/Plugins/Payments.Payza/Views/PaymentPayza/Payza.cshtml");
        }
    }
}