using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ManufacturerHistoryMap : NopEntityTypeConfiguration<ManufacturerHistory>
    {
        public ManufacturerHistoryMap()
        {
            this.ToTable("ManufacturerHistory");
            this.HasKey(on => on.Id);
            this.Property(on => on.Description).IsRequired();

            this.HasRequired(ph => ph.Manufacturer)
                .WithMany()
                .HasForeignKey(ph => ph.ManufacturerId);

            this.HasRequired(ph => ph.Customer)
                .WithMany()
                .HasForeignKey(ph => ph.CustomerId);
        }
    }
}