using Nop.Web.Models.Catalog;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.AlgoliaSearch.Models.AlgoliaProduct
{
    public class AlgoliaProductOverviewModel : ProductOverviewModel
    {
        public AlgoliaProductOverviewModel(ProductOverviewModel obj)
        {
            foreach (var prop in obj.GetType().GetProperties())
            {
                this.GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(obj, null), null);
            }

            Specifications = new List<AlgoliaSpecificationModel>();
            Categories = new List<AlgoliaCategoryModel>();
            Manufacturers = new List<AlgoliaManufacturerModel>();
            Vendor = new AlgoliaVendorModel();
        }

        public AlgoliaProductOverviewModel()
        {
            Specifications = new List<AlgoliaSpecificationModel>();
            Categories = new List<AlgoliaCategoryModel>();
            Manufacturers = new List<AlgoliaManufacturerModel>();
            Vendor = new AlgoliaVendorModel();
        }

        public string AutoCompleteImageUrl { get; set; }

        public decimal OldPrice { get; set; }

        public decimal Price { get; set; }

        public bool EnableEmi { get; set; }  

        public IList<AlgoliaSpecificationModel> Specifications { get; set; }

        public IList<AlgoliaCategoryModel> Categories { get; set; }

        public IList<AlgoliaManufacturerModel> Manufacturers { get; set; }

        public AlgoliaVendorModel Vendor { get; set; }

        public string objectID { get; set; }

        public bool DisableWishlistButton { get; set; }

        public bool DisableBuyButton { get; set; }

        public int Rating { get; set; }

        public string Sku { get; set; }

        public bool SoldOut { get; set; }

        public bool EkshopOnly { get; set; }
    }
}
