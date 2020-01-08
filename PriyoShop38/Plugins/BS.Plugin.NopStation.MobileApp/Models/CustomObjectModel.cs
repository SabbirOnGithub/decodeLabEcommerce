using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileApp.Models
{
   public class CustomObjectModel
       {
       public int NotificationTypeId { get; set; }
       public string NotificationType { get; set; }
       public  int ItemId { get; set; }

       public string Subject { get; set; }
    }
}
