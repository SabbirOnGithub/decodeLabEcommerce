using BS.Plugin.NopStation.MobileWebApi.Models.Slider;
using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace BS.Plugin.NopStation.MobileWebApi.Validator.Slider
{
    public class SliderImageValidator : BaseNopValidator<SliderImageModel>
    {
        public SliderImageValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.PictureId).NotEqual(0).WithMessage(localizationService.GetResource("Admin.SliderImage.Fields.Picture.Required"));
        }
    }
}
