using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models
{
    public class IconModel
    {
        public IconModel()
        {
            CategoryIcons = new List<CategoryIconModel>();
        }

        public bool ShowCategoriesIcon { get; set; }

        public string CategoriesIconUrl { get; set; }

        public bool ShowManufacturersIcon { get; set; }

        public string ManufacturersIconUrl { get; set; }

        public IList<CategoryIconModel> CategoryIcons { get; set; }

        public class CategoryIconModel
        {
            public int CategoryId { get; set; }

            public string CategoryName { get; set; }

            public string IconUrl { get; set; }
        }
    }
}
