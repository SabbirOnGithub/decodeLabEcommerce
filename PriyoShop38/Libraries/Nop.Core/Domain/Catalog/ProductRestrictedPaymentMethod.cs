using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Catalog
{
    public partial class ProductRestrictedPaymentMethod : BaseEntity
    {
        public string SystemName { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public virtual Product Product { get; set; }
    }
}
