using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Infrastructure.Cache;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Core.Domain.Localization;
using Nop.Web.Models.Media;
using Nop.Core.Domain.Tax;
using BS.Plugin.NopStation.MobileWebApi.PluginSettings;
using Newtonsoft.Json;
using BS.Plugin.NopStation.MobileWebApi.Models;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Banner;
using BS.Plugin.NopStation.MobileWebApi.Models.HomePage;
using Nop.Plugin.Widgets.AlgoliaSearch.Factories;
using Nop.Plugin.Widgets.AlgoliaSearch.Models;
using VendorModel = BS.Plugin.NopStation.MobileWebApi.Models.Vendor.VendorModel;
using VendorNavigationModel = BS.Plugin.NopStation.MobileWebApi.Models.Vendor.VendorNavigationModel;
using VendorBriefInfoModel = BS.Plugin.NopStation.MobileWebApi.Models.Vendor.VendorBriefInfoModel;
using Nop.Plugin.Widgets.AlgoliaSearch;
using Nop.Web.Models.Catalog;
using CatalogPagingFilteringModel = BS.Plugin.NopStation.MobileWebApi.Models.Catalog.CatalogPagingFilteringModel;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class HomePageApiController : WebApiController
    {
        #region Fields

        private int pageSize = 6;

        private readonly ICategoryService _categoryService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly ICacheManager _cacheManager;
        private readonly IPictureService _pictureService;
        private readonly MediaSettings _mediaSettings;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPermissionService _permissionService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ICurrencyService _currencyService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductService _productService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ITaxService _taxService;
        private readonly IProductServiceApi _productServiceApi;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ISearchTermService _searchTermService;
        private readonly IProductTagService _productTagService;
        private readonly IVendorService _vendorService;
        private readonly VendorSettings _vendorSettings;
        private readonly ICategoryIconService _categoryIconService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ILanguageService _languageService;
        private readonly ICustomerServiceApi _customerServiceApi;
        private readonly IOrderReportService _orderReportService;
        private readonly TaxSettings _taxSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly ApiSettings _apiSettings;
        private readonly IProductModelFactory _productModelFactory;
        private readonly AlgoliaSettings _algoliaSettings;
        private readonly IEventBannerService _eventBannerService;

        #endregion

        #region Ctor

        public HomePageApiController(ICategoryService categoryService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IWebHelper webHelper,
            ICacheManager cacheManager,
            IPictureService pictureService,
            MediaSettings mediaSettings,
            IManufacturerService manufacturerService,
            IPermissionService permissionService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            ICurrencyService currencyService,
            CatalogSettings catalogSettings,
            IPriceFormatter priceFormatter,
            IProductService productService,
            ISpecificationAttributeService specificationAttributeService,
            ILocalizationService localizationService,
            IPriceCalculationService priceCalculationService,
            ITaxService taxService,
            IProductServiceApi productServiceApi,
            IGenericAttributeService genericAttributeService,
            ISearchTermService searchTermService,
            IProductTagService productTagService,
            IVendorService vendorService,
            VendorSettings vendorSettings,
            ICategoryIconService categoryIconService,
            LocalizationSettings localizationSettings,
            ILanguageService languageService,
            ICustomerServiceApi customerServiceApi,
            IOrderReportService orderReportService,
            TaxSettings taxSettings,
            ShoppingCartSettings shoppingCartSettings,
            ApiSettings apiSettings,
            IProductModelFactory productModelFactory,
            AlgoliaSettings algoliaSettings,
            IEventBannerService eventBannerService)
        {
            this._categoryService = categoryService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._webHelper = webHelper;
            this._cacheManager = cacheManager;
            this._pictureService = pictureService;
            this._mediaSettings = mediaSettings;
            this._manufacturerService = manufacturerService;
            this._permissionService = permissionService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._currencyService = currencyService;
            this._catalogSettings = catalogSettings;
            this._priceFormatter = priceFormatter;
            this._productService = productService;
            this._specificationAttributeService = specificationAttributeService;
            this._localizationService = localizationService;
            this._priceCalculationService = priceCalculationService;
            this._taxService = taxService;
            this._productServiceApi = productServiceApi;
            this._genericAttributeService = genericAttributeService;
            this._searchTermService = searchTermService;
            this._productTagService = productTagService;
            this._vendorService = vendorService;
            this._vendorSettings = vendorSettings;
            this._categoryIconService = categoryIconService;
            this._localizationSettings = localizationSettings;
            this._languageService = languageService;
            this._customerServiceApi = customerServiceApi;
            this._orderReportService = orderReportService;
            this._taxSettings = taxSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._apiSettings = apiSettings;
            this._productModelFactory = productModelFactory;
            this._algoliaSettings = algoliaSettings;
            this._eventBannerService = eventBannerService;
        }

        #endregion

        #region Utility


        public static string GetRootUrl()
        {
            var tmpURi = HttpContext.Current.Request.Url;
            var tmpPort = tmpURi.Port > 0 && tmpURi.Port != 80 ? ":" + tmpURi.Port : "";
            var rootUrl = tmpURi.Scheme + "://" + tmpURi.Host + tmpPort + "/";
            return rootUrl;
        }

        [System.Web.Http.NonAction]
        protected AlgoliaPagingFilteringModel PrepareFilteringModel(SearchProductResponseModel model, CatalogPagingFilteringModel command)
        {
            var filteringModel = new AlgoliaPagingFilteringModel();

            IList<int> alreadyFilteredSpecOptionIds = model.PagingFilteringContext.SpecificationFilter.GetAlreadyFilteredSpecOptionIds(_webHelper);
            filteringModel.SelectedCategoryIds = model.PagingFilteringContext.CategoryFilter.GetAlreadyFilteredCategoryIds(_webHelper);
            filteringModel.SelectedManufacturerIds = model.PagingFilteringContext.ManufacturerFilter.GetAlreadyFilteredManufacturerIds(_webHelper);
            filteringModel.SelectedVendorIds = model.PagingFilteringContext.VendorFilter.GetAlreadyFilteredVendorIds(_webHelper);
            filteringModel.EmiProductsOnly = model.PagingFilteringContext.EmiFilter.GetEmiFilterStatus(_webHelper);
            filteringModel.PageSize = command.PageSize;
            filteringModel.PageNumber = command.PageNumber;

            var selectedPriceRange = model.PagingFilteringContext.PriceRangeFilter.GetSelectedPriceRange(_webHelper);
            if (selectedPriceRange != null)
            {
                if (selectedPriceRange.From.HasValue)
                    filteringModel.MinPrice = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.From.Value, _workContext.WorkingCurrency);

                if (selectedPriceRange.To.HasValue)
                    filteringModel.MaxPrice = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.To.Value, _workContext.WorkingCurrency);
            }

            filteringModel.EmiProductsOnly = model.PagingFilteringContext.EmiFilter.GetEmiFilterStatus(_webHelper);
            filteringModel.IncludeEkshopProducts = false;
            filteringModel.OrderBy = command.OrderBy;
            filteringModel.q = model.q;

            return filteringModel;
        }

        [System.Web.Http.NonAction]
        protected virtual void PreparePageSizeOptions(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command,
            bool allowCustomersToSelectPageSize, string pageSizeOptions, int fixedPageSize)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException("pagingFilteringModel");

            if (allowCustomersToSelectPageSize && pageSizeOptions != null)
            {
                var pageSizes = pageSizeOptions.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (pageSizes.Any())
                {
                    // get the first page size entry to use as the default (category page load) or if customer enters invalid value via query string
                    if (command.PageSize <= 0 || !pageSizes.Contains(command.PageSize.ToString()))
                    {
                        int temp;
                        if (int.TryParse(pageSizes.FirstOrDefault(), out temp))
                        {
                            if (temp > 0)
                            {
                                command.PageSize = temp;
                            }
                        }
                    }

                    foreach (var ps in pageSizes)
                    {
                        int temp;
                        if (!int.TryParse(ps, out temp))
                        {
                            continue;
                        }
                        if (temp <= 0)
                        {
                            continue;
                        }

                        pagingFilteringModel.PageSizeOptions.Add(new SelectListItem
                        {
                            Text = ps,
                            Value = ps,
                            Selected = ps.Equals(command.PageSize.ToString(), StringComparison.InvariantCultureIgnoreCase)
                        });
                    }

                    if (pagingFilteringModel.PageSizeOptions.Any())
                    {
                        pagingFilteringModel.PageSizeOptions = pagingFilteringModel.PageSizeOptions.OrderBy(x => int.Parse(x.Text)).ToList();
                        pagingFilteringModel.AllowCustomersToSelectPageSize = true;

                        if (command.PageSize <= 0)
                        {
                            command.PageSize = int.Parse(pagingFilteringModel.PageSizeOptions.FirstOrDefault().Text);
                        }
                    }
                }
            }
            else
            {
                //customer is not allowed to select a page size
                command.PageSize = fixedPageSize;
            }

            //ensure pge size is specified
            if (command.PageSize <= 0)
            {
                command.PageSize = fixedPageSize;
            }
        }

        //[System.Web.Http.NonAction]
        //protected void PrepareFilterAttributes(AdvanceSearchPagingFilteringModel pagingFilteringModel, PagingFilteringModel command)
        //{
        //    CatalogFilterings filteringModel = null;

        //    if (!string.IsNullOrWhiteSpace(command.q))
        //        filteringModel = _productModelFactory.GetAlgoliaFilterings(command.q);
        //    else
        //        filteringModel = _productService.SearchCatalogFilterings(command.CategoryId, command.ManufacturerId, command.VendorId);

        //    pagingFilteringModel.AllowEmiFilter = pagingFilteringModel.AllowEmiFilter && filteringModel.EmiProductsAvailable;
        //    pagingFilteringModel.AllowPriceRangeFilter = pagingFilteringModel.AllowPriceRangeFilter && filteringModel.MaxPrice > filteringModel.MinPrice;

        //    if (pagingFilteringModel.AllowCategoryFilter)
        //    {
        //        foreach (var category in filteringModel.AvailableCategories)
        //        {
        //            pagingFilteringModel.AvailableCategories.Add(new AdvanceSearchPagingFilteringModel.SelectListItemDetails
        //            {
        //                Count = category.Count,
        //                Text = category.Text,
        //                Value = category.Value
        //            });
        //        }
        //    }

        //    if (pagingFilteringModel.AllowManufacturerFilter)
        //    {
        //        foreach (var manufacturer in filteringModel.AvailableManufacturers)
        //        {
        //            pagingFilteringModel.AvailableManufacturers.Add(new AdvanceSearchPagingFilteringModel.SelectListItemDetails
        //            {
        //                Count = manufacturer.Count,
        //                Text = manufacturer.Text,
        //                Value = manufacturer.Value
        //            });
        //        }
        //    }

        //    if (pagingFilteringModel.AllowVendorFilter)
        //    {
        //        foreach (var vendor in filteringModel.AvailableVendors)
        //        {
        //            pagingFilteringModel.AvailableVendors.Add(new AdvanceSearchPagingFilteringModel.SelectListItemDetails
        //            {
        //                Count = vendor.Count,
        //                Text = vendor.Text,
        //                Value = vendor.Value
        //            });
        //        }
        //    }

        //    if (pagingFilteringModel.AllowSpecificationFilter)
        //    {
        //        foreach (var spec in filteringModel.AvailableSpecifications)
        //        {
        //            pagingFilteringModel.AvailableSpecifications.Add(new AdvanceSearchPagingFilteringModel.SelectListItemDetails
        //            {
        //                GroupName = spec.GroupName,
        //                Count = spec.Count,
        //                Text = spec.Text,
        //                Value = spec.Value
        //            });
        //        }
        //    }

        //    if (pagingFilteringModel.AllowRatingFilter)
        //    {
        //        foreach (var rate in filteringModel.AvailableRatings)
        //        {
        //            pagingFilteringModel.AvailableRatings.Add(new AdvanceSearchPagingFilteringModel.SelectListItemDetails
        //            {
        //                Count = rate.Count,
        //                Text = rate.Text,
        //                Value = rate.Value
        //            });
        //        }
        //    }

        //    if (pagingFilteringModel.AllowPriceRangeFilter)
        //    {
        //        pagingFilteringModel.PriceRange.MaxPrice = filteringModel.MaxPrice;
        //        pagingFilteringModel.PriceRange.MinPrice = filteringModel.MinPrice;
        //        pagingFilteringModel.PriceRange.CurrentMaxPrice = filteringModel.MaxPrice;
        //        pagingFilteringModel.PriceRange.CurrentMinPrice = filteringModel.MinPrice;
        //        pagingFilteringModel.PriceRange.CurrencySymbol = "Tk";
        //    }
        //}

        //[System.Web.Http.NonAction]
        //protected void PrepareFilterOptions(AdvanceSearchPagingFilteringModel pagingFilteringContext, PagingFilteringModel command)
        //{
        //    if (!string.IsNullOrEmpty(command.q))
        //    {
        //        pagingFilteringContext.AllowProductSorting = _algoliaSettings.AllowProductSorting;
        //        pagingFilteringContext.AllowCustomersToSelectPageSize = _algoliaSettings.AllowCustomersToSelectPageSize;
        //        pagingFilteringContext.AllowProductViewModeChanging = _algoliaSettings.AllowProductViewModeChanging;
        //        pagingFilteringContext.AllowPriceRangeFilter = _algoliaSettings.AllowPriceRangeFilter;
        //        pagingFilteringContext.AllowVendorFilter = _algoliaSettings.AllowVendorFilter;
        //        pagingFilteringContext.AllowEmiFilter = _algoliaSettings.AllowEmiFilter;
        //        pagingFilteringContext.AllowRatingFilter = _algoliaSettings.AllowRatingFilter;
        //        pagingFilteringContext.AllowCategoryFilter = _algoliaSettings.AllowCategoryFilter;
        //        pagingFilteringContext.AllowSpecificationFilter = _algoliaSettings.AllowSpecificationFilter;
        //        pagingFilteringContext.AllowManufacturerFilter = _algoliaSettings.AllowManufacturerFilter;
        //    }
        //    else
        //    {
        //        var allowToSelectPageSize = false;
        //        if (command.CategoryId > 0)
        //        {
        //            var category = _categoryService.GetCategoryById(command.CategoryId);
        //            if (category == null)
        //                throw new ArgumentNullException(nameof(category));

        //            allowToSelectPageSize = category.AllowCustomersToSelectPageSize;
        //        }
        //        else if (command.ManufacturerId > 0)
        //        {
        //            var manufacturer = _manufacturerService.GetManufacturerById(command.ManufacturerId);
        //            if (manufacturer == null)
        //                throw new ArgumentNullException(nameof(manufacturer));

        //            allowToSelectPageSize = manufacturer.AllowCustomersToSelectPageSize;
        //        }
        //        else if (command.VendorId > 0)
        //        {
        //            var vendor = _vendorService.GetVendorById(command.VendorId);
        //            if (vendor == null)
        //                throw new ArgumentNullException(nameof(vendor));

        //            allowToSelectPageSize = vendor.AllowCustomersToSelectPageSize;
        //        }

        //        pagingFilteringContext.AllowProductSorting = _catalogSettings.AllowProductSorting;
        //        pagingFilteringContext.AllowCustomersToSelectPageSize = allowToSelectPageSize;
        //        pagingFilteringContext.AllowProductViewModeChanging = _catalogSettings.AllowProductViewModeChanging;
        //        pagingFilteringContext.AllowPriceRangeFilter = true;
        //        pagingFilteringContext.AllowVendorFilter = command.VendorId == 0;
        //        pagingFilteringContext.AllowEmiFilter = true;
        //        pagingFilteringContext.AllowRatingFilter = true;
        //        pagingFilteringContext.AllowCategoryFilter = command.CategoryId == 0;
        //        pagingFilteringContext.AllowSpecificationFilter = true;
        //        pagingFilteringContext.AllowManufacturerFilter = true;
        //    }
        //}

        [System.Web.Http.NonAction]
        protected int GetAlgoliaSearchPageSize()
        {
            var pageSize = 0;
            if (_algoliaSettings.AllowCustomersToSelectPageSize)
            {
                if (!string.IsNullOrWhiteSpace(_algoliaSettings.SelectablePageSizes))
                {
                    pageSize = _algoliaSettings
                        .SelectablePageSizes
                        .Split(new[] { ',', ' ' })
                        .Select(int.Parse)
                        .FirstOrDefault();
                }
            }
            else
                pageSize = _algoliaSettings.PageSize;

            if (pageSize < 1)
                pageSize = this.pageSize;

            return pageSize;
        }

        [System.Web.Http.NonAction]
        protected virtual IEnumerable<ProductOverViewModelApi> PrepareProductOverviewModels(IEnumerable<Product> products,
            bool preparePriceModel = true, bool preparePictureModel = true,
            int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
            bool forceRedirectionAfterAddingToCart = false)
        {
            return products.PrepareProductOverviewModels(_workContext,
                _storeContext, _categoryService, _productService,
                _priceCalculationService, _priceFormatter, _permissionService,
                _localizationService, _taxService, _currencyService,
                _pictureService, _webHelper, _cacheManager,
                _catalogSettings, _mediaSettings,
                preparePriceModel, preparePictureModel,
                productThumbPictureSize, prepareSpecificationAttributes);
        }

        [System.Web.Http.NonAction]
        protected virtual List<int> GetChildCategoryIds(int parentCategoryId)
        {
            string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_CHILD_IDENTIFIERS_MODEL_KEY,
                parentCategoryId,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            return _cacheManager.Get(cacheKey, () =>
            {
                var categoriesIds = new List<int>();
                var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId);
                foreach (var category in categories)
                {
                    categoriesIds.Add(category.Id);
                    categoriesIds.AddRange(GetChildCategoryIds(category.Id));
                }
                return categoriesIds;
            });
        }


        [System.Web.Http.NonAction]
        protected virtual void PrepareSortingOptions(CatalogPagingFilteringModel pagingFilteringModel, int orderBy, IList<int> excludeSortItems = null)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException("pagingFilteringModel");

            var allDisabled = _catalogSettings.ProductSortingEnumDisabled.Count == Enum.GetValues(typeof(ProductSortingEnum)).Length;
            pagingFilteringModel.AllowProductSorting = _catalogSettings.AllowProductSorting && !allDisabled;

            var activeOptions = Enum.GetValues(typeof(ProductSortingEnum)).Cast<int>()
                .Except(excludeSortItems)
                .Select((idOption) =>
                {
                    int order;
                    return new KeyValuePair<int, int>(idOption, _catalogSettings.ProductSortingEnumDisplayOrder.TryGetValue(idOption, out order) ? order : idOption);
                })
                .OrderBy(x => x.Value);

            if (pagingFilteringModel.AllowProductSorting)
            {
                foreach (var option in activeOptions)
                {
                    var sortValue = ((ProductSortingEnum)option.Key).GetLocalizedEnum(_localizationService, _workContext);
                    pagingFilteringModel.AvailableSortOptions.Add(new SelectListItem
                    {
                        Text = sortValue,
                        Value = option.Key.ToString(),
                        Selected = option.Key == orderBy
                    });
                }
            }

        }

        protected IList<Product> GetProductsByCategoryId(int categoryId, int itemsNumber)
        {
            var categoryIds = new List<int> { categoryId };
            if (_catalogSettings.ShowProductsFromSubcategories)
            {
                //include subcategories
                categoryIds.AddRange(GetChildCategoryIds(categoryId));
            }
            //products
            var products = new List<Product>();
            var featuredProducts = _productService.SearchProducts(
                       categoryIds: new List<int> { categoryId },
                       storeId: _storeContext.CurrentStore.Id,
                       visibleIndividuallyOnly: true,
                       featuredProducts: true,
                       activeVendorOnly: true,
                       includeEkshopProducts: false);
            products.AddRange(featuredProducts);
            int remainingProducts = itemsNumber - products.Count();
            if (remainingProducts > 0)
            {
                IList<int> filterableSpecificationAttributeOptionIds;
                var extraProucts = _productService.SearchProducts(out filterableSpecificationAttributeOptionIds, false,
                categoryIds: categoryIds,
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                featuredProducts: false,
                orderBy: (ProductSortingEnum)15,
                pageSize: itemsNumber,
                pageIndex: 0,
                activeVendorOnly: true,
                includeEkshopProducts: false);
                products.AddRange(extraProucts);
            }
            return products;
        }

        protected IList<Product> GetAllProductsByCategoryId(int categoryId)
        {
            var categoryIds = new List<int> { categoryId };
            if (_catalogSettings.ShowProductsFromSubcategories)
            {
                //include subcategories
                categoryIds.AddRange(GetChildCategoryIds(categoryId));
            }
            //products
            var products = _productService.SearchProducts(
                       categoryIds: categoryIds,
                       storeId: _storeContext.CurrentStore.Id,
                       visibleIndividuallyOnly: true,
                       activeVendorOnly: true,
                       includeEkshopProducts: false);

            return products;
        }

        [System.Web.Http.NonAction]
        protected virtual IEnumerable<SubCategoryModelApi> PrepareCategoryFilterOnSale(IEnumerable<Product> products, int pictureSize)
        {
            List<SubCategoryModelApi> categoryList = new List<SubCategoryModelApi>();

            foreach (var item in products)
            {
                var cList = _categoryService.GetProductCategoriesByProductId(item.Id).ToList().Select(s => new SubCategoryModelApi()
                {
                    Id = s.CategoryId,
                    Name = s.Category.Name,
                    PictureModel = new PictureModel()
                    {
                        FullSizeImageUrl = _pictureService.GetPictureUrl(s.Category.PictureId),
                        ImageUrl = _pictureService.GetPictureUrl(s.Category.PictureId, pictureSize),
                        Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), s.Category.Name),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), s.Category.Name)
                    }

                });

                categoryList = categoryList.Union(cList).ToList();
            }

            return categoryList;
        }

        public List<CategoryNavigationModelApi> FlatToHierarchy(IEnumerable<CategoryNavigationModelApi> list, int parentId = 0)
        {
            return (from i in list
                    where i.ParentCategoryId == parentId
                    select new CategoryNavigationModelApi
                    {
                        Id = i.Id,
                        ParentCategoryId = i.ParentCategoryId,
                        Name = i.Name,
                        Extension = i.Extension,
                        ProductCount = i.ProductCount,
                        DisplayOrder = i.DisplayOrder,
                        ImageUrl = i.ImageUrl,
                        Children = FlatToHierarchy(list, i.Id)
                    }).ToList();
        }

        #endregion

        #region Action Method

        #region Homepage items api for Mobile

        [System.Web.Http.Route("api/homePageApi/loadAll")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult HomepageApiCategories(int? thumbPictureSize = null, bool forBLApp = false)
        {
            var homepageApiModel = new HomepageApiListModel();

            #region MainBanner

            

            #endregion

            //----- load Event Banner list -----//
            #region EventBanner

            var eventBannerList = _eventBannerService.GetAllEventBanners();
            var eventBannerVmList = new List<EventBannerResponseModel.EventBannerModel>();

            if (eventBannerList.Any())
                foreach (var eventBanner in eventBannerList)
                {
                    var eventBannerVm = new EventBannerResponseModel.EventBannerModel();
                    eventBannerVm.BannerName = eventBanner.BannerName;
                    eventBannerVm.CategoryId = eventBanner.CategoryId;
                    eventBannerVm.Id = eventBanner.Id;
                    eventBannerVm.CategoryName = _categoryService.GetCategoryById(eventBanner.CategoryId).Name;
                    eventBannerVm.PictureId = eventBanner.PictureId;
                    eventBannerVm.PictureUrl = GetRootUrl() + "content/images/eventBanner/" + eventBanner.PictureUrl;
                    eventBannerVmList.Add(eventBannerVm);
                }

            homepageApiModel.EventBannerModels = eventBannerVmList;

            #endregion


            if (forBLApp)
            {
                var blHomepageProducts = new HomepageApiListModel.BlHomepage();
                blHomepageProducts.EventBannerModels = eventBannerVmList;


                int mobileCatId = 117;
                var mobiles = GetProductsByCategoryId(mobileCatId, 10);
                var mobilesList = PrepareProductOverviewModels(mobiles);
                blHomepageProducts.MobileCategoryProducts = mobilesList.ToList();

                int laptopCatId = 195;
                var laptops = GetProductsByCategoryId(laptopCatId, 10);
                var laptopsList = PrepareProductOverviewModels(laptops);
                blHomepageProducts.LaptopCategoryProducts = laptopsList.ToList();

                var blResult = new GeneralResponseModel<HomepageApiListModel.BlHomepage>
                {
                    Data = blHomepageProducts
                };
                return Ok(blResult);

            }

            //----- load top Category list -----//
            #region Top Catgory

            // 298 => Buy 1 Get 1 Free
            // 312 => App Only
            // 498 => Free Delivery
            // 499 => 99 Offer
            int[] categoryIds = { 298, 312, 498, 499 };

            var categories = _categoryService.GetCategoriesByIds(categoryIds)
                .Select(x =>
                {
                    var catModel = x.MapTo<Category, CategoryOverViewModelApi>();
                    int pictureSize = _mediaSettings.CategoryThumbPictureSize;
                    //prepare picture model
                    if (thumbPictureSize.HasValue)
                    {
                        pictureSize = thumbPictureSize.GetValueOrDefault();
                    }
                    var picture = _pictureService.GetPictureById(x.PictureId);
                    var pictureModel = new PictureModel
                    {
                        ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                    };
                    catModel.DefaultPictureModel = pictureModel;
                    return catModel;
                })
                .ToList();

            homepageApiModel.TopCategoryList = categories;


            #endregion


            //----- load Flash Sale Products list -----//
            #region Flash Sale Products

            int flashSaleCatId = 500;
            var products = GetProductsByCategoryId(flashSaleCatId, 10);
            var productOverviewViewModelList = PrepareProductOverviewModels(products);
            homepageApiModel.FlashSaleProducts = productOverviewViewModelList.ToList();

            #endregion



            //----- load Most Popular Products list -----//
            #region Most Popular Products

            var mostPopular = _orderReportService.BestSellersReport(
                    storeId: _storeContext.CurrentStore.Id,
                    pageSize: _catalogSettings.NumberOfBestsellersOnHomepage)
                .ToList();
            var mostPopularProducts = _productService.GetProductsByIds(mostPopular.Select(x => x.ProductId).ToArray());
            mostPopularProducts = mostPopularProducts.Where(p => p.IsAvailable()).Take(10).ToList();
            var mostPopularProductViewModelList = PrepareProductOverviewModels(mostPopularProducts);
            homepageApiModel.MostPopular = mostPopularProductViewModelList.ToList();

            #endregion



            //----- load On Sale Products list -----//

            #region OnSaleProducts

            var onSaleProducts = _productServiceApi.SearchOnSaleProducts(visibleIndividuallyOnly: true, pageIndex: 1, pageSize: 10);
            OnSaleProductModel model = new OnSaleProductModel();
            model.Products = PrepareProductOverviewModels(products, true, true, thumbPictureSize).ToList();
            homepageApiModel.OnSaleProductModel = model;

            #endregion

            //----- load 70% sale list -----//

            #region 70% Sale Products

            int parcentSaleCatId = 500;
            var parcentProducts = GetProductsByCategoryId(parcentSaleCatId, 10);
            var parcentProductOverviewViewModelList = PrepareProductOverviewModels(parcentProducts);
            homepageApiModel.Parcent70Sale = parcentProductOverviewViewModelList.ToList();

            #endregion


            //----- Recent Products ------//
            #region Recent Products

            var RecentProducts = _productService.SearchProducts(
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                markedAsNewOnly: true,
                orderBy: ProductSortingEnum.CreatedOn,
                pageSize: _catalogSettings.NewProductsNumber);

            var recentProductModel = new List<ProductOverViewModelApi>();
            recentProductModel.AddRange(PrepareProductOverviewModels(products, true, true, thumbPictureSize).ToList());

            homepageApiModel.RecentProducts = recentProductModel;

            #endregion

            var result = new GeneralResponseModel<HomepageApiListModel>
            {
                Data = homepageApiModel
            };
            return Ok(result);
        }

        #endregion

        #endregion
    }
}
