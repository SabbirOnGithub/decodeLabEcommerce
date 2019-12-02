using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Device
{
    public class AppStartModel
    {
        public int DeviceTypeId { get; set; }
        public string SubscriptionId { get; set; }

        public string EmailAddress { get; set; }
    }
}
