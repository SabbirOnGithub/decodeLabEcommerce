using System;
using System.Collections.Generic;
using System.Linq;
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
using Nop.Plugin.Widgets.AlgoliaSearch.Factories;
using Nop.Plugin.Widgets.AlgoliaSearch.Models;
using VendorModel = BS.Plugin.NopStation.MobileWebApi.Models.Vendor.VendorModel;
using VendorNavigationModel = BS.Plugin.NopStation.MobileWebApi.Models.Vendor.VendorNavigationModel;
using VendorBriefInfoModel = BS.Plugin.NopStation.MobileWebApi.Models.Vendor.VendorBriefInfoModel;
using Nop.Plugin.Widgets.AlgoliaSearch;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class CatalogController : WebApiController
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

        #endregion

        #region Ctor

        public CatalogController(ICategoryService categoryService,
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
            AlgoliaSettings algoliaSettings)
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
        }

        #endregion

        #region Utility

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

        #region Category

        /// <summary>
        /// Get all categories, languages, currencies
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.Route("api/categories")]
        [System.Web.Http.HttpGet]
        //[TokenAuthorize]
        public IHttpActionResult Categories()
        {
            string categoryCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_MENU_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()) + "__v1",
                _storeContext.CurrentStore.Id);

//            var cachedCategoriesModel = _cacheManager.Get(categoryCacheKey, 3600, () =>
//            {
//                List<CategoryNavigationModelApi> categoryApiList = new List<CategoryNavigationModelApi>();
//                string jsonMenuCategory = null;
//                    var filePath = CommonHelper.MapPath("~/ApiJson/menu-category-json.json");
//
//                    try
//                    {
//                        jsonMenuCategory = System.IO.File.ReadAllText(filePath);
//                    }
//                    catch { }
//
//                    if (string.IsNullOrWhiteSpace(jsonMenuCategory))
//                    {
//                        try
//                        {
//                            filePath = CommonHelper.MapPath("~/ApiJson/menu-category-backup-json.json");
//                            jsonMenuCategory = System.IO.File.ReadAllText(filePath);
//                        }
//                        catch { }
//                    }
//                    return JsonConvert.DeserializeObject<List<CategoryNavigationModelApi>>(jsonMenuCategory);
//            });

            List<CategoryNavigationModelApi> categoryApiList = new List<CategoryNavigationModelApi>();
            var categoryList = _categoryService.GetAllCategories().Where(x => x.Deleted == false && x.Published && x.ParentCategoryId == 0).ToList();
            if (categoryList.Any())
            {
                foreach (var category in categoryList)
                {
                    var categoryApi = new CategoryNavigationModelApi();
                    categoryApi.Id = category.Id;
                    categoryApi.Name = category.GetLocalized(x => x.Name);
                    categoryApi.ParentCategoryId = category.ParentCategoryId;
                    categoryApi.DisplayOrder = category.DisplayOrder;
                    categoryApi.PictureId = category.PictureId;
                    var picture = _pictureService.GetPictureById(category.PictureId);
                    categoryApi.ImageUrl = _pictureService.GetPictureUrl(picture);

                    categoryApiList.Add(categoryApi);
                }
            }

