using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.MobileLogin.Models
{
    public class MobileLoginSettingsModel : BaseNopEntityModel
    {
        [Display(Name = "Country Code")]
        public string CountryCode { get; set; }
        [Display(Name = "User Name")]
        public string SmsGatewayUserName { get; set; }
        [Display(Name = "Password")]
        public string SmsGatewayPassword { get; set; }
    }
}
