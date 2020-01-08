using Nop.Plugin.Misc.HomePageProduct.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Nop.Plugin.Misc.HomePageProduct.Data
{
    public class HomePageProductCategoryMap : EntityTypeConfiguration<HomePageProductCategory>
    {
        public HomePageProductCategoryMap()
        {
            ToTable("HomePageProductCategory");

            HasKey(x => x.Id);
            Property(x => x.ProductId);
            Property(x => x.CategoryId);
            Property(x => x.Priority);
            Property(x => x.CreatedOnUtc);
            Property(x => x.UpdateOnUtc);



            //this.HasRequired(pc => pc.HomePageCategory)
            //    .WithMany()
            //    .HasForeignKey(pc => pc.CategoryId);
        }
    }
}
