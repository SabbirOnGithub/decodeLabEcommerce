using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Customers
{
    public partial class CustomerPriyoCoin : BaseEntity
    {
        public int CustomerId { get; set; }
        public int? PointNumber { get; set; }
        public string PointSourceDescription { get; set; }
        public string PointType { get; set; }
        public DateTime? Date { get; set; }
        public virtual Customer Customer { get; set; }

    }
}
