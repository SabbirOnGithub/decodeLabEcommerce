using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel
{
   public class ContactInfoModel :BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [Required]
        [NopResourceDisplayName("Plugins.NopMobile.WebApi.ContactNumber")]
        public string MobContactNumber { get; set; }
        public bool MobContactNumber_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopMobile.WebApi.ContactEmail")]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string MobContactEmail { get; set; }
        public bool MobContactEmail_OverrideForStore { get; set; }
    }
}
