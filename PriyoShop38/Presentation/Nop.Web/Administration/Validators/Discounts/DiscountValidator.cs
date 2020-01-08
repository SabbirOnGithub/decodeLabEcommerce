using FluentValidation;
using Nop.Admin.Models.Discounts;
using Nop.Core.Domain.Discounts;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Discounts
{
    public partial class DiscountValidator : BaseNopValidator<DiscountModel>
    {
        public DiscountValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Promotions.Discounts.Fields.Name.Required"));

            RuleFor(x => x.BulkTrack).NotEmpty().When(x => x.IsBulkCreate).WithMessage(localizationService.GetResource("Admin.Promotions.Discounts.Fields.BulkTrack.Required"));

            #region ferdous

            RuleFor(x => x.ForNthOrder).NotEmpty().When(x=> x.LimitationOnOrderId == (int)(LimitationOnOrder.ForNthOrder)).WithMessage(localizationService.GetResource("Admin.Promotions.Discounts.Fields.ForNthOrder.Required"));
            RuleFor(x => x.ForNthOrder).NotEqual(0).When(x=> x.LimitationOnOrderId == (int)(LimitationOnOrder.ForNthOrder)).WithMessage(localizationService.GetResource("Admin.Promotions.Discounts.Fields.ForNthOrder.Required"));

            RuleFor(x => x.FromNthOrder).NotEmpty().When(x => x.LimitationOnOrderId == (int)(LimitationOnOrder.FromNthToMthOrder)).WithMessage(localizationService.GetResource("Admin.Promotions.Discounts.Fields.FromNthOrder.Required"));
            RuleFor(x => x.FromNthOrder).NotEqual(0).When(x => x.LimitationOnOrderId == (int)(LimitationOnOrder.FromNthToMthOrder)).WithMessage(localizationService.GetResource("Admin.Promotions.Discounts.Fields.FromNthOrder.Required"));
            RuleFor(x => x.ToMthOrder).NotEmpty().When(x => x.LimitationOnOrderId == (int)(LimitationOnOrder.FromNthToMthOrder)).WithMessage(localizationService.GetResource("Admin.Promotions.Discounts.Fields.ToMthOrder.Required"));
            RuleFor(x => x.ToMthOrder).NotEqual(0).When(x => x.LimitationOnOrderId == (int)(LimitationOnOrder.FromNthToMthOrder)).WithMessage(localizationService.GetResource("Admin.Promotions.Discounts.Fields.ToMthOrder.Required"));


            #endregion


            SetStringPropertiesMaxLength<Discount>(dbContext);
        }
    }
}