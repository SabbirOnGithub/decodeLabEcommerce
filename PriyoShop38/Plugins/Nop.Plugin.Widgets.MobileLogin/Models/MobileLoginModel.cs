using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Plugin.Widgets.MobileLogin.Validators.Customer;

namespace Nop.Plugin.Widgets.MobileLogin.Models
{
    [Validator(typeof(MobileLoginValidator))]
    public class MobileLoginModel : BaseNopModel
    {        
        [NopResourceDisplayName("Account.Login.Fields.MobileNumber")]
        public string MobileNumber { get; set; }
    }
}