using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Vendors
{
    public partial class VendorRestrictedPaymentMethodMap : NopEntityTypeConfiguration<VendorRestrictedPaymentMethod>
    {
        public VendorRestrictedPaymentMethodMap()
        {
            this.ToTable("VendorRestrictedPaymentMethod");
            this.HasKey(p => p.Id);

            this.HasRequired(pm => pm.Vendor)
                .WithMany(p => p.RestrictedPaymentMethods)
                .HasForeignKey(pm => pm.VendorId);
        }
    }
}
