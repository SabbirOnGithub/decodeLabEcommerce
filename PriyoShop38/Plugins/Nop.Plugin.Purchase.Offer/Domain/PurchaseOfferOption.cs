using Nop.Core;
using System.Collections.Generic;

namespace Nop.Plugin.Purchase.Offer.Domain
{
    public class PurchaseOfferOption : BaseEntity
    {
        public virtual string ProductName { get; set; }
        public int PictureId { get; set; }
        public virtual string ProductImage { get; set; }
        public virtual int Quantity { get; set; }
        public virtual decimal MinimumPurchaseAmount { get; set; }
        public virtual string Note { get; set; }
    } 
}