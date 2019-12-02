using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Validators.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.Customer
{
    [Validator(typeof(MobileLoginStep2Validator))]
    public class MobileLoginStep2Model
    {
        [NopResourceDisplayName("Account.Login.Fields.OTP")]
        public string OTP { get; set; }

        [NopResourceDisplayName("Account.Login.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Account.Login.Fields.Email")]
        public string Email { get; set; }

        public bool OTPValidationRequired { get; set; }

        public bool NewCustomer { get; set; }
    }
}