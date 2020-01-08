using Nop.Core.Domain.Discounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Discounts
{
    public partial class PurchaseOfferManufacturerMap : NopEntityTypeConfiguration<PurchaseOfferManufacturer>
    {
        public PurchaseOfferManufacturerMap()
        {
            this.ToTable("PurchaseOffer_AppliedToManufacturers");
            this.HasKey(pom => pom.Id);

            this.HasRequired(pom => pom.PurchaseOffer)
                .WithMany(po => po.AppliedToManufacturers)
                .HasForeignKey(pom => pom.PurchaseOfferId);

            this.HasRequired(pom => pom.Manufacturer)
                .WithMany()
                .HasForeignKey(pom => pom.ManufacturerId);
        }
    }
}
