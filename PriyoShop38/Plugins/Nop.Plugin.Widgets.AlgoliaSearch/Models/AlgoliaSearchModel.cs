using Nop.Plugin.Widgets.AlgoliaSearch.Models.AlgoliaProduct;
using Nop.Web.Models.Catalog;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.AlgoliaSearch.Models
{
    public class AlgoliaSearchModel
    {
        public AlgoliaSearchModel()
        {
            Products = new List<ProductOverviewModel>();
            PagingFilteringContext = new AdvanceSearchPagingFilteringModel();
        }

        public IList<ProductOverviewModel> Products { get; set; }

        public string Warning { get; set; }
        public bool NoResults { get; set; }

        public string q { get; set; }

        public AdvanceSearchPagingFilteringModel PagingFilteringContext { get; set; }
    }
}
