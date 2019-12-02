using Nop.Data.Mapping;
using BS.Plugin.NopStation.MobileApp.Domain;

namespace BS.Plugin.NopStation.MobileApp.Data
{
    public partial class SmartGroupsMap : NopEntityTypeConfiguration<SmartGroup>
    {
        public SmartGroupsMap()
        {
            this.ToTable("Bs_SmartGroups");
            this.HasKey(x => x.Id);
            
        }
    }
}