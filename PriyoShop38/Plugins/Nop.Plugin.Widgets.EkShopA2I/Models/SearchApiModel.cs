using System.Collections.Generic;

namespace Nop.Plugin.Widgets.EkShopA2I.Models
{
    public class SearchApiModel
    {
        public SearchApiModel()
        {
            meta = new MetaModel();
            response = new SearchResponseModel();
        }

        public MetaModel meta { get; set; }
        public SearchResponseModel response { get; set; }
    }

    public class SearchResponseModel
    {
        public SearchResponseModel()
        {
            products = new List<SearchProductModel>();
        }

        public string message { get; set; }
        public List<SearchProductModel> products { get; set; }
    }

    public class SearchProductModel
    {
        public string product_name { get; set; }
        public decimal product_price { get; set; }
        public string product_url { get; set; }
        public string product_image { get; set; } 
    }
}
