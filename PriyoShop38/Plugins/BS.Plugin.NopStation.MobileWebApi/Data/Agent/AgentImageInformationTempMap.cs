using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using BS.Plugin.NopStation.MobileWebApi.Domain.Agent;

namespace BS.Plugin.NopStation.MobileWebApi.Data.Agent
{
    public partial class AgentImageInformationTempMap : EntityTypeConfiguration<AgentImageInformationTemp>
    {
        public AgentImageInformationTempMap()
        {
            ToTable("AgentImageInformationTemp", "Agent");
            HasKey(e => e.Id);

            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}
