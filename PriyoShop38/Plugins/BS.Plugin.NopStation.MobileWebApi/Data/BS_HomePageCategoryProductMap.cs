using BS.Plugin.NopStation.MobileWebApi.Domain;
using System.Data.Entity.ModelConfiguration;

namespace BS.Plugin.NopStation.MobileWebApi.Data
{
    public class BS_HomePageCategoryProductMap : EntityTypeConfiguration<BS_HomePageCategoryProduct>
    {
        public BS_HomePageCategoryProductMap()
        {
            this.ToTable("BS_HomePageCategoryProduct");
            this.HasKey(x => x.Id);

            this.HasRequired(pm => pm.HomePageCategory)
                .WithMany(p => p.HomePageCategoryProducts)
                .HasForeignKey(pm => pm.HomePageCategoryId);
        }
    }
}
