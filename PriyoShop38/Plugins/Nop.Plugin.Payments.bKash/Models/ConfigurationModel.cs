using System.Web.Mvc;
using Nop.Web.Framework;

namespace Nop.Plugin.Payments.Bkash.Models
{
    public class ConfigurationModel
    {
        [AllowHtml]
        [NopResourceDisplayName("Plugins.Payment.Bkash.DescriptionText")]
        public string DescriptionText { get; set; }

        [NopResourceDisplayName("Plugins.Payment.Bkash.AdditionalFee")]
        public decimal AdditionalFee { get; set; }

        [NopResourceDisplayName("Plugins.Payment.Bkash.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }        

        [NopResourceDisplayName("Plugins.Payment.Bkash.BkashSendMoneyPhoneNumber")]
        public string BkashSendMoneyPhoneNumber { get; set; }
        [NopResourceDisplayName("Plugins.Payment.Bkash.BkashPaymentPhoneNumber")]
        public string BkashPaymentPhoneNumber { get; set; }

        [NopResourceDisplayName("Plugins.Payment.Bkash.CustomerSupport")]
        public string CustomerSupportPhoneNumber { get; set; }

        [NopResourceDisplayName("Plugins.Payment.Bkash.DialNumber")]
        public string DialingNumber { get; set; }
    }
}