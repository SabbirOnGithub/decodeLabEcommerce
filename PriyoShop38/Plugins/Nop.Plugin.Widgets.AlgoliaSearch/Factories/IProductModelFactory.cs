using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Widgets.AlgoliaSearch.Models;
using Nop.Plugin.Widgets.AlgoliaSearch.Models.AlgoliaProduct;
using Nop.Web.Models.Catalog;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.AlgoliaSearch.Factories
{
    public interface IProductModelFactory
    {
        AlgoliaProductOverviewModel PrepareAlgoliaUploadModel(Product product);

        IList<AlgoliaProductOverviewModel> PrepareAlgoliaUploadModel(IPagedList<Product> products);

        IPagedList<ProductOverviewModel> SearchProducts(AlgoliaPagingFilteringModel command, bool preparePriceModel = true);

        CatalogFilterings GetAlgoliaFilterings(string q, bool includeEkshopProducts = false);

        void UpdateAlgoliaModel();
    }
}