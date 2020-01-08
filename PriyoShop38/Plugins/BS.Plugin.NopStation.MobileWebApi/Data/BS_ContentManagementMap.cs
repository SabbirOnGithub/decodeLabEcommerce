using BS.Plugin.NopStation.MobileWebApi.Domain;
using System.Data.Entity.ModelConfiguration;

namespace BS.Plugin.NopStation.MobileWebApi.Data
{
    public partial class BS_ContentManagementMap : EntityTypeConfiguration<BS_ContentManagement>
    {
        public BS_ContentManagementMap()
        {
            this.ToTable("BS_ContentManagement");
            this.HasKey(t => t.Id);
        }
    }
}
