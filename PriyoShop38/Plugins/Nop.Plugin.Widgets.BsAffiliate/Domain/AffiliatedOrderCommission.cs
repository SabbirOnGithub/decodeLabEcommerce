using Nop.Core;
using System;

namespace Nop.Plugin.Widgets.BsAffiliate.Domain
{
    public partial class AffiliatedOrderCommission : BaseEntity
    {
        public int OrderId { get; set; }

        public int AffiliateId { get; set; }

        public decimal TotalCommission { get; set; }

        public CommissionPaymentStatus PaymentStatus { get; set; } 

        public DateTime? MarkedAsPaidOn { get; set; }
    }
}
