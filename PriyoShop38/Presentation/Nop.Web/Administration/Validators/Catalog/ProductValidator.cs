using FluentValidation;
using Nop.Admin.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Catalog
{
    public partial class ProductValidator : BaseNopValidator<ProductModel>
    {
        public ProductValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.Name.Required"));

//            When(x => x.SpecialProductCostStartDateTimeUtc.HasValue &&
//                      x.SpecialProductCostEndDateTimeUtc.HasValue, () => {
//                RuleFor(x => x.SpecialProductCostStartDateTimeUtc)
//                    .LessThan(x => x.SpecialProductCostEndDateTimeUtc)
//                    .WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.SpecialProductCostDates.ShouldBeConsistent"));
//            });

            SetStringPropertiesMaxLength<Product>(dbContext);
        }
    }
}