using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Plugin.Widgets.MobileLogin.Models;

namespace Nop.Plugin.Widgets.MobileLogin.Validators.Customer
{
    public partial class MobileLoginStep2Validator : BaseNopValidator<MobileLoginStep2Model>
    {
        public MobileLoginStep2Validator(ILocalizationService localizationService)
        {            
            //login by mobile number
            RuleFor(x => x.OTP).NotEmpty().WithMessage("One time pin is required.");
            //RuleFor(x => x.FirstName).NotEmpty().WithMessage("First Name is required.");
            //RuleFor(x => x.LastName).NotEmpty().WithMessage("Last Name is required.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            //RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Account.Login.Fields.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
        }
    }
}