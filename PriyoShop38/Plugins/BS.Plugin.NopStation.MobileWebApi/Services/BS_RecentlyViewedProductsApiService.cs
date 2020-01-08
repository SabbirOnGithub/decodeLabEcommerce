using BS.Plugin.NopStation.MobileWebApi.Domain;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Services.Catalog;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public partial class BS_RecentlyViewedProductsApiService : IBS_RecentlyViewedProductsApiService
    {
        private readonly IRepository<BS_RecentlyViewedProductsApi> _bsRecentlyViewedProductsApi;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly CatalogSettings _catalogSettings;

        public BS_RecentlyViewedProductsApiService(IRepository<BS_RecentlyViewedProductsApi> bsRecentlyViewedProductsApi,
            IProductService productService,
            IWorkContext workContext,
            IEventPublisher eventPublisher,
            CatalogSettings catalogSettings)
        {
            this._bsRecentlyViewedProductsApi = bsRecentlyViewedProductsApi;
            this._productService = productService;
            this._workContext = workContext;
            this._eventPublisher = eventPublisher;
            this._catalogSettings = catalogSettings;
        }

        #region Utilities
        protected IList<int> GetRecentlyViewedProductsIds()
        {
            return GetRecentlyViewedProductsIds(int.MaxValue);
        }

        protected IList<int> GetRecentlyViewedProductsIds(int number)
        {
            if (_workContext.CurrentCustomer == null)
                return null;            

            var productIds = new List<int>();

            var query = from rvpi in _bsRecentlyViewedProductsApi.Table
                where rvpi.CustomerId == _workContext.CurrentCustomer.Id
                select rvpi.ProductId;
            if (!query.Any())
                return productIds;

            productIds = query.ToList();
            if (productIds.Count > number)
                productIds = productIds.Take(number).ToList();

            return productIds;
        }
        #endregion

        #region Methods
        public virtual IList<Product> GetRecentlyViewedProducts(int number)
        {
            var products = new List<Product>();
            var productIds = GetRecentlyViewedProductsIds(number);
            foreach (var product in _productService.GetProductsByIds(productIds.ToArray()))
                if (product.Published && !product.Deleted)
                    products.Add(product);
            return products;
        }

        public virtual void AddProductToRecentlyViewedList(int productId)
        {
            if (!_catalogSettings.RecentlyViewedProductsEnabled)
                return;

            if(_workContext.CurrentCustomer == null)
                return;

            var oldProductIds = GetRecentlyViewedProductsIds();
            var alreadyExists = oldProductIds.Contains(productId);

            if (!alreadyExists)
            {
                var entity = new BS_RecentlyViewedProductsApi
                {
                    CustomerId = _workContext.CurrentCustomer.Id,
                    ProductId = productId
                };

                _bsRecentlyViewedProductsApi.Insert(entity);

                //event notification
                _eventPublisher.EntityInserted(entity);
            }
        }
        #endregion
    }
}
