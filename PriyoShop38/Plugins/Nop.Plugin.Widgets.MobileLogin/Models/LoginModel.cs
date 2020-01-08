using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Plugin.Widgets.MobileLogin.Validators.Customer;

namespace Nop.Plugin.Widgets.MobileLogin.Models
{
    [Validator(typeof(LoginValidator))]
    public partial class LoginModel : BaseNopModel
    {
        [NopResourceDisplayName("Account.Login.Fields.MobileNumber")]
        //[RegularExpression("^([0|\\+[0-9]{1,5})?([0-9]{10})$", ErrorMessage = "Mobile Number is Not valid")]
        public string MobileNumber { get; set; }

        public bool CheckoutAsGuest { get; set; }

        [NopResourceDisplayName("Account.Login.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        public bool UsernamesEnabled { get; set; }
        [NopResourceDisplayName("Account.Login.Fields.UserName")]
        [AllowHtml]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [NoTrim]
        [NopResourceDisplayName("Account.Login.Fields.Password")]
        [AllowHtml]
        public string Password { get; set; }

        [NopResourceDisplayName("Account.Login.Fields.RememberMe")]
        public bool RememberMe { get; set; }

        public bool DisplayCaptcha { get; set; }

        public string LoginType { get; set; }
    }
}