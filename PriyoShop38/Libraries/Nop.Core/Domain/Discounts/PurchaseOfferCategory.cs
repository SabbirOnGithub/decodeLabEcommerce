using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Discounts
{
    public partial class PurchaseOfferCategory : BaseEntity
    {
        public int CategoryId { get; set; }

        public int PurchaseOfferId { get; set; }
        

        public virtual PurchaseOffer PurchaseOffer { get; set; }

        public virtual Category Category { get; set; }
    }
}
