using System.ComponentModel;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.Payza.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Payments.Payza.GatewayUrl")]
        public string GatewayUrl { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Payza.MerchantAccount")]
        public string MerchantAccount { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Payza.SuccessUrl")]
        public string SuccessUrl { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Payza.CancelUrl")]
        public string CancelUrl { get; set; }
    }
}