//            return categoryApiList;


            int count = _workContext.CurrentCustomer.ShoppingCartItems
                       .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                       .LimitPerStore(_storeContext.CurrentStore.Id)
                       .ToList()
                       .GetTotalProducts();

            //Tax settings
            bool displayTaxInOrderSummary = !(_taxSettings.HideTaxInOrderSummary && _workContext.TaxDisplayType == TaxDisplayType.IncludingTax);

            //Discount Box Settings
            bool showDiscountBox = _shoppingCartSettings.ShowDiscountBox;

            // Language Selector
            var availableLanguages = _cacheManager.Get(string.Format(ModelCacheEventConsumer.AVAILABLE_LANGUAGES_MODEL_KEY, _storeContext.CurrentStore.Id), () =>
            {
                var languages = _languageService
                    .GetAllLanguages(storeId: _storeContext.CurrentStore.Id)
                    .Select(x => new LanguageNavModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        FlagImageFileName = x.FlagImageFileName,
                    })
                    .ToList();
                return languages;
            });

            var langNavModel = new LanguageNavSelectorModel
            {
                CurrentLanguageId = _workContext.WorkingLanguage.Id,
                AvailableLanguages = availableLanguages,
                UseImages = _localizationSettings.UseImagesForLanguageSelection
            };

            // Currency Selector
            var availableCurrencies = _cacheManager.Get(string.Format(ModelCacheEventConsumer.AVAILABLE_CURRENCIES_MODEL_KEY, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id), () =>
            {
                string jsonMenuCategory = null;
                var filePath = CommonHelper.MapPath("~/ApiJson/currency-json.json");

                try
                {
                    jsonMenuCategory = System.IO.File.ReadAllText(filePath);
                }
                catch { }

                if (string.IsNullOrWhiteSpace(jsonMenuCategory))
                {
                    try
                    {
                        filePath = CommonHelper.MapPath("~/ApiJson/currency-backup-json.json");
                        jsonMenuCategory = System.IO.File.ReadAllText(filePath);
                    }
                    catch { }
                }
                return JsonConvert.DeserializeObject<List<CurrencyNavModel>>(jsonMenuCategory);
            });

            var currNavModel = new CurrencyNavSelectorModel
            {
                CurrentCurrencyId = _workContext.WorkingCurrency.Id,
                AvailableCurrencies = availableCurrencies
            };


            var result = new AllResponseModel
            {
                Data = categoryApiList,
                Count = count,
                DisplayTaxInOrderSummary = displayTaxInOrderSummary,
                ShowDiscountBox = showDiscountBox,
                Language = langNavModel,
                Currency = currNavModel
            };

            return Ok(result);
        }

        [System.Web.Http.Route("api/v1/categories")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult V1Categories()
        {
            string categoryCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_MENU_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);

            var cachedCategoriesModel = _cacheManager.Get(categoryCacheKey, () =>
            {
                string jsonMenuCategory = null;
                var filePath = CommonHelper.MapPath("~/ApiJson/menu-category-json-v1.json");

                try
                {
                    jsonMenuCategory = System.IO.File.ReadAllText(filePath);
                }
                catch { }

                if (string.IsNullOrWhiteSpace(jsonMenuCategory))
                {
                    try
                    {
                        filePath = CommonHelper.MapPath("~/ApiJson/menu-category-backup-json-v1.json");
                        jsonMenuCategory = System.IO.File.ReadAllText(filePath);
                    }
                    catch { }
                }
                return JsonConvert.DeserializeObject<List<CategoryNavigationModelApi>>(jsonMenuCategory);
            });

            int count = _workContext.CurrentCustomer.ShoppingCartItems
                       .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                       .LimitPerStore(_storeContext.CurrentStore.Id)
                       .ToList()
                       .GetTotalProducts();
            // Language Selector

            var availableLanguages = _cacheManager.Get(string.Format(ModelCacheEventConsumer.AVAILABLE_LANGUAGES_MODEL_KEY, _storeContext.CurrentStore.Id), () =>
            {
                var languages = _languageService
                    .GetAllLanguages(storeId: _storeContext.CurrentStore.Id)
                    .Select(x => new LanguageNavModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        FlagImageFileName = x.FlagImageFileName,
                    })
                    .ToList();
                return languages;
            });

            var langNavModel = new LanguageNavSelectorModel
            {
                CurrentLanguageId = _workContext.WorkingLanguage.Id,
                AvailableLanguages = availableLanguages,
                UseImages = _localizationSettings.UseImagesForLanguageSelection
            };

            // Currency Selector
            var availableCurrencies = _cacheManager.Get(string.Format(ModelCacheEventConsumer.AVAILABLE_CURRENCIES_MODEL_KEY, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id), () =>
            {
                string jsonMenuCategory = null;
                var filePath = CommonHelper.MapPath("~/ApiJson/currency-json.json");

                try
                {
                    jsonMenuCategory = System.IO.File.ReadAllText(filePath);
                }
                catch { }

                if (string.IsNullOrWhiteSpace(jsonMenuCategory))
                {
                    try
                    {
                        filePath = CommonHelper.MapPath("~/ApiJson/currency-backup-json.json");
                        jsonMenuCategory = System.IO.File.ReadAllText(filePath);
                    }
                    catch { }
                }
                return JsonConvert.DeserializeObject<List<CurrencyNavModel>>(jsonMenuCategory);
            });

            var currNavModel = new CurrencyNavSelectorModel
            {
                CurrentCurrencyId = _workContext.WorkingCurrency.Id,
                AvailableCurrencies = availableCurrencies
            };

            var result = new AllResponseModel { Data = cachedCategoriesModel, Count = count, Language = langNavModel, Currency = currNavModel };
            return Ok(result);

        }

        [System.Web.Http.Route("api/homepagecategories")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult HomepageCategories(int? thumbPictureSize = null)
        {
            string categoriesCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_HOMEPAGE_KEY,
               string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
               _storeContext.CurrentStore.Id,
               _workContext.WorkingLanguage.Id,
               _webHelper.IsCurrentConnectionSecured());

            var model = _cacheManager.Get(categoriesCacheKey, () =>
                _categoryService.GetAllCategoriesDisplayedOnHomePage()
                .Select(x =>
                {
                    var catModel = x.MapTo<Category, CategoryOverViewModelApi>();
                    int pictureSize = _mediaSettings.CategoryThumbPictureSize;
                    //prepare picture model
                    if (thumbPictureSize.HasValue)
                    {
                        pictureSize = thumbPictureSize.GetValueOrDefault();
                    }

                    var categoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, x.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                    catModel.DefaultPictureModel = _cacheManager.Get(categoryPictureCacheKey, () =>
                    {
                        var picture = _pictureService.GetPictureById(x.PictureId);
                        var pictureModel = new PictureModel
                        {
                            ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                        };
                        return pictureModel;
                    });
                    catModel.ProductCount = GetAllProductsByCategoryId(catModel.Id).Count;
                    return catModel;
                })
                .ToList()
            );

            var result = new GeneralResponseModel<List<CategoryOverViewModelApi>>()
            {
                Data = model
            };
            return Ok(result);
        }

        [System.Web.Http.Route("api/catalog/homepagecategorieswithproduct")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult HomepageCategoriesWithProduct(int? thumbPictureSize = null)
        {
            string categoriesCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_HOMEPAGE_KEY_PRODUCT_CATEGORY,
               string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
               _storeContext.CurrentStore.Id,
               _workContext.WorkingLanguage.Id,
               _webHelper.IsCurrentConnectionSecured());

            var model = _cacheManager.Get(categoriesCacheKey, () =>
                _categoryService.GetAllCategoriesDisplayedOnHomePage()
                .Select(x =>
                {
                    var catWithProduct = new CatalogFeaturedCategoryWithProduct();
                    var subCategory = _categoryService.GetAllCategoriesByParentCategoryId(x.Id).Select(c => new CatalogFeaturedCategoryWithProduct.SubCategoriesWithNameAndId() { Id = c.Id, Name = c.Name, IconPath = _categoryIconService.GetCategoryIconByCategoryId(c.Id) != null ? string.Format("{0}{1}", c.Id.ToString(), _categoryIconService.GetCategoryIconByCategoryId(c.Id)).IconImagePath(_webHelper) : ("defaultIcon.png").IconImagePath(_webHelper) }).ToList();
                    //var categoryIds = subCategory.Select(c => c.Id).ToList();
                    var catModel = x.MapTo<Category, CategoryOverViewModelApi>();
                    int pictureSize = _mediaSettings.CategoryThumbPictureSize;
                    //prepare picture model
                    if (thumbPictureSize.HasValue)
                    {
                        pictureSize = thumbPictureSize.GetValueOrDefault();
                    }

                    var categoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, x.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                    catModel.DefaultPictureModel = _cacheManager.Get(categoryPictureCacheKey, () =>
                    {
                        var picture = _pictureService.GetPictureById(x.PictureId);
                        var pictureModel = new PictureModel
                        {
                            ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                        };
                        return pictureModel;
                    });
                    catWithProduct.SubCategory = subCategory;
                    var products = GetProductsByCategoryId(x.Id, 6);
                    catWithProduct.Product = PrepareProductOverviewModels(products);
                    catModel.ProductCount = products.Count;
                    catWithProduct.Category = catModel;
                    return catWithProduct;
                })
                .ToList()
            );

            var result = new GeneralResponseModel<List<CatalogFeaturedCategoryWithProduct>>()
            {
                Data = model
            };
            return Ok(result);
        }

        [System.Web.Http.Route("api/Category/{categoryId}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult Category(int categoryId)
        {
            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null || category.Deleted)
                return NotFound();

            if (!category.Published && !_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
            {
                if (category.Name != "App Only")
                    return NotFound();
            }

            //ACL (access control list)
            if (!_aclService.Authorize(category))
                return NotFound();

            //Store mapping
            if (!_storeMappingService.Authorize(category))
                return NotFound();

            var model = category.ToResponseModel();
            var command = new CatalogPagingFilteringModel();
            command.LoadAdvanceFilterData(_webHelper);

            //sorting
            PrepareSortingOptions(model.PagingFilteringContext, command.OrderBy, _catalogSettings.ProductSortingEnumDisabled);

            //page size
            PreparePageSizeOptions(model.PagingFilteringContext, command,
                category.AllowCustomersToSelectPageSize, category.PageSizeOptions, category.PageSize);

            decimal? minPriceConverted = null;
            decimal? maxPriceConverted = null;
            if (command.LoadAdvanceFilters)
            {
                var selectedPriceRange = model.PagingFilteringContext.PriceRangeFilter.GetSelectedPriceRange(_webHelper);
                if (selectedPriceRange != null)
                {
                    if (selectedPriceRange.From.HasValue)
                        minPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.From.Value, _workContext.WorkingCurrency);

                    if (selectedPriceRange.To.HasValue)
                        maxPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.To.Value, _workContext.WorkingCurrency);
                }
            }

            IList<int> alreadyFilteredSpecOptionIds = model.PagingFilteringContext.SpecificationFilter.GetAlreadyFilteredSpecOptionIds(_webHelper);
            IList<int> alreadyFilteredCategoryIds = model.PagingFilteringContext.CategoryFilter.GetAlreadyFilteredCategoryIds(_webHelper);
            IList<int> alreadyFilteredManufacturerIds = model.PagingFilteringContext.ManufacturerFilter.GetAlreadyFilteredManufacturerIds(_webHelper);
            IList<int> alreadyFilteredVendorIds = model.PagingFilteringContext.VendorFilter.GetAlreadyFilteredVendorIds(_webHelper);
            bool emiProductsOnly = model.PagingFilteringContext.EmiFilter.GetEmiFilterStatus(_webHelper);

            //products
            var products = _productServiceApi.SearchProducts(
                out IDictionary<int, int> filterableSpecificationAttributeOptionIds,
                out IDictionary<int, int> filterableCategoryIds,
                out IDictionary<int, int> filterableManufacturerIds,
                out IDictionary<int, int> filterableVendorIds,
                out IDictionary<int, int> filterableRatings,
                out decimal filterableMaxPrice,
                out decimal filterableMinPrice,
                out bool filterableEmi,
                loadAdvanceFilterOptions: command.LoadAdvanceFilters,
                categoryId: categoryId,
                storeId: _storeContext.CurrentStore.Id,
                priceMin: minPriceConverted,
                priceMax: maxPriceConverted,
                orderBy: (ProductSortingEnum)command.OrderBy,
                filteredCategoryIds: alreadyFilteredCategoryIds,
                filteredManufacturerIds: alreadyFilteredManufacturerIds,
                filteredVendorIds: alreadyFilteredVendorIds,
                filteredSpecs: alreadyFilteredSpecOptionIds,
                emiProductOnly: emiProductsOnly,
                pageIndex: command.PageIndex,
                pageSize: command.PageSize);

            model.Products = PrepareProductOverviewModels(products).ToList();
            model.PagingFilteringContext.LoadPagedList(products);

            if (command.LoadAdvanceFilters)
            {
                model.PagingFilteringContext.LoadAdvanceFilters = true;
                model.PagingFilteringContext.PriceRangeFilter.LoadPriceRangeFilters(filterableMinPrice, filterableMaxPrice, minPriceConverted, maxPriceConverted);
                model.PagingFilteringContext.SpecificationFilter.PrepareSpecsFilters(alreadyFilteredSpecOptionIds, filterableSpecificationAttributeOptionIds, _specificationAttributeService);
                model.PagingFilteringContext.CategoryFilter.PrepareCategoryFilters(alreadyFilteredCategoryIds, filterableCategoryIds, _categoryService);
                model.PagingFilteringContext.ManufacturerFilter.PrepareManufacturerFilters(alreadyFilteredManufacturerIds, filterableManufacturerIds, _manufacturerService);
                model.PagingFilteringContext.VendorFilter.PrepareVendorFilters(alreadyFilteredVendorIds, filterableVendorIds, _vendorService);
                model.PagingFilteringContext.EmiFilter.PrepareEmiFilters(emiProductsOnly, filterableEmi);
            }
            return Ok(model);
        }

        #endregion

        #region Homepage items

        [System.Web.Http.Route("api/homepageitems")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult HomePageAll()
        {
            string jsonHomePageData = null;
            var filePath = CommonHelper.MapPath("~/ApiJson/homepage-response-model-json.json");

            try
            {
                jsonHomePageData = System.IO.File.ReadAllText(filePath);
            }
            catch { }

            if (string.IsNullOrWhiteSpace(jsonHomePageData))
            {
                try
                {
                    filePath = CommonHelper.MapPath("~/ApiJson/homepage-response-model-backup-json.json");
                    jsonHomePageData = System.IO.File.ReadAllText(filePath);
                }
                catch { }
            }

            var model = JsonConvert.DeserializeObject<HomePageResponseModel>(jsonHomePageData);

            if (model.Language == null)
                model.Language = new LanguageNavSelectorModel();

            model.Language.CurrentLanguageId = _workContext.WorkingLanguage.Id;
            model.Language.UseImages = _localizationSettings.UseImagesForLanguageSelection;

            if (model.Banners.Count > 0)
            {
                model.BannerIsEnabled = true;
            }

            int count = _workContext.CurrentCustomer.ShoppingCartItems
                       .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                       .LimitPerStore(_storeContext.CurrentStore.Id)
                       .ToList()
                       .GetTotalProducts();

            model.Count = count;

            return Ok(model);
        }

        #endregion

        #region BLink Homepage items

        [System.Web.Http.Route("api/blhomepageitems")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult BLinkHomePageAll()
        {
            string jsonHomePageData = null;
            var filePath = CommonHelper.MapPath("~/ApiJson/homepage-response-model-json.json");

            try
            {
                jsonHomePageData = System.IO.File.ReadAllText(filePath);
            }
            catch { }

            if (string.IsNullOrWhiteSpace(jsonHomePageData))
            {
                try
                {
                    filePath = CommonHelper.MapPath("~/ApiJson/homepage-response-model-backup-json.json");
                    jsonHomePageData = System.IO.File.ReadAllText(filePath);
                }
                catch { }
            }

            var model = JsonConvert.DeserializeObject<HomePageResponseModel>(jsonHomePageData);

            model.CategoriesWithProducts = model.CategoriesWithProducts.Where(CategoryWithProducts => CategoryWithProducts.ApplicableFor == 1).ToList();

            if (model.Language == null)
                model.Language = new LanguageNavSelectorModel();

            model.Language.CurrentLanguageId = _workContext.WorkingLanguage.Id;
            model.Language.UseImages = _localizationSettings.UseImagesForLanguageSelection;

            if (model.Banners.Count > 0)
            {
                model.BannerIsEnabled = true;
            }

            int count = _workContext.CurrentCustomer.ShoppingCartItems
                       .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                       .LimitPerStore(_storeContext.CurrentStore.Id)
                       .ToList()
                       .GetTotalProducts();

            model.Count = count;

            return Ok(model);
        }

        #endregion

        #region Manufacturer

        [System.Web.Http.Route("api/homepagemanufacture")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult ManufacturerAll(int? thumbPictureSize = null)
        {
            string jsonMenuCategory = null;
            var filePath = CommonHelper.MapPath("~/ApiJson/manufacturer-json.json");

            try
            {
                jsonMenuCategory = System.IO.File.ReadAllText(filePath);
            }
            catch { }

            if (string.IsNullOrWhiteSpace(jsonMenuCategory))
            {
                try
                {
                    filePath = CommonHelper.MapPath("~/ApiJson/manufacturer-backup-json.json");
                    jsonMenuCategory = System.IO.File.ReadAllText(filePath);
                }
                catch { }
            }
            var model = JsonConvert.DeserializeObject<List<MenufactureOverViewModelApi>>(jsonMenuCategory);

            var result = new GeneralResponseModel<List<MenufactureOverViewModelApi>>()
            {
                Data = model
            };
            return Ok(result);
        }

        [System.Web.Http.Route("api/Manufacturer/{manufacturerId}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult Manufacturer(int manufacturerId, int pageNumber = 1, int orderBy = 0)
        {
            var manufacturer = _manufacturerService.GetManufacturerById(manufacturerId);
            if (manufacturer == null || manufacturer.Deleted)
                return NotFound();

            //Check whether the current user has a "Manage catalog" permission
            //It allows him to preview a manufacturer before publishing
            if (!manufacturer.Published && !_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return NotFound();

            //ACL (access control list)
            if (!_aclService.Authorize(manufacturer))
                return NotFound();

            //Store mapping
            if (!_storeMappingService.Authorize(manufacturer))
                return NotFound();

            var model = manufacturer.ToResponseModel();

            var command = new CatalogPagingFilteringModel();
            command.LoadAdvanceFilterData(_webHelper);

            //sorting
            PrepareSortingOptions(model.PagingFilteringContext, command.OrderBy, _catalogSettings.ProductSortingEnumDisabled);

            //page size
            PreparePageSizeOptions(model.PagingFilteringContext, command,
                manufacturer.AllowCustomersToSelectPageSize, manufacturer.PageSizeOptions, manufacturer.PageSize);

            decimal? minPriceConverted = null;
            decimal? maxPriceConverted = null;
            if (command.LoadAdvanceFilters)
            {
                var selectedPriceRange = model.PagingFilteringContext.PriceRangeFilter.GetSelectedPriceRange(_webHelper);
                if (selectedPriceRange != null)
                {
                    if (selectedPriceRange.From.HasValue)
                        minPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.From.Value, _workContext.WorkingCurrency);

                    if (selectedPriceRange.To.HasValue)
                        maxPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.To.Value, _workContext.WorkingCurrency);
                }
            }

            IList<int> alreadyFilteredSpecOptionIds = model.PagingFilteringContext.SpecificationFilter.GetAlreadyFilteredSpecOptionIds(_webHelper);
            IList<int> alreadyFilteredCategoryIds = model.PagingFilteringContext.CategoryFilter.GetAlreadyFilteredCategoryIds(_webHelper);
            IList<int> alreadyFilteredManufacturerIds = model.PagingFilteringContext.ManufacturerFilter.GetAlreadyFilteredManufacturerIds(_webHelper);
            IList<int> alreadyFilteredVendorIds = model.PagingFilteringContext.VendorFilter.GetAlreadyFilteredVendorIds(_webHelper);
            bool emiProductsOnly = model.PagingFilteringContext.EmiFilter.GetEmiFilterStatus(_webHelper);

            //products
            var products = _productServiceApi.SearchProducts(
                out IDictionary<int, int> filterableSpecificationAttributeOptionIds,
                out IDictionary<int, int> filterableCategoryIds,
                out IDictionary<int, int> filterableManufacturerIds,
                out IDictionary<int, int> filterableVendorIds,
                out IDictionary<int, int> filterableRatings,
                out decimal filterableMaxPrice,
                out decimal filterableMinPrice,
                out bool filterableEmi,
                loadAdvanceFilterOptions: command.LoadAdvanceFilters,
                manufacturerId: manufacturerId,
                storeId: _storeContext.CurrentStore.Id,
                priceMin: minPriceConverted,
                priceMax: maxPriceConverted,
                orderBy: (ProductSortingEnum)command.OrderBy,
                filteredCategoryIds: alreadyFilteredCategoryIds,
                filteredManufacturerIds: alreadyFilteredManufacturerIds,
                filteredVendorIds: alreadyFilteredVendorIds,
                filteredSpecs: alreadyFilteredSpecOptionIds,
                emiProductOnly: emiProductsOnly,
                pageIndex: command.PageIndex,
                pageSize: command.PageSize);

            model.Products = PrepareProductOverviewModels(products).ToList();
            model.PagingFilteringContext.LoadPagedList(products);

            if (command.LoadAdvanceFilters)
            {
                model.PagingFilteringContext.LoadAdvanceFilters = true;
                model.PagingFilteringContext.PriceRangeFilter.LoadPriceRangeFilters(filterableMinPrice, filterableMaxPrice, minPriceConverted, maxPriceConverted);
                model.PagingFilteringContext.SpecificationFilter.PrepareSpecsFilters(alreadyFilteredSpecOptionIds, filterableSpecificationAttributeOptionIds, _specificationAttributeService);
                model.PagingFilteringContext.CategoryFilter.PrepareCategoryFilters(alreadyFilteredCategoryIds, filterableCategoryIds, _categoryService);
                model.PagingFilteringContext.ManufacturerFilter.PrepareManufacturerFilters(alreadyFilteredManufacturerIds, filterableManufacturerIds, _manufacturerService);
                model.PagingFilteringContext.VendorFilter.PrepareVendorFilters(alreadyFilteredVendorIds, filterableVendorIds, _vendorService);
                model.PagingFilteringContext.EmiFilter.PrepareEmiFilters(emiProductsOnly, filterableEmi);
            }

            return Ok(model);
        }

        [System.Web.Http.Route("api/manufacturerfeaturedproduct/{manufacturerId}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult ManufacturerFeaturedProduct(int manufacturerId)
        {
            var manufacturer = _manufacturerService.GetManufacturerById(manufacturerId);
            if (manufacturer == null || manufacturer.Deleted)
                return NotFound();

            //Check whether the current user has a "Manage catalog" permission
            //It allows him to preview a manufacturer before publishing
            if (!manufacturer.Published && !_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return NotFound();

            //ACL (access control list)
            if (!_aclService.Authorize(manufacturer))
                return NotFound();

            //Store mapping
            if (!_storeMappingService.Authorize(manufacturer))
                return NotFound();

            var model = manufacturer.ToModel();

            //featured products
            if (!_catalogSettings.IgnoreFeaturedProducts)
            {
                IPagedList<Product> featuredProducts = null;

                //We cache a value indicating whether we have featured products
                string cacheKey = string.Format(ModelCacheEventConsumer.MANUFACTURER_HAS_FEATURED_PRODUCTS_KEY,
                    manufacturerId,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                    _storeContext.CurrentStore.Id);
                var hasFeaturedProductsCache = _cacheManager.Get<bool?>(cacheKey);
                if (!hasFeaturedProductsCache.HasValue)
                {
                    //no value in the cache yet
                    //let's load products and cache the result (true/false)
                    featuredProducts = _productService.SearchProducts(
                       manufacturerId: manufacturer.Id,
                       storeId: _storeContext.CurrentStore.Id,
                       visibleIndividuallyOnly: true,
                       featuredProducts: true,
                       activeVendorOnly: true,
                       includeEkshopProducts: false);
                    hasFeaturedProductsCache = featuredProducts.TotalCount > 0;
                    _cacheManager.Set(cacheKey, hasFeaturedProductsCache, 60);
                }
                if (hasFeaturedProductsCache.Value && featuredProducts == null)
                {
                    //cache indicates that the manufacturer has featured products
                    //let's load them
                    featuredProducts = _productService.SearchProducts(
                       manufacturerId: manufacturer.Id,
                       storeId: _storeContext.CurrentStore.Id,
                       visibleIndividuallyOnly: true,
                       featuredProducts: true,
                       activeVendorOnly: true,
                       includeEkshopProducts: false);
                }
                if (featuredProducts != null)
                {
                    model.FeaturedProducts = PrepareProductOverviewModels(featuredProducts).ToList();
                }
            }

            var result = model.MapTo<ManuFactureModelApi, ManufacturerDetailFeaturedProductResponseModel>();
            return Ok(result);
        }

        [System.Web.Http.Route("api/Tag/{productTagId}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult ProductsByTag(int productTagId, int pageNumber = 1, int orderBy = 0)
        {
            var productTag = _productTagService.GetProductTagById(productTagId);
            if (productTag == null)
                return NotFound();

            var model = productTag.ToModel();

            //sorting
            PrepareSortingOptions(model.PagingFilteringContext, orderBy, _catalogSettings.ProductSortingEnumDisabled);
            //view mode

            //products
            var products = _productService.SearchProducts(
                storeId: _storeContext.CurrentStore.Id,
                productTagId: productTag.Id,
                visibleIndividuallyOnly: true,
                orderBy: (ProductSortingEnum)orderBy,
                pageIndex: pageNumber - 1,
                pageSize: this.pageSize,
                activeVendorOnly: true,
                includeEkshopProducts: false);

            model.Products = PrepareProductOverviewModels(products).ToList();

            model.PagingFilteringContext.LoadPagedList(products);
            var result = model.ToModel();
            return Ok(result);
        }

        #endregion

        #region Vendors

        [System.Web.Http.Route("api/vendor/{vendorId}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult Vendor(int vendorId)
        {
            var vendor = _vendorService.GetVendorById(vendorId);

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                SystemCustomerAttributeNames.LastContinueShoppingPage,
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);

            var model = new VendorResponseModel
            {
                Id = vendor.Id,
                Name = vendor.GetLocalized(x => x.Name),
                Description = vendor.GetLocalized(x => x.Description),
                MetaKeywords = vendor.GetLocalized(x => x.MetaKeywords),
                MetaDescription = vendor.GetLocalized(x => x.MetaDescription),
                MetaTitle = vendor.GetLocalized(x => x.MetaTitle),
                SeName = vendor.GetSeName(),
                AllowCustomersToContactVendors = _vendorSettings.AllowCustomersToContactVendors
            };

            //prepare picture model
            int pictureSize = _mediaSettings.VendorThumbPictureSize;
            var pictureCacheKey = string.Format(ModelCacheEventConsumer.VENDOR_PICTURE_MODEL_KEY, vendor.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
            model.PictureModel = _cacheManager.Get(pictureCacheKey, () =>
            {
                var picture = _pictureService.GetPictureById(vendor.PictureId);
                var pictureModel = new PictureModel
                {
                    FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                    ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                    Title = string.Format(_localizationService.GetResource("Media.Vendor.ImageLinkTitleFormat"), model.Name),
                    AlternateText = string.Format(_localizationService.GetResource("Media.Vendor.ImageAlternateTextFormat"), model.Name)
                };
                return pictureModel;
            });

            var customerVendor = _customerServiceApi.GetCustomerByVendorId(vendor.Id);
            model.LogoModel = _cacheManager.Get(pictureCacheKey, () =>
            {
                var picture = _pictureService.GetPictureById(customerVendor.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId));
                var pictureModel = new PictureModel
                {
                    FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                    ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                    Title = string.Format(_localizationService.GetResource("Media.Vendor.ImageLinkTitleFormat"), model.Name),
                    AlternateText = string.Format(_localizationService.GetResource("Media.Vendor.ImageAlternateTextFormat"), model.Name)
                };
                return pictureModel;
            });

            var command = new CatalogPagingFilteringModel();
            command.LoadAdvanceFilterData(_webHelper);

            //sorting
            PrepareSortingOptions(model.PagingFilteringContext, command.OrderBy, _catalogSettings.ProductSortingEnumDisabled);

            //page size
            PreparePageSizeOptions(model.PagingFilteringContext, command,
                vendor.AllowCustomersToSelectPageSize, vendor.PageSizeOptions, vendor.PageSize);

            decimal? minPriceConverted = null;
            decimal? maxPriceConverted = null;
            if (command.LoadAdvanceFilters)
            {
                var selectedPriceRange = model.PagingFilteringContext.PriceRangeFilter.GetSelectedPriceRange(_webHelper);
                if (selectedPriceRange != null)
                {
                    if (selectedPriceRange.From.HasValue)
                        minPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.From.Value, _workContext.WorkingCurrency);

                    if (selectedPriceRange.To.HasValue)
                        maxPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.To.Value, _workContext.WorkingCurrency);
                }
            }

            IList<int> alreadyFilteredSpecOptionIds = model.PagingFilteringContext.SpecificationFilter.GetAlreadyFilteredSpecOptionIds(_webHelper);
            IList<int> alreadyFilteredCategoryIds = model.PagingFilteringContext.CategoryFilter.GetAlreadyFilteredCategoryIds(_webHelper);
            IList<int> alreadyFilteredManufacturerIds = model.PagingFilteringContext.ManufacturerFilter.GetAlreadyFilteredManufacturerIds(_webHelper);
            IList<int> alreadyFilteredVendorIds = model.PagingFilteringContext.VendorFilter.GetAlreadyFilteredVendorIds(_webHelper);
            bool emiProductsOnly = model.PagingFilteringContext.EmiFilter.GetEmiFilterStatus(_webHelper);

            //products
            var products = _productServiceApi.SearchProducts(
                out IDictionary<int, int> filterableSpecificationAttributeOptionIds,
                out IDictionary<int, int> filterableCategoryIds,
                out IDictionary<int, int> filterableManufacturerIds,
                out IDictionary<int, int> filterableVendorIds,
                out IDictionary<int, int> filterableRatings,
                out decimal filterableMaxPrice,
                out decimal filterableMinPrice,
                out bool filterableEmi,
                loadAdvanceFilterOptions: command.LoadAdvanceFilters,
                vendorId: vendorId,
                storeId: _storeContext.CurrentStore.Id,
                priceMin: minPriceConverted,
                priceMax: maxPriceConverted,
                orderBy: (ProductSortingEnum)command.OrderBy,
                filteredCategoryIds: alreadyFilteredCategoryIds,
                filteredManufacturerIds: alreadyFilteredManufacturerIds,
                filteredVendorIds: alreadyFilteredVendorIds,
                filteredSpecs: alreadyFilteredSpecOptionIds,
                emiProductOnly: emiProductsOnly,
                pageIndex: command.PageIndex,
                pageSize: command.PageSize);

            model.Products = PrepareProductOverviewModels(products).ToList();
            model.PagingFilteringContext.LoadPagedList(products);

            if (command.LoadAdvanceFilters)
            {
                model.PagingFilteringContext.LoadAdvanceFilters = true;
                model.PagingFilteringContext.PriceRangeFilter.LoadPriceRangeFilters(filterableMinPrice, filterableMaxPrice, minPriceConverted, maxPriceConverted);
                model.PagingFilteringContext.SpecificationFilter.PrepareSpecsFilters(alreadyFilteredSpecOptionIds, filterableSpecificationAttributeOptionIds, _specificationAttributeService);
                model.PagingFilteringContext.CategoryFilter.PrepareCategoryFilters(alreadyFilteredCategoryIds, filterableCategoryIds, _categoryService);
                model.PagingFilteringContext.ManufacturerFilter.PrepareManufacturerFilters(alreadyFilteredManufacturerIds, filterableManufacturerIds, _manufacturerService);
                model.PagingFilteringContext.VendorFilter.PrepareVendorFilters(alreadyFilteredVendorIds, filterableVendorIds, _vendorService);
                model.PagingFilteringContext.EmiFilter.PrepareEmiFilters(emiProductsOnly, filterableEmi);
            }

            return Ok(model);
        }

        [System.Web.Http.Route("api/vendorall")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult VendorAll()
        {
            var result = new GeneralResponseModel<List<VendorModel>>();
            var model = new List<VendorModel>();
            var vendors = _vendorService.GetAllVendors();
            foreach (var vendor in vendors)
            {
                var vendorModel = new VendorModel
                {
                    Id = vendor.Id,
                    Name = vendor.GetLocalized(x => x.Name),
                    Description = vendor.GetLocalized(x => x.Description),
                    MetaKeywords = vendor.GetLocalized(x => x.MetaKeywords),
                    MetaDescription = vendor.GetLocalized(x => x.MetaDescription),
                    MetaTitle = vendor.GetLocalized(x => x.MetaTitle),
                    SeName = vendor.GetSeName(),
                    AllowCustomersToContactVendors = _vendorSettings.AllowCustomersToContactVendors
                };
                //prepare picture model
                int pictureSize = _mediaSettings.VendorThumbPictureSize;
                var pictureCacheKey = string.Format(ModelCacheEventConsumer.VENDOR_PICTURE_MODEL_KEY, vendor.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                vendorModel.PictureModel = _cacheManager.Get(pictureCacheKey, () =>
                {
                    var picture = _pictureService.GetPictureById(vendor.PictureId);
                    var pictureModel = new PictureModel
                    {
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                        ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                        Title = string.Format(_localizationService.GetResource("Media.Vendor.ImageLinkTitleFormat"), vendorModel.Name),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Vendor.ImageAlternateTextFormat"), vendorModel.Name)
                    };
                    return pictureModel;
                });

                var customerVendor = _customerServiceApi.GetCustomerByVendorId(vendor.Id);
                vendorModel.LogoModel = _cacheManager.Get(pictureCacheKey, () =>
                {
                    var picture = _pictureService.GetPictureById(customerVendor.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId));
                    var pictureModel = new PictureModel
                    {
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                        ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                        Title = string.Format(_localizationService.GetResource("Media.Vendor.ImageLinkTitleFormat"), vendorModel.Name),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Vendor.ImageAlternateTextFormat"), vendorModel.Name)
                    };
                    return pictureModel;
                });

                model.Add(vendorModel);
            }

            result.Data = model;

            return Ok(result);
        }

        [System.Web.Http.Route("api/vendornav")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult VendorNavigation()
        {
            string cacheKey = ModelCacheEventConsumer.VENDOR_NAVIGATION_MODEL_KEY;
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                var vendors = _vendorService.GetAllVendors(pageSize: _vendorSettings.VendorsBlockItemsToDisplay);
                var model = new VendorNavigationModel
                {
                    TotalVendors = vendors.TotalCount
                };

                foreach (var vendor in vendors)
                {
                    model.Vendors.Add(new VendorBriefInfoModel
                    {
                        Id = vendor.Id,
                        Name = vendor.GetLocalized(x => x.Name),
                        SeName = vendor.GetSeName(),
                    });
                }
                return model;
            });

            return Ok(cacheModel);
        }

        #endregion

        #region OnSale

        // On Sale

        [System.Web.Http.Route("api/onsaleall")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult OnSalesAll(int? thumbPictureSize = null, int pageIndex = 0, int pageSize = 25)
        {
            if (!thumbPictureSize.HasValue)
            {
                thumbPictureSize = _mediaSettings.ProductThumbPictureSize;
            }

            var products = _productServiceApi.SearchOnSaleProducts(visibleIndividuallyOnly: true, pageIndex: pageIndex, pageSize: pageSize);

            OnSaleProductModel model = new OnSaleProductModel();

            model.Products = PrepareProductOverviewModels(products, true, true, thumbPictureSize).ToList();

            model.SubCategories = PrepareCategoryFilterOnSale(products, (int)thumbPictureSize).ToList();

            var result = new GeneralResponseModel<OnSaleProductModel>()
            {
                Data = model
            };

            return Ok(result);
        }

        [System.Web.Http.Route("api/onsalecategory/{categoryId}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult OnSalesCategory(int categoryId, int? thumbPictureSize = null, int pageIndex = 0, int pageSize = 25)
        {
            var category = _categoryService.GetCategoryById(categoryId);
            var model = category.ToModel();

            var categoryIds = new List<int>();
            categoryIds.Add(category.Id);
            if (_catalogSettings.ShowProductsFromSubcategories)
            {
                //include subcategories
                categoryIds.AddRange(GetChildCategoryIds(category.Id));
            }

            // sub categories
            //subcategories
            string subCategoriesCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_SUBCATEGORIES_KEY,
                category.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id,
                _workContext.WorkingLanguage.Id,
                _webHelper.IsCurrentConnectionSecured());
            model.SubCategories = _cacheManager.Get(subCategoriesCacheKey, () =>
                _categoryService.GetAllCategoriesByParentCategoryId(category.Id)
                .Select(x =>
                {
                    var subCatModel = new SubCategoryModelApi
                    {
                        Id = x.Id,
                        Name = x.GetLocalized(y => y.Name)
                    };

                    //prepare picture model
                    int pictureSize = 0;
                    if (thumbPictureSize == null)
                    {
                        pictureSize = _mediaSettings.CategoryThumbPictureSize;
                    }
                    else
                    {
                        pictureSize = (int)thumbPictureSize;
                    }
                    var subCategoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, x.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                    subCatModel.PictureModel = _cacheManager.Get(subCategoryPictureCacheKey, () =>
                    {
                        var picture = _pictureService.GetPictureById(x.PictureId);
                        var pictureModel = new PictureModel
                        {
                            FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                            ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                            Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), subCatModel.Name),
                            AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), subCatModel.Name)
                        };
                        return pictureModel;
                    });

                    return subCatModel;
                })
                .ToList()
            );

            // prepare category picture model
            int cpictureSize = 0;
            if (thumbPictureSize == null)
            {
                cpictureSize = _mediaSettings.CategoryThumbPictureSize;
            }
            else
            {
                cpictureSize = (int)thumbPictureSize;
            }
            var categoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, model.Id, cpictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
            model.PictureModel = _cacheManager.Get(categoryPictureCacheKey, () =>
            {
                var picture = _pictureService.GetPictureById(category.PictureId);
                var pictureModel = new PictureModel
                {
                    FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                    ImageUrl = _pictureService.GetPictureUrl(picture, cpictureSize),
                    Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), model.Name),
                    AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), model.Name)
                };
                return pictureModel;
            });

            //products              
            var products = _productServiceApi.SearchOnSaleProducts(
                categoryIds: categoryIds,
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                pageIndex: pageIndex,
                pageSize: pageSize);

            model.Products = PrepareProductOverviewModels(products).ToList();

            // model.ToModel(price: price);
            model.ProductCount = products.Count;
            var result = new GeneralResponseModel<CategoryModelApi>();
            result.Data = model;

            return Ok(result);
        }

        [System.Web.Http.Route("api/onsaleweekly")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult OnSalesWeekly(int? thumbPictureSize = null, int pageIndex = 0, int pageSize = 25)
        {
            if (!thumbPictureSize.HasValue)
            {
                thumbPictureSize = _mediaSettings.ProductThumbPictureSize;
            }

            var products = _productServiceApi.SearchOnSaleProducts(visibleIndividuallyOnly: true, hasTierPrice: false, hasDiscountApplied: false, pageIndex: pageIndex, pageSize: pageSize);

            OnSaleProductModel model = new OnSaleProductModel();

            model.Products = PrepareProductOverviewModels(products, true, true, thumbPictureSize).ToList();

            model.SubCategories = PrepareCategoryFilterOnSale(products, (int)thumbPictureSize).ToList();

            var result = new GeneralResponseModel<OnSaleProductModel>()
            {
                Data = model
            };

            return Ok(result);
        }

        [System.Web.Http.Route("api/onsaletopdeals")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult OnSalesTopDeals(int? thumbPictureSize = null, int pageIndex = 0, int pageSize = 25)
        {
            if (!thumbPictureSize.HasValue)
            {
                thumbPictureSize = _mediaSettings.ProductThumbPictureSize;
            }

            //load and cache report
            var report = _cacheManager.Get(string.Format(ModelCacheEventConsumer.HOMEPAGE_BESTSELLERS_IDS_KEY, _storeContext.CurrentStore.Id),
                () =>
                    _orderReportService.BestSellersReport(storeId: _storeContext.CurrentStore.Id,
                    pageSize: pageSize));


            //load products
            var products = _productServiceApi.GetTopDealsProductsByIds(report.Select(x => x.ProductId).ToArray());
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            OnSaleProductModel model = new OnSaleProductModel();
            //prepare model
            model.Products = PrepareProductOverviewModels(products, true, true, thumbPictureSize).ToList();
            model.SubCategories = PrepareCategoryFilterOnSale(products, (int)thumbPictureSize).ToList();

            var result = new GeneralResponseModel<OnSaleProductModel>()
            {
                Data = model
            };

            return Ok(result);
        }

        #endregion

        #region Categories & manufactures

        /// <summary>
        /// Get all categories and manufacturers
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.Route("api/categoriesNmanufactures/search")]
        [System.Web.Http.HttpGet]
        //[TokenAuthorize]
        public IHttpActionResult CategoriesAndManufacturesSearch()
        {
            var model = new CategoriesAndManufacturersModelApi();

            string cacheKey = string.Format(ModelCacheEventConsumer.SEARCH_CATEGORIES_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);

            var categories = _cacheManager.Get(cacheKey, () =>
            {
                var categoriesModel = new List<CatalogSearchyModelApi>();
                //all categories
                var allCategories = _categoryService.GetAllCategories(storeId: _storeContext.CurrentStore.Id);
                foreach (var c in allCategories)
                {
                    categoriesModel.Add(new CatalogSearchyModelApi
                    {
                        Id = c.Id,
                        Name = c.Name
                    });
                }
                return categoriesModel;
            });

            if (categories.Any())
            {
                foreach (var category in categories)
                {
                    model.Categories.Add(new CatalogSearchyModelApi
                    {
                        Id = category.Id,
                        Name = category.Name
                    });
                }
            }

            var manufacturers = _manufacturerService.GetAllManufacturers(storeId: _storeContext.CurrentStore.Id);

            if (manufacturers.Any())
            {
                foreach (var manufacturer in manufacturers)
                {
                    model.Manufacturers.Add(new CatalogSearchyModelApi
                    {
                        Id = manufacturer.Id,
                        Name = manufacturer.Name
                    });
                }
            }

            return Ok(model);
        }
        #endregion

        #region Search

        [System.Web.Http.Route("api/catalog/search")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult Search(string q)
        {
            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                SystemCustomerAttributeNames.LastContinueShoppingPage,
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);

            var model = new SearchProductResponseModel();

            if (!string.IsNullOrWhiteSpace(model.q) && model.q.Length < _catalogSettings.ProductSearchTermMinimumLength)
            {
                model.ErrorList.Add(string.Format(_localizationService.GetResource("Search.SearchTermMinimumLengthIsNCharacters"), _catalogSettings.ProductSearchTermMinimumLength));
            }
            else
            {
                var command = new CatalogPagingFilteringModel();
                command.LoadAdvanceFilterData(_webHelper);

                model.q = q.Trim();
                //sorting
                PrepareSortingOptions(model.PagingFilteringContext, command.OrderBy, new List<int> { 15 });

                //page size
                PreparePageSizeOptions(model.PagingFilteringContext, command,
                    _algoliaSettings.AllowCustomersToSelectPageSize, _algoliaSettings.SelectablePageSizes, _algoliaSettings.PageSize);

                var filteringModel = PrepareFilteringModel(model, command);

                var products = _productModelFactory.SearchProducts(filteringModel, true);

                model.Products = products.ToList();
                model.PagingFilteringContext.LoadPagedList(products);

                if (command.LoadAdvanceFilters)
                {
                    var filters = _productModelFactory.GetAlgoliaFilterings(model.q);

                    model.PagingFilteringContext.LoadAdvanceFilters = true;
                    model.PagingFilteringContext.PriceRangeFilter.LoadPriceRangeFilters(filters.MinPrice, filters.MaxPrice, filteringModel.MinPrice, filteringModel.MaxPrice);
                    model.PagingFilteringContext.CategoryFilter.PrepareCategoryFilters(filteringModel.SelectedCategoryIds, GetDictionaryMode(filters.AvailableCategories), _categoryService);
                    model.PagingFilteringContext.ManufacturerFilter.PrepareManufacturerFilters(filteringModel.SelectedManufacturerIds, GetDictionaryMode(filters.AvailableManufacturers), _manufacturerService);
                    model.PagingFilteringContext.VendorFilter.PrepareVendorFilters(filteringModel.SelectedVendorIds, GetDictionaryMode(filters.AvailableVendors), _vendorService);
                    model.PagingFilteringContext.EmiFilter.PrepareEmiFilters(filteringModel.EmiProductsOnly, filters.EmiProductsAvailable);
                }

                //search term statistics
                if (!string.IsNullOrEmpty(model.q))
                {
                    var searchTerm = _searchTermService.GetSearchTermByKeyword(model.q, _storeContext.CurrentStore.Id);
                    if (searchTerm != null)
                    {
                        searchTerm.Count++;
                        _searchTermService.UpdateSearchTerm(searchTerm);
                    }
                    else
                    {
                        searchTerm = new SearchTerm
                        {
                            Keyword = model.q,
                            StoreId = _storeContext.CurrentStore.Id,
                            Count = 1
                        };
                        _searchTermService.InsertSearchTerm(searchTerm);
                    }
                }
            }
            return Ok(model);
        }

        private IDictionary<int, int> GetDictionaryMode(IList<CatalogFilterings.SelectListItemDetails> list)
        {
            var dictionary = new Dictionary<int, int>();

            if (list != null && list.Any())
            {
                foreach (var item in list)
                {
                    dictionary.Add(int.Parse(item.Value), item.Count);
                }
            }

            return dictionary;
        }

        #endregion

        #endregion
    }
}
