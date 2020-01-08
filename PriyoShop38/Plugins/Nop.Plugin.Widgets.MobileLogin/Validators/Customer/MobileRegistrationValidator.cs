using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Plugin.Widgets.MobileLogin.Models;

namespace Nop.Plugin.Widgets.MobileLogin.Validators.Customer
{
    public partial class MobileRegistrationValidator : BaseNopValidator<MobileRegistrationModel>
    {
        public MobileRegistrationValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.MobileNumber).NotEmpty().WithMessage("Mobile number is required.");            
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Account.Login.Fields.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
        }
    }
}