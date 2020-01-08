using FluentValidation;
using Nop.Web.Framework.Validators;
using Nop.Plugin.Widgets.MobileLogin.Models;

namespace Nop.Plugin.Widgets.MobileLogin.Validators.Customer
{
    public partial class MobileLoginValidator : BaseNopValidator<MobileLoginModel>
    {
        public MobileLoginValidator()
        {            
            //login by mobile number
            RuleFor(x => x.MobileNumber).NotEmpty().WithMessage("Mobile number is required.");                            
        }
    }
}