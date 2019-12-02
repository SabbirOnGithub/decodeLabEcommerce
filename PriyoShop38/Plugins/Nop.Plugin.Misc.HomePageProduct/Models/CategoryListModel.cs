using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Misc.HomePageProduct.Models
{
    public partial class CategoryListModel : BaseNopEntityModel
    {
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public bool Publish { get; set; }
        public int CategoryPriority { get; set; }
        public string CategoryDisplayName { get; set; }
    }
}