using FluentValidation;
using Nop.Plugin.Widgets.BsAffiliate.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Widgets.BsAffiliate.Extensions
{
    public class AddressValidator : BaseNopValidator<AddressModel>
    {
        public AddressValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Widgets.BsAffiliate.AddressModel.FirstName.Required"));
            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Widgets.BsAffiliate.AddressModel.LastName.Required"));
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Widgets.BsAffiliate.AddressModel.Email.Required"));
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage(localizationService.GetResource("Admin.Common.WrongEmail"));
            RuleFor(x => x.CountryId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Plugins.Widgets.BsAffiliate.AddressModel.Country.Required"));
            RuleFor(x => x.CountryId)
                .NotEqual(0)
                .WithMessage(localizationService.GetResource("Plugins.Widgets.BsAffiliate.AddressModel.Country.Required"));
            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Widgets.BsAffiliate.AddressModel.City.Required"));
            RuleFor(x => x.Address1)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Widgets.BsAffiliate.AddressModel.Address1.Required"));
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Widgets.BsAffiliate.AddressModel.PhoneNumber.Required"));
            RuleFor(x => x.ZipPostalCode)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Widgets.BsAffiliate.AddressModel.ZipPostalCode.Required"));
        }
    }
}
