using Nop.Plugin.Widgets.EkShopA2I.Domain;
using Nop.Web.Framework;

namespace Nop.Plugin.Widgets.EkShopA2I.Models
{
    public class UdcCommissionRateModel
    {
        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.UdcCommissionRate.EntityName")]
        public string EntityName { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.UdcCommissionRate.EntityId")]
        public int EntityId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.UdcCommissionRate.EntityActive")]
        public bool EntityActive { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.UdcCommissionRate.RateMappingId")]
        public int RateMappingId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.UdcCommissionRate.Type")]
        public EntityType EntityType { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.UdcCommissionRate.Type")]
        public int EntityTypeId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.UdcCommissionRate.CommissionRate")]
        public decimal CommissionRate { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.UdcCommissionRate.CommissionRate")]
        public bool MappedCommissionRate { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.UdcCommissionRate.SearchVendorName")]
        public string SearchVendorName { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.UdcCommissionRate.SearchCategoryName")]
        public string SearchCategoryName { get; set; }

        public bool CanManageCommission { get; internal set; }
    }
}
