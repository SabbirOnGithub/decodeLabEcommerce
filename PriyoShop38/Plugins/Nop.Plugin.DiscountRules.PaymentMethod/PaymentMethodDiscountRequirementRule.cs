using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Orders;
using System.Collections.Generic;

namespace Nop.Plugin.DiscountRules.PaymentMethod
{
    public partial class PaymentMethodDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
    {
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly IGenericAttributeService _genericAttributeService;


        public PaymentMethodDiscountRequirementRule(IStoreContext storeContext, ISettingService settingService,
            IOrderService orderService,
            IGenericAttributeService genericAttributeService)
        {
            this._storeContext = storeContext;
            this._settingService = settingService;
            this._orderService = orderService;
            this._genericAttributeService = genericAttributeService;
        }

        /// <summary>
        /// Check discount requirement
        /// </summary>
        /// <param name="request">Object that contains all information required to check the requirement (Current customer, discount, etc)</param>
        /// <returns>true - requirement is met; otherwise, false</returns>
       /* public bool CheckRequirement(CheckDiscountRequirementRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.DiscountRequirement == null)
                throw new NopException("Discount requirement is not set");

            var paymentMethodSystemName = _settingService.GetSettingByKey<string>(string.Format("DiscountRequirement.PaymentMethod-{0}", request.DiscountRequirement.Id));

            if (string.IsNullOrWhiteSpace(paymentMethodSystemName))
                return false;

            if (request.Customer == null || request.Customer.IsGuest())
                return false;

            var customerSelectedPaymentMethodSystemName = request.Customer.GetAttribute<string>(SystemCustomerAttributeNames.SelectedPaymentMethod);

            if (string.IsNullOrWhiteSpace(customerSelectedPaymentMethodSystemName))
                return false;

            return customerSelectedPaymentMethodSystemName == paymentMethodSystemName;
        }*/

        public DiscountRequirementValidationResult CheckRequirement(DiscountRequirementValidationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.DiscountRequirementId == 0)
                throw new NopException("Discount requirement is not set");

            //invalid by default
            var result = new DiscountRequirementValidationResult();

            var paymentMethodSystemName = _settingService.GetSettingByKey<string>(string.Format("DiscountRequirement.PaymentMethod-{0}", request.DiscountRequirementId));
            
            if (string.IsNullOrWhiteSpace(paymentMethodSystemName))
            {
                result.IsValid = false;
                return result;
            }                

            //if (request.Customer == null || request.Customer.IsGuest())
            //    return false;

            //var customerSelectedPaymentMethodSystemName = request.Customer.GetAttribute<string>(SystemCustomerAttributeNames.SelectedPaymentMethod);

            var customerSelectedPaymentMethodSystemName = request.Customer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod,
                    _genericAttributeService,
                    _storeContext.CurrentStore.Id);

            if (string.IsNullOrWhiteSpace(customerSelectedPaymentMethodSystemName))
            {
                result.IsValid = false;
                return result;
            }

            if (customerSelectedPaymentMethodSystemName == paymentMethodSystemName)
            {
                //valid
                result.IsValid = true;
                return result;
            }

            return result;
        }

        /// <summary>
        /// Get URL for rule configuration
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <param name="discountRequirementId">Discount requirement identifier (if editing)</param>
        /// <returns>URL</returns>
        public string GetConfigurationUrl(int discountId, int? discountRequirementId)
        {
            //configured in RouteProvider.cs
            string result = "Plugins/DiscountRulesPaymentMethod/Configure/?discountId=" + discountId;
            if (discountRequirementId.HasValue)
                result += string.Format("&discountRequirementId={0}", discountRequirementId.Value);
            return result;
        }

        public override void Install()
        {
            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.PaymentMethod.Fields.SelectPaymentMethod", "Select Payment Method");
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.PaymentMethod.Fields.Method", "Payment Method to be discounted");
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.PaymentMethod.Fields.Method.Hint", "Discount will be applied if customer selected this payment method.");
            base.Install();
        }

        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.DiscountRules.PaymentMethod.Fields.SelectPaymentMethod");
            this.DeletePluginLocaleResource("Plugins.DiscountRules.PaymentMethod.Fields.Method");
            this.DeletePluginLocaleResource("Plugins.DiscountRules.PaymentMethod.Fields.Method.Hint");
            base.Uninstall();
        }
    }
}