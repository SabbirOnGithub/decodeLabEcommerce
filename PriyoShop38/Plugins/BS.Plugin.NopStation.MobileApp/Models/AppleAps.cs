using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BS.Plugin.NopStation.MobileApp.Models
{
    public class AppleAps
    {
        public AppleAps()
        {
            CustomObject = new AppleApsCustomObject();
        }
        [JsonProperty(PropertyName = "badge")]
        public int Badge { get; set; }
        [JsonProperty(PropertyName = "alert")]
        public string Alert { get; set; }
        [JsonProperty(PropertyName = "sound")]
        public string Sound { get; set; }
        [JsonProperty(PropertyName = "customObject")]
        public AppleApsCustomObject CustomObject { get; set; }
        public string Subject { get; set; }
    }
}
