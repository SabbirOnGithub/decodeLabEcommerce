using Nop.Plugin.Misc.HomePageProduct.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Nop.Plugin.Misc.HomePageProduct.Data
{
    public class HomePageSubCategoryMap : EntityTypeConfiguration<HomePageSubCategory>
    {
        public HomePageSubCategoryMap()
        {
            ToTable("HomePageSubCategory");

            HasKey(x => x.Id);
            Property(x => x.TabName);
            Property(x => x.CategoryId);
            Property(x => x.SubCategoryId);
            Property(x => x.SubCategoryPriority);
            Property(x => x.CreatedOnUtc);
            Property(x => x.UpdateOnUtc);

            //this.HasRequired(hpc => hpc.HomePageCategory)
            //    .WithMany()
            //    .HasForeignKey(hpc => hpc.CategoryId);
        }
    }
}
