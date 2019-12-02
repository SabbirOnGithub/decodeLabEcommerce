using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace BS.Plugin.NopStation.MobileApp.Models
{
    public class FcmDataModel : FcmNotificationModel
    {
        [JsonProperty(PropertyName = "itemType")]
        public int NotificationTypeId { get; set; }
        [JsonProperty(PropertyName = "itemId")]
        public int ItemId { get; set; }
        [JsonProperty(PropertyName = "image")]
        public string Image { get; set; }
    }
}
