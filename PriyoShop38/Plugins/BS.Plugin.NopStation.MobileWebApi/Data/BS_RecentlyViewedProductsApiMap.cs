using BS.Plugin.NopStation.MobileWebApi.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Data
{
    public partial class BS_RecentlyViewedProductsApiMap : EntityTypeConfiguration<BS_RecentlyViewedProductsApi>
    {
        public BS_RecentlyViewedProductsApiMap()
        {
            this.ToTable("BS_RecentlyViewedProductsApi");
            this.HasKey(x => x.Id);
        }
    }
}
