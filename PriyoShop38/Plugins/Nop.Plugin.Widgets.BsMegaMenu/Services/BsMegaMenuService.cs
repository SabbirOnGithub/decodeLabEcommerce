using Nop.Core.Data;
using Nop.Plugin.Widgets.BsMegaMenu.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;

namespace Nop.Plugin.Widgets.BsMegaMenu.Services
{
    public partial class BsMegaMenuService : IBsMegaMenuService
    {
        private IRepository<BsMegaMenuDomain> _bsMegaMenuRepo;
        private ICategoryService _categoryService;

        public BsMegaMenuService(IRepository<BsMegaMenuDomain> bsMegaMenuRepo,
            ICategoryService categoryService)
        {
            this._bsMegaMenuRepo = bsMegaMenuRepo;
            this._categoryService = categoryService;
        }

        public bool CategoryExist(int CategoryId)
        {
            var exists = _bsMegaMenuRepo.Table.Any(x => x.CategoryId == CategoryId);
            return exists;
        }

        public int MenuColumnNumber(int CategoryId)
        {
            var menuColumnNumber = _bsMegaMenuRepo.Table.Where(x => x.CategoryId == CategoryId)
                .Select(y => y.ColumnPosition).FirstOrDefault();

            return menuColumnNumber;
        }

        public List<BsMegaMenuDomain> Menus()
        {
            return _bsMegaMenuRepo.Table.ToList();
        }

        public Category ParentCategory(BsMegaMenuDomain Category)
        {
            var category = _categoryService.GetCategoryById(Category.CategoryId);
            var parentCategory = _categoryService.GetCategoryById(category.ParentCategoryId);

            return parentCategory;
        }

        public int ParentCategoryNumberOfColums(Category ParentCategory)
        {
            if (ParentCategory != null)
            {
                var parentCategoryNumberOfColums = _bsMegaMenuRepo.Table.
                Where(x => x.CategoryId == ParentCategory.Id).Select(y => y.NumberOfColums).FirstOrDefault();
                return parentCategoryNumberOfColums;
            }
            else
                return 0;
        }
    }
}
