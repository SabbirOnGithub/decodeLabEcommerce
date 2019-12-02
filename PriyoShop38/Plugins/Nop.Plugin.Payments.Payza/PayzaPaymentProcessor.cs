using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.Payza.Controllers;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Vendors;
using Nop.Web.Framework;

namespace Nop.Plugin.Payments.Payza
{
    /// <summary>
    /// CyberSource payment processor
    /// </summary>
    public class PayzaPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly PayzaPaymentSettings _payzaPaymentSettings;
        private readonly ISettingService _settingService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IWebHelper _webHelper;
        private readonly HttpContextBase _httpContext;
        private readonly IVendorService _vendorService;

        #endregion

        #region Ctor

        public PayzaPaymentProcessor(PayzaPaymentSettings payzaPaymentSettings,
            ISettingService settingService, 
            ICurrencyService currencyService,
            CurrencySettings currencySettings,
            IWebHelper webHelper, 
            HttpContextBase httpContext,
            IVendorService vendorService)
        {
            this._vendorService = vendorService;
            this._payzaPaymentSettings = payzaPaymentSettings;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._settingService = settingService;
            this._webHelper = webHelper;
            this._httpContext=httpContext;
        }

        #endregion

        #region Utilities

        #endregion

        #region Methods

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.NewPaymentStatus = PaymentStatus.Pending;
            return result;
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            
            
            NameValueCollection collection = HostedPaymentHelper.CreatingCollection(_payzaPaymentSettings.MerchantAccount,_payzaPaymentSettings.GatewayUrl,_payzaPaymentSettings.SuccessUrl,_payzaPaymentSettings.CancelUrl,postProcessPaymentRequest.Order);
            var sb = new StringBuilder();
            sb.Append("<html>");
            sb.AppendFormat(@"<body onload='document.forms[""form""].submit()'>");
            sb.AppendFormat("<form name='form' action='{0}' method='post'>", _payzaPaymentSettings.GatewayUrl);
            foreach (var key in collection.AllKeys)
            {
                sb.AppendFormat("<input type=\"hidden\"  name=\"" + key + "\" value=\"" + collection[key] + "\"/>");
                
            }
            sb.Append("</form>");
            sb.Append("</body>");
            sb.Append("</html>");
            _httpContext.Response.Write(sb.ToString());
            _httpContext.Response.End();
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>true - hide; false - display.</returns>
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
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return false;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return 0;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return result;
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return result;
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return result;
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //CyberSource is the redirection payment method
            //It also validates whether order is also paid (after redirection) so customers will not be able to pay twice
            
            //payment status should be Pending
            if (order.PaymentStatus != PaymentStatus.Pending)
                return false;

            //let's ensure that at least 1 minute passed after order is placed
            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalMinutes < 1)
                return false;

            return true;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentPayza";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.Payza.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for payment info
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentPayza";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.Payza.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(PaymentPayzaController);
        }

        public override void Install()
        {
            var settings = new PayzaPaymentSettings()
            {
                GatewayUrl = "https://testsecureacceptance.cybersource.com/pay"
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CyberSource.RedirectionTip", "You will be redirected to CyberSource site to complete the order.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CyberSource.GatewayUrl", "Gateway URL");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CyberSource.GatewayUrl.Hint", "Enter gateway URL.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CyberSource.SecretKey", "SecretKey");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CyberSource.SecretKey.Hint", "Enter SecretKey.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CyberSource.AccessKey", "AccessKey");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CyberSource.AccessKey.Hint", "Enter AccessKey.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CyberSource.ProfileId", "ProfileId");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CyberSource.ProfileId.Hint", "Enter ProfileId.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CyberSource.AdditionalFee", "Additional fee");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CyberSource.AdditionalFee.Hint", "Enter additional fee to charge your customers.");
            
            base.Install();
        }



        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.Payments.CyberSource.RedirectionTip");
            this.DeletePluginLocaleResource("Plugins.Payments.CyberSource.GatewayUrl");
            this.DeletePluginLocaleResource("Plugins.Payments.CyberSource.GatewayUrl.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.CyberSource.SecretKey");
            this.DeletePluginLocaleResource("Plugins.Payments.CyberSource.SecretKey.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.CyberSource.AccessKey");
            this.DeletePluginLocaleResource("Plugins.Payments.CyberSource.AccessKey.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.CyberSource.ProfileId");
            this.DeletePluginLocaleResource("Plugins.Payments.CyberSource.ProfileId.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.CyberSource.AdditionalFee");
            this.DeletePluginLocaleResource("Plugins.Payments.CyberSource.AdditionalFee.Hint");
            

            base.Uninstall();
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
                return false;
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
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get
            {
                return false;
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
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo
        {
            get { return true; }
        }

        #endregion
    }
}
