using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Models.Media;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Catalog
{
    public class FreeDeliveryModelApi : CategoryBaseModelApi
    {
        public FreeDeliveryModelApi()
        {
            PictureModel = new PictureModel();
            Products = new List<ProductOverViewModelApi>();
            PagingFilteringContext = new CatalogPagingFilteringModel();
        }

        public string Description { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        
        public PictureModel PictureModel { get; set; }

        public CatalogPagingFilteringModel PagingFilteringContext { get; set; }

        public IList<ProductOverViewModelApi> Products { get; set; }
    }
}
