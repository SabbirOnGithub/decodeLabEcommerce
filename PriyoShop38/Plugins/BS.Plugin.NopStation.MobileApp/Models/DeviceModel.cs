using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Mvc;
using FluentValidation.Attributes;
using BS.Plugin.NopStation.MobileApp.Validators;

namespace BS.Plugin.NopStation.MobileApp.Models
{
    [Validator(typeof(DeviceValidator))]
   public class DeviceModel:BaseNopEntityModel
    {
        public string DeviceToken { get; set; }
        public int DeviceTypeId { get; set; }

        public string DeviceType { get; set; }
        public int CustomerId { get; set; }

        public string CustomerName { get; set; }
        public string SubscriptionId { get; set; }
       
        public DateTime CreatedOnUtc { get; set; }
        
        public DateTime UpdatedOnUtc { get; set; }

        
    }
}
