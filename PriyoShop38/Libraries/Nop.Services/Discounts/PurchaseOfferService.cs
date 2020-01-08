using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Orders;

namespace Nop.Services.Discounts
{
    public partial class PurchaseOfferService : IPurchaseOfferService
    {
        #region Constants
        
        private const string PURCHASE_OFFER_BY_ID_KEY = "Nop.purchase.offer.id-{0}";
        private const string PURCHASE_OFFER_ALL_KEY = "Nop.purchase.offer.all-{0}";
        private const string PURCHASE_OFFER_PATTERN_KEY = "Nop.purchase.offer.";

        #endregion

        #region Fields

        private readonly IRepository<PurchaseOffer> _purchaseOfferRepository;
        private readonly IRepository<PurchaseOfferUsageHistory> _pouhRepository;
        private readonly IRepository<PurchaseOfferProduct> _popRepository;
        private readonly IRepository<PurchaseOfferCategory> _pocRepository;
        private readonly IRepository<PurchaseOfferVendor> _povRepository;
        private readonly IRepository<PurchaseOfferManufacturer> _pomRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IDbContext _dbContext;
        private readonly HttpContextBase _httpContext;
        private readonly ICurrencyService _currencyService;
        private readonly TaxSettings _taxSettings;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;

        #endregion

        #region Ctor

        public PurchaseOfferService(ICacheManager cacheManager,
            IStoreContext storeContext,
            IRepository<PurchaseOffer> purchaseOfferRepository,
            IRepository<PurchaseOfferUsageHistory> pouhRepository,
            IRepository<PurchaseOfferProduct> popRepository,
            IRepository<PurchaseOfferCategory> pocRepository,
            IRepository<PurchaseOfferVendor> povRepository,
            IRepository<PurchaseOfferManufacturer> pomRepository,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IEventPublisher eventPublisher,
            IDbContext dbContext,
            HttpContextBase httpContext,
            ICurrencyService currencyService,
            TaxSettings taxSettings,
            IOrderTotalCalculationService orderTotalCalculationService)
        {
            this._cacheManager = cacheManager;
            this._storeContext = storeContext;
            this._purchaseOfferRepository = purchaseOfferRepository;
            this._pouhRepository = pouhRepository;
            this._popRepository = popRepository;
            this._pocRepository = pocRepository;
            this._pomRepository = pomRepository;
            this._povRepository = povRepository;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._eventPublisher = eventPublisher;
            this._dbContext = dbContext;
            this._httpContext = httpContext;
            this._currencyService = currencyService;
            this._taxSettings = taxSettings;
            this._orderTotalCalculationService = orderTotalCalculationService;
        }

        #endregion

        #region Utilities

        protected bool ValidCartItem(Product product, PurchaseOffer purchaseOffer)
        {
            if (purchaseOffer.ForAllProducts)
                return true;

            if (purchaseOffer.AppliedToVendors.Any(x => x.VendorId == product.VendorId))
                return true;

            if (purchaseOffer.AppliedToProducts.Any(x => x.ProductId == product.Id))
                return true;

            if (purchaseOffer.AppliedToManufacturers.Any())
            {
                foreach (var appliedManufacturer in purchaseOffer.AppliedToManufacturers)
                {
                    if (product.ProductManufacturers.Any(x => x.ManufacturerId == appliedManufacturer.ManufacturerId))
                        return true;
                }
            }

            if (purchaseOffer.AppliedToCategories.Any())
            {
                foreach (var appliedCategory in purchaseOffer.AppliedToCategories)
                {
                    if (product.ProductCategories.Any(x => x.CategoryId == appliedCategory.CategoryId))
                        return true;
                }
            }

            return false;
        }
        #endregion

        #region Methods

        public void DeletePurchaseOffer(PurchaseOffer purchaseOffer)
        {
            if (purchaseOffer == null)
                throw new ArgumentNullException("purchaseOffer");

            _purchaseOfferRepository.Delete(purchaseOffer);

            _cacheManager.RemoveByPattern(PURCHASE_OFFER_PATTERN_KEY);
        }

        public IPagedList<PurchaseOffer> GetAllPurchaseOffers(bool showHidden = false,
            PurchaseOfferSortingEnum orderBy = PurchaseOfferSortingEnum.Default, 
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string key = string.Format(PURCHASE_OFFER_ALL_KEY, showHidden);
            var result = _cacheManager.Get(key, () =>
            {
                var query = _purchaseOfferRepository.Table;
                if (!showHidden)
                {
                    //The function 'CurrentUtcDateTime' is not supported by SQL Server Compact. 
                    //That's why we pass the date value
                    var nowUtc = DateTime.UtcNow;
                    query = query.Where(d =>
                        (!d.StartDateUtc.HasValue || d.StartDateUtc <= nowUtc)
                        && (!d.EndDateUtc.HasValue || d.EndDateUtc >= nowUtc)
                        && d.GiftProduct != null && !d.GiftProduct.Deleted
                        );
                }

                if(orderBy == PurchaseOfferSortingEnum.MinimumCartAmountAsc)
                    query = query.OrderBy(d => d.MinimumCartAmount);
                else if (orderBy == PurchaseOfferSortingEnum.MinimumCartAmountDesc)
                    query = query.OrderByDescending(d => d.MinimumCartAmount);
                else
                    query = query.OrderByDescending(d => d.Id);

                return new PagedList<PurchaseOffer>(query, pageIndex, pageSize);
            });

            return result;
        }

