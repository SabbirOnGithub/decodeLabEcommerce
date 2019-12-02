using Nop.Core.Domain.Discounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Discounts
{
    public partial class PurchaseOfferProductMap : NopEntityTypeConfiguration<PurchaseOfferProduct>
    {
        public PurchaseOfferProductMap()
        {
            this.ToTable("PurchaseOffer_AppliedToProducts");
            this.HasKey(pop => pop.Id);

            this.HasRequired(pop => pop.PurchaseOffer)
                .WithMany(po => po.AppliedToProducts)
                .HasForeignKey(pop => pop.PurchaseOfferId);

            this.HasRequired(pop => pop.Product)
                .WithMany()
                .HasForeignKey(pop => pop.ProductId);
        }
    }
}
