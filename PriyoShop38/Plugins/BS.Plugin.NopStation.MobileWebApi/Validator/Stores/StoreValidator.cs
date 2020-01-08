using FluentValidation;
using BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace BS.Plugin.NopStation.MobileWebApi.Validators.Stores
{
    public class StoreValidator : BaseNopValidator<StoreModel>
    {
        public StoreValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Stores.Fields.Name.Required"));
            RuleFor(x => x.Url).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Stores.Fields.Url.Required"));
        }
    }
}