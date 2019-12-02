using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;

namespace Nop.Web.Validators.Customer
{
    public class MobileLoginStep2Validator : BaseNopValidator<MobileLoginStep2Model>
    {
        public MobileLoginStep2Validator(ILocalizationService localizationService, CustomerSettings customerSettings)
        {
            if (customerSettings.OTPValidationRequired)
            {
                RuleFor(x => x.OTP).NotEmpty().WithMessage("One time pin is required.");
            }

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");

            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
        }
    }
}