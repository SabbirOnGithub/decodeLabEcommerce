using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using Nop.Web.Models.Media;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Catalog
{
    public class VendorResponseModel : BaseResponse
    {
        public VendorResponseModel()
        {
            LogoModel = new PictureModel();
            PictureModel = new PictureModel();
            Products = new List<ProductOverViewModelApi>();
            PagingFilteringContext = new CatalogPagingFilteringModel();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        public bool AllowCustomersToContactVendors { get; set; }

        public PictureModel PictureModel { get; set; }

        public IList<ProductOverViewModelApi> Products { get; set; }

        public CatalogPagingFilteringModel PagingFilteringContext { get; set; }

        public PictureModel LogoModel { get; set; }
    }
}
