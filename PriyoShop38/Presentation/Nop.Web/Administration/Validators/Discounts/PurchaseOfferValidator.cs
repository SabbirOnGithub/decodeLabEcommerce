using FluentValidation;
using Nop.Admin.Models.Discounts;
using Nop.Core.Domain.Discounts;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Discounts
{
    public partial class PurchaseOfferValidator : BaseNopValidator<PurchaseOfferModel>
    {
        public PurchaseOfferValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.MinimumCartAmount).GreaterThan(0).WithMessage(localizationService.GetResource("Admin.Promotions.PurchaseOffers.Fields.MinimumCartAmount.GreaterThanZero"));

            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage(localizationService.GetResource("Admin.Promotions.PurchaseOffers.Fields.Quantity.GreaterThanZero"));

            SetStringPropertiesMaxLength<PurchaseOffer>(dbContext);
        }
    }
}