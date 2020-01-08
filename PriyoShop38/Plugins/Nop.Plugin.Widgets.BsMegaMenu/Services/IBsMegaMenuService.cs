using Nop.Core.Domain.Catalog;
using Nop.Plugin.Widgets.BsMegaMenu.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.BsMegaMenu.Services
{
    public partial interface IBsMegaMenuService
    {
        int MenuColumnNumber(int CategoryId);
        List<BsMegaMenuDomain> Menus();
        int ParentCategoryNumberOfColums(Category ParentCategory);
        Category ParentCategory(BsMegaMenuDomain Category);
        bool CategoryExist(int CategoryId);
    }
}
