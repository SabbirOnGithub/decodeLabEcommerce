using BS.Plugin.NopStation.MobileWebApi.Controllers;
using Nop.Core;
using Nop.Plugin.Widgets.AlgoliaSearch.Factories;
using Nop.Plugin.Widgets.AlgoliaSearch.Models;
using Nop.Plugin.Widgets.EkShopA2I.Extensions;
using Nop.Plugin.Widgets.EkShopA2I.Models;
using Nop.Plugin.Widgets.EkShopA2I.Services;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using System;
using System.Web;
using System.Web.Http;

namespace Nop.Plugin.Widgets.EkShopA2I.Controllers
{
    [EkshopAuthorize]
    public class EkshopApiController : BaseApiController
    {
        private readonly ILogger _logger;
        private readonly IEkshopEpService _esEpService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IStoreContext _storeContext;
        private readonly EkshopSettings _ekShopA2ISettings;
        private readonly ISettingService _settingService;

        public EkshopApiController(ILogger logger,
            IEkshopEpService esEpService,
            IProductModelFactory productModelFactory,
            IStoreContext storeContext,
            EkshopSettings ekShopA2ISettings,
            ISettingService settingService)
        {
            this._logger = logger;
            this._esEpService = esEpService;
            this._productModelFactory = productModelFactory;
            this._storeContext = storeContext;
            this._ekShopA2ISettings = ekShopA2ISettings;
            this._settingService = settingService;
        }

        [Route("api/es/placeorder")]
        [HttpPost]
        public IHttpActionResult PlaceOrder(PlaceOrderRootModel model)
        {
            if (_ekShopA2ISettings.EnableLog)
                _logger.Information("Ekshop order place started");

            var responseModel = _esEpService.PlaceEkShopOrder(model);

            if (_ekShopA2ISettings.EnableLog)
                _logger.Information("Ekshop order place ended");

            return Ok(responseModel);
        }

        [Route("api/es/search")]
        [HttpGet]
        public IHttpActionResult Search(string search_query = "", string orderway = "desc", int limit = 10, int offset = 0)
        {
            if (_ekShopA2ISettings.EnableLog)
                _logger.Information("Ekshop search started");

            if (!string.IsNullOrWhiteSpace(search_query) && HttpContext.Current.Server.UrlDecode(search_query).Contains("/"))
            {
                var tK = HttpContext.Current.Server.UrlDecode(search_query).Split('/');
                search_query = tK[tK.Length - 1].Split('?')[0].Split('/')[0];
            }

            var pageSize = limit < 1 ? 1 : limit;
            var pageNumber = limit > 0 ? offset / limit + 1 : 1;

            var filterModel = new AlgoliaPagingFilteringModel()
            {
                q = search_query,
                IncludeEkshopProducts = true,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var products = _productModelFactory.SearchProducts(filterModel);
            var model = new SearchApiModel();

            foreach (var product in products)
            {
                model.response.products.Add(new SearchProductModel()
                {
                    product_name = product.Name,
                    product_price = product.Price,
                    product_url = new Uri(new Uri(_storeContext.CurrentStore.Url), product.SeName).AbsoluteUri,
                    product_image = product.DefaultPictureModel.ImageUrl
                });
            }

            if (_ekShopA2ISettings.EnableLog)
                _logger.Information("Ekshop search ended");

            return Ok(model);
        }
    }
}
