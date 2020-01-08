using System;
using FluentValidation;
using Nop.Plugin.Misc.BiponeePreOrder.Models;
using Nop.Services.Localization;

namespace Nop.Plugin.Misc.BiponeePreOrder.Validators
{
    public class BiponeePreOrderSubscribeModelValidator : AbstractValidator<BiponeePreOrderSubscribeModel>
    {
        public BiponeePreOrderSubscribeModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CustomerName).NotEmpty().WithMessage("Customer Name can not be empty.");
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
            RuleFor(x => x.PhoneNumber).NotNull().Length(11)
                .Must(x => x.StartsWith("017") || x.StartsWith("016") || x.StartsWith("018") || x.StartsWith("019") || x.StartsWith("011") || x.StartsWith("015"))
                .Must(x => { try { int num = Convert.ToInt32(x); return true; } catch (Exception) { return false; } }).WithMessage("is not valid phone number");
        }
    }
}