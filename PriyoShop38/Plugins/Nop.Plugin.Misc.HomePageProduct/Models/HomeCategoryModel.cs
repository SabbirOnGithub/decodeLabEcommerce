using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.HomePageProduct.Models
{
    public partial class HomeCategoryModel : BaseNopModel
    {
        public HomeCategoryModel()
        {
            AvailableCategories = new List<SelectListItem>();
        }
        public IList<SelectListItem> AvailableCategories { get; set; }

        public int CategoryId { get; set; }
        public int CategoryPriority { get; set; }

        public string CategoryDisplayName { get; set; }

    }
}