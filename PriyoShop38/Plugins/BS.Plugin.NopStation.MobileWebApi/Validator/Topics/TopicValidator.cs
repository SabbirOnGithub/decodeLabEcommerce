using FluentValidation;
using BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace BS.Plugin.NopStation.MobileWebApi.Validators.Stores
{
    public class TopicValidator : BaseNopValidator<TopicModel>
    {
        public TopicValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SystemName).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.Topics.Fields.SystemName.Required"));
        }
    }
}