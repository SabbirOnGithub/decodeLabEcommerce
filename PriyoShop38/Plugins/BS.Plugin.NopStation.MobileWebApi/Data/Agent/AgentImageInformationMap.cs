using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using BS.Plugin.NopStation.MobileWebApi.Domain.Agent;

namespace BS.Plugin.NopStation.MobileWebApi.Data.Agent
{
    public partial class AgentImageInformationMap : EntityTypeConfiguration<AgentImageInformation>
    {
        public AgentImageInformationMap()
        {
            ToTable("AgentImageInformation", "Agent");
            HasKey(e => e.AgentImageId);

            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}
