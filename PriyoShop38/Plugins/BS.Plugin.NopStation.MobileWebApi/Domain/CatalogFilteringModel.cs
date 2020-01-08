using System.Collections.Generic;

namespace BS.Plugin.NopStation.MobileWebApi.Domain
{
    public class CatalogFilteringModel
    {
        public CatalogFilteringModel()
        {
            AvailableRatings = new List<SelectListItemDetails>();
            AvailableVendors = new List<SelectListItemDetails>();
            AvailableSpecifications = new List<SelectListItemDetails>();
            AvailableCategories = new List<SelectListItemDetails>();
            AvailableManufacturers = new List<SelectListItemDetails>();
        }

        public IList<SelectListItemDetails> AvailableRatings { get; set; }

        public IList<SelectListItemDetails> AvailableVendors { get; set; }

        public IList<SelectListItemDetails> AvailableSpecifications { get; set; }

        public IList<SelectListItemDetails> AvailableCategories { get; set; }

        public IList<SelectListItemDetails> AvailableManufacturers { get; set; }

        public decimal MaxPrice { get; set; }

        public decimal MinPrice { get; set; }

        public bool EmiProductsAvailable { get; set; }


        public class SelectListItemDetails
        {
            public int Count { get; set; }

            public string Text { get; set; }

            public string Value { get; set; }
        }
    }
}
