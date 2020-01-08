using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.EblSkyPay.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("Plugins.Payment.EblSkyPay.DescriptionText")]
        public string DescriptionText { get; set; }

        [NopResourceDisplayName("Plugins.Payment.EblSkyPay.AdditionalFee")]
        public decimal AdditionalFee { get; set; }

        [NopResourceDisplayName("Plugins.Payment.EblSkyPay.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }

        [NopResourceDisplayName("Plugins.Payment.EblSkyPay.EnableLogging")]
        public bool EnableLogging { get; set; }

        [NopResourceDisplayName("Plugins.Payment.EblSkyPay.MerchantId")]
        public string MerchantId { get; set; }
        
        [NopResourceDisplayName("Plugins.Payment.EblSkyPay.MerchantName")]
        public string MerchantName { get; set; }

        [NopResourceDisplayName("Plugins.Payment.EblSkyPay.OperatorId")]
        public string OperatorId { get; set; }
        
        [NopResourceDisplayName("Plugins.Payment.EblSkyPay.Password")]
        public string Password { get; set; }

        [NopResourceDisplayName("Plugins.Payment.EblSkyPay.UseSandBox")]
        public bool UseSandBox { get; set; }

        [NopResourceDisplayName("Plugins.Payment.EblSkyPay.AutoRedirectEnable")]
        public bool AutoRedirectEnable { get; set; }

        [NopResourceDisplayName("Plugins.Payment.EblSkyPay.UseHttsForRedirection")]
        public bool UseHttsForRedirection { get; set; }

        [NopResourceDisplayName("Plugins.Payment.EblSkyPay.GatewayUrl")]
        public string GatewayUrl { get; set; }
    }
}