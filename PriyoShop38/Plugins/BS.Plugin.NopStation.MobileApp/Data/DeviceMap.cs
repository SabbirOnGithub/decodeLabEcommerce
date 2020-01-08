using BS.Plugin.NopStation.MobileApp.Domain;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileApp.Data
{
    public partial class DeviceMap : NopEntityTypeConfiguration<Device>
    {
        public DeviceMap()
        {
            this.ToTable("Bs_WebApi_Device");
            this.HasKey(x => x.Id);
            //this.Ignore(x => x.DeviceType);
        }
    }
}
