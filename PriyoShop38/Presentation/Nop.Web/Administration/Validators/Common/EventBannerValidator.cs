using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using Nop.Admin.Models.Common;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Common
{
    public class EventBannerValidator : BaseNopValidator<EventBannerModel>
    {
        public EventBannerValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.BannerName)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Configuration.Countries.Fields.Name.Required"));
            RuleFor(p => p.BannerName).Length(1, 100);
        }
    }
}