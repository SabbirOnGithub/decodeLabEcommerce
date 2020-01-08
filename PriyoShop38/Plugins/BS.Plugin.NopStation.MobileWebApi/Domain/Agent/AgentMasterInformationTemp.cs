using System;

using Nop.Core;

namespace BS.Plugin.NopStation.MobileWebApi.Domain.Agent
{
    public partial class AgentMasterInformationTemp : BaseEntity
    {
        public int AgentContactNo { get; set; }
        public string AgentName { get; set; }
        public string AgentOrganizationName { get; set; }
        public string AgentContactAddress { get; set; }
        public string AgentNID { get; set; }
        public string AgentPassword { get; set; }
        public string OTP { get; set; }
        public DateTime OTPGenerationDate { get; set; }
        public int OTPExpireInMinute { get; set; }
        public DateTime EntryDate { get; set; }
    }
}
