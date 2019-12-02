using Nop.Core.Domain.Catalog;
using Nop.Plugin.Search.Elastic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Search.Elastic.Services
{
   public partial interface IElasticSearchService
    {

        bool ReIndex();
        bool SyncProducts();
        ElasticSearchResults Search(string searchKey,int @from = 0, int size = 24 );
        IList<Product> GetProductsByIds(int[] productIds);
    }
}
