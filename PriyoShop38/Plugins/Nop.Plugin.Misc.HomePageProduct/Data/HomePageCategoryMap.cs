using Nop.Plugin.Misc.HomePageProduct.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Nop.Plugin.Misc.HomePageProduct.Data
{
    public class HomePageCategoryMap : EntityTypeConfiguration<HomePageCategory>
    {
        public HomePageCategoryMap()
        {
            ToTable("HomePageCategory");

            HasKey(x => x.Id);
            Property(x => x.CategoryId);
            Property(x => x.CategoryPriority);
            Property(x => x.CreatedOnUtc);
            Property(x => x.UpdateOnUtc);
            Property(x => x.CategoryDisplayName);
        }
    }
}
