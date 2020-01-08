using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Misc.HomePageProduct.Models
{
    public partial class HomePageSubCategoryModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Catalog.HomePageProduct.Fields.SubCategoryName")]
        [AllowHtml]
        public string Name { get; set; }
        public string TabName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.SubCategoryShowOnHomePage")]
        public virtual bool SubCategoryShowOnHomePage { get; set; }
        public int Priority { get; set; }
    }
}