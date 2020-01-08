using Nop.Core;
using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Orders
{
    public class AffiliatedOrderSummary
    {
        public decimal TotalCommission { get; set; }

        public decimal PayableCommission { get; set; }

        public decimal PaidCommission { get; set; }

        public decimal UnpaidCommission { get; set; }

        public IPagedList<Order> Orders { get; set; }
    }
}
