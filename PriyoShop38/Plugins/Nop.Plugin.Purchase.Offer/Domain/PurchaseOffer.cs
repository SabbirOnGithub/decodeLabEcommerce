using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Purchase.Offer.Domain
{
    public class PurchaseOffer : BaseEntity
    {
        public virtual string Name { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual DateTime ValidFrom { get; set; }
        public virtual DateTime ValidTo { get; set; }
        public virtual DateTime CreatedOnUtc { get; set; } 
        public virtual DateTime UpdatedOnUtc { get; set; }
        public virtual string Description { get; set; }
        public virtual bool ShowNotificationOnCart { get; set; }
    }
}
