using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.NexusPay.Controllers;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Vendors;

namespace Nop.Plugin.Payments.NexusPay
{
    /// <summary>
    /// NexusPay payment processor
    /// </summary>
    public class NexusPayPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields
        
        private readonly ISettingService _settingService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly NexusPayPaymentSettings _NexusPayPaymentSettings;
        private readonly IVendorService _vendorService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public NexusPayPaymentProcessor(ISettingService settingService, 
            IOrderTotalCalculationService orderTotalCalculationService,
            NexusPayPaymentSettings NexusPayPaymentSettings,
            IVendorService vendorService,
            ILogger logger)
        {
            this._settingService = settingService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._NexusPayPaymentSettings = NexusPayPaymentSettings;
            this._vendorService = vendorService;
            _logger = logger;
        }

        #endregion
        
        #region Methods
        
        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult {NewPaymentStatus = PaymentStatus.Pending};


            return result;
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //nothing
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
            return _NexusPayPaymentSettings.ShippableProductRequired && !cart.RequiresShipping();
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            var result = this.CalculateAdditionalFee(_orderTotalCalculationService, cart,
                _NexusPayPaymentSettings.AdditionalFee, _NexusPayPaymentSettings.AdditionalFeePercentage);

            return result;
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

            //it's not a redirection payment method. So we always return false
            return false;
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
            controllerName = "PaymentNexusPay";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.NexusPay.Controllers" }, { "area", null } };
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
            controllerName = "PaymentNexusPay";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.NexusPay.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(PaymentNexusPayController);
        }

        public override void Install()
        {
            var settings = new NexusPayPaymentSettings
            {
                DescriptionText = "<p>In cases where an order is placed, an authorized representative will contact you, personally or over telephone, to confirm the order.<br />After the order is confirmed, it will be processed.<br />Orders once confirmed, cannot be cancelled.</p><p>P.S. You can edit this text from admin panel.</p>"
            };

            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.NexusPay.DescriptionText", "Description");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.NexusPay.DescriptionText.Hint", "Enter info that will be shown to customers during checkout");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.NexusPay.AdditionalFee", "Additional fee");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.NexusPay.AdditionalFee.Hint", "The additional fee.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.NexusPay.AdditionalFeePercentage", "Additional fee. Use percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.NexusPay.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.NexusPay.ShippableProductRequired", "Shippable product required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.NexusPay.ShippableProductRequired.Hint", "An option indicating whether shippable products are required in order to display this payment method during checkout.");

            
            base.Install();
        }
        
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<NexusPayPaymentSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Payment.NexusPay.DescriptionText");
            this.DeletePluginLocaleResource("Plugins.Payment.NexusPay.DescriptionText.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.NexusPay.AdditionalFee");
            this.DeletePluginLocaleResource("Plugins.Payment.NexusPay.AdditionalFee.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.NexusPay.AdditionalFeePercentage");
            this.DeletePluginLocaleResource("Plugins.Payment.NexusPay.AdditionalFeePercentage.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.NexusPay.ShippableProductRequired");
            this.DeletePluginLocaleResource("Plugins.Payment.NexusPay.ShippableProductRequired.Hint");
            
            base.Uninstall();
        }

        public void PostProcessMakePayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //nothing
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
                return PaymentMethodType.Standard;
            }
        }

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo
        {
            get
            {
                return false;
            }
        }

        #endregion
        
    }
}
