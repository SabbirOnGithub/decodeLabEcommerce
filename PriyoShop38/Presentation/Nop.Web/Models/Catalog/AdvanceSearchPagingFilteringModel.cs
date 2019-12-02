using Nop.Web.Framework.UI.Paging;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Web.Models.Catalog
{
    public class AdvanceSearchPagingFilteringModel : BasePageableModel
    {
        #region Ctor

        public AdvanceSearchPagingFilteringModel()
        {
            this.AvailableSortOptions = new List<SelectListItem>();
            this.AvailableViewModes = new List<SelectListItem>();
            this.PageSizeOptions = new List<SelectListItem>();
            this.AvailableRatings = new List<SelectListItemDetails>();
            this.AvailableVendors = new List<SelectListItemDetails>();
            this.AvailableSpecifications = new List<SelectListItemDetails>();
            this.AvailableCategories = new List<SelectListItemDetails>();
            this.AvailableManufacturers = new List<SelectListItemDetails>();
            this.PriceRange = new PriceRangeModel();
        }

        #endregion

        #region Properties

        public bool AllowProductSorting { get; set; }
        public IList<SelectListItem> AvailableSortOptions { get; set; }

        public bool AllowProductViewModeChanging { get; set; }
        public IList<SelectListItem> AvailableViewModes { get; set; }

        public bool AllowCustomersToSelectPageSize { get; set; }
        public IList<SelectListItem> PageSizeOptions { get; set; }
        
        public bool AllowRatingFilter { get; set; }
        public IList<SelectListItemDetails> AvailableRatings { get; set; }

        public bool AllowVendorFilter { get; set; }
        public IList<SelectListItemDetails> AvailableVendors { get; set; }

        public bool AllowSpecificationFilter { get; set; }
        public IList<SelectListItemDetails> AvailableSpecifications { get; set; }

        public bool AllowCategoryFilter { get; set; }
        public IList<SelectListItemDetails> AvailableCategories { get; set; }

        public bool AllowManufacturerFilter { get; set; }
        public IList<SelectListItemDetails> AvailableManufacturers { get; set; }

        public bool AllowPriceRangeFilter { get; set; }
        public PriceRangeModel PriceRange { get; set; }

        public bool AllowEmiFilter { get; set; }
        public bool EmiProductOnly { get; set; }


        public int? OrderBy { get; set; }

        public string ViewMode { get; set; }
        public string EmiUrl { get; set; }

        #endregion

        #region Nested class

        public class SelectListItemDetails
        {
            public int Count { get; set; }

            public string GroupName { get; set; }

            public bool Selected { get; set; }

            public string Text { get; set; }

            public string Value { get; set; }
        }

        public class PriceRangeModel
        {
            public string CurrencySymbol { get; set; }

            public decimal MinPrice { get; set; }

            public decimal MaxPrice { get; set; }

            public decimal CurrentMinPrice { get; set; }

            public decimal CurrentMaxPrice { get; set; }
        }

        #endregion
    }
}