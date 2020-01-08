using Nop.Web.Framework.UI.Paging;
using System.Collections.Generic;

namespace BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Catalog
{
    public class PagingFilteringModel : BasePageableModel
    {
        public PagingFilteringModel()
        {
            SelectedCategories = new List<int>();
            SelectedManufacturers = new List<int>();
            SelectedRatings = new List<int>();
            SelectedSpecifications = new List<string>();
            SelectedVendors = new List<int>();
        }

        public int CategoryId { get; set; }

        public int ManufacturerId { get; set; }

        public int VendorId { get; set; }

        public string q { get; set; }

        public List<int> SelectedManufacturers { get; set; }

        public List<int> SelectedVendors { get; set; }

        public List<int> SelectedCategories { get; set; }

        public List<string> SelectedSpecifications { get; set; }

        public List<int> SelectedRatings { get; set; }

        public decimal MaxPrice { get; set; }

        public decimal MinPrice { get; set; }

        public bool EmiProductsOnly { get; set; }

        public int? OrderBy { get; set; }

        public string ViewMode { get; set; }

        public bool IncludeEkshopProducts { get; set; }
    }
}
