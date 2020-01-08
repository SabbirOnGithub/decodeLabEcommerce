using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductRestrictedPaymentMethodMap : NopEntityTypeConfiguration<ProductRestrictedPaymentMethod>
    {
        public ProductRestrictedPaymentMethodMap()
        {
            this.ToTable("ProductRestrictedPaymentMethod");
            this.HasKey(p => p.Id);

            this.HasRequired(pm => pm.Product)
                .WithMany(p => p.RestrictedPaymentMethods)
                .HasForeignKey(pm => pm.ProductId);
        }
    }
}
