using Nop.Core.Domain.Vendors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Discounts
{
    public partial class PurchaseOfferVendor : BaseEntity
    {
        public int VendorId { get; set; }

        public int PurchaseOfferId { get; set; }

        public virtual PurchaseOffer PurchaseOffer { get; set; }

        public virtual Vendor Vendor { get; set; }
    }
}
