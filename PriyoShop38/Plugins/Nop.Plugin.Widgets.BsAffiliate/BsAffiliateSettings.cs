using Nop.Core.Configuration;
using Nop.Plugin.Widgets.BsAffiliate.Domain;

namespace Nop.Plugin.Widgets.BsAffiliate
{
    public class BsAffiliateSettings : ISettings
    {
        public decimal DefaultCommission { get; set; }

        public CommissionType CommissionType { get; set; }
    }
}
