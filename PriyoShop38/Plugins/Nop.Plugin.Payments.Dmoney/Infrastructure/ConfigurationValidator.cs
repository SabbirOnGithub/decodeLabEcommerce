using FluentValidation;
using Nop.Plugin.Payments.Dmoney.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.Dmoney.Infrastructure
{
    public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
    {
        public ConfigurationValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.GatewayUrl).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Payments.Dmoney.GatewayUrl.Required"));

            RuleFor(x => x.TransactionVerificationUrl).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Payments.Dmoney.GatewayUrl.TransactionVerificationUrl"));

            RuleFor(x => x.OrgCode).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Payments.Dmoney.OrgCode.Required"));

            RuleFor(x => x.Password).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Payments.Dmoney.Password.Required"));

            RuleFor(x => x.SecretKey).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Payments.Dmoney.SecretKey.Required"));
        }
    }
}
