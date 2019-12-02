using BS.Plugin.NopStation.MobileWebApi.Domain.Agent;
using BS.Plugin.NopStation.MobileWebApi.Models.Agent;

namespace BS.Plugin.NopStation.MobileWebApi.Services.Agent
{
    public interface IAgentService
    {
        void RegisterZeroAgent(ZeroAgentInfo ZeroAgentInfo, out ZeroAgentResponse ZeroAgentResponse);
        bool VerifyOTP(ZeroAgentOTP ZeroAgentOTP, out ZeroAgentResponse ZeroAgentResponse);
    }
}
