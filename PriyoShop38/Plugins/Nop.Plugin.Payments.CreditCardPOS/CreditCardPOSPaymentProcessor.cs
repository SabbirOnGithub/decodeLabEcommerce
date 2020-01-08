using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.CreditCardPOS.Controllers;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Vendors;

namespace Nop.Plugin.Payments.CreditCardPOS
{
    /// <summary>
    /// CreditCardPOS payment processor
    /// </summary>
    public class CreditCardPOSPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields
        
        private readonly ISettingService _settingService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly CreditCardPOSPaymentSettings _creditCardPosPaymentSettings;
        private readonly IVendorService _vendorService;

        #endregion

        #region Ctor

        public CreditCardPOSPaymentProcessor(ISettingService settingService, 
            IOrderTotalCalculationService orderTotalCalculationService,
            CreditCardPOSPaymentSettings creditCardPosPaymentSettings,
            IVendorService vendorService)
        {
            this._vendorService = vendorService;
            this._settingService = settingService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._creditCardPosPaymentSettings = creditCardPosPaymentSettings;
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
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country

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

            return _creditCardPosPaymentSettings.ShippableProductRequired && !cart.RequiresShipping();
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            var result = this.CalculateAdditionalFee(_orderTotalCalculationService, cart,
                _creditCardPosPaymentSettings.AdditionalFee, _creditCardPosPaymentSettings.AdditionalFeePercentage);

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
            controllerName = "PaymentCreditCardPOS";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.CreditCardPOS.Controllers" }, { "area", null } };
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
            controllerName = "PaymentCreditCardPOS";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.CreditCardPOS.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(PaymentCreditCardPOSController);
        }

        public override void Install()
        {
            var settings = new CreditCardPOSPaymentSettings
            {
                DescriptionText = "<p>In cases where an order is placed, an authorized representative will contact you, personally or over telephone, to confirm the order.<br />After the order is confirmed, it will be processed.<br />Orders once confirmed, cannot be cancelled.</p><p>P.S. You can edit this text from admin panel.</p>"
            };

            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.CreditCardPOS.DescriptionText", "Description");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.CreditCardPOS.DescriptionText.Hint", "Enter info that will be shown to customers during checkout");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.CreditCardPOS.AdditionalFee", "Additional fee");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.CreditCardPOS.AdditionalFee.Hint", "The additional fee.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.CreditCardPOS.AdditionalFeePercentage", "Additional fee. Use percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.CreditCardPOS.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.CreditCardPOS.ShippableProductRequired", "Shippable product required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.CreditCardPOS.ShippableProductRequired.Hint", "An option indicating whether shippable products are required in order to display this payment method during checkout.");

            
            base.Install();
        }
        
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<CreditCardPOSPaymentSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Payment.CreditCardPOS.DescriptionText");
            this.DeletePluginLocaleResource("Plugins.Payment.CreditCardPOS.DescriptionText.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.CreditCardPOS.AdditionalFee");
            this.DeletePluginLocaleResource("Plugins.Payment.CreditCardPOS.AdditionalFee.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.CreditCardPOS.AdditionalFeePercentage");
            this.DeletePluginLocaleResource("Plugins.Payment.CreditCardPOS.AdditionalFeePercentage.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.CreditCardPOS.ShippableProductRequired");
            this.DeletePluginLocaleResource("Plugins.Payment.CreditCardPOS.ShippableProductRequired.Hint");
            
            base.Uninstall();
        }

        public void PostProcessMakePayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var result = new ProcessPaymentResult { NewPaymentStatus = PaymentStatus.Pending };


           
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