        public PurchaseOffer GetPurchaseOfferById(int purchaseOfferId)
        {
            if (purchaseOfferId == 0)
                return null;

            string key = string.Format(PURCHASE_OFFER_BY_ID_KEY, purchaseOfferId);
            return _cacheManager.Get(key, () => _purchaseOfferRepository.GetById(purchaseOfferId));
        }

        public void InsertPurchaseOffer(PurchaseOffer purchaseOffer)
        {
            if (purchaseOffer == null)
                throw new ArgumentNullException("purchaseOffer");

            _purchaseOfferRepository.Insert(purchaseOffer);

            _cacheManager.RemoveByPattern(PURCHASE_OFFER_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(purchaseOffer);
        }

        public void UpdatePurchaseOffer(PurchaseOffer purchaseOffer)
        {
            if (purchaseOffer == null)
                throw new ArgumentNullException("purchaseOffer");

            _purchaseOfferRepository.Update(purchaseOffer);

            _cacheManager.RemoveByPattern(PURCHASE_OFFER_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(purchaseOffer);
        }

        public void DeleteAppliedProduct(PurchaseOfferProduct purchaseOfferProduct)
        {
            _popRepository.Delete(purchaseOfferProduct);

            _cacheManager.RemoveByPattern(PURCHASE_OFFER_PATTERN_KEY);
        }

        public void DeleteAppliedProduct(ICollection<PurchaseOfferProduct> purchaseOfferProducts)
        {
            _popRepository.Delete(purchaseOfferProducts);

            _cacheManager.RemoveByPattern(PURCHASE_OFFER_PATTERN_KEY);
        }

        public void DeleteAppliedManufacturer(PurchaseOfferManufacturer purchaseOfferManufacturer)
        {
            _pomRepository.Delete(purchaseOfferManufacturer);

            _cacheManager.RemoveByPattern(PURCHASE_OFFER_PATTERN_KEY);
        }

        public void DeleteAppliedManufacturer(ICollection<PurchaseOfferManufacturer> purchaseOfferManufacturers)
        {
            _pomRepository.Delete(purchaseOfferManufacturers);

            _cacheManager.RemoveByPattern(PURCHASE_OFFER_PATTERN_KEY);
        }

        public void DeleteAppliedVendor(PurchaseOfferVendor purchaseOfferVendor)
        {
            _povRepository.Delete(purchaseOfferVendor);

            _cacheManager.RemoveByPattern(PURCHASE_OFFER_PATTERN_KEY);
        }

        public void DeleteAppliedVendor(ICollection<PurchaseOfferVendor> purchaseOfferVendors)
        {
            _povRepository.Delete(purchaseOfferVendors);

            _cacheManager.RemoveByPattern(PURCHASE_OFFER_PATTERN_KEY);
        }

        public void DeleteAppliedCategory(PurchaseOfferCategory purchaseOfferCategory)
        {
            _pocRepository.Delete(purchaseOfferCategory);

            _cacheManager.RemoveByPattern(PURCHASE_OFFER_PATTERN_KEY);
        }

        public void DeleteAppliedCategory(ICollection<PurchaseOfferCategory> purchaseOfferCategories)
        {
            _pocRepository.Delete(purchaseOfferCategories);

            _cacheManager.RemoveByPattern(PURCHASE_OFFER_PATTERN_KEY);
        }

        public PurchaseOffer GetCurrentPurchaseOffer(Customer customer, TaxDisplayType taxDisplayType,
            Currency workingCurrency)
        {
            var purchaseOffers = GetAllPurchaseOffers(orderBy: PurchaseOfferSortingEnum.MinimumCartAmountDesc);

            if (!purchaseOffers.Any())
                return null;

            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            PurchaseOffer purchaseOffer = null;

            foreach (var po in purchaseOffers)
            {
                var tempCart = cart.Where(x => ValidCartItem(x.Product, po)).ToList();

                decimal orderSubTotalDiscountAmountBase;
                List<Discount> orderSubTotalAppliedDiscounts;
                decimal subTotalWithoutDiscountBase;
                decimal subTotalWithDiscountBase;
                var subTotalIncludingTax = taxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
                _orderTotalCalculationService.GetShoppingCartSubTotal(tempCart, subTotalIncludingTax,
                    out orderSubTotalDiscountAmountBase, out orderSubTotalAppliedDiscounts,
                    out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
                decimal subtotalBase = subTotalWithoutDiscountBase;
                decimal subtotal = _currencyService.ConvertFromPrimaryStoreCurrency(subtotalBase, workingCurrency);

                if (po.MinimumCartAmount <= subtotal)
                {
                    purchaseOffer = po;
                    break;
                }
            }

            return purchaseOffer;
        }

        public PurchaseOfferUsageHistory GetUsageHistoryByOrderId(int orderid)
        {
            return _pouhRepository.Table.FirstOrDefault(x => x.Order.Id == orderid);
        }

        public IPagedList<PurchaseOfferUsageHistory> GetPurchaseOfferUsageHistory(int purchaseOfferId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _pouhRepository.Table.Where(x => x.PurchaseOfferId == purchaseOfferId);
            query = query.OrderByDescending(x => x.CreatedOnUtc);

            return new PagedList<PurchaseOfferUsageHistory>(query, pageIndex, pageSize);
        }

        public PurchaseOfferUsageHistory GetPurchaseOfferUsageHistoryById(int id)
        {
            return _pouhRepository.GetById(id);
        }

        public void DeletePurchaseOffeUsageHistory(PurchaseOfferUsageHistory pouh)
        {
            _pouhRepository.Delete(pouh);
        }

        #endregion
    }
}
