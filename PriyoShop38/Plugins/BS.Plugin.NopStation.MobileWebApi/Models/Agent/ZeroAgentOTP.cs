using Nop.Web.Framework.Mvc;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Agent
{

    /// <summary>
    /// Represents an agent OTP payload
    /// </summary>
    public class ZeroAgentOTP : BaseNopEntityModel
    {
        public string OTP { get; set; }
    }
}
