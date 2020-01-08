using Nop.Plugin.Widgets.BsAffiliate.Domain;
using Nop.Web.Framework;

namespace Nop.Plugin.Widgets.BsAffiliate.Models
{
    public class AffiliateConfigureModel
    {
        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateConfigureModel.DefaultCommission")]
        public decimal DefaultCommission { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateConfigureModel.CommissionType")]
        public CommissionType CommissionType { get; set; }
    }
}
