using FluentValidation;
using FluentValidation.Attributes;
using Nop.Services.Localization;

namespace Nop.Web.Framework
{
    [Validator(typeof(AffiliateTypeModelValidator))]
    public class AffiliateTypeModels
    {
        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateType.ID")]
        public int Id { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateType.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateType.NameUrlParameter")]
        public string NameUrlParameter { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateType.IdUrlParameter")]
        public string IdUrlParameter { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateType.Active")]
        public bool Active { get; set; }
    }

    public class AffiliateTypeModelValidator : AbstractValidator<AffiliateTypeModels>
    {
        public AffiliateTypeModelValidator(ILocalizationService localizationService)
        {
            When(x => !string.IsNullOrEmpty(x.IdUrlParameter) && !string.IsNullOrEmpty(x.NameUrlParameter), () => {
                RuleFor(x => x.IdUrlParameter)
                    .NotEqual(x => x.NameUrlParameter)
                    .WithMessage(localizationService.GetResource("Plugins.Widgets.BsAffiliate.AffiliateType.Validation.NameAndIdMustNoteBeSame"));
            });

            When(x => !string.IsNullOrEmpty(x.NameUrlParameter), () =>
            {
                RuleFor(x => x.NameUrlParameter)
                    .Must(x => !x.Contains(" "))
                    .WithMessage(localizationService.GetResource("Plugins.Widgets.BsAffiliate.AffiliateType.Validation.NameUrlParameter.CanNotContainSpaces"));
            });

            When(x => !string.IsNullOrEmpty(x.IdUrlParameter), () =>
            {
                RuleFor(x => x.IdUrlParameter)
                    .Must(x => !x.Contains(" "))
                    .WithMessage(localizationService.GetResource("Plugins.Widgets.BsAffiliate.AffiliateType.Validation.IdUrlParameter.CanNotContainSpaces"));
            });
        }        
    }
}
