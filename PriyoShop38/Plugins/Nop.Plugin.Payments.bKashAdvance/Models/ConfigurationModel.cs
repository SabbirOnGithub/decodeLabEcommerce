using Nop.Web.Framework;

namespace Nop.Plugin.Payments.bKashAdvance.Models
{
    public class ConfigurationModel
    {
        [NopResourceDisplayName("Plugins.Payment.BkashAdvance.AppKey")]
        public string AppKey { get; set; }
        [NopResourceDisplayName("Plugins.Payment.BkashAdvance.AppSecret")]
        public string AppSecret { get; set; }
        [NopResourceDisplayName("Plugins.Payment.BkashAdvance.Username")]
        public string Username { get; set; }
        [NopResourceDisplayName("Plugins.Payment.BkashAdvance.Password")]
        public string Password { get; set; }
        [NopResourceDisplayName("Plugins.Payment.BkashAdvance.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        [NopResourceDisplayName("Plugins.Payment.BkashAdvance.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        [NopResourceDisplayName("Plugins.Payment.BkashAdvance.Description")]
        public string Description { get; set; }
        [NopResourceDisplayName("Plugins.Payment.BkashAdvance.UseSandbox")]
        public bool UseSandbox { get; set; }
        [NopResourceDisplayName("Plugins.Payment.BkashAdvance.EnableCapture")]
        public bool EnableCapture { get; set; }
    }
}
