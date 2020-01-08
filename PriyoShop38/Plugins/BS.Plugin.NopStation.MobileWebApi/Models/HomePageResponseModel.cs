using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;

using System.Collections.Generic;
using static BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Banner.HomePageBannerResponseModel;

namespace BS.Plugin.NopStation.MobileWebApi.Models
{
    public class HomePageResponseModel : BaseResponse
    {
        public HomePageResponseModel()
        {
            Banners = new List<BannerModel>();
            Categories = new List<CategoryNavigationModelApi>();
            Manufacturers = new List<MenufactureOverViewModelApi>();
            V1Categories = new List<CategoryNavigationModelApi>();
            CategoriesWithProducts = new List<HomePageCategoryWithProductsModel>();
            Language = new LanguageNavSelectorModel();
            Icons = new IconModel();
        }

        public IList<BannerModel> Banners { set; get; }

        public bool BannerIsEnabled { get; set; }

        public IList<CategoryNavigationModelApi> Categories { get; set; }

        public IList<CategoryNavigationModelApi> V1Categories { get; set; }

        public string CategoryListIcon { get; set; }

        public IList<MenufactureOverViewModelApi> Manufacturers { get; set; }

        public string ManufacturerListIcon { get; set; }

        public IList<HomePageCategoryWithProductsModel> CategoriesWithProducts { get; set; }

        public LanguageNavSelectorModel Language { get; set; }

        public IconModel Icons { get; set; }

        public int Count { get; set; }
    }
}
