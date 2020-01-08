using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Discounts
{
    public partial class PurchaseOfferUsageHistory : BaseEntity
    {
        public int PurchaseOfferId { get; set; }
        
        public DateTime CreatedOnUtc { get; set; }

        public int GiftProductId { get; set; }

        public int Quantity { get; set; }


        public virtual PurchaseOffer PurchaseOffer { get; set; }
        
        public virtual Order Order { get; set; }

        public virtual Product GiftProduct { get; set; }
    }
}
