using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.EkShopA2I
{
    public class EkshopSettings : ISettings
    {
        public string AccessToken { get; set; }
        public string ApiKey { get; set; }
        public string Authorization { get; set; }
        public decimal UdcCommission { get; set; }

        public bool ShowUdcCommissionOnProductBox { get; set; }
        public bool ShowUdcCommissionOnProductDetails { get; set; }
        public bool EnableLog { get; set; }

        public decimal ShippingCharge { get; set; }
        public bool EnableFreeShipping { get; set; }
        public decimal MinimumCartValue { get; set; }

        public int Requests { get; set; }
    }
}
