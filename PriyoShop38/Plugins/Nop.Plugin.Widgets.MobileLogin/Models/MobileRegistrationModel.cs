using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Plugin.Widgets.MobileLogin.Validators.Customer;


namespace Nop.Plugin.Widgets.MobileLogin.Models
{
    [Validator(typeof(MobileRegistrationValidator))]
    public class MobileRegistrationModel : BaseNopModel
    {
        [NopResourceDisplayName("Account.Login.Fields.MobileNumber")]
        public string MobileNumber { get; set; }
        [NopResourceDisplayName("Account.Login.Fields.OTP")]
        public string OTP { get; set; }        
        [NopResourceDisplayName("Account.Login.Fields.Name")]
        public string Name { get; set; }
        [NopResourceDisplayName("Account.Login.Fields.Email")]
        public string Email { get; set; }       
    }
}