using Nop.Plugin.Misc.HomePageProduct.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Nop.Plugin.Misc.HomePageProduct.Data
{
    public class HomePageProductCategoryImageMap : EntityTypeConfiguration<HomePageProductCategoryImage>
    {
        public HomePageProductCategoryImageMap()
        {
            ToTable("HomePageProductCategoryImage");

            HasKey(x => x.Id);
            Property(x => x.ImageId);
            Property(x => x.CategoryId);
            Property(x => x.CategoryColor);
            Property(x => x.CreatedOnUtc);
            Property(x => x.UpdateOnUtc);

           // this.HasRequired(hpc => hpc.HomePageCategory)
           //.WithMany()
           //.HasForeignKey(hpc => hpc.CategoryId);

        }
    }
}
