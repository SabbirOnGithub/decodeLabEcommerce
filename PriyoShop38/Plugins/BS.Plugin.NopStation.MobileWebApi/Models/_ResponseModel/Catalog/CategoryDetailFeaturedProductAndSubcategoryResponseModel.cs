using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Catalog
{
    public class CategoryDetailFeaturedProductAndSubcategoryResponseModel :BaseResponse
    {
        public CategoryDetailFeaturedProductAndSubcategoryResponseModel()
        {
            FeaturedProducts = new List<ProductOverViewModelApi>();
            SubCategories = new List<SubCategoryModelApi>();
        }
        public IList<SubCategoryModelApi> SubCategories { get; set; }
        public IList<ProductOverViewModelApi> FeaturedProducts { get; set; }
  
    }
}
