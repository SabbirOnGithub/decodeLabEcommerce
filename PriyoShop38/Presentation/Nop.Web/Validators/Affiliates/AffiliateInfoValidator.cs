using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Affiliates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Validators.Affiliates
{
    public class AffiliateInfoValidator : BaseNopValidator<AffiliateInfoModel>
    {
        public AffiliateInfoValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Account.Affiliates.FirstName.Required"));
            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Account.Affiliates.LastName.Required"));
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Account.Affiliates.Email.Required"));
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage(localizationService.GetResource("Admin.Common.WrongEmail"));
            RuleFor(x => x.CountryId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Account.Affiliates.Country.Required"));
            RuleFor(x => x.CountryId)
                .NotEqual(0)
                .WithMessage(localizationService.GetResource("Account.Affiliates.Country.Required"));
            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Account.Affiliates.City.Required"));
            RuleFor(x => x.Address1)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Account.Affiliates.Address1.Required"));
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Account.Affiliates.PhoneNumber.Required"));
            RuleFor(x => x.ZipPostalCode)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Account.Affiliates.ZipPostalCode.Required"));
            RuleFor(x => x.BKashNumber)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Account.Affiliates.BKashNumber.Required"));
        }
    }
}