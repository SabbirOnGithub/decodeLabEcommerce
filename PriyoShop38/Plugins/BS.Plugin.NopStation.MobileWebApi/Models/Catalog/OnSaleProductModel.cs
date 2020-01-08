using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Catalog
{
    public class OnSaleProductModel
    {
        public OnSaleProductModel()
        {
            Products = new List<ProductOverViewModelApi>();
            SubCategories = new List<SubCategoryModelApi>();
        }

        public List<ProductOverViewModelApi> Products { get; set; }

        public List<SubCategoryModelApi> SubCategories { get; set; }
    }
}
