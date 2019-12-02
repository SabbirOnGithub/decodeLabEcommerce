using FluentValidation;
using BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace BS.Plugin.NopStation.MobileWebApi.Validators.Stores
{
    public class BannerSliderValidator : BaseNopValidator<BannerSliderModel>
    {
        public BannerSliderValidator(ILocalizationService localizationService)
        {
            RuleFor( x=> x.Link1).Matches("^((product)|(category))[/]\\d+/ig?$").When(x => x.Link1 != null).WithMessage(localizationService.GetResource("Admin.BannerSlider.Fields.Link.FormtError"));
        }
    }
}