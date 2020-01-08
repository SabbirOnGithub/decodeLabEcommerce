using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileApp.Models
{
    public class AppleNotificationResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public int Id { get; set; }
        public DateTime? SentOnUtc { get; set; }
    }
}
