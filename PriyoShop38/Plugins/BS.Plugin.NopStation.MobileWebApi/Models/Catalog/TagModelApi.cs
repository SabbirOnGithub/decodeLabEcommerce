using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Catalog
{
    public class TagModelApi:CategoryBaseModelApi
    {
        public TagModelApi()
        {
            Products = new List<ProductOverViewModelApi>();
            PagingFilteringContext = new CatalogPagingFilteringModel();
           
        }

        public string TagName { get; set; }
        
        public CatalogPagingFilteringModel PagingFilteringContext { get; set; }

        public IList<ProductOverViewModelApi> Products { get; set; }
        
    }
}
