using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using BS.Plugin.NopStation.MobileWebApi.Domain.Agent;

namespace BS.Plugin.NopStation.MobileWebApi.Data.Agent
{
    public partial class AgentMasterInformationMap : EntityTypeConfiguration<AgentMasterInformation>
    {
        public AgentMasterInformationMap()
        {
            ToTable("AgentMasterInformation", "Agent");
            HasKey(e => e.AgentId);

            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}
