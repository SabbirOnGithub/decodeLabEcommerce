using System;

using Nop.Core;

namespace BS.Plugin.NopStation.MobileWebApi.Domain.Agent
{
    public partial class AgentMasterInformation : BaseEntity
    {
        public long AgentId { get; set; }
        public int AgentContactNo { get; set; }
        public string AgentName { get; set; }
        public string AgentOrganizationName { get; set; }
        public string AgentContactAddress { get; set; }
        public string AgentNID { get; set; }
        public string AgentPassword { get; set; }
        public DateTime EntryDate { get; set; }
    }
}
