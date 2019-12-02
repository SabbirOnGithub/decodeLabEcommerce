using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductUnpublishRequestByVendorMap : NopEntityTypeConfiguration<ProductUnpublishRequestByVendor>
    {
        public ProductUnpublishRequestByVendorMap()
        {
            this.ToTable("ProductUnpublishRequestByVendor");
            this.HasKey(purv => purv.Id);

            this.HasRequired(purv => purv.Product)
                .WithMany()
                .HasForeignKey(purv => purv.ProductId);
        }
    }
}
