using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Vendors
{
    public partial class VendorRestrictedPaymentMethod : BaseEntity
    {
        public string SystemName { get; set; }
        public int VendorId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public virtual Vendor Vendor { get; set; }
    }
}
