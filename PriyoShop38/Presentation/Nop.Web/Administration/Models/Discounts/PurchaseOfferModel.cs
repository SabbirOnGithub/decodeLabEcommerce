using FluentValidation.Attributes;
using Nop.Admin.Validators.Discounts;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Nop.Admin.Models.Discounts
{
    [Validator(typeof(PurchaseOfferValidator))]
    public class PurchaseOfferModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Promotions.PurchaseOffer.Fields.MinimumCartAmount")]
        public decimal MinimumCartAmount { get; set; }

        [NopResourceDisplayName("Admin.Promotions.PurchaseOffer.Fields.GiftProductId")]
        public int GiftProductId { get; set; }

        [NopResourceDisplayName("Admin.Promotions.PurchaseOffer.Fields.GiftProduct")]
        public string GiftProductName { get; set; }

        [NopResourceDisplayName("Admin.Promotions.PurchaseOffer.Fields.PictureUrl")]
        public string PictureUrl { get; set; }

        [NopResourceDisplayName("Admin.Promotions.PurchaseOffer.Fields.Quantity")]
        public int Quantity { get; set; }

        [NopResourceDisplayName("Admin.Promotions.PurchaseOffer.Fields.StartDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? StartDateUtc { get; set; }

        [NopResourceDisplayName("Admin.Promotions.PurchaseOffer.Fields.EndDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? EndDateUtc { get; set; }

        [NopResourceDisplayName("Admin.Promotions.PurchaseOffer.Fields.ForAllProducts")]
        public bool ForAllProducts { get; set; }


        #region Nested class

        public class AppliedToProductModel : BaseNopEntityModel
        {
            public int ProductId { get; set; }

            public string ProductName { get; set; }
        }

        public partial class AddProductToPurchaseOfferModel : BaseNopModel
        {
            public AddProductToPurchaseOfferModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
                AvailableStores = new List<SelectListItem>();
                AvailableVendors = new List<SelectListItem>();
                AvailableProductTypes = new List<SelectListItem>();
            }

            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
            [AllowHtml]
            public string SearchProductName { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
            public int SearchCategoryId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchManufacturer")]
            public int SearchManufacturerId { get; set; }
            [NopResourceDisplayName("Admin.PurchaseOffer.Products.List.SearchStore")]
            public int SearchStoreId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchVendor")]
            public int SearchVendorId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductType")]
            public int SearchProductTypeId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableManufacturers { get; set; }
            public IList<SelectListItem> AvailableStores { get; set; }
            public IList<SelectListItem> AvailableVendors { get; set; }
            public IList<SelectListItem> AvailableProductTypes { get; set; }

            public int PurchaseOfferId { get; set; }
            public int[] SelectedProductIds { get; set; }
        }

        public class AppliedToManufacturerModel : BaseNopEntityModel
        {
            public int ManufacturerId { get; set; }

            public string ManufacturerName { get; set; }
        }

        public partial class AddManufacturerToPurchaseOfferModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Catalog.Manufacturers.List.SearchManufacturerName")]
            [AllowHtml]
            public string SearchManufacturerName { get; set; }

            public int PurchaseOfferId { get; set; }

            public int[] SelectedManufacturerIds { get; set; }
        }

        public class AppliedToCategoryModel : BaseNopEntityModel
        {
            public int CategoryId { get; set; }

            public string CategoryName { get; set; }
        }

        public partial class AddCategoryToPurchaseOfferModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Catalog.Categories.List.SearchCategoryName")]
            [AllowHtml]
            public string SearchCategoryName { get; set; }

            public int PurchaseOfferId { get; set; }

            public int[] SelectedCategoryIds { get; set; }
        }

        public class AppliedToVendorModel : BaseNopEntityModel
        {
            public int VendorId { get; set; }

            public string VendorName { get; set; }
        }

        public class AddVendorToPurchaseOfferModel
        {
            public string SearchVendorName { get; set; }

            public int PurchaseOfferId { get; set; }

            public int[] SelectedVendorIds { get; set; }
        }

        public partial class PurchaseOfferHistoryModel : BaseNopEntityModel
        {
            public int PurchaseOfferId { get; set; }

            [NopResourceDisplayName("Admin.Promotions.PurchaseOffers.History.Order")]
            public int OrderId { get; set; }

            [NopResourceDisplayName("Admin.Promotions.PurchaseOffers.History.OrderTotal")]
            public string OrderTotal { get; set; }

            [NopResourceDisplayName("Admin.Promotions.PurchaseOffers.History.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }

        #endregion
    }
}