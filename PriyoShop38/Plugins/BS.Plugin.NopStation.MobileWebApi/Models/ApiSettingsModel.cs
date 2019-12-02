using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BS.Plugin.NopStation.MobileWebApi.Models
{
    public class ApiSettingsModel
    {
        public ApiSettingsModel()
        {
            Availablecategories = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.ApiSettingsModel.BogoCategoryId")]
        public int BogoCategoryId { get; set; }
        public IList<SelectListItem> Availablecategories { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.ApiSettingsModel.AppOnlyCategoryId")]
        public int AppOnlyCategoryId { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.ApiSettingsModel.Offer99CategoryId")]
        public int Offer99CategoryId { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.ApiSettingsModel.OmgSaleCategoryId")]
        public int OmgSaleCategoryId { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.ApiSettingsModel.CartThumbPictureSize")]
        public int CartThumbPictureSize { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.ApiSettingsModel.ShowHomePageTopCategoryListIcon")]
        public bool ShowHomePageTopCategoryListIcon { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.ApiSettingsModel.CategoryListIconId")]
        [UIHint("Picture")]
        public int CategoryListIconId { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.ApiSettingsModel.ShowHomePageTopManufacturerListIcon")]
        public bool ShowHomePageTopManufacturerListIcon { get; set; } 

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.ApiSettingsModel.ManufacturerListIconId")]
        [UIHint("Picture")]
        public int ManufacturerListIconId { get; set; }
    }
}
