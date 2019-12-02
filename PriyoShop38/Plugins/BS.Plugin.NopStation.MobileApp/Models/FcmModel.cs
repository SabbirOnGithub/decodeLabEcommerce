using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BS.Plugin.NopStation.MobileApp.Models
{        
    public class FcmModel
    {
        public FcmModel()
        {
            Data= new FcmDataModel();
            Notification= new FcmNotificationModel();
        }
        [JsonProperty(PropertyName = "to")]
        public string To { get; set; }
        [JsonProperty(PropertyName = "data")]
        public FcmDataModel Data { get; set; }
        [JsonProperty(PropertyName = "notification")]
        public FcmNotificationModel Notification { get; set; }
    }
}
