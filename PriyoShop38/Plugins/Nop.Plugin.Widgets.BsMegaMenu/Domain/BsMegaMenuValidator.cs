using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Nop.Services.Catalog;
using Nop.Plugin.Widgets.BsMegaMenu.Services;
using FluentValidation.Results;

namespace Nop.Plugin.Widgets.BsMegaMenu.Domain
{
    public class BsMegaMenuValidator : AbstractValidator<BsMegaMenuDomain>
    {
        public BsMegaMenuValidator(ICategoryService categoryService,
            IBsMegaMenuService bsMegaMenuService)
        {
            RuleFor(x => x.NumberOfColums)
                .LessThanOrEqualTo(4).WithMessage("Number of colums must be less than or equal to 4");

            Custom(x =>
            {
                var parentCategory = bsMegaMenuService.ParentCategory(x);
                var parentCategoryNumberOfColumns = bsMegaMenuService.ParentCategoryNumberOfColums(
                bsMegaMenuService.ParentCategory(x));

                if (!(parentCategory == null) && (x.ColumnPosition > parentCategoryNumberOfColumns))
                {
                    return new ValidationFailure("ColumnPosition", "Columns position must be less than or equal to number of columns of parent category");
                }

                else if (!(parentCategory == null) && 
                !(bsMegaMenuService.CategoryExist(bsMegaMenuService.ParentCategory(x).Id)))
                {
                    return new ValidationFailure("ColumnPosition", "Please, insert it's Parent Category First.");
                }

                else
                {
                    return null;
                }
                    
            });
        }
    }
}
