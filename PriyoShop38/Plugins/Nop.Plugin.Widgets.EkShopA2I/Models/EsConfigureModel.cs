using Nop.Core.Domain.Customers;
using Nop.Web.Framework;

namespace Nop.Plugin.Widgets.EkShopA2I.Models
{
    public class EsConfigureModel
    {
        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.AccessToken")]
        public string AccessToken { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.ApiKey")]
        public string ApiKey { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.Authorization")]
        public string Authorization { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.UdcCommission")]
        public decimal UdcCommission { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.Configure.ShowUdcCommissionOnProductBox")]
        public bool ShowUdcCommissionOnProductBox { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.Configure.ShowUdcCommissionOnProductDetails")]
        public bool ShowUdcCommissionOnProductDetails { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.Configure.EnableLog")]
        public bool EnableLog { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.Configure.ShippingCharge")]
        public decimal ShippingCharge { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.Configure.EnableFreeShipping")]
        public bool EnableFreeShipping { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.Configure.MinimumCartValue")]
        public decimal MinimumCartValue { get; set; }
    }
}
