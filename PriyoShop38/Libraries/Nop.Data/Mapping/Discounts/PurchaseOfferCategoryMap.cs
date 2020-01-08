using Nop.Core.Domain.Discounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Discounts
{
    public partial class PurchaseOfferCategoryMap : NopEntityTypeConfiguration<PurchaseOfferCategory>
    {
        public PurchaseOfferCategoryMap()
        {
            this.ToTable("PurchaseOffer_AppliedToCategories");
            this.HasKey(poc => poc.Id);

            this.HasRequired(poc => poc.PurchaseOffer)
                .WithMany(po => po.AppliedToCategories)
                .HasForeignKey(poc => poc.PurchaseOfferId);

            this.HasRequired(poc => poc.Category)
                .WithMany()
                .HasForeignKey(poc => poc.CategoryId);
        }
    }
}
