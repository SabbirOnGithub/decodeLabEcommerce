using Nop.Web.Framework;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Plugin.Purchase.Offer.ViewModel
{
    public class AddOptionModel : PurchaseOfferOptionViewModel
    {
        public AddOptionModel()
        {
            AvailableCategories = new List<SelectListItem>();
            AvailableManufacturers = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();
            AvailableProductTypes = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Purchase.Offer.SearchProductName")]
        public string SearchProductName { get; set; }
        [NopResourceDisplayName("Admin.Purchase.Offer.SearchCategoryId")]
        public int SearchCategoryId { get; set; }
        [NopResourceDisplayName("Admin.Purchase.Offer.SearchManufacturerId")]
        public int SearchManufacturerId { get; set; }
        [NopResourceDisplayName("Admin.Purchase.Offer.SearchStoreId")]
        public int SearchStoreId { get; set; }
        [NopResourceDisplayName("Admin.Purchase.Offer.SearchVendorId")]
        public int SearchVendorId { get; set; }
        [NopResourceDisplayName("Admin.Purchase.Offer.SearchProductTypeId")]
        public int SearchProductTypeId { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }
        public IList<SelectListItem> AvailableManufacturers { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }
        public IList<SelectListItem> AvailableProductTypes { get; set; }
    }
}
