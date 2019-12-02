using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using FluentValidation.Attributes;
using BS.Plugin.NopStation.MobileWebApi.Validator.Catalog;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel
{
    [Validator(typeof(CategoryIconValidator))]
    public class CategoryIconsModel : BaseNopEntityModel
    {
        public CategoryIconsModel()
        {
            AvailableCategories = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.CategoryIcons.CategoryId")]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.CategoryIcons.TextPrompt")]
        public string TextPrompt { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.CategoryIcons.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.CategoryIcons.PictureId")]
        [UIHint("Picture")]
        public int PictureId { get; set; }

        public string PictureUrl { get; set; }

        public List<SelectListItem> AvailableCategories { get; set; }

    }
}