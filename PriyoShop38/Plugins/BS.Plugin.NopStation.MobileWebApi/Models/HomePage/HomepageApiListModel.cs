using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Banner;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;

namespace BS.Plugin.NopStation.MobileWebApi.Models.HomePage
{
    public class HomepageApiListModel
    {
        public HomepageApiListModel()
        {
            TopCategoryList = new List<CategoryOverViewModelApi>();
            EventBannerModels = new List<EventBannerResponseModel.EventBannerModel>();
            FlashSaleProducts = new List<ProductOverViewModelApi>();
            MostPopular = new List<ProductOverViewModelApi>();
            Parcent70Sale = new List<ProductOverViewModelApi>();
            RecentProducts = new List<ProductOverViewModelApi>();
        }

        public List<EventBannerResponseModel.EventBannerModel> EventBannerModels { get; set; }
        public List<CategoryOverViewModelApi> TopCategoryList { get; set; }
        public List<ProductOverViewModelApi> FlashSaleProducts { get; set; }
        public List<ProductOverViewModelApi> MostPopular { get; set; }
        public OnSaleProductModel OnSaleProductModel { get; set; }
        public List<ProductOverViewModelApi> Parcent70Sale { get; set; }
        public List<ProductOverViewModelApi> RecentProducts { get; set; }

        public class BlHomepage
        {
            public BlHomepage()
            {
                EventBannerModels = new List<EventBannerResponseModel.EventBannerModel>();
                MobileCategoryProducts = new List<ProductOverViewModelApi>();
                LaptopCategoryProducts = new List<ProductOverViewModelApi>();
            }
            public List<EventBannerResponseModel.EventBannerModel> EventBannerModels { get; set; }
            public List<ProductOverViewModelApi> MobileCategoryProducts { get; set; }
            public List<ProductOverViewModelApi> LaptopCategoryProducts { get; set; }
        }
    }
}
