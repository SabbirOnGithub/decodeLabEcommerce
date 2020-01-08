using BS.Plugin.NopStation.MobileWebApi.Domain;
using System.Data.Entity.ModelConfiguration;

namespace BS.Plugin.NopStation.MobileWebApi.Data
{
    public partial class BS_FeaturedProductsMap : EntityTypeConfiguration<BS_FeaturedProducts>
    {
        public BS_FeaturedProductsMap()
        {
            this.ToTable("BS_FeaturedProducts");
            this.HasKey(x => x.Id);
        }
    }
}
