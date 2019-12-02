using FluentValidation;
using BS.Plugin.NopStation.MobileApp.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace BS.Plugin.NopStation.MobileApp.Validators
{
    public class NotificationMessageTemplateValidator : BaseNopValidator<NotificationMessageTemplateModel>
    {
        public NotificationMessageTemplateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("BS.Plugin.NopStation.MobileApp.MessageTemplates.Fields.Name.Required"));
            RuleFor(x => x.Subject).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Fields.Subject.Required"));
            RuleFor(x => x.Body).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Fields.Body.Required"));
        }
    }
}