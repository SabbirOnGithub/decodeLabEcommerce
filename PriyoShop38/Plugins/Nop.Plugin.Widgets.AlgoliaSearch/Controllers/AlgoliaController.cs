using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Widgets.AlgoliaSearch.Factories;
using Nop.Plugin.Widgets.AlgoliaSearch.Models;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.AlgoliaSearch.Controllers
{
    public class AlgoliaController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly AlgoliaSettings _algoliaSettings;
        private readonly IProductModelFactory _productModelFactory;
        private readonly HttpContextBase _httpContextBase;
        private readonly CatalogSettings _catalogSettings;
        private readonly IWorkContext _workContext;
        private readonly ISearchTermService _searchTermService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public AlgoliaController(AlgoliaSettings algoliaSettings,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            IProductModelFactory productModelFactory,
            HttpContextBase httpContextBase,
            CatalogSettings catalogSettings,
            IWorkContext workContext,
            ISearchTermService searchTermService,
            IStoreContext storeContext)
        {
            this._algoliaSettings = algoliaSettings;
            this._localizationService = localizationService;
            this._webHelper = webHelper;
            this._productModelFactory = productModelFactory;
            this._httpContextBase = httpContextBase;
            this._catalogSettings = catalogSettings;
            this._workContext = workContext;
            this._searchTermService = searchTermService;
            this._storeContext = storeContext;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected void PrepareFilterAttributes(AdvanceSearchPagingFilteringModel pagingFilteringModel, AlgoliaPagingFilteringModel command)
        {
            var currentPageUrl = _webHelper.GetThisPageUrl(true);
            var filteringModel = _productModelFactory.GetAlgoliaFilterings(command.q, command.IncludeEkshopProducts);
            currentPageUrl = _webHelper.RemoveQueryString(currentPageUrl, "pagenumber");

            pagingFilteringModel.AllowEmiFilter = pagingFilteringModel.AllowEmiFilter && filteringModel.EmiProductsAvailable;
            pagingFilteringModel.AllowPriceRangeFilter = pagingFilteringModel.AllowPriceRangeFilter && filteringModel.MaxPrice > filteringModel.MinPrice;

            if (pagingFilteringModel.AllowCategoryFilter)
            {
                foreach (var category in filteringModel.AvailableCategories)
                {
                    var selected = command.SelectedCategoryIds.Any(x => x == Convert.ToInt32(category.Value));
                    List<int> ids;
                    if (selected)
                        ids = command.SelectedCategoryIds.Except(new List<int>() { Convert.ToInt32(category.Value) }).ToList();
                    else
                        ids = command.SelectedCategoryIds.Concat(new List<int>() { Convert.ToInt32(category.Value) }).ToList();

                    pagingFilteringModel.AvailableCategories.Add(new AdvanceSearchPagingFilteringModel.SelectListItemDetails
                    {
                        Count = category.Count,
                        Selected = selected,
                        Text = category.Text,
                        Value = !ids.Any() ? _webHelper.RemoveQueryString(currentPageUrl, "c") :
                            _webHelper.ModifyQueryString(currentPageUrl, "c=" + string.Join(",", ids), null)
                    });
                }
            }

            if (pagingFilteringModel.AllowManufacturerFilter)
            {
                foreach (var manufacturer in filteringModel.AvailableManufacturers)
                {
                    var selected = command.SelectedManufacturerIds.Any(x => x == Convert.ToInt32(manufacturer.Value));
                    List<int> ids;
                    if (selected)
                        ids = command.SelectedManufacturerIds.Except(new List<int>() { Convert.ToInt32(manufacturer.Value) }).ToList();
                    else
                        ids = command.SelectedManufacturerIds.Concat(new List<int>() { Convert.ToInt32(manufacturer.Value) }).ToList();

                    pagingFilteringModel.AvailableManufacturers.Add(new AdvanceSearchPagingFilteringModel.SelectListItemDetails
                    {
                        Count = manufacturer.Count,
                        Selected = selected,
                        Text = manufacturer.Text,
                        Value = !ids.Any() ? _webHelper.RemoveQueryString(currentPageUrl, "m") :
                            _webHelper.ModifyQueryString(currentPageUrl, "m=" + string.Join(",", ids), null)
                    });
                }
            }

            if (pagingFilteringModel.AllowVendorFilter)
            {
                foreach (var vendor in filteringModel.AvailableVendors)
                {
                    var selected = command.SelectedVendorIds.Any(x => x == Convert.ToInt32(vendor.Value));
                    List<int> ids;
                    if (selected)
                        ids = command.SelectedVendorIds.Except(new List<int>() { Convert.ToInt32(vendor.Value) }).ToList();
                    else
                        ids = command.SelectedVendorIds.Concat(new List<int>() { Convert.ToInt32(vendor.Value) }).ToList();

                    pagingFilteringModel.AvailableVendors.Add(new AdvanceSearchPagingFilteringModel.SelectListItemDetails
                    {
                        Count = vendor.Count,
                        Selected = selected,
                        Text = vendor.Text,
                        Value = !ids.Any() ? _webHelper.RemoveQueryString(currentPageUrl, "v") :
                            _webHelper.ModifyQueryString(currentPageUrl, "v=" + string.Join(",", ids), null)
                    });
                }
            }
            
            if (pagingFilteringModel.AllowRatingFilter)
            {
                foreach (var rate in filteringModel.AvailableRatings)
                {
                    var selected = command.SelectedRatings.Any(x => x == Convert.ToInt32(rate.Value));
                    List<int> ids;
                    if (selected)
                        ids = command.SelectedRatings.Except(new List<int>() { Convert.ToInt32(rate.Value) }).ToList();
                    else
                        ids = command.SelectedRatings.Concat(new List<int>() { Convert.ToInt32(rate.Value) }).ToList();

                    pagingFilteringModel.AvailableRatings.Add(new AdvanceSearchPagingFilteringModel.SelectListItemDetails
                    {
                        Count = rate.Count,
                        Selected = selected,
                        Text = rate.Text,
                        Value = !ids.Any() ? _webHelper.RemoveQueryString(currentPageUrl, "r") :
                            _webHelper.ModifyQueryString(currentPageUrl, "r=" + string.Join(",", ids), null)
                    });
                }
            }

            if (pagingFilteringModel.AllowPriceRangeFilter)
            {
                if (command.MinPrice < filteringModel.MinPrice)
                    pagingFilteringModel.PriceRange.CurrentMinPrice = filteringModel.MinPrice;
                else if (command.MinPrice > filteringModel.MaxPrice)
                    pagingFilteringModel.PriceRange.CurrentMinPrice = filteringModel.MaxPrice;
                else
                    pagingFilteringModel.PriceRange.CurrentMinPrice = command.MinPrice;

                if (command.MaxPrice == 0)
                    pagingFilteringModel.PriceRange.CurrentMaxPrice = filteringModel.MaxPrice;
                else if (command.MaxPrice < filteringModel.MinPrice)
                    pagingFilteringModel.PriceRange.CurrentMaxPrice = filteringModel.MinPrice;
                else if (command.MaxPrice > filteringModel.MaxPrice)
                    pagingFilteringModel.PriceRange.CurrentMaxPrice = filteringModel.MaxPrice;
                else
                    pagingFilteringModel.PriceRange.CurrentMaxPrice = command.MaxPrice;
                
                pagingFilteringModel.PriceRange.MaxPrice = filteringModel.MaxPrice;
                pagingFilteringModel.PriceRange.MinPrice = filteringModel.MinPrice;
                pagingFilteringModel.PriceRange.CurrencySymbol = "Tk ";
            }

            if (pagingFilteringModel.AllowEmiFilter)
            {
                pagingFilteringModel.EmiProductOnly = command.EmiProductsOnly;
                if (pagingFilteringModel.EmiProductOnly)
                    pagingFilteringModel.EmiUrl = _webHelper.RemoveQueryString(currentPageUrl, "emi");
                else
                    pagingFilteringModel.EmiUrl = _webHelper.ModifyQueryString(currentPageUrl, "emi=true", null);
            }
        }

        [NonAction]
        protected void PrepareFilterOptions(AdvanceSearchPagingFilteringModel pagingFilteringContext)
        {
            pagingFilteringContext.AllowProductSorting = _algoliaSettings.AllowProductSorting;
            pagingFilteringContext.AllowCustomersToSelectPageSize = _algoliaSettings.AllowCustomersToSelectPageSize;
            pagingFilteringContext.AllowProductViewModeChanging = _algoliaSettings.AllowProductViewModeChanging;
            pagingFilteringContext.AllowPriceRangeFilter = _algoliaSettings.AllowPriceRangeFilter;
            pagingFilteringContext.AllowVendorFilter = _algoliaSettings.AllowVendorFilter;
            pagingFilteringContext.AllowEmiFilter = _algoliaSettings.AllowEmiFilter;
            pagingFilteringContext.AllowRatingFilter = _algoliaSettings.AllowRatingFilter;
            pagingFilteringContext.AllowCategoryFilter = _algoliaSettings.AllowCategoryFilter;
            pagingFilteringContext.AllowSpecificationFilter = _algoliaSettings.AllowSpecificationFilter;
            pagingFilteringContext.AllowManufacturerFilter = _algoliaSettings.AllowManufacturerFilter;
        }

        [NonAction]
        protected virtual void PrepareSortingOptions(AdvanceSearchPagingFilteringModel pagingFilteringModel, AlgoliaPagingFilteringModel command)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException("pagingFilteringModel");

            if (command == null)
                throw new ArgumentNullException("command");

            pagingFilteringModel.AllowProductSorting = _algoliaSettings.AllowProductSorting;

            var activeOptions = Enum.GetValues(typeof(ProductSortingEnum)).Cast<int>()
                .Except(new List<int> { 15 })
                .Select((idOption) =>
                {
                    int order;
                    return new KeyValuePair<int, int>(idOption, _catalogSettings.ProductSortingEnumDisplayOrder.TryGetValue(idOption, out order) ? order : idOption);
                })
                .OrderBy(x => x.Value);

            if (command.OrderBy == null)
                command.OrderBy = activeOptions.First().Key;

            if (pagingFilteringModel.AllowProductSorting)
            {
                foreach (var option in activeOptions)
                {
                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    var sortUrl = _webHelper.ModifyQueryString(currentPageUrl, "orderby=" + (option.Key).ToString(), null);

                    var sortValue = ((ProductSortingEnum)option.Key).GetLocalizedEnum(_localizationService, _workContext);
                    pagingFilteringModel.AvailableSortOptions.Add(new SelectListItem
                    {
                        Text = sortValue,
                        Value = sortUrl,
                        Selected = option.Key == command.OrderBy
                    });
                }
            }
        }

        [NonAction]
        protected virtual void PrepareViewModes(AdvanceSearchPagingFilteringModel pagingFilteringModel, AlgoliaPagingFilteringModel command)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException("pagingFilteringModel");

            if (command == null)
                throw new ArgumentNullException("command");

            pagingFilteringModel.AllowProductViewModeChanging = _algoliaSettings.AllowProductViewModeChanging;

            var viewMode = !string.IsNullOrEmpty(command.ViewMode)
                ? command.ViewMode
                : _catalogSettings.DefaultViewMode;
            pagingFilteringModel.ViewMode = viewMode;
            if (pagingFilteringModel.AllowProductViewModeChanging)
            {
                var currentPageUrl = _webHelper.GetThisPageUrl(true);
                //grid
                pagingFilteringModel.AvailableViewModes.Add(new SelectListItem
                {
                    Text = _localizationService.GetResource("Catalog.ViewMode.Grid"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode=grid", null),
                    Selected = viewMode == "grid"
                });
                //list
                pagingFilteringModel.AvailableViewModes.Add(new SelectListItem
                {
                    Text = _localizationService.GetResource("Catalog.ViewMode.List"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode=list", null),
                    Selected = viewMode == "list"
                });
            }
        }

        [NonAction]
        protected virtual void PreparePageSizeOptions(AdvanceSearchPagingFilteringModel pagingFilteringModel, AlgoliaPagingFilteringModel command)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException("pagingFilteringModel");

            if (command == null)
                throw new ArgumentNullException("command");

            if (command.PageNumber <= 0)
            {
                command.PageNumber = 1;
            }
            pagingFilteringModel.AllowCustomersToSelectPageSize = _algoliaSettings.AllowCustomersToSelectPageSize;
            if (_algoliaSettings.AllowCustomersToSelectPageSize && !string.IsNullOrWhiteSpace(_algoliaSettings.SelectablePageSizes))
            {
                var pageSizes = _algoliaSettings.SelectablePageSizes.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

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

                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    var sortUrl = _webHelper.ModifyQueryString(currentPageUrl, "pagesize={0}", null);
                    sortUrl = _webHelper.RemoveQueryString(sortUrl, "pagenumber");

                    foreach (var pageSize in pageSizes)
                    {
                        int temp;
                        if (!int.TryParse(pageSize, out temp))
                        {
                            continue;
                        }
                        if (temp <= 0)
                        {
                            continue;
                        }

                        pagingFilteringModel.PageSizeOptions.Add(new SelectListItem
                        {
                            Text = pageSize,
                            Value = String.Format(sortUrl, pageSize),
                            Selected = pageSize.Equals(command.PageSize.ToString(), StringComparison.InvariantCultureIgnoreCase)
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
                command.PageSize = _algoliaSettings.PageSize;
            }

            //ensure pge size is specified
            if (command.PageSize <= 0)
            {
                command.PageSize = _algoliaSettings.PageSize;
            }
        }

        #endregion

        #region Methods

        public ActionResult AlgoliaSearch(AlgoliaPagingFilteringModel command)
        {
            if (string.IsNullOrWhiteSpace(command.q) || command.q.Length < _algoliaSettings.MinimumQueryLength)
                return RedirectToRoute("HomePage");
            
            command.LoadFilters(_webHelper);
            command.IncludeEkshopProducts = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.EkshopSessionToken) != null;

            var model = new AlgoliaSearchModel();

            //sotring option
            PrepareSortingOptions(model.PagingFilteringContext, command);
            //view mode
            PrepareViewModes(model.PagingFilteringContext, command);
            //page size
            PreparePageSizeOptions(model.PagingFilteringContext, command);
            //filter
            PrepareFilterOptions(model.PagingFilteringContext);

            var products = _productModelFactory.SearchProducts(command);

            model.Products = products;
            model.q = command.q;
            model.NoResults = !model.Products.Any();

            model.PagingFilteringContext.LoadPagedList(products);

            PrepareFilterAttributes(model.PagingFilteringContext, command);

            //search term statistics
            if (!string.IsNullOrEmpty(command.q))
            {
                var searchTerm = _searchTermService.GetSearchTermByKeyword(command.q, _storeContext.CurrentStore.Id);
                if (searchTerm != null)
                {
                    searchTerm.Count++;
                    _searchTermService.UpdateSearchTerm(searchTerm);
                }
                else
                {
                    searchTerm = new SearchTerm
                    {
                        Keyword = command.q,
                        StoreId = _storeContext.CurrentStore.Id,
                        Count = 1
                    };
                    _searchTermService.InsertSearchTerm(searchTerm);
                }
            }

            return View("~/Plugins/Widgets.AlgoliaSearch/Views/AlgoliaSearch.cshtml", model);
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {
            var model = new ConfigureModel()
            {
                ApplicationId = _algoliaSettings.ApplicationId,
                SeachOnlyKey = _algoliaSettings.SeachOnlyKey,
                MinimumQueryLength = _algoliaSettings.MinimumQueryLength
            };
            return View("~/Plugins/Widgets.AlgoliaSearch/Views/AlgoliaSearchBox.cshtml", model);
        }

        #endregion
    }
}
