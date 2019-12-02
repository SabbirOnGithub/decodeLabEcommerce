using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Nop.Admin.Models.Catalog;
using Nop.Web.Framework;

namespace Nop.Plugin.Misc.OnePageCheckOutAdmin.Models.ProductModel
{
    public class SeachProductModel : CategoryModel.AddCategoryProductModel
    {
        public SeachProductModel():base()
        {
            Warnings= new List<string>();
        }
        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchSku")]
        [AllowHtml]
        public string SearchProductSku { get; set; }

        public IList<string> Warnings { get; set; }
    }
}
