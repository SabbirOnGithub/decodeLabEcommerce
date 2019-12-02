namespace BS.Plugin.NopStation.MobileWebApi.Models.Agent
{

    /// <summary>
    /// Represents a Zero Agent API response
    /// </summary>
    public class ZeroAgentResponse
    {
        public long Id { get; set; } = -1;
        public string Status { get; set; } = "failure";
        public string Message { get; set; } = "problem occured";
    }
}
