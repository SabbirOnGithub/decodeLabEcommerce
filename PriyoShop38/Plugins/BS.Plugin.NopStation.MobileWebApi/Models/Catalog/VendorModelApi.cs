using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Catalog
{
    public class VendorModelApi : CategoryBaseModelApi
    {
        public VendorModelApi()
        {
            Products = new List<ProductOverViewModelApi>();
            PagingFilteringContext = new CatalogPagingFilteringModel();

        }

        //public string Name { get; set; }
        public string Description { get; set; }
        public bool AllowCustomersToContactVendors { get; set; }
        
        public CatalogPagingFilteringModel PagingFilteringContext { get; set; }

        public IList<ProductOverViewModelApi> Products { get; set; }
    }
}
