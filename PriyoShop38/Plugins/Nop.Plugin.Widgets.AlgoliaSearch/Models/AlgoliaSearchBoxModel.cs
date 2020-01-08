using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Widgets.AlgoliaSearch.Models
{
    public class AlgoliaSearchBoxModel 
    {
        public AlgoliaSearchBoxModel()
        {
            _highlightResult = new HighlightResult();
            ProductPrice = new ProductOverviewModel.ProductPriceModel();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string SeName { get; set; }

        public string AutoCompleteImageUrl { get; set; }

        public ProductOverviewModel.ProductPriceModel ProductPrice { get; set; }

        public HighlightResult _highlightResult { get; set; }



        public class HighlightResult
        {
            public HighlightResult()
            {
                Name = new SingleValue();
            }

            public SingleValue Name { get; set; }
        }


        public class SingleValue
        {
            public string Value { get; set; }
        }
    }
}
