using FluentValidation;
using Nop.Admin.Models.Affiliates;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Validators.Affiliates
{
    public class AffiliateTypeValidator : AbstractValidator<AffiliateTypeModel>
    {
        public AffiliateTypeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.AffiliateTypes.Fields.Name.Required"));

            RuleFor(x => x.IdUrlParameter)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.AffiliateTypes.Fields.IdUrlParameter.Required"));

            RuleFor(x => x.NameUrlParameter)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.AffiliateTypes.Fields.NameUrlParameter.Required"))
                .Must(x => !x.Contains(" "))
                .Must(x => !x.StartsWith(" "))
                .Must(x => !x.EndsWith(" "))
                .WithMessage(localizationService.GetResource("Admin.AffiliateTypes.Fields.NameUrlParameter.SpaceNotAllowed"));

            RuleFor(x => x.IdUrlParameter)
                .NotEqual(x=> x.NameUrlParameter)
                .WithMessage(localizationService.GetResource("Admin.AffiliateTypes.Fields.IdUrlParameter.CanNotSameAsNameUrlParameter"))
                .Must(x => !x.Contains(" "))
                .Must(x => !x.StartsWith(" "))
                .Must(x => !x.EndsWith(" "))
                .WithMessage(localizationService.GetResource("Admin.AffiliateTypes.Fields.NameUrlParameter.SpaceNotAllowed"));
        }
    }
}