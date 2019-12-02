using Nop.Data.Mapping;
using BS.Plugin.NopStation.MobileWebApi.Domain;

namespace BS.Plugin.NopStation.MobileWebApi.Data
{
    public partial class DeviceMap : NopEntityTypeConfiguration<Device>
    {
        public DeviceMap()
        {
            this.ToTable("BS_WebApi_Device");
            this.HasKey(x => x.Id);
            //this.Ignore(x => x.DeviceType);
        }
    }
}