using System.Collections.Generic;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Catalog
{
    public class SearchProductResponseModel : BaseResponse
    {
        public SearchProductResponseModel()
        {
            this.PagingFilteringContext = new CatalogPagingFilteringModel();
            this.Products = new List<Nop.Web.Models.Catalog.ProductOverviewModel>();
        }

        public string Warning { get; set; }

        public bool NoResults { get; set; }
        
        public string q { get; set; }

        public CatalogPagingFilteringModel PagingFilteringContext { get; set; }
        public IList<Nop.Web.Models.Catalog.ProductOverviewModel> Products { get; set; }
    }
}
