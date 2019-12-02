using Nop.Web.Framework.Mvc;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Agent
{

    /// <summary>
    /// Represents an agent
    /// </summary>
    public class ZeroAgentInfo : BaseNopEntityModel
    {
        public string AgentName { get; set; }
        public string AgentOrganizationName { get; set; }
        public int AgentContactNo { get; set; }
        public string AgentContactAddress { get; set; }
        public string AgentNID { get; set; }
        public byte[] AgentImage { get; set; }
        public string AgentPassword { get; set; }
        //public string OTP { get; set; }
    }
}
