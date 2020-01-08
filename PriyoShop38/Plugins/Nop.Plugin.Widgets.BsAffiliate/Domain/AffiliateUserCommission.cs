using Nop.Core;
using System;

namespace Nop.Plugin.Widgets.BsAffiliate.Domain
{
    public partial class AffiliateUserCommission : BaseEntity
    {
        public decimal CommissionRate { get; set; }

        public CommissionType CommissionType { get; set; }

        public int AffiliateId { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime UpdatedOnUtc { get; set; }
    }
}
