using FluentValidation;
using Nop.Plugin.Widgets.BsAffiliate.Domain;
using Nop.Plugin.Widgets.BsAffiliate.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Widgets.BsAffiliate.Extensions
{
    public class CommissionRateValidator : BaseNopValidator<AffiliateCommissionRateModel>
    {
        public CommissionRateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.EntityType)
                .NotEqual((EntityType)0)
                .WithMessage("Type is required.");

            RuleFor(x => x.EntityType)
                .NotNull()
                .WithMessage("Type is required.");

            RuleFor(x => x.CategoryId)
                .NotNull()
                .When(x => x.EntityType == EntityType.Category)
                .WithMessage("Category is required.");

            RuleFor(x => x.CategoryId)
                .NotEqual(0)
                .When(x => x.EntityType == EntityType.Category)
                .WithMessage("Category is required.");

            RuleFor(x => x.VendorId)
                .NotNull()
                .When(x => x.EntityType == EntityType.Vendor)
                .WithMessage("Vendor is required.");

            RuleFor(x => x.VendorId)
                .NotEqual(0)
                .When(x => x.EntityType == EntityType.Vendor)
                .WithMessage("Vendor is required.");

            RuleFor(x => x.CommissionType)
                .NotNull()
                .WithMessage("Commission type is required.");

            RuleFor(x => x.CommissionType)
                .NotEqual((CommissionType)0)
                .WithMessage("Commission type is required.");

            RuleFor(x => x.CommissionRate)
                .NotNull()
                .WithMessage("Commission rate must be non-negative.").
                GreaterThanOrEqualTo(0);
        }
    }
}
