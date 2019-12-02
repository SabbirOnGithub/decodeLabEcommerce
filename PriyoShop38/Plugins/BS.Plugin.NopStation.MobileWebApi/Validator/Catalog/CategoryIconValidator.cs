using BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel;
using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Validator.Catalog
{
    public class CategoryIconValidator : BaseNopValidator<CategoryIconsModel>
    {
        public CategoryIconValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CategoryId).NotEqual(0).WithMessage("Category is required");
            RuleFor(x => x.TextPrompt).NotEmpty().WithMessage("Text prompt is required");
        }
    }
}
