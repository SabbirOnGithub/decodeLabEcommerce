using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BS.Plugin.NopStation.MobileApp.Models
{
    public class FcmErrorObject
    {
        [JsonProperty(PropertyName = "multicast_id")]
        public long MulticastID { get; set; }
        [JsonProperty(PropertyName = "success")]
        public int Success { get; set; }
        [JsonProperty(PropertyName = "failure")]
        public int Failure { get; set; }
        [JsonProperty(PropertyName = "canonical_ids")]
        public int CanonicalIds { get; set; }
        [JsonProperty(PropertyName = "results")]
        public List<FcmSingleResult> Results { get; set; }
    }
}
