using Nop.Core.Domain.Discounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Discounts
{
    public partial class PurchaseOfferVendorMap : NopEntityTypeConfiguration<PurchaseOfferVendor>
    {
        public PurchaseOfferVendorMap()
        {
            this.ToTable("PurchaseOffer_AppliedToVendors");
            this.HasKey(pov => pov.Id);

            this.HasRequired(pov => pov.PurchaseOffer)
                .WithMany(po => po.AppliedToVendors)
                .HasForeignKey(pov => pov.PurchaseOfferId);

            this.HasRequired(pov => pov.Vendor)
                .WithMany()
                .HasForeignKey(pov => pov.VendorId);
        }
    }
}
