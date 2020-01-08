using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Discounts
{
    public partial class PurchaseOfferManufacturer : BaseEntity
    {
        public int ManufacturerId { get; set; }

        public int PurchaseOfferId { get; set; }
        

        public virtual PurchaseOffer PurchaseOffer { get; set; }

        public virtual Manufacturer Manufacturer { get; set; }
    }
}
