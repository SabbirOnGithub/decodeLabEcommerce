using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Misc.HomePageProduct.Models
{
    public partial class HomePageProductSubCategoryListModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchIncludeSubCategories")]
        public bool SearchIncludeSubCategories { get; set; }
        [NopResourceDisplayName("Admin.CategoryName")]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
