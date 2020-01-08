using Nop.Core;

namespace Nop.Plugin.Widgets.BsAffiliate.Domain
{
    public class AffiliateCommissionRate : BaseEntity
    {
        public int EntityId { get; set; }

        public EntityType EntityType { get; set; }

        public decimal CommissionRate { get; set; }

        public CommissionType CommissionType { get; set; }
    } 
}
