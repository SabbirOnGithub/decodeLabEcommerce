using FluentValidation.Attributes;
using Nop.Plugin.Payments.Dmoney.Infrastructure;
using Nop.Web.Framework;

namespace Nop.Plugin.Payments.Dmoney.Models
{
    [Validator(typeof(ConfigurationValidator))]
    public class ConfigurationModel
    {
        [NopResourceDisplayName("Plugins.Payments.Dmoney.GatewayUrl")]
        public string GatewayUrl { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Dmoney.TransactionVerificationUrl")]
        public string TransactionVerificationUrl { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Dmoney.OrgCode")]
        public string OrgCode { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Dmoney.Password")]
        public string Password { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Dmoney.SecretKey")]
        public string SecretKey { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Dmoney.BillerCode")]
        public string BillerCode { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Dmoney.EnableLog")]
        public bool EnableLog { get; set; }
    }
}
