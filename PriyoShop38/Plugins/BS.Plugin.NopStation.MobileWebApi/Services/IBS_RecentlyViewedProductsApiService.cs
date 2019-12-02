using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public interface IBS_RecentlyViewedProductsApiService
    {
        IList<Product> GetRecentlyViewedProducts(int number);
        void AddProductToRecentlyViewedList(int productId);
    }
}
