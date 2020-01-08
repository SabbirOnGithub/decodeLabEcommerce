using System.Data.Entity.ModelConfiguration;
using BS.Plugin.NopStation.MobileWebApi.Domain;

namespace BS.Plugin.NopStation.MobileWebApi.Data
{
    public partial class BS_SliderMap : EntityTypeConfiguration<BS_Slider>
   {
        public BS_SliderMap()
        {
            this.ToTable("BS_Slider");
            this.HasKey(x => x.Id);

            this.Ignore(x => x.SliderDomainType);
        }
    }
}
