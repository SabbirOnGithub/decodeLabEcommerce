using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class CategoryHistoryMap : NopEntityTypeConfiguration<CategoryHistory>
    {
        public CategoryHistoryMap()
        {
            this.ToTable("CategoryHistory");
            this.HasKey(on => on.Id);
            this.Property(on => on.Description).IsRequired();

            this.HasRequired(ph => ph.Category)
                .WithMany()
                .HasForeignKey(ph => ph.CategoryId);

            this.HasRequired(ph => ph.Customer)
                .WithMany()
                .HasForeignKey(ph => ph.CustomerId);
        }
    }
}