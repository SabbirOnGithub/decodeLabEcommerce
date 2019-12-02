using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Nop.Core.Domain.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Catalog
{
    public class CategoryDetailProductResponseModel : BaseResponse
    {
        public CategoryDetailProductResponseModel()
        {
            Products = new List<ProductOverViewModelApi>();
            NotFilteredItems=new List<CatalogPagingFilteringModel.SpecificationFilterItem>();
            AlreadyFilteredItems = new List<CatalogPagingFilteringModel.SpecificationFilterItem>();
            FilterItems = new List<CatalogPagingFilteringModel.FilterItem>();
            PriceRange = new PriceRange();
            AvailableSortOptions= new List<SelectListItem>();
        }

        public string Name { get; set; }
        public PriceRange PriceRange { get; set; }
        public IList<ProductOverViewModelApi> Products { get; set; }
        public IList<CatalogPagingFilteringModel.SpecificationFilterItem> NotFilteredItems { get; set; }
        public IList<CatalogPagingFilteringModel.SpecificationFilterItem> AlreadyFilteredItems { get; set; }
        public IList<CatalogPagingFilteringModel.FilterItem> FilterItems { get; set; }
        public IList<SelectListItem> AvailableSortOptions { get; set; }
        public int TotalPages { get; set; }
    }

}
