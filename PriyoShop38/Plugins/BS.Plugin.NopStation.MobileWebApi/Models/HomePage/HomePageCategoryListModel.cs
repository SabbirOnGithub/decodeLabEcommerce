using System.Collections.Generic;
using System.Web.Mvc;

namespace BS.Plugin.NopStation.MobileWebApi.Models.HomePage
{
    public class HomePageCategoryListModel
    {
        public HomePageCategoryListModel()
        {
            AvailablePublishedOptions = new List<SelectListItem>();
            AvailableCategories = new List<SelectListItem>();
        }

        public string TextPrompt { get; set; }

        public bool SearchIncludeSubCategories { get; set; }

        public int SearchCategoryId { get; set; }

        public int SearchPublishedId { get; set; }


        public IList<SelectListItem> AvailablePublishedOptions { get; set; }
        public IList<SelectListItem> AvailableCategories { get; set; }
    }
}
