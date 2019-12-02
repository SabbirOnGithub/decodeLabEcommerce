using BS.Plugin.NopStation.MobileWebApi.Domain;
using System.Data.Entity.ModelConfiguration;

namespace BS.Plugin.NopStation.MobileWebApi.Data
{
    public class BS_HomePageCategoryMap : EntityTypeConfiguration<BS_HomePageCategory>
    {
        public BS_HomePageCategoryMap()
        {
            this.ToTable("BS_HomePageCategory");
            this.HasKey(x => x.Id);
        }
    }
}
