using System;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BS.Plugin.NopStation.MobileWebApi.Models.HomePage
{
    public class HomePageCategoryModel : BaseNopEntityModel
    {
        public HomePageCategoryModel()
        {
            AvailableCategories = new List<SelectListItem>();
            AddProductModel = new HomePagecategoryProductModel();
        }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.HomePageCategory.PictureId")]
        [UIHint("Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.HomePageCategory.CategoryId")]
        public int CategoryId { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.HomePageCategory.CategoryId")]
        public string CategoryName { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.HomePageCategory.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.HomePageCategory.TextPrompt")]
        public string TextPrompt { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.HomePageCategory.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.HomePageCategory.ForBlApp")]
        public bool ForBlApp { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.HomePageCategory.PercentValue")] 
        public int PercentValue { get; set; }

        [NopResourceDisplayName("Admin.SliderImage.Fields.SliderActiveStartDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? ActiveStartDate { get; set; }

        [NopResourceDisplayName("Admin.SliderImage.Fields.SliderActiveEndDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? ActiveEndDate { get; set; }

        public HomePagecategoryProductModel AddProductModel { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }


        public class HomePagecategoryProductModel : BaseNopEntityModel
        {
            public HomePagecategoryProductModel()
            {
                AvailableProducts = new List<SelectListItem>();
            }

            public int HomePageCategoryId { get; set; }

            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public int DisplayOrder { get; set; }

            public IList<SelectListItem> AvailableProducts { get; set; }
        }
    }
}
