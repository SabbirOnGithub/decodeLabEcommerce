using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Tax;
using System.Collections.Generic;

namespace Nop.Services.Discounts
{
    public interface IPurchaseOfferService
    {
        void InsertPurchaseOffer(PurchaseOffer purchaseOffer);

        void UpdatePurchaseOffer(PurchaseOffer purchaseOffer);

        void DeletePurchaseOffer(PurchaseOffer purchaseOffer);

        PurchaseOffer GetPurchaseOfferById(int purchaseOfferId);

        PurchaseOffer GetCurrentPurchaseOffer(Customer customer, TaxDisplayType taxDisplayType,
            Currency workingCurrency);

        IPagedList<PurchaseOffer> GetAllPurchaseOffers(bool showHidden = false,
            PurchaseOfferSortingEnum orderBy = PurchaseOfferSortingEnum.Default, 
            int pageIndex = 0, int pageSize = int.MaxValue);

        void DeleteAppliedProduct(PurchaseOfferProduct purchaseOfferProduct);

        void DeleteAppliedProduct(ICollection<PurchaseOfferProduct> purchaseOfferProducts);

        void DeleteAppliedManufacturer(PurchaseOfferManufacturer purchaseOfferManufacturer);

        void DeleteAppliedManufacturer(ICollection<PurchaseOfferManufacturer> purchaseOfferManufacturers);

        void DeleteAppliedVendor(PurchaseOfferVendor purchaseOfferVendor);

        void DeleteAppliedVendor(ICollection<PurchaseOfferVendor> purchaseOfferVendors);

        void DeleteAppliedCategory(PurchaseOfferCategory purchaseOfferCategory);

        void DeleteAppliedCategory(ICollection<PurchaseOfferCategory> purchaseOfferCategories);

        PurchaseOfferUsageHistory GetUsageHistoryByOrderId(int orderid);

        IPagedList<PurchaseOfferUsageHistory> GetPurchaseOfferUsageHistory(int purchaseOfferId, int pageIndex = 0, int pageSize = int.MaxValue);

        PurchaseOfferUsageHistory GetPurchaseOfferUsageHistoryById(int id);

        void DeletePurchaseOffeUsageHistory(PurchaseOfferUsageHistory pouh);
    }
}