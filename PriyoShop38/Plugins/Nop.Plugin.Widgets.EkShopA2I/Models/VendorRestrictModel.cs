using Nop.Web.Framework;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.EkShopA2I.Models
{
    public class VendorRestrictModel
    {
        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.UdcCommissionRate.SearchVendorName")]
        public string SearchVendorName { get; set; }
    }

    public class VendorModel
    {
        public int Id { get; set; }

        public string VendorName { get; set; }

        public bool Active { get; set; }

        public bool Restricted { get; set; }
    }
}
