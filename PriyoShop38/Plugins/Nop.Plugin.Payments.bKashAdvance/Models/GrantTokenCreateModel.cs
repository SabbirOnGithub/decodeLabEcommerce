using Newtonsoft.Json;

namespace Nop.Plugin.Payments.bKashAdvance.Models
{
    public class GrantTokenCreateModel
    {
        [JsonProperty(PropertyName = "app_key")]
        public string AppKey { get; set; }

        [JsonProperty(PropertyName = "app_secret")]
        public string AppSecret { get; set; }
    }
}
