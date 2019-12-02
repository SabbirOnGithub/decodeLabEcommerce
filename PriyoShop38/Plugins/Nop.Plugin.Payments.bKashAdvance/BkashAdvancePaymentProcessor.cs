using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.bKashAdvance.Controllers;
using Nop.Plugin.Payments.bKashAdvance.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Vendors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Nop.Plugin.Payments.bKashAdvance
{
    public class BkashAdvancePaymentProcessor : BasePlugin, IPaymentMethod
    {

        #region Fields

        private readonly BkashAdvancePaymentSettings _bkashAdvancePaymentSettings;
        private readonly IBkashAdvanceService _bkashAdvanceService;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ILocalizationService _localizationService;
        private readonly IGenericAttributeService _gaService;
        private readonly IVendorService _vendorService;

        #endregion

        #region Ctor

        public BkashAdvancePaymentProcessor(BkashAdvancePaymentSettings bkashAdvancePaymentSettings, 
            ISettingService settingService, 
            IOrderTotalCalculationService orderTotalCalculationService, 
            ILocalizationService localizationService,
            IGenericAttributeService gaService,
            IWorkContext workContext,
            IBkashAdvanceService bkashAdvanceService,
            IVendorService vendorService)
        {
            this._bkashAdvancePaymentSettings = bkashAdvancePaymentSettings;
            this._settingService = settingService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            _localizationService = localizationService;
            _gaService = gaService;
            _workContext = workContext;
            _bkashAdvanceService = bkashAdvanceService;
            _vendorService = vendorService;
        }

        #endregion

        #region Properies

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get
            {
                return _bkashAdvancePaymentSettings.EnableCapture;
            }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get
            {
                return _bkashAdvancePaymentSettings.EnableRefund;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get
            {
                return _bkashAdvancePaymentSettings.EnableVoid;
            }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return RecurringPaymentType.NotSupported;
            }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Redirection;
            }
        }

        /// <summary>
        /// Skip payment info during checkout
        /// </summary>
        public bool SkipPaymentInfo
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Methods
        
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult { NewPaymentStatus = PaymentStatus.Pending };
            return result;
        }

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            HttpContext.Current.Response.RedirectToRoute("BkashPay", new { orderId = postProcessPaymentRequest.Order.Id, customerId = _workContext.CurrentCustomer.Id });
        }

        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            foreach (var item in cart)
            {
                if (item.Product.RestrictedPaymentMethods.Any(x => string.Equals(x.SystemName, this.PluginDescriptor.SystemName, StringComparison.OrdinalIgnoreCase)))
                    return true;

                var vendor = _vendorService.GetVendorById(item.Product.VendorId);
                if (vendor != null)
                {
                    if (vendor.RestrictedPaymentMethods.Any(x => string.Equals(x.SystemName, this.PluginDescriptor.SystemName, StringComparison.OrdinalIgnoreCase)))
                        return true;
                }
            }
            return false;
        }
        
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            var result = this.CalculateAdditionalFee(_orderTotalCalculationService, cart,
                _bkashAdvancePaymentSettings.AdditionalFee, _bkashAdvancePaymentSettings.AdditionalFeePercentage);
            return result;
        }

        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {

            var result = new CapturePaymentResult();
            if (!SupportCapture)
                result.AddError("Capture method not supported");
            else
                result = _bkashAdvanceService.CapturePayment(capturePaymentRequest.Order);
            return result;
        }

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();

            if (!SupportRefund)
                result.AddError("Refund method not supported");
            else
                result = _bkashAdvanceService.RefundPayment(refundPaymentRequest.Order);
            return result;
        }
        
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();

            if (!SupportRefund)
                result.AddError("Void method not supported");
            else
                result = _bkashAdvanceService.VoidPayment(voidPaymentRequest.Order);
            return result;
        }
        
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");
            
            return false;
        }
        
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "BkashAdvance";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.bKashAdvance.Controllers" }, { "area", null } };
        }
        
        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "BkashAdvance";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.bKashAdvance.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(BkashAdvanceController);
        }

        public override void Install()
        {
            var settings = new BkashAdvancePaymentSettings()
            {
                AppKey = "App key",
                AppSecret = "App secret",
                Username = "Username",
                Password = "Password",
                AdditionalFee  = 0,
                AdditionalFeePercentage = false,
                Description = "Description text here",
                UseSandbox = true
            };
            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.AppKey", "App Key");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.AppKey.Hint", "Enter app key");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.AppSecret", "App Secret");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.AppSecret.Hint", "The app secret.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.Username", "Username");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.Username.Hint", "Enter username");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.Password", "Password");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.Password.Hint", "Enter password");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.AdditionalFee", "Additional Fee");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.AdditionalFee.Hint", "Enter additional fee");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.AdditionalFeePercentage", "Additional Fee Percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.AdditionalFeePercentage.Hint", "Additional Fee Percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.Description", "Description");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.Description.Hint", "Enter description.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.BaseUrl", "Base Url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.BaseUrl.Hint", "Enter Base Url.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.UseSandbox", "Use Sandbox");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.UseSandbox.Hint", "Use Sandbox");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.EnableCapture", "Enable Capture");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.EnableCapture.Hint", "Enable Capture");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.BaseUrl.Hint", "Enter Base Url.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.EnableRefund", "Enable Refund");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.EnableRefund.Hint", "Enable Refund");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.EnableVoid", "Enable Void");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.BkashAdvance.EnableVoid.Hint", "Enable Void");

            base.Install();
        }

        public override void Uninstall()
        {
            _settingService.DeleteSetting<BkashAdvancePaymentSettings>();

            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.AppKey");
            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.AppKey.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.AppSecret");
            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.AppSecret.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.Username");
            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.Username.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.Password");
            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.Password.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.AdditionalFee");
            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.AdditionalFee.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.AdditionalFeePercentage");
            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.AdditionalFeePercentage.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.Description");
            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.Description.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.BaseUrl");
            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.BaseUrl.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.UseSandbox");
            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.UseSandbox.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.EnableCapture");
            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.EnableCapture.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.EnableRefund");
            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.EnableRefund.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.EnableVoid");
            this.DeletePluginLocaleResource("Plugins.Payment.BkashAdvance.EnableVoid.Hint");

            base.Uninstall();
        }

        public void PostProcessMakePayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            HttpContext.Current.Response.RedirectToRoute("BkashPay", new { orderId = postProcessPaymentRequest.Order.Id, customerId = _workContext.CurrentCustomer.Id });
        }

        #endregion

    }
}
