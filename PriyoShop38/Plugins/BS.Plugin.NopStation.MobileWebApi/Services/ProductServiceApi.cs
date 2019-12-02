using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using System.Data;
using Nop.Data;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public class ProductServiceApi : IProductServiceApi
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly CatalogSettings _catalogSettings;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;
        private readonly IRepository<BS_FeaturedProducts> _featuredProdRepository;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;


        public ProductServiceApi(IRepository<Product> productRepository,
            IRepository<LocalizedProperty> localizedPropertyRepository,
            CatalogSettings catalogSettings,
            IRepository<AclRecord> aclRepository,
            IRepository<StoreMapping> storeMappingRepository,
            ILanguageService languageService,
            IWorkContext workContext,
            IRepository<BS_FeaturedProducts> featuredProdRepository,
            IDataProvider dataProvider,
            IDbContext dbContext)
        {
            this._productRepository = productRepository;
            this._localizedPropertyRepository = localizedPropertyRepository;
            this._catalogSettings = catalogSettings;
            this._aclRepository = aclRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._languageService = languageService;
            this._workContext=workContext;
            this._featuredProdRepository = featuredProdRepository;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
        }

        public virtual IPagedList<Product> SearchProducts(
            out IDictionary<int, int> filterableSpecificationAttributeOptionIds,
            out IDictionary<int, int> filterableCategoryIds,
            out IDictionary<int, int> filterableManufacturerIds,
            out IDictionary<int, int> filterableVendorIds,
            out IDictionary<int, int> filterableRatings,
            out decimal filterableMaxPrice,
            out decimal filterableMinPrice,
            out bool filterableEmi,
            bool loadAdvanceFilterOptions = false,
            int categoryId = 0,
            int manufacturerId = 0,
            int vendorId = 0,
            int storeId = 0,
            int warehouseId = 0,
            ProductType? productType = null,
            bool? featuredProducts = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            int languageId = 0,
            IList<int> filteredCategoryIds = null,
            IList<int> filteredManufacturerIds = null,
            IList<int> filteredVendorIds = null,
            IList<int> filteredSpecs = null,
            IList<int> filteredRatings = null,
            bool emiProductOnly = false,
            ProductSortingEnum orderBy = ProductSortingEnum.Position,
            int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            filterableSpecificationAttributeOptionIds = new Dictionary<int, int>();
            filterableCategoryIds = new Dictionary<int, int>();
            filterableManufacturerIds = new Dictionary<int, int>();
            filterableVendorIds = new Dictionary<int, int>();
            filterableSpecificationAttributeOptionIds = new Dictionary<int, int>();
            filterableRatings = new Dictionary<int, int>();
            filterableMaxPrice = 0;
            filterableMinPrice = 0;
            filterableEmi = false;

            //search by keyword
            var totalPublishedLanguages = _languageService.GetAllLanguages().Count;
            bool searchLocalizedValue = totalPublishedLanguages >= 2;

            //Access control list. Allowed customer roles
            var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();

            //pass customer role identifiers as comma-delimited string
            string commaSeparatedAllowedCustomerRoleIds = string.Join(",", allowedCustomerRolesIds);


            //pass ratings as comma-delimited string
            string commaSeparatedFilteredCategoryIds = filterableCategoryIds == null ? "" : string.Join(",", filterableCategoryIds);
            string commaSeparatedFilteredManufacturerIds = filteredManufacturerIds == null ? "" : string.Join(",", filteredManufacturerIds);
            string commaSeparatedFilteredVendorIds = filteredVendorIds == null ? "" : string.Join(",", filteredVendorIds);

            //pass specification identifiers as comma-delimited string
            string commaSeparatedSpecIds = "";
            if (filteredSpecs != null)
            {
                ((List<int>)filteredSpecs).Sort();
                commaSeparatedSpecIds = string.Join(",", filteredSpecs);
            }
            string commaSeparatedFilteredRatings = filteredRatings == null ? "" : string.Join(",", filteredRatings);

            //some databases don't support int.MaxValue
            if (pageSize == int.MaxValue)
                pageSize = int.MaxValue - 1;


            //prepare parameters
            var pCategoryId = _dataProvider.GetParameter();
            pCategoryId.ParameterName = "CategoryId";
            pCategoryId.Value = categoryId;
            pCategoryId.DbType = DbType.Int32;

            var pManufacturerId = _dataProvider.GetParameter();
            pManufacturerId.ParameterName = "ManufacturerId";
            pManufacturerId.Value = manufacturerId;
            pManufacturerId.DbType = DbType.Int32;

            var pVendorId = _dataProvider.GetParameter();
            pVendorId.ParameterName = "VendorId";
            pVendorId.Value = vendorId;
            pVendorId.DbType = DbType.Int32;

            var pStoreId = _dataProvider.GetParameter();
            pStoreId.ParameterName = "StoreId";
            pStoreId.Value = !_catalogSettings.IgnoreStoreLimitations ? storeId : 0;
            pStoreId.DbType = DbType.Int32;

            var pWarehouseId = _dataProvider.GetParameter();
            pWarehouseId.ParameterName = "WarehouseId";
            pWarehouseId.Value = warehouseId;
            pWarehouseId.DbType = DbType.Int32;

            var pProductTypeId = _dataProvider.GetParameter();
            pProductTypeId.ParameterName = "ProductTypeId";
            pProductTypeId.Value = productType.HasValue ? (object)productType.Value : DBNull.Value;
            pProductTypeId.DbType = DbType.Int32;

            var pFeaturedProducts = _dataProvider.GetParameter();
            pFeaturedProducts.ParameterName = "FeaturedProducts";
            pFeaturedProducts.Value = featuredProducts.HasValue ? (object)featuredProducts.Value : DBNull.Value;
            pFeaturedProducts.DbType = DbType.Boolean;

            var pPriceMin = _dataProvider.GetParameter();
            pPriceMin.ParameterName = "PriceMin";
            pPriceMin.Value = priceMin.HasValue ? (object)priceMin.Value : DBNull.Value;
            pPriceMin.DbType = DbType.Decimal;

            var pPriceMax = _dataProvider.GetParameter();
            pPriceMax.ParameterName = "PriceMax";
            pPriceMax.Value = priceMax.HasValue ? (object)priceMax.Value : DBNull.Value;
            pPriceMax.DbType = DbType.Decimal;

            var pLanguageId = _dataProvider.GetParameter();
            pLanguageId.ParameterName = "LanguageId";
            pLanguageId.Value = searchLocalizedValue ? languageId : 0;
            pLanguageId.DbType = DbType.Int32;

            var pFilteredCategories = _dataProvider.GetParameter();
            pFilteredCategories.ParameterName = "FilteredCategories";
            pFilteredCategories.Value = (object)commaSeparatedFilteredCategoryIds ?? DBNull.Value;
            pFilteredCategories.DbType = DbType.String;

            var pFilteredManufacturers = _dataProvider.GetParameter();
            pFilteredManufacturers.ParameterName = "FilteredManufacturers";
            pFilteredManufacturers.Value = (object)commaSeparatedFilteredManufacturerIds ?? DBNull.Value;
            pFilteredManufacturers.DbType = DbType.String;

            var pFilteredVendors = _dataProvider.GetParameter();
            pFilteredVendors.ParameterName = "FilteredVendors";
            pFilteredVendors.Value = (object)commaSeparatedFilteredVendorIds ?? DBNull.Value;
            pFilteredVendors.DbType = DbType.String;

            var pFilteredSpecs = _dataProvider.GetParameter();
            pFilteredSpecs.ParameterName = "FilteredSpecs";
            pFilteredSpecs.Value = (object)commaSeparatedSpecIds ?? DBNull.Value;
            pFilteredSpecs.DbType = DbType.String;

            var pFilteredRatings = _dataProvider.GetParameter();
            pFilteredRatings.ParameterName = "FilteredRatings";
            pFilteredRatings.Value = (object)commaSeparatedFilteredRatings ?? DBNull.Value;
            pFilteredRatings.DbType = DbType.String;

            var pEmiProductOnly = _dataProvider.GetParameter();
            pEmiProductOnly.ParameterName = "EmiProductOnly";
            pEmiProductOnly.Value = emiProductOnly;
            pEmiProductOnly.DbType = DbType.Boolean;

            var pOrderBy = _dataProvider.GetParameter();
            pOrderBy.ParameterName = "OrderBy";
            pOrderBy.Value = (int)orderBy;
            pOrderBy.DbType = DbType.Int32;

            var pAllowedCustomerRoleIds = _dataProvider.GetParameter();
            pAllowedCustomerRoleIds.ParameterName = "AllowedCustomerRoleIds";
            pAllowedCustomerRoleIds.Value = !_catalogSettings.IgnoreAcl ? commaSeparatedAllowedCustomerRoleIds : "";
            pAllowedCustomerRoleIds.DbType = DbType.String;

            var pPageIndex = _dataProvider.GetParameter();
            pPageIndex.ParameterName = "PageIndex";
            pPageIndex.Value = pageIndex;
            pPageIndex.DbType = DbType.Int32;

            var pPageSize = _dataProvider.GetParameter();
            pPageSize.ParameterName = "PageSize";
            pPageSize.Value = pageSize;
            pPageSize.DbType = DbType.Int32;

            var pLoadAdvanceFilterOptions = _dataProvider.GetParameter();
            pLoadAdvanceFilterOptions.ParameterName = "LoadAdvanceFilterOptions";
            pLoadAdvanceFilterOptions.Value = loadAdvanceFilterOptions;
            pLoadAdvanceFilterOptions.DbType = DbType.Boolean;

            var pFilterableSpecificationAttributeOptionIds = _dataProvider.GetParameter();
            pFilterableSpecificationAttributeOptionIds.ParameterName = "FilterableSpecificationAttributeOptionIds";
            pFilterableSpecificationAttributeOptionIds.Direction = ParameterDirection.Output;
            pFilterableSpecificationAttributeOptionIds.Size = int.MaxValue - 1;
            pFilterableSpecificationAttributeOptionIds.DbType = DbType.String;

            var pFilterableCategoryIds = _dataProvider.GetParameter();
            pFilterableCategoryIds.ParameterName = "FilterableCategoryIds";
            pFilterableCategoryIds.Direction = ParameterDirection.Output;
            pFilterableCategoryIds.Size = int.MaxValue - 1;
            pFilterableCategoryIds.DbType = DbType.String;

            var pFilterableManufacturerIds = _dataProvider.GetParameter();
            pFilterableManufacturerIds.ParameterName = "FilterableManufacturerIds";
            pFilterableManufacturerIds.Direction = ParameterDirection.Output;
            pFilterableManufacturerIds.Size = int.MaxValue - 1;
            pFilterableManufacturerIds.DbType = DbType.String;

            var pFilterableVendorIds = _dataProvider.GetParameter();
            pFilterableVendorIds.ParameterName = "FilterableVendorIds";
            pFilterableVendorIds.Direction = ParameterDirection.Output;
            pFilterableVendorIds.Size = int.MaxValue - 1;
            pFilterableVendorIds.DbType = DbType.String;

            var pFilterableRatings = _dataProvider.GetParameter();
            pFilterableRatings.ParameterName = "FilterableRatings";
            pFilterableRatings.Direction = ParameterDirection.Output;
            pFilterableRatings.Size = int.MaxValue - 1;
            pFilterableRatings.DbType = DbType.String;

            var pFilterableMaxPrice = _dataProvider.GetParameter();
            pFilterableMaxPrice.ParameterName = "FilterableMaxPrice";
            pFilterableMaxPrice.Direction = ParameterDirection.Output;
            pFilterableMaxPrice.DbType = DbType.Decimal;

            var pFilterableMinPrice = _dataProvider.GetParameter();
            pFilterableMinPrice.ParameterName = "FilterableMinPrice";
            pFilterableMinPrice.Direction = ParameterDirection.Output;
            pFilterableMinPrice.DbType = DbType.Decimal;

            var pFilterableEmi = _dataProvider.GetParameter();
            pFilterableEmi.ParameterName = "FilterableEmi";
            pFilterableEmi.Direction = ParameterDirection.Output;
            pFilterableEmi.DbType = DbType.Boolean;

            var pTotalRecords = _dataProvider.GetParameter();
            pTotalRecords.ParameterName = "TotalRecords";
            pTotalRecords.Direction = ParameterDirection.Output;
            pTotalRecords.DbType = DbType.Int32;


            //invoke stored procedure
            var products = _dbContext.ExecuteStoredProcedureList<Product>(
                "ProductLoadAdvanceAllPaged",
                pCategoryId,
                pManufacturerId,
                pVendorId,
                pStoreId,
                pWarehouseId,
                pProductTypeId,
                pFeaturedProducts,
                pPriceMin,
                pPriceMax,
                pLanguageId,
                pFilteredCategories,
                pFilteredManufacturers,
                pFilteredVendors,
                pFilteredSpecs,
                pFilteredRatings,
                pEmiProductOnly,
                pOrderBy,
                pAllowedCustomerRoleIds,
                pPageIndex,
                pPageSize,
                pLoadAdvanceFilterOptions,
                pFilterableSpecificationAttributeOptionIds,
                pFilterableCategoryIds,
                pFilterableManufacturerIds,
                pFilterableVendorIds,
                pFilterableRatings,
                pFilterableMaxPrice,
                pFilterableMinPrice,
                pFilterableEmi,
                pTotalRecords);

            if (loadAdvanceFilterOptions)
            {
                string filterableSpecificationAttributeOptionIdsStr = (pFilterableSpecificationAttributeOptionIds.Value != DBNull.Value) ? (string)pFilterableSpecificationAttributeOptionIds.Value : "";
                if (!string.IsNullOrWhiteSpace(filterableSpecificationAttributeOptionIdsStr))
                {
                    var filterableSpecificationAttributeOptionIdsArr = filterableSpecificationAttributeOptionIdsStr
                      .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(int.Parse)
                      .ToList();
                    foreach (var item in filterableSpecificationAttributeOptionIdsArr)
                    {
                        filterableSpecificationAttributeOptionIds.Add(item, 0);
                    }
                }

                string filterableCategoryIdsStr = (pFilterableCategoryIds.Value != DBNull.Value) ? (string)pFilterableCategoryIds.Value : "";
                if (!string.IsNullOrWhiteSpace(filterableCategoryIdsStr))
                {
                    var filterableCategoryIdsArr = filterableCategoryIdsStr
                      .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                      .ToList();
                    foreach (var item in filterableCategoryIdsArr)
                    {
                        var token = item.Split(new[] { "___" }, StringSplitOptions.None);
                        if (token.Length == 2)
                        {
                            filterableCategoryIds.Add(int.Parse(token[0]), int.Parse(token[1]));
                        }
                    }
                }

                string filterableManufacturerIdsStr = (pFilterableManufacturerIds.Value != DBNull.Value) ? (string)pFilterableManufacturerIds.Value : "";
                if (!string.IsNullOrWhiteSpace(filterableManufacturerIdsStr))
                {
                    var filterableManufacturerIdsArr = filterableManufacturerIdsStr
                      .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                      .ToList();
                    foreach (var item in filterableManufacturerIdsArr)
                    {
                        var token = item.Split(new[] { "___" }, StringSplitOptions.None);
                        if (token.Length == 2)
                        {
                            filterableManufacturerIds.Add(int.Parse(token[0]), int.Parse(token[1]));
                        }
                    }
                }

                string filterableVendorIdsStr = (pFilterableVendorIds.Value != DBNull.Value) ? (string)pFilterableVendorIds.Value : "";
                if (!string.IsNullOrWhiteSpace(filterableVendorIdsStr))
                {
                    var filterableVendorIdsArr = filterableVendorIdsStr
                      .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                      .ToList();
                    foreach (var item in filterableVendorIdsArr)
                    {
                        var token = item.Split(new[] { "___" }, StringSplitOptions.None);
                        if (token.Length == 2)
                        {
                            filterableVendorIds.Add(int.Parse(token[0]), int.Parse(token[1]));
                        }
                    }
                }

                string filterableRatingsStr = (pFilterableRatings.Value != DBNull.Value) ? (string)pFilterableRatings.Value : "";
                if (!string.IsNullOrWhiteSpace(filterableRatingsStr))
                {
                    var filterableRatingsArr = filterableRatingsStr
                      .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                      .ToList();
                    foreach (var item in filterableRatingsArr)
                    {
                        var token = item.Split(new[] { "___" }, StringSplitOptions.None);
                        if (token.Length == 2)
                        {
                            filterableRatings.Add(int.Parse(token[0]), int.Parse(token[1]));
                        }
                    }
                }

                filterableMaxPrice = (pFilterableMaxPrice.Value != DBNull.Value) ? (decimal)pFilterableMaxPrice.Value : 0;
                filterableMinPrice = (pFilterableMinPrice.Value != DBNull.Value) ? (decimal)pFilterableMinPrice.Value : 0;
                filterableEmi = (pFilterableEmi.Value != DBNull.Value) ? (bool)pFilterableEmi.Value : false;
            }

            //return products
            int totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            return new PagedList<Product>(products, pageIndex, pageSize, totalRecords);
        }

        public IList<Product> GetProductsByCategoryId(List<int> categoryIds, int itemsNumber)
        {
            var query = _productRepository.Table;
            query = (from p in query
                     from pc in p.ProductCategories.Where(pc => categoryIds.Contains(pc.CategoryId))
                     orderby pc.IsFeaturedProduct descending
                     select p).Take(itemsNumber);
            return query.ToList();//where (pc.IsFeaturedProduct.Equals(true))
        }

        public IList<Product> GetProductsOnlyOfParentCategory(int categoryId, int itemsNumber)
        {
            var query = _productRepository.Table;
            query = (from p in query
                     from pc in p.ProductCategories.Where(pc => pc.CategoryId == categoryId)
                     orderby pc.IsFeaturedProduct descending
                     select p).Take(itemsNumber);
            return query.ToList();
        }

        public IList<int> GetPreviousAndNextProducts(int categoryId, int productId)
        {
            var query = _productRepository.Table;
            query = from p in query
                    from pc in p.ProductCategories.Where(pc => pc.CategoryId==categoryId)
                    select p;
            query = query.OrderBy(p => p.ProductCategories.FirstOrDefault(pc => pc.CategoryId == categoryId).DisplayOrder);
            var queryIds = query.Select(p => p.Id);
            var nextProduct = queryIds.FirstOrDefault(p=> p> productId);
            var prevProduct = queryIds.FirstOrDefault(p => p < productId);

            return new List<int>()
            {
                nextProduct,
                prevProduct
            };
        }

        public IList<BS_FeaturedProducts> GetFeaturedProducts()
        {
            var query = from p in _featuredProdRepository.Table
                        select p;

            var products = query.ToList();
            return products;
        }

        /// <summary>
        /// Search products
        /// </summary>
        /// <param name="filterableSpecificationAttributeOptionIds">The specification attribute option identifiers applied to loaded products (all pages)</param>
        /// <param name="loadFilterableSpecificationAttributeOptionIds">A value indicating whether we should load the specification attribute option identifiers applied to loaded products (all pages)</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="manufacturerId">Manufacturer identifier; 0 to load all records</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="warehouseId">Warehouse identifier; 0 to load all records</param>
        /// <param name="productType">Product type; 0 to load all records</param>
        /// <param name="visibleIndividuallyOnly">A values indicating whether to load only products marked as "visible individually"; "false" to load all records; "true" to load "visible individually" only</param>
        /// <param name="markedAsNewOnly">A values indicating whether to load only products marked as "new"; "false" to load all records; "true" to load "marked as new" only</param>
        /// <param name="featuredProducts">A value indicating whether loaded products are marked as featured (relates only to categories and manufacturers). 0 to load featured products only, 1 to load not featured products only, null to load all products</param>
        /// <param name="priceMin">Minimum price; null to load all records</param>
        /// <param name="priceMax">Maximum price; null to load all records</param>
        /// <param name="productTagId">Product tag identifier; 0 to load all records</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search by a specified "keyword" in product descriptions</param>
        /// <param name="searchSku">A value indicating whether to search by a specified "keyword" in product SKU</param>
        /// <param name="searchProductTags">A value indicating whether to search by a specified "keyword" in product tags</param>
        /// <param name="languageId">Language identifier (search for text searching)</param>
        /// <param name="filteredSpecs">Filtered product specification identifiers</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="overridePublished">
        /// null - process "Published" property according to "showHidden" parameter
        /// true - load only "Published" products
        /// false - load only "Unpublished" products
        /// </param>
        /// <returns>Products</returns>
        public IPagedList<Product> SearchOnSaleProducts(
            int pageIndex = 0,
            int pageSize = 2147483647,  //Int32.MaxValue
            IList<int> categoryIds = null,
            int manufacturerId = 0,
            int storeId = 0,
            int vendorId = 0,
            int warehouseId = 0,
            ProductType? productType = null,
            bool visibleIndividuallyOnly = false,
            bool markedAsNewOnly = false,
            bool? featuredProducts = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            int productTagId = 0,
            string keywords = null,
            bool searchDescriptions = false,
            bool searchSku = true,
            bool searchProductTags = false,
            int languageId = 0,
            IList<int> filteredSpecs = null,
            ProductSortingEnum orderBy = ProductSortingEnum.Position,
            bool showHidden = false,
            bool? overridePublished = null,
            bool specialPrice = true,
            bool hasTierPrice = true,
            bool hasDiscountApplied = true)
        {
            //search by keyword
            bool searchLocalizedValue = false;
            if (languageId > 0)
            {
                if (showHidden)
                {
                    searchLocalizedValue = true;
                }
                else
                {
                    //ensure that we have at least two published languages
                    var totalPublishedLanguages = _languageService.GetAllLanguages().Count;
                    searchLocalizedValue = totalPublishedLanguages >= 2;
                }
            }

            //validate "categoryIds" parameter
            if (categoryIds != null && categoryIds.Contains(0))
                categoryIds.Remove(0);

            //Access control list. Allowed customer roles
            var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();

            #region Search products

            //products
            var query = _productRepository.Table;
            query = query.Where(p => !p.Deleted);
            if (!overridePublished.HasValue)
            {
                //process according to "showHidden"
                if (!showHidden)
                {
                    query = query.Where(p => p.Published);
                }
            }
            else if (overridePublished.Value)
            {
                //published only
                query = query.Where(p => p.Published);
            }
            else if (!overridePublished.Value)
            {
                //unpublished only
                query = query.Where(p => !p.Published);
            }
            if (visibleIndividuallyOnly)
            {
                query = query.Where(p => p.VisibleIndividually);
            }
            //The function 'CurrentUtcDateTime' is not supported by SQL Server Compact. 
            //That's why we pass the date value
            var nowUtc = DateTime.UtcNow;
            if (markedAsNewOnly)
            {
                query = query.Where(p => p.MarkAsNew);
                query = query.Where(p =>
                    (!p.MarkAsNewStartDateTimeUtc.HasValue || p.MarkAsNewStartDateTimeUtc.Value < nowUtc) &&
                    (!p.MarkAsNewEndDateTimeUtc.HasValue || p.MarkAsNewEndDateTimeUtc.Value > nowUtc));
            }
            if (productType.HasValue)
            {
                var productTypeId = (int)productType.Value;
                query = query.Where(p => p.ProductTypeId == productTypeId);
            }

            if (priceMin.HasValue)
            {
                //min price
                query = query.Where(p =>
                    //special price (specified price and valid date range)
                                    ((p.SpecialPrice.HasValue &&
                                      ((!p.SpecialPriceStartDateTimeUtc.HasValue ||
                                        p.SpecialPriceStartDateTimeUtc.Value < nowUtc) &&
                                       (!p.SpecialPriceEndDateTimeUtc.HasValue ||
                                        p.SpecialPriceEndDateTimeUtc.Value > nowUtc))) &&
                                     (p.SpecialPrice >= priceMin.Value))
                                    ||
                                        //regular price (price isn't specified or date range isn't valid)
                                    ((!p.SpecialPrice.HasValue ||
                                      ((p.SpecialPriceStartDateTimeUtc.HasValue &&
                                        p.SpecialPriceStartDateTimeUtc.Value > nowUtc) ||
                                       (p.SpecialPriceEndDateTimeUtc.HasValue &&
                                        p.SpecialPriceEndDateTimeUtc.Value < nowUtc))) &&
                                     (p.Price >= priceMin.Value)));
            }
            if (priceMax.HasValue)
            {
                //max price
                query = query.Where(p =>
                    //special price (specified price and valid date range)
                                    ((p.SpecialPrice.HasValue &&
                                      ((!p.SpecialPriceStartDateTimeUtc.HasValue ||
                                        p.SpecialPriceStartDateTimeUtc.Value < nowUtc) &&
                                       (!p.SpecialPriceEndDateTimeUtc.HasValue ||
                                        p.SpecialPriceEndDateTimeUtc.Value > nowUtc))) &&
                                     (p.SpecialPrice <= priceMax.Value))
                                    ||
                                        //regular price (price isn't specified or date range isn't valid)
                                    ((!p.SpecialPrice.HasValue ||
                                      ((p.SpecialPriceStartDateTimeUtc.HasValue &&
                                        p.SpecialPriceStartDateTimeUtc.Value > nowUtc) ||
                                       (p.SpecialPriceEndDateTimeUtc.HasValue &&
                                        p.SpecialPriceEndDateTimeUtc.Value < nowUtc))) &&
                                     (p.Price <= priceMax.Value)));
            }
            if (!showHidden)
            {
                //available dates
                query = query.Where(p =>
                    (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc.Value < nowUtc) &&
                    (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc.Value > nowUtc));
            }

            //searching by keyword
            if (!String.IsNullOrWhiteSpace(keywords))
            {
                query = from p in query
                        join lp in _localizedPropertyRepository.Table on p.Id equals lp.EntityId into p_lp
                        from lp in p_lp.DefaultIfEmpty()
                        from pt in p.ProductTags.DefaultIfEmpty()
                        where (p.Name.Contains(keywords)) ||
                              (searchDescriptions && p.ShortDescription.Contains(keywords)) ||
                              (searchDescriptions && p.FullDescription.Contains(keywords)) ||
                              (searchProductTags && pt.Name.Contains(keywords)) ||
                            //localized values
                              (searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Product" && lp.LocaleKey == "Name" && lp.LocaleValue.Contains(keywords)) ||
                              (searchDescriptions && searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Product" && lp.LocaleKey == "ShortDescription" && lp.LocaleValue.Contains(keywords)) ||
                              (searchDescriptions && searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Product" && lp.LocaleKey == "FullDescription" && lp.LocaleValue.Contains(keywords))
                        select p;
            }

            if (!showHidden && !_catalogSettings.IgnoreAcl)
            {
                //ACL (access control list)
                query = from p in query
                        join acl in _aclRepository.Table
                        on new { c1 = p.Id, c2 = "Product" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into p_acl
                        from acl in p_acl.DefaultIfEmpty()
                        where !p.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                        select p;
            }

            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                //Store mapping
                query = from p in query
                        join sm in _storeMappingRepository.Table
                        on new { c1 = p.Id, c2 = "Product" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into p_sm
                        from sm in p_sm.DefaultIfEmpty()
                        where !p.LimitedToStores || storeId == sm.StoreId
                        select p;
            }

            //search by specs
            if (filteredSpecs != null && filteredSpecs.Count > 0)
            {
                query = from p in query
                        where !filteredSpecs
                                   .Except(
                                       p.ProductSpecificationAttributes.Where(psa => psa.AllowFiltering).Select(
                                           psa => psa.SpecificationAttributeOptionId))
                                   .Any()
                        select p;
            }

            //category filtering
            if (categoryIds != null && categoryIds.Count > 0)
            {
                query = from p in query
                        from pc in p.ProductCategories.Where(pc => categoryIds.Contains(pc.CategoryId))
                        where (!featuredProducts.HasValue || featuredProducts.Value == pc.IsFeaturedProduct)
                        select p;
            }

            //manufacturer filtering
            if (manufacturerId > 0)
            {
                query = from p in query
                        from pm in p.ProductManufacturers.Where(pm => pm.ManufacturerId == manufacturerId)
                        where (!featuredProducts.HasValue || featuredProducts.Value == pm.IsFeaturedProduct)
                        select p;
            }

            //vendor filtering
            if (vendorId > 0)
            {
                query = query.Where(p => p.VendorId == vendorId);
            }

            //warehouse filtering
            if (warehouseId > 0)
            {
                var manageStockInventoryMethodId = (int)ManageInventoryMethod.ManageStock;
                query = query.Where(p =>
                    //"Use multiple warehouses" enabled
                    //we search in each warehouse
                    (p.ManageInventoryMethodId == manageStockInventoryMethodId &&
                     p.UseMultipleWarehouses &&
                     p.ProductWarehouseInventory.Any(pwi => pwi.WarehouseId == warehouseId))
                    ||
                        //"Use multiple warehouses" disabled
                        //we use standard "warehouse" property
                    ((p.ManageInventoryMethodId != manageStockInventoryMethodId ||
                      !p.UseMultipleWarehouses) &&
                      p.WarehouseId == warehouseId));
            }

            //related products filtering
            //if (relatedToProductId > 0)
            //{
            //    query = from p in query
            //            join rp in _relatedProductRepository.Table on p.Id equals rp.ProductId2
            //            where (relatedToProductId == rp.ProductId1)
            //            select p;
            //}

            //tag filtering
            if (productTagId > 0)
            {
                query = from p in query
                        from pt in p.ProductTags.Where(pt => pt.Id == productTagId)
                        select p;
            }

            // On Sale Filtering

            if (hasTierPrice && hasDiscountApplied)
            {
                query = from p in query
                        where (p.SpecialPrice.HasValue && (!p.MarkAsNewStartDateTimeUtc.HasValue || p.MarkAsNewStartDateTimeUtc.Value < nowUtc) &&
                        (!p.MarkAsNewEndDateTimeUtc.HasValue || p.MarkAsNewEndDateTimeUtc.Value > nowUtc)) || p.HasTierPrices || p.HasDiscountsApplied
                        select p;
            }
            else
            {
                DateTime weekUtc = nowUtc.AddDays(7);
                query = from p in query
                        where (p.SpecialPrice.HasValue && (!p.MarkAsNewStartDateTimeUtc.HasValue || p.MarkAsNewStartDateTimeUtc.Value < weekUtc) &&
                        (!p.MarkAsNewEndDateTimeUtc.HasValue || p.MarkAsNewEndDateTimeUtc.Value > nowUtc))
                        select p;
            }

            //only distinct products (group by ID)
            //if we use standard Distinct() method, then all fields will be compared (low performance)
            //it'll not work in SQL Server Compact when searching products by a keyword)
            query = from p in query
                    group p by p.Id
                        into pGroup
                        orderby pGroup.Key
                        select pGroup.FirstOrDefault();

            //sort products
            if (orderBy == ProductSortingEnum.Position && categoryIds != null && categoryIds.Count > 0)
            {
                //category position
                var firstCategoryId = categoryIds[0];
                query = query.OrderBy(p => p.ProductCategories.FirstOrDefault(pc => pc.CategoryId == firstCategoryId).DisplayOrder);
            }
            else if (orderBy == ProductSortingEnum.Position && manufacturerId > 0)
            {
                //manufacturer position
                query =
                    query.OrderBy(p => p.ProductManufacturers.FirstOrDefault(pm => pm.ManufacturerId == manufacturerId).DisplayOrder);
            }
            else if (orderBy == ProductSortingEnum.Position)
            {
                //otherwise sort by name
                query = query.OrderBy(p => p.Name);
            }
            else if (orderBy == ProductSortingEnum.NameAsc)
            {
                //Name: A to Z
                query = query.OrderBy(p => p.Name);
            }
            else if (orderBy == ProductSortingEnum.NameDesc)
            {
                //Name: Z to A
                query = query.OrderByDescending(p => p.Name);
            }
            else if (orderBy == ProductSortingEnum.PriceAsc)
            {
                //Price: Low to High
                query = query.OrderBy(p => p.Price);
            }
            else if (orderBy == ProductSortingEnum.PriceDesc)
            {
                //Price: High to Low
                query = query.OrderByDescending(p => p.Price);
            }
            else if (orderBy == ProductSortingEnum.CreatedOn)
            {
                //creation date
                query = query.OrderByDescending(p => p.CreatedOnUtc);
            }
            else
            {
                //actually this code is not reachable
                query = query.OrderBy(p => p.Name);
            }



            var products = new PagedList<Product>(query, pageIndex, pageSize);

            //return products
            return products;

            #endregion
        }

        public virtual IList<Product> GetTopDealsProductsByIds(int[] productIds)
        {
            if (productIds == null || productIds.Length == 0)
                return new List<Product>();

            var query = from p in _productRepository.Table
                        where productIds.Contains(p.Id)
                        select p;

            var nowUtc = DateTime.UtcNow;
            query = from p in query
                    where (p.SpecialPrice.HasValue && (!p.MarkAsNewStartDateTimeUtc.HasValue || p.MarkAsNewStartDateTimeUtc.Value < nowUtc) &&
                    (!p.MarkAsNewEndDateTimeUtc.HasValue || p.MarkAsNewEndDateTimeUtc.Value > nowUtc)) || p.HasTierPrices || p.HasDiscountsApplied
                    select p;

            var products = query.ToList();
            //sort by passed identifiers
            var sortedProducts = new List<Product>();
            foreach (int id in productIds)
            {
                var product = products.Find(x => x.Id == id);
                if (product != null)
                    sortedProducts.Add(product);
            }
            return sortedProducts;
        }
    }
}
