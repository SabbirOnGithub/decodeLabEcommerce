using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Catalog
{
    public class ProductTagDetailResponseModel
    {
        public ProductTagDetailResponseModel()
        {
            Products = new List<ProductOverViewModelApi>();
            NotFilteredItems = new List<CatalogPagingFilteringModel.SpecificationFilterItem>();
            AlreadyFilteredItems = new List<CatalogPagingFilteringModel.SpecificationFilterItem>();
            AvailableSortOptions = new List<SelectListItem>();
        }
        public string Name { get; set; }
        public IList<ProductOverViewModelApi> Products { get; set; }
        public IList<CatalogPagingFilteringModel.SpecificationFilterItem> NotFilteredItems { get; set; }
        public IList<CatalogPagingFilteringModel.SpecificationFilterItem> AlreadyFilteredItems { get; set; }
        public int TotalPages { get; set; }
        public IList<SelectListItem> AvailableSortOptions { get; set; }
    }
}
