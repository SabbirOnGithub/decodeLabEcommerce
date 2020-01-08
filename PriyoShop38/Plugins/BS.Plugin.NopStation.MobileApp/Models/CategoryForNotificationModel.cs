using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Nop.Web.Framework;

namespace BS.Plugin.NopStation.MobileApp.Models
{
    public class CategoryForNotificationModel
    {
      public  CategoryForNotificationModel()
        {
            AvailableStores = new List<SelectListItem>(); //change 3.8
        }
        [NopResourceDisplayName("Admin.Catalog.Categories.List.SearchCategoryName")]
        [AllowHtml]
        public string SearchCategoryName { get; set; }

        public int ScheduleIdId { get; set; }

        public int[] SelectedCategoryIds { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.List.SearchStore")]
        public int SearchStoreId { get; set; }  //change 3.8
        public IList<SelectListItem> AvailableStores { get; set; } //change 3.8
    }
}
