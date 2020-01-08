using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Customers
{ 
    public class CustomerCommission: BaseEntity
    {
        public int? CustomerId { get; set; }
        public decimal? TotalCommission { get; set; }
        public decimal? TotalWithDrawn { get; set; }
        public decimal? TotalBalance { get; set; }
    }
}
