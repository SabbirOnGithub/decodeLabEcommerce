using Nop.Core;

namespace Nop.Plugin.Widgets.EkShopA2I.Domain
{
    public class EsUdcCommissionRate : BaseEntity
    {
        public int EntityId { get; set; }
        public EntityType Type { get; set; }
        public decimal CommissionRate { get; set; }
    } 
}
