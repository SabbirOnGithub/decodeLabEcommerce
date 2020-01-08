using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Catalog
{
    public partial class VendorHistoryMap : NopEntityTypeConfiguration<VendorHistory>
    {
        public VendorHistoryMap()
        {
            this.ToTable("VendorHistory");
            this.HasKey(on => on.Id);
            this.Property(on => on.Description).IsRequired();

            this.HasRequired(ph => ph.Vendor)
                .WithMany()
                .HasForeignKey(ph => ph.VendorId);

            this.HasRequired(ph => ph.Customer)
                .WithMany()
                .HasForeignKey(ph => ph.CustomerId);
        }
    }
}