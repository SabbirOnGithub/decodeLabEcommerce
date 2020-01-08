using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Vendors;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.UI.Paging;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Catalog
{
    public partial class CatalogPagingFilteringModel : BasePageableModel
    {
        #region Methods

        public void LoadAdvanceFilterData(IWebHelper webHelper)
        {
            var orderByStr = webHelper.QueryString<string>("orderby");
            if (!string.IsNullOrWhiteSpace(orderByStr) && int.TryParse(orderByStr, out int ob))
                OrderBy = ob;

            var pageSizeStr = webHelper.QueryString<string>("pagesize");
            if (!string.IsNullOrWhiteSpace(pageSizeStr) && int.TryParse(pageSizeStr, out int ps))
                PageSize = ps;

            var pageNumberStr = webHelper.QueryString<string>("pagenumber");
            if (!string.IsNullOrWhiteSpace(pageNumberStr) && int.TryParse(pageNumberStr, out int pn))
                PageNumber = pn;

            var advStr = webHelper.QueryString<string>("adv");
            if (!string.IsNullOrWhiteSpace(advStr) && bool.TryParse(advStr, out bool adv))
                LoadAdvanceFilters = adv;
        }

        #endregion

        #region Constructors

        public CatalogPagingFilteringModel()
        {
            this.AvailableSortOptions = new List<SelectListItem>();
            this.AvailableViewModes = new List<SelectListItem>();
            this.PageSizeOptions = new List<SelectListItem>();

            this.PriceRangeFilter = new PriceRangeFilterModel();
            this.SpecificationFilter = new SpecificationFilterModel();
            this.CategoryFilter = new CategoryFilterModel();
            this.ManufacturerFilter = new ManufacturerFilterModel();
            this.VendorFilter = new VendorFilterModel();
            this.EmiFilter = new EmiFilterModel();
        }

        #endregion

        #region Properties

        public bool LoadAdvanceFilters { get; set; }
        /// <summary>
        /// Price range filter model
        /// </summary>
        public PriceRangeFilterModel PriceRangeFilter { get; set; }

        /// <summary>
        /// Specification filter model
        /// </summary>
        public SpecificationFilterModel SpecificationFilter { get; set; }

        public CategoryFilterModel CategoryFilter { get; set; }

        public ManufacturerFilterModel ManufacturerFilter { get; set; }

        public VendorFilterModel VendorFilter { get; set; }

        public EmiFilterModel EmiFilter { get; set; }

        public bool AllowProductSorting { get; set; }
        public IList<SelectListItem> AvailableSortOptions { get; set; }

        public bool AllowProductViewModeChanging { get; set; }
        public IList<SelectListItem> AvailableViewModes { get; set; }

        public bool AllowCustomersToSelectPageSize { get; set; }
        public IList<SelectListItem> PageSizeOptions { get; set; }

        /// <summary>
        /// Order by
        /// </summary>
        public int OrderBy { get; set; }

        /// <summary>
        /// Product sorting
        /// </summary>
        public string ViewMode { get; set; }


        #endregion

        #region Nested classes

        public partial class EmiFilterModel
        {

            #region Const

            private const string QUERYSTRINGPARAM = "emi";

            #endregion

            #region Methods

            public virtual bool GetEmiFilterStatus(IWebHelper webHelper)
            {
                var emiStr = webHelper.QueryString<string>(QUERYSTRINGPARAM);
                if (!String.IsNullOrWhiteSpace(emiStr) && bool.TryParse(emiStr, out bool r))
                    return r;
                
                return false;
            }

            public virtual void PrepareEmiFilters(bool emiProductsOnly, bool filterableEmi)
            {
                this.Enabled = filterableEmi;
                this.EmiProductsOnly = emiProductsOnly;
            }

            #endregion

            #region Properties

            public bool Enabled { get; set; }

            public bool EmiProductsOnly { get; set; }

            #endregion
        }

        public partial class VendorFilterModel
        {
            #region Const

            private const string QUERYSTRINGPARAM = "vends";

            #endregion

            #region Ctor

            public VendorFilterModel()
            {
                AlreadyFilteredItems = new List<VendorFilterItem>();
                NotFilteredItems = new List<VendorFilterItem>();
            }

            #endregion

            #region Methods

            public virtual List<int> GetAlreadyFilteredVendorIds(IWebHelper webHelper)
            {
                var result = new List<int>();

                var alreadyFilteredCatsStr = webHelper.QueryString<string>(QUERYSTRINGPARAM);
                if (String.IsNullOrWhiteSpace(alreadyFilteredCatsStr))
                    return result;

                foreach (var spec in alreadyFilteredCatsStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int catId;
                    int.TryParse(spec.Trim(), out catId);
                    if (!result.Contains(catId))
                        result.Add(catId);
                }
                return result;
            }

            public virtual void PrepareVendorFilters(IList<int> alreadyFilteredVendorIds,
                IDictionary<int, int> filterableVendorIds, IVendorService vendorService)
            {
                var allFilters = new List<SpecificationAttributeOptionFilter>();
                if (filterableVendorIds != null &&
                    filterableVendorIds.Count == 0)
                    return;

                var vendors = vendorService.GetVendorsByIds(filterableVendorIds.Keys.ToArray());

                if (vendors.Any())
                {
                    foreach (var vendor in vendors)
                    {
                        if (alreadyFilteredVendorIds != null && alreadyFilteredVendorIds.Contains(vendor.Id))
                        {
                            AlreadyFilteredItems.Add(new VendorFilterItem()
                            {
                                VendorName = vendor.Name,
                                Count = filterableVendorIds[vendor.Id],
                                FilterId = vendor.Id
                            });
                        }
                        else
                        {
                            NotFilteredItems.Add(new VendorFilterItem()
                            {
                                VendorName = vendor.Name,
                                Count = filterableVendorIds[vendor.Id],
                                FilterId = vendor.Id
                            });
                        }
                    }

                    this.AlreadyFilteredItems = this.AlreadyFilteredItems.OrderByDescending(x => x.Count).ToList();
                    this.NotFilteredItems = this.NotFilteredItems.OrderByDescending(x => x.Count).ToList();
                    this.Enabled = true;
                }
                else
                {
                    Enabled = false;
                }
            }

            #endregion

            #region Properties

            public bool Enabled { get; set; }
            public IList<VendorFilterItem> AlreadyFilteredItems { get; set; }
            public IList<VendorFilterItem> NotFilteredItems { get; set; }

            #endregion
        }

        public partial class VendorFilterItem : BaseNopModel
        {
            public int Count { get; set; }
            public string VendorName { get; set; }
            public int FilterId { get; set; }
        }

        public partial class ManufacturerFilterModel
        {
            #region Const

            private const string QUERYSTRINGPARAM = "manfs";

            #endregion

            #region Ctor

            public ManufacturerFilterModel()
            {
                AlreadyFilteredItems = new List<ManufacturerFilterItem>();
                NotFilteredItems = new List<ManufacturerFilterItem>();
            }

            #endregion

            #region Methods

            public virtual List<int> GetAlreadyFilteredManufacturerIds(IWebHelper webHelper)
            {
                var result = new List<int>();

                var alreadyFilteredCatsStr = webHelper.QueryString<string>(QUERYSTRINGPARAM);
                if (String.IsNullOrWhiteSpace(alreadyFilteredCatsStr))
                    return result;

                foreach (var spec in alreadyFilteredCatsStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int catId;
                    int.TryParse(spec.Trim(), out catId);
                    if (!result.Contains(catId))
                        result.Add(catId);
                }
                return result;
            }

            public virtual void PrepareManufacturerFilters(IList<int> alreadyFilteredManufacturerIds,
                IDictionary<int, int> filterableManufacturerIds, IManufacturerService manufacturerService)
            {
                var allFilters = new List<SpecificationAttributeOptionFilter>();
                if (filterableManufacturerIds != null && 
                    filterableManufacturerIds.Count == 0)
                    return;

                var manufacturers = manufacturerService.GetManufacturersByIds(filterableManufacturerIds.Keys.ToArray());

                if (manufacturers.Any())
                {
                    foreach (var manufacturer in manufacturers)
                    {
                        if (alreadyFilteredManufacturerIds != null && alreadyFilteredManufacturerIds.Contains(manufacturer.Id))
                        {
                            AlreadyFilteredItems.Add(new ManufacturerFilterItem()
                            {
                                ManufacturerName = manufacturer.Name,
                                Count = filterableManufacturerIds[manufacturer.Id],
                                FilterId = manufacturer.Id
                            });
                        }
                        else
                        {
                            NotFilteredItems.Add(new ManufacturerFilterItem()
                            {
                                ManufacturerName = manufacturer.Name,
                                Count = filterableManufacturerIds[manufacturer.Id],
                                FilterId = manufacturer.Id
                            });
                        }
                    }

                    this.AlreadyFilteredItems = this.AlreadyFilteredItems.OrderByDescending(x => x.Count).ToList();
                    this.NotFilteredItems = this.NotFilteredItems.OrderByDescending(x => x.Count).ToList();
                    this.Enabled = true;
                }
                else
                {
                    Enabled = false;
                }
            }

            #endregion

            #region Properties

            public bool Enabled { get; set; }
            public IList<ManufacturerFilterItem> AlreadyFilteredItems { get; set; }
            public IList<ManufacturerFilterItem> NotFilteredItems { get; set; }

            #endregion
        }

        public partial class ManufacturerFilterItem : BaseNopModel
        {
            public int Count { get; set; }
            public string ManufacturerName { get; set; }
            public int FilterId { get; set; }
        }

        public partial class CategoryFilterModel
        {
            #region Const

            private const string QUERYSTRINGPARAM = "cats";

            #endregion

            #region Ctor
            
            public CategoryFilterModel()
            {
                AlreadyFilteredItems = new List<CategoryFilterItem>();
                NotFilteredItems = new List<CategoryFilterItem>();
            }

            #endregion

            #region Methods

            public virtual List<int> GetAlreadyFilteredCategoryIds(IWebHelper webHelper)
            {
                var result = new List<int>();

                var alreadyFilteredCatsStr = webHelper.QueryString<string>(QUERYSTRINGPARAM);
                if (String.IsNullOrWhiteSpace(alreadyFilteredCatsStr))
                    return result;

                foreach (var spec in alreadyFilteredCatsStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int catId;
                    int.TryParse(spec.Trim(), out catId);
                    if (!result.Contains(catId))
                        result.Add(catId);
                }
                return result;
            }

            public virtual void PrepareCategoryFilters(IList<int> alreadyFilteredCategoryIds,
                IDictionary<int, int> filterableCategoryIds, ICategoryService categoryService)
            {
                var allFilters = new List<SpecificationAttributeOptionFilter>();
                if (filterableCategoryIds != null &&
                    filterableCategoryIds.Count == 0)
                    return;

                var categories = categoryService.GetCategoriesByIds(filterableCategoryIds.Keys.ToArray());

                if (categories.Any())
                {
                    foreach (var category in categories)
                    {
                        var item = filterableCategoryIds[category.Id];
                        if (alreadyFilteredCategoryIds != null && alreadyFilteredCategoryIds.Contains(category.Id))
                        {
                            AlreadyFilteredItems.Add(new CategoryFilterItem()
                            {
                                CategoryName = category.Name,
                                Count = filterableCategoryIds[category.Id],
                                FilterId = category.Id
                            });
                        }
                        else
                        {
                            NotFilteredItems.Add(new CategoryFilterItem()
                            {
                                CategoryName = category.Name,
                                Count = filterableCategoryIds[category.Id],
                                FilterId = category.Id
                            });
                        }
                    }

                    this.AlreadyFilteredItems = this.AlreadyFilteredItems.OrderByDescending(x => x.Count).ToList();
                    this.NotFilteredItems = this.NotFilteredItems.OrderByDescending(x => x.Count).ToList();
                    this.Enabled = true;
                }
                else
                {
                    Enabled = false;
                }
            }

            #endregion

            #region Properties

            public bool Enabled { get; set; }
            public IList<CategoryFilterItem> AlreadyFilteredItems { get; set; }
            public IList<CategoryFilterItem> NotFilteredItems { get; set; }

            #endregion
        }

        public partial class CategoryFilterItem : BaseNopModel
        {
            public int Count { get; set; }
            public string CategoryName { get; set; }
            public int FilterId { get; set; }
        }

        public partial class PriceRangeFilterModel : BaseNopModel
        {
            #region Const

            private const string QUERYSTRINGPARAM = "price";

            #endregion 

            #region Ctor

            public PriceRangeFilterModel()
            {
            }

            #endregion

            #region Utilities

            protected virtual string ExcludeQueryStringParams(string url, IWebHelper webHelper)
            {
                var excludedQueryStringParams = "pagenumber"; //remove page filtering
                if (!String.IsNullOrEmpty(excludedQueryStringParams))
                {
                    string[] excludedQueryStringParamsSplitted = excludedQueryStringParams.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string exclude in excludedQueryStringParamsSplitted)
                        url = webHelper.RemoveQueryString(url, exclude);
                }

                return url;
            }

            #endregion

            #region Methods

            public virtual PriceRange GetSelectedPriceRange(IWebHelper webHelper)
            {
                var range = webHelper.QueryString<string>(QUERYSTRINGPARAM);
                if (String.IsNullOrEmpty(range))
                    return null;
                string[] fromTo = range.Trim().Split(new [] { '-' });
                if (fromTo.Length == 2)
                {
                    decimal? from = null;
                    if (!String.IsNullOrEmpty(fromTo[0]) && !String.IsNullOrEmpty(fromTo[0].Trim()))
                        from = decimal.Parse(fromTo[0].Trim(), new CultureInfo("en-US"));
                    decimal? to = null;
                    if (!String.IsNullOrEmpty(fromTo[1]) && !String.IsNullOrEmpty(fromTo[1].Trim()))
                        to = decimal.Parse(fromTo[1].Trim(), new CultureInfo("en-US"));

                    var priceRangeList = new PriceRange()
                    {
                        From = from.GetValueOrDefault(),
                        To = to.GetValueOrDefault()
                    };
                    return priceRangeList;
                }
                return null;
            }

            public virtual void LoadPriceRangeFilters(decimal minPrice, decimal maxPrice, decimal? minPriceConverted, decimal? maxPriceConverted)
            {
                this.MinPrice = minPrice;
                this.MaxPrice = maxPrice;
                this.CurrentMaxPrice = maxPriceConverted.HasValue ? maxPriceConverted.Value : this.MaxPrice;
                this.CurrentMinPrice = minPriceConverted.HasValue ? minPriceConverted.Value : this.MinPrice;

                this.Enabled = MaxPrice > 0;
            }
            
            #endregion

            #region Properties

            public bool Enabled { get; set; }

            public string CurrencySymbol { get; set; }

            public decimal MinPrice { get; set; }

            public decimal MaxPrice { get; set; }

            public decimal CurrentMinPrice { get; set; }

            public decimal CurrentMaxPrice { get; set; }

            #endregion
        }

        public partial class SpecificationFilterModel : BaseNopModel
        {
            #region Const

            private const string QUERYSTRINGPARAM = "specs";

            #endregion

            #region Ctor

            public SpecificationFilterModel()
            {
                this.AlreadyFilteredItems = new List<SpecificationFilterItem>();
                this.NotFilteredItems = new List<SpecificationFilterItem>();
                this.FilterItems = new List<FilterItem>();
            }

            #endregion

            #region Methods

            public virtual List<int> GetAlreadyFilteredSpecOptionIds(IWebHelper webHelper)
            {
                var result = new List<int>();

                var alreadyFilteredSpecsStr = webHelper.QueryString<string>(QUERYSTRINGPARAM);
                if (String.IsNullOrWhiteSpace(alreadyFilteredSpecsStr))
                    return result;

                foreach (var spec in alreadyFilteredSpecsStr.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int specId;
                    int.TryParse(spec.Trim(), out specId);
                    if (!result.Contains(specId))
                        result.Add(specId);
                }
                return result;
            }

            public virtual void PrepareSpecsFilters(IList<int> alreadyFilteredSpecOptionIds,
                IDictionary<int, int> filterableSpecificationAttributeOptionIds,
                ISpecificationAttributeService specificationAttributeService)
            {
                var allFilters = new List<SpecificationAttributeOptionFilter>();
                if (filterableSpecificationAttributeOptionIds != null &&
                    filterableSpecificationAttributeOptionIds.Count == 0)
                    return;

                var specificationAttributeOptions = specificationAttributeService
                    .GetSpecificationAttributeOptionsByIds(filterableSpecificationAttributeOptionIds.Keys.ToArray());

                foreach (var sao in specificationAttributeOptions)
                {
                    var sa = sao.SpecificationAttribute;
                    if (sa != null)
                    {
                        allFilters.Add(new SpecificationAttributeOptionFilter
                        {
                            SpecificationAttributeId = sa.Id,
                            SpecificationAttributeName = sa.Name,
                            SpecificationAttributeDisplayOrder = sa.DisplayOrder,
                            SpecificationAttributeOptionId = sao.Id,
                            SpecificationAttributeOptionName = sao.Name,
                            SpecificationAttributeOptionDisplayOrder = sao.DisplayOrder
                        });
                    }
                }

                //sort loaded options
                allFilters = allFilters.OrderBy(saof => saof.SpecificationAttributeDisplayOrder)
                    .ThenBy(saof => saof.SpecificationAttributeName)
                    .ThenBy(saof => saof.SpecificationAttributeOptionDisplayOrder)
                    .ThenBy(saof => saof.SpecificationAttributeOptionName).ToList();
                
                //get already filtered specification options
                var alreadyFilteredOptions = allFilters
                    .Where(x => alreadyFilteredSpecOptionIds.Contains(x.SpecificationAttributeOptionId))
                    .Select(x => x)
                    .ToList();

                //get not filtered specification options
                var notFilteredOptions = new List<SpecificationAttributeOptionFilter>();
                foreach (var saof in allFilters)
                {
                    //do not add already filtered specification options
                    if (alreadyFilteredOptions.FirstOrDefault(x => x.SpecificationAttributeOptionId == saof.SpecificationAttributeOptionId) != null)
                        continue;

                    //else add it
                    notFilteredOptions.Add(saof);
                }

                //filter items with selected or not property
                var specAtrrNames = allFilters.GroupBy(x => x.SpecificationAttributeName).ToList();
                var filterItems = new List<FilterItem>();
                foreach (var spa in specAtrrNames)
                {
                    var filterItem = new FilterItem();
                    filterItem.SpecificationAttributeName = spa.Key;
                    var options = new List<SpecificationAttributeOption>();
                    foreach (var saof in allFilters)
                    {
                        
                        if (saof.SpecificationAttributeName == spa.Key)
                        {
                            var option = new SpecificationAttributeOption();
                            option.SpecificationAttributeOptionName = saof.SpecificationAttributeOptionName;
                            option.FilterId = saof.SpecificationAttributeOptionId;
                            if (alreadyFilteredOptions.FirstOrDefault(x => x.SpecificationAttributeOptionId == saof.SpecificationAttributeOptionId) != null)
                                option.Selected = true;
                            options.Add(option);
                        }
                    }
                    filterItem.SpecificationAttributeOptions = options;
                    filterItems.Add(filterItem);
                }

                //prepare the model properties
                if (alreadyFilteredOptions.Count > 0 || notFilteredOptions.Count > 0)
                {
                    this.Enabled = true;
                    
                    this.AlreadyFilteredItems = alreadyFilteredOptions.ToList().Select(x =>
                    {
                        var item = new SpecificationFilterItem();
                        item.SpecificationAttributeName = x.SpecificationAttributeName;
                        item.SpecificationAttributeOptionName = x.SpecificationAttributeOptionName;
                        item.FilterId = x.SpecificationAttributeOptionId;
                        return item;
                    }).ToList();

                    this.NotFilteredItems = notFilteredOptions.ToList().Select(x =>
                    {
                        var item = new SpecificationFilterItem();
                        item.SpecificationAttributeName = x.SpecificationAttributeName;
                        item.SpecificationAttributeOptionName = x.SpecificationAttributeOptionName;
                        item.FilterId = x.SpecificationAttributeOptionId;
                        
                        return item;
                    }).ToList();

                    this.FilterItems = filterItems;
                }
                else
                {
                    this.Enabled = false;
                }
            }

            #endregion

            #region Properties

            public bool Enabled { get; set; }
            public IList<SpecificationFilterItem> AlreadyFilteredItems { get; set; }
            public IList<SpecificationFilterItem> NotFilteredItems { get; set; }
            public IList<FilterItem> FilterItems { get; set; }

            #endregion
        }

        public partial class SpecificationFilterItem : BaseNopModel
        {
            public string SpecificationAttributeName { get; set; }
            public string SpecificationAttributeOptionName { get; set; }
            public int FilterId { get; set; }
        }

        public partial class FilterItem : BaseNopModel
        {
            public FilterItem()
            {
                this.SpecificationAttributeOptions = new List<SpecificationAttributeOption>();
            }
            public string SpecificationAttributeName { get; set; }
            public IList<SpecificationAttributeOption> SpecificationAttributeOptions { get; set; }
        }        

        public partial class SpecificationAttributeOption : BaseNopModel
        {
            public string SpecificationAttributeOptionName { get; set; }
            public int FilterId { get; set; }
            public bool Selected { get; set; }
        }

        #endregion
    }
}