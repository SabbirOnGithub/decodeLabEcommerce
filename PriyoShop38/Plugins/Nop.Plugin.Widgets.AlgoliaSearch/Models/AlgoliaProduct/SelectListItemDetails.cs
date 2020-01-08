using System.Web.Mvc;

namespace Nop.Plugin.Widgets.AlgoliaSearch.Models.AlgoliaProduct
{
    public class SelectListItemDetails 
    {
        public int Count { get; set; }

        public string GroupName { get; set; }

        public bool Selected { get; set; }

        public string Text { get; set; }

        public string Value { get; set; }
    }
}
