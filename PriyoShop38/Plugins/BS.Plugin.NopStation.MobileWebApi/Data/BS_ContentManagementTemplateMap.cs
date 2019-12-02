using BS.Plugin.NopStation.MobileWebApi.Domain;
using System.Data.Entity.ModelConfiguration;

namespace BS.Plugin.NopStation.MobileWebApi.Data
{
    public partial class BS_ContentManagementTemplateMap : EntityTypeConfiguration<BS_ContentManagementTemplate>
    {
        public BS_ContentManagementTemplateMap()
        {
            this.ToTable("BS_ContentManagementTemplate");
            this.HasKey(t => t.Id);
            this.Property(t => t.Name).IsRequired().HasMaxLength(400);
            this.Property(t => t.ViewPath).IsRequired().HasMaxLength(400);
        }
    }
}
