using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductHistoryMap : NopEntityTypeConfiguration<ProductHistory>
    {
        public ProductHistoryMap()
        {
            this.ToTable("ProductHistory");
            this.HasKey(on => on.Id);
            this.Property(on => on.Description).IsRequired();

            this.HasRequired(ph => ph.Product)
                .WithMany()
                .HasForeignKey(ph => ph.ProductId);

            this.HasRequired(ph => ph.Customer)
                .WithMany()
                .HasForeignKey(ph => ph.CustomerId);
        }
    }
}