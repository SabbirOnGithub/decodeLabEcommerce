using Nop.Core.Domain.Discounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Discounts
{
    public class PurchaseOfferMap : NopEntityTypeConfiguration<PurchaseOffer>
    {
        public PurchaseOfferMap()
        {
            this.ToTable("PurchaseOffer");
            this.HasKey(po => po.Id);

            this.HasRequired(poc => poc.GiftProduct)
                .WithMany()
                .HasForeignKey(poc => poc.GiftProductId);
        }
    }
}
