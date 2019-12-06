using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Nop.Core.Domain.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Common;

namespace Nop.Web.Validators.Common
{
    public partial class EventBannerValidator : BaseNopValidator<EventBanner>
    {
        public EventBannerValidator(ILocalizationService localizationService,
            IStateProvinceService stateProvinceService)
        {
            RuleFor(x => x.BannerName)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("EventBanner.Fields.BannerName.Required"));

            RuleFor(x => x.CategoryId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("EventBanner.Fields.CategoryId.Required"));
        }

    }
}