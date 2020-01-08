using Algolia.Search;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Data;
using Nop.Plugin.Widgets.AlgoliaSearch.Models;
using Nop.Plugin.Widgets.AlgoliaSearch.Models.AlgoliaProduct;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.AlgoliaSearch.Factories
{
    public class ProductModelFactory : IProductModelFactory
    {
        #region Properties

        private AlgoliaClient _algoliaClient;
        private AlgoliaClient AlgoliaClient
        {
            get

            {
                if (_algoliaClient == null)
                    _algoliaClient = new AlgoliaClient(_algoliaSettings.ApplicationId, _algoliaSettings.SeachOnlyKey);
                return _algoliaClient;
            }
        }

        #endregion

        #region Fields

        private readonly ILogger _logger;
        private readonly IRepository<Product> _productRepository;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService; 
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IMeasureService _measureService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IWebHelper _webHelper;
        private readonly ICacheManager _cacheManager;
        private readonly CatalogSettings _catalogSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
        private readonly AlgoliaSettings _algoliaSettings;
        private readonly IVendorService _vendorService;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;

        #endregion

        #region Ctor

        public ProductModelFactory(ILogger logger,
            IRepository<Product> productRepository,
            ISpecificationAttributeService specificationAttributeService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ITaxService taxService,
            ICurrencyService currencyService,
            IMeasureService measureService,
            IManufacturerService manufacturerService,
            IWebHelper webHelper,
            ICacheManager cacheManager,
            CatalogSettings _catalogSettings,
            MediaSettings mediaSettings,
            IWorkContext workContext,
            IStoreContext storeContext,
            ICategoryService categoryService,
            IProductService productService,
            IPictureService pictureService,
            AlgoliaSettings algoliaSettings,
            IVendorService vendorService,
            IDataProvider dataProvider,
            IDbContext dbContext)
        {
            this._logger = logger;
            this._productRepository = productRepository;
            this._specificationAttributeService = specificationAttributeService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._permissionService = permissionService;
            this._localizationService = localizationService;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._measureService = measureService;
            this._manufacturerService = manufacturerService;
            this._webHelper = webHelper;
            this._cacheManager = cacheManager;
            this._catalogSettings = _catalogSettings;
            this._mediaSettings = mediaSettings;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._categoryService = categoryService;
            this._productService = productService;
            this._pictureService = pictureService;
            this._algoliaSettings = algoliaSettings;
            this._vendorService = vendorService;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
        }

        #endregion

        #region Utilities

        protected JObject GetSearchObject(AlgoliaPagingFilteringModel command)
        {
            var filter = BuildFilterString(command);
            var index = GetIndex(command.OrderBy);
            
            if (string.IsNullOrWhiteSpace(filter))
                return index.Search(new Query(command.q)
                    .SetPage(command.PageIndex)
                    .SetNbHitsPerPage(command.PageSize));

            return index.Search(new Query(command.q)
                .SetPage(command.PageIndex)
                .SetFilters(filter)
                .SetNbHitsPerPage(command.PageSize));
        }

        protected Index GetIndex(int? orderBy = null)
        {
            if (orderBy == (int)ProductSortingEnum.NameAsc)
                return AlgoliaClient.InitIndex("name_asc");
            else if (orderBy == (int)ProductSortingEnum.NameDesc)
                return AlgoliaClient.InitIndex("name_desc");
            else if (orderBy == (int)ProductSortingEnum.PriceAsc)
                return AlgoliaClient.InitIndex("price_asc");
            else if (orderBy == (int)ProductSortingEnum.PriceDesc)
                return AlgoliaClient.InitIndex("price_desc");
            else
                return AlgoliaClient.InitIndex("Products");
        }

        protected string BuildFilterString(AlgoliaPagingFilteringModel filterModel)
        {
            var sb = new StringBuilder();
            if (_algoliaSettings.AllowManufacturerFilter && filterModel.SelectedManufacturerIds.Count() > 0)
            {
                sb.AppendFormat("(");
                var qr = "";
                foreach (var item in filterModel.SelectedManufacturerIds)
                {
                    qr += "Manufacturers.Id=" + item + " OR ";
                }
                qr = qr.Trim();

                if (qr.EndsWith("OR"))
                    qr = qr.Substring(0, qr.Length - 2).Trim();

                sb.AppendFormat(qr + ") AND ");
            }

            if (_algoliaSettings.AllowVendorFilter && filterModel.SelectedVendorIds.Count() > 0)
            {
                sb.AppendFormat("(");
                var qr = "";
                foreach (var item in filterModel.SelectedVendorIds)
                {
                    qr += "Vendor.Id=" + item + " OR ";
                }
                qr = qr.Trim();

                if (qr.EndsWith("OR"))
                    qr = qr.Substring(0, qr.Length - 2).Trim();

                sb.AppendFormat(qr + ") AND ");
            }

            if (_algoliaSettings.AllowRatingFilter && filterModel.SelectedRatings.Count() > 0)
            {
                sb.AppendFormat("(");
                var qr = "";
                foreach (var item in filterModel.SelectedRatings)
                {
                    qr += "Rating=" + item + " OR ";
                }
                qr = qr.Trim();

                if (qr.EndsWith("OR"))
                    qr = qr.Substring(0, qr.Length - 2).Trim();

                sb.AppendFormat(qr + ") AND ");
            }
            
            if (_algoliaSettings.AllowCategoryFilter && filterModel.SelectedCategoryIds.Count > 0)
            {
                sb.AppendFormat("(");
                var qr = "";
                foreach (var item in filterModel.SelectedCategoryIds)
                {
                    qr += "Categories.Id=" + item + " OR ";
                }
                qr = qr.Trim();

                if (qr.EndsWith("OR"))
                    qr = qr.Substring(0, qr.Length - 2).Trim();

                sb.AppendFormat(qr + ") AND ");
            }

            if (_algoliaSettings.AllowEmiFilter && filterModel.EmiProductsOnly)
            {
                sb.AppendFormat("EnableEmi=1 AND ");
            }

            if (_algoliaSettings.AllowPriceRangeFilter)
            {
                if (filterModel.MaxPrice > 0)
                    sb.AppendFormat("Price <= " + filterModel.MaxPrice + " AND ");
                if (filterModel.MinPrice > 0)
                    sb.AppendFormat("Price >= " + filterModel.MinPrice + " AND ");
            }

            if (_algoliaSettings.HideSoldOutProducts)
            {
                sb.AppendFormat("SoldOut=0 AND ");
            }

            if (!filterModel.IncludeEkshopProducts)
            {
                sb.AppendFormat("EkshopOnly=0 AND ");
            }

            sb = new StringBuilder(sb.ToString().Trim());

            if (sb.ToString().EndsWith("AND"))
                sb = new StringBuilder(sb.ToString().Substring(0, sb.Length - 3).Trim());

            return sb.ToString();
        }

        protected IPagedList<Product> GetPagedProducts(int pageIndex, int pageSize, bool deleteProduct)
        {
            var pDeleteProduct = _dataProvider.GetParameter();
            pDeleteProduct.ParameterName = "DeleteProduct";
            pDeleteProduct.Value = deleteProduct;
            pDeleteProduct.DbType = DbType.Boolean;

            var pPageIndex = _dataProvider.GetParameter();
            pPageIndex.ParameterName = "PageIndex";
            pPageIndex.Value = pageIndex;
            pPageIndex.DbType = DbType.Int32;

            var pPageSize = _dataProvider.GetParameter();
            pPageSize.ParameterName = "PageSize";
            pPageSize.Value = pageSize;
            pPageSize.DbType = DbType.Int32;

            var pTotalRecords = _dataProvider.GetParameter();
            pTotalRecords.ParameterName = "TotalRecords";
            pTotalRecords.Direction = ParameterDirection.Output;
            pTotalRecords.DbType = DbType.Int32;

            //invoke stored procedure
            var products = _dbContext.ExecuteStoredProcedureList<Product>(
                "ProductLoadAlgoliaUpdatePaged",
                pDeleteProduct,
                pPageIndex,
                pPageSize,
                pTotalRecords);

            //return products
            int totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            return new PagedList<Product>(products, pageIndex, pageSize, totalRecords);
        }

        protected ProductReviewOverviewModel PrepareProductReviewOverviewModel(Product product)
        {
            ProductReviewOverviewModel productReview;

            if (_catalogSettings.ShowProductReviewsPerStore)
            {
                string cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_REVIEWS_MODEL_KEY, product.Id, _storeContext.CurrentStore.Id);

                productReview = _cacheManager.Get(cacheKey, () =>
                {
                    return new ProductReviewOverviewModel
                    {
                        RatingSum = product.ProductReviews
                                .Where(pr => pr.IsApproved && pr.StoreId == _storeContext.CurrentStore.Id)
                                .Sum(pr => pr.Rating),
                        TotalReviews = product
                                .ProductReviews
                                .Count(pr => pr.IsApproved && pr.StoreId == _storeContext.CurrentStore.Id)
                    };
                });
            }
            else
            {
                productReview = new ProductReviewOverviewModel()
                {
                    RatingSum = product.ApprovedRatingSum,
                    TotalReviews = product.ApprovedTotalReviews
                };
            }
            if (productReview != null)
            {
                productReview.ProductId = product.Id;
                productReview.AllowCustomerReviews = product.AllowCustomerReviews;
            }
            return productReview;
        }

        protected IList<AlgoliaSpecificationModel> PrepareProductSpecificationModel(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            string cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_SPECS_MODEL_KEY, product.Id, _workContext.WorkingLanguage.Id);
            return _cacheManager.Get(cacheKey, () =>
                _specificationAttributeService.GetProductSpecificationAttributes(product.Id, 0, null, true)
                .Select(psa =>
                {
                    var m = new AlgoliaSpecificationModel
                    {
                        SpecificationAttributeId = psa.SpecificationAttributeOption.SpecificationAttributeId,
                        SpecificationAttributeName = psa.SpecificationAttributeOption.SpecificationAttribute.GetLocalized(x => x.Name),
                        ColorSquaresRgb = psa.SpecificationAttributeOption.ColorSquaresRgb
                    };

                    switch (psa.AttributeType)
                    {
                        case SpecificationAttributeType.Option:
                            m.ValueRaw = HttpUtility.HtmlEncode(psa.SpecificationAttributeOption.GetLocalized(x => x.Name));
                            break;
                        case SpecificationAttributeType.CustomText:
                            m.ValueRaw = HttpUtility.HtmlEncode(psa.CustomValue);
                            break;
                        case SpecificationAttributeType.CustomHtmlText:
                            m.ValueRaw = psa.CustomValue;
                            break;
                        case SpecificationAttributeType.Hyperlink:
                            m.ValueRaw = string.Format("<a href='{0}' target='_blank'>{0}</a>", psa.CustomValue);
                            break;
                        default:
                            break;
                    }
                    m.IdOption = m.SpecificationAttributeId + "___" + m.ValueRaw;
                    m.IdOptionGroup = m.SpecificationAttributeId + "___" + m.ValueRaw + "___" + m.SpecificationAttributeName;
                    return m;
                }).ToList()
            );
        }

        protected List<AlgoliaManufacturerModel> PrepareManufacturerModel(Product product)
        {
            int productId = product.Id;
            var manufacturers = new List<AlgoliaManufacturerModel>();
            var productManufacturers = _manufacturerService.GetProductManufacturersByProductId(productId);

            if (productManufacturers != null && productManufacturers.Count > 0)
            {
                foreach (var productManufacturer in productManufacturers)
                {
                    var manf = productManufacturer.Manufacturer;
                    if (manf == null)
                        manf = _manufacturerService.GetManufacturerById(productManufacturer.ManufacturerId);

                    manufacturers.Add(new AlgoliaManufacturerModel()
                    {
                        Name = manf.Name,
                        SeName = manf.GetSeName(),
                        Id = productManufacturer.ManufacturerId,
                        IdName = productManufacturer.ManufacturerId + "___" + manf.Name
                    });
                }
            }
            return manufacturers;
        }

        protected AlgoliaVendorModel PrepareVendorModel(Product product)
        {
            var vendorModel = new AlgoliaVendorModel();

            var vendor = _vendorService.GetVendorById(product.VendorId);
            if (vendor != null)
            {
                vendorModel.Id = product.VendorId;
                vendorModel.SeName = vendor.GetSeName();
                vendorModel.Name = vendor.Name;
                vendorModel.IdName = vendor.Id + "___" + vendor.Name;
            }

            return vendorModel;
        }

        protected IList<AlgoliaCategoryModel> GetCategoryModel(Product product)
        {
            var categoryModel = new List<AlgoliaCategoryModel>();
            var productCategories = _categoryService.GetProductCategoriesByProductId(product.Id);

            foreach (var productCategory in productCategories)
            {
                var cat = productCategory.Category;
                categoryModel.Add(new AlgoliaCategoryModel()
                {
                    Name = cat.Name,
                    SeName = cat.GetSeName(),
                    Id = cat.Id,
                    NameSeName = cat.Name + "___" + cat.GetSeName(),
                    IdName = cat.Id + "___" + cat.Name
                });
            }
            return categoryModel;
        }

        #endregion

        #region Methods

        public AlgoliaProductOverviewModel PrepareAlgoliaUploadModel(Product product)
        {
            var model = new AlgoliaProductOverviewModel
            {
                Id = product.Id,
                Name = product.GetLocalized(x => x.Name),
                ShortDescription = product.GetLocalized(x => x.ShortDescription),
                SeName = product.GetSeName(),
                ProductType = product.ProductType,
                MarkAsNew = product.MarkAsNew &&
                            (!product.MarkAsNewStartDateTimeUtc.HasValue || product.MarkAsNewStartDateTimeUtc.Value < DateTime.UtcNow) &&
                            (!product.MarkAsNewEndDateTimeUtc.HasValue || product.MarkAsNewEndDateTimeUtc.Value > DateTime.UtcNow),
                Price = product.Price,
                OldPrice = product.OldPrice,
                Sku = product.Sku,
                DisableBuyButton = product.DisableBuyButton,
                DisableWishlistButton = product.DisableWishlistButton,
                objectID = product.Id.ToString(),
                EnableEmi = product.EnableEmi,
                SoldOut = product.GetTotalStockQuantity() < 1,
                EkshopOnly = product.EkshopOnly
            };

            //picture
            #region Prepare product picture

            //If a size has been set in the view, we use it in priority
            int pictureSize = _mediaSettings.ProductThumbPictureSize;

            var picture = _pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();

            //prepare picture model
            var defaultProductPictureCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_DEFAULTPICTURE_MODEL_KEY, product.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
            model.DefaultPictureModel = _cacheManager.Get(defaultProductPictureCacheKey, () =>
            {
                var pictureModel = new PictureModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                    FullSizeImageUrl = _pictureService.GetPictureUrl(picture)
                };
                //"title" attribute
                pictureModel.Title = (picture != null && !string.IsNullOrEmpty(picture.TitleAttribute)) ?
                            picture.TitleAttribute :
                            string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), model.Name);
                //"alt" attribute
                pictureModel.AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute)) ?
                            picture.AltAttribute :
                            string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), model.Name);

                return pictureModel;
            });

            model.AutoCompleteImageUrl = _pictureService.GetPictureUrl(picture, _algoliaSettings.SearchBoxThumbnailSize);

            #endregion

            model.Categories = GetCategoryModel(product);
            model.Manufacturers = PrepareManufacturerModel(product);
            model.Specifications = PrepareProductSpecificationModel(product);
            model.Vendor = PrepareVendorModel(product);

            //reviews
            model.ReviewOverviewModel = PrepareProductReviewOverviewModel(product);

            if (model.ReviewOverviewModel.TotalReviews > 0)
                model.Rating = model.ReviewOverviewModel.RatingSum / model.ReviewOverviewModel.TotalReviews;

            return model;
        }

        public IList<AlgoliaProductOverviewModel> PrepareAlgoliaUploadModel(IPagedList<Product> products)
        {
            if (products == null)
                throw new ArgumentNullException("products");
            var models = new List<AlgoliaProductOverviewModel>();
            foreach (var product in products)
            {
                try
                {
                    var model = PrepareAlgoliaUploadModel(product);
                    models.Add(model);
                }
                catch (Exception ex)
                {
                    _logger.Information("Failed to prepare algolia model. Product id: " + product.Id);
                    _logger.Error(ex.Message, ex);

                    continue;
                }

            }
            return models;
        }

        public IPagedList<ProductOverviewModel> SearchProducts(AlgoliaPagingFilteringModel command, bool preparePriceModel = true)
        {
            var searchObject = GetSearchObject(command);

            var products = new List<ProductOverviewModel>();
            var ids = new List<int>();
            foreach (var item in searchObject["hits"])
            {
                if (item != null)
                {
                    var str = item.ToString();
                    var product = JsonConvert.DeserializeObject<ProductOverviewModel>(item.ToString());
                    products.Add(product);
                    ids.Add(product.Id);
                }
            }

            #region Price

            if (preparePriceModel && products.Any())
            {
                var prs = _productRepository.Table.Where(x => ids.Contains(x.Id)).ToList();
                foreach (var product in prs)
                {
                    var pModel = products.First(x => x.Id == product.Id);
                    var cacheKey = string.Format("AlgoliaSearchBox.PriceModel.{0}.{1}", product.Id, _workContext.CurrentCustomer.Id);

                    pModel.ProductPrice = _cacheManager.Get(cacheKey, () =>
                    {

                        #region Prepare product price

                        var priceModel = new ProductOverviewModel.ProductPriceModel
                        {
                            ForceRedirectionAfterAddingToCart = false
                        };

                        switch (product.ProductType)
                        {
                            case ProductType.GroupedProduct:
                                {
                                    #region Grouped product

                                    var associatedProducts = _productService.GetAssociatedProducts(product.Id, _storeContext.CurrentStore.Id);

                                    //add to cart button (ignore "DisableBuyButton" property for grouped products)
                                    priceModel.DisableBuyButton = !_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart) ||
                                        !_permissionService.Authorize(StandardPermissionProvider.DisplayPrices);

                                    //add to wishlist button (ignore "DisableWishlistButton" property for grouped products)
                                    priceModel.DisableWishlistButton = !_permissionService.Authorize(StandardPermissionProvider.EnableWishlist) ||
                                        !_permissionService.Authorize(StandardPermissionProvider.DisplayPrices);

                                    //compare products
                                    priceModel.DisableAddToCompareListButton = !_catalogSettings.CompareProductsEnabled;
                                    switch (associatedProducts.Count)
                                    {
                                        case 0:
                                            {
                                                //no associated products
                                            }
                                            break;
                                        default:
                                            {
                                                //we have at least one associated product
                                                //compare products
                                                priceModel.DisableAddToCompareListButton = !_catalogSettings.CompareProductsEnabled;
                                                //priceModel.AvailableForPreOrder = false;

                                                if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                                                {
                                                    //find a minimum possible price
                                                    decimal? minPossiblePrice = null;
                                                    Product minPriceProduct = null;
                                                    foreach (var associatedProduct in associatedProducts)
                                                    {
                                                        //calculate for the maximum quantity (in case if we have tier prices)
                                                        var tmpPrice = _priceCalculationService.GetFinalPrice(associatedProduct,
                                                            _workContext.CurrentCustomer, decimal.Zero, true, int.MaxValue);
                                                        if (!minPossiblePrice.HasValue || tmpPrice < minPossiblePrice.Value)
                                                        {
                                                            minPriceProduct = associatedProduct;
                                                            minPossiblePrice = tmpPrice;
                                                        }
                                                    }
                                                    if (minPriceProduct != null && !minPriceProduct.CustomerEntersPrice)
                                                    {
                                                        if (minPriceProduct.CallForPrice)
                                                        {
                                                            priceModel.OldPrice = null;
                                                            priceModel.Price = _localizationService.GetResource("Products.CallForPrice");
                                                        }
                                                        else if (minPossiblePrice.HasValue)
                                                        {
                                                            //calculate prices
                                                            decimal taxRate;
                                                            decimal finalPriceBase = _taxService.GetProductPrice(minPriceProduct, minPossiblePrice.Value, out taxRate);
                                                            decimal finalPrice = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceBase, _workContext.WorkingCurrency);

                                                            priceModel.OldPrice = null;
                                                            priceModel.Price = String.Format(_localizationService.GetResource("Products.PriceRangeFrom"), _priceFormatter.FormatPrice(finalPrice));
                                                            priceModel.PriceValue = finalPrice;

                                                            //PAngV baseprice (used in Germany)
                                                            priceModel.BasePricePAngV = product.FormatBasePrice(finalPrice,
                                                                _localizationService, _measureService, _currencyService, _workContext, _priceFormatter);
                                                        }
                                                        else
                                                        {
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //hide prices
                                                    priceModel.OldPrice = null;
                                                    priceModel.Price = null;
                                                }
                                            }
                                            break;
                                    }

                                    #endregion
                                }
                                break;
                            case ProductType.SimpleProduct:
                            default:
                                {
                                    #region Simple product

                                    //add to cart button
                                    priceModel.DisableBuyButton = product.DisableBuyButton ||
                                        !_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart) ||
                                        !_permissionService.Authorize(StandardPermissionProvider.DisplayPrices);

                                    //add to wishlist button
                                    priceModel.DisableWishlistButton = product.DisableWishlistButton ||
                                        !_permissionService.Authorize(StandardPermissionProvider.EnableWishlist) ||
                                        !_permissionService.Authorize(StandardPermissionProvider.DisplayPrices);
                                    //compare products
                                    priceModel.DisableAddToCompareListButton = !_catalogSettings.CompareProductsEnabled;

                                    //rental
                                    priceModel.IsRental = product.IsRental;

                                    //pre-order
                                    if (product.AvailableForPreOrder)
                                    {
                                        priceModel.AvailableForPreOrder = !product.PreOrderAvailabilityStartDateTimeUtc.HasValue ||
                                            product.PreOrderAvailabilityStartDateTimeUtc.Value >= DateTime.UtcNow;
                                        priceModel.PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc;
                                    }

                                    //prices
                                    if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                                    {
                                        if (!product.CustomerEntersPrice)
                                        {
                                            if (product.CallForPrice)
                                            {
                                                //call for price
                                                priceModel.OldPrice = null;
                                                priceModel.Price = _localizationService.GetResource("Products.CallForPrice");
                                            }
                                            else
                                            {
                                                //prices

                                                //calculate for the maximum quantity (in case if we have tier prices)
                                                decimal minPossiblePrice = _priceCalculationService.GetFinalPrice(product,
                                                    _workContext.CurrentCustomer, decimal.Zero, true, int.MaxValue);

                                                decimal taxRate;
                                                decimal oldPriceBase = _taxService.GetProductPrice(product, product.OldPrice, out taxRate);
                                                decimal finalPriceBase = _taxService.GetProductPrice(product, minPossiblePrice, out taxRate);

                                                decimal oldPrice = _currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, _workContext.WorkingCurrency);
                                                decimal finalPrice = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceBase, _workContext.WorkingCurrency);

                                                //do we have tier prices configured?
                                                var tierPrices = new List<TierPrice>();
                                                if (product.HasTierPrices)
                                                {
                                                    tierPrices.AddRange(product.TierPrices
                                                        .OrderBy(tp => tp.Quantity)
                                                        .ToList()
                                                        .FilterByStore(_storeContext.CurrentStore.Id)
                                                        .FilterForCustomer(_workContext.CurrentCustomer)
                                                        .RemoveDuplicatedQuantities());
                                                }
                                                //When there is just one tier (with  qty 1), 
                                                //there are no actual savings in the list.
                                                bool displayFromMessage = tierPrices.Any() &&
                                                    !(tierPrices.Count == 1 && tierPrices[0].Quantity <= 1);
                                                if (displayFromMessage)
                                                {
                                                    priceModel.OldPrice = null;
                                                    priceModel.Price = String.Format(_localizationService.GetResource("Products.PriceRangeFrom"), _priceFormatter.FormatPrice(finalPrice));
                                                    priceModel.PriceValue = finalPrice;
                                                }
                                                else
                                                {
                                                    if (finalPriceBase != oldPriceBase && oldPriceBase != decimal.Zero)
                                                    {
                                                        priceModel.OldPrice = _priceFormatter.FormatPrice(oldPrice);
                                                        priceModel.Price = _priceFormatter.FormatPrice(finalPrice);
                                                        priceModel.PriceValue = finalPrice;
                                                        priceModel.Discount = (int)(Math.Round((finalPrice - oldPrice) / oldPrice * 100)) + "%";
                                                    }
                                                    else
                                                    {
                                                        priceModel.OldPrice = null;
                                                        priceModel.Price = _priceFormatter.FormatPrice(finalPrice);
                                                        priceModel.PriceValue = finalPrice;
                                                    }
                                                }
                                                if (product.IsRental)
                                                {
                                                    //rental product
                                                    priceModel.OldPrice = _priceFormatter.FormatRentalProductPeriod(product, priceModel.OldPrice);
                                                    priceModel.Price = _priceFormatter.FormatRentalProductPeriod(product, priceModel.Price);
                                                }


                                                //property for German market
                                                //we display tax/shipping info only with "shipping enabled" for this product
                                                //we also ensure this it's not free shipping
                                                priceModel.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoProductBoxes
                                                    && product.IsShipEnabled &&
                                                    !product.IsFreeShipping;


                                                //PAngV baseprice (used in Germany)
                                                priceModel.BasePricePAngV = product.FormatBasePrice(finalPrice,
                                                    _localizationService, _measureService, _currencyService, _workContext, _priceFormatter);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //hide prices
                                        priceModel.OldPrice = null;
                                        priceModel.Price = null;
                                    }

                                    #endregion
                                }
                                break;
                        }

                        return priceModel;

                        #endregion
                    });
                }
            }

            #endregion

            int val;
            command.TotalItems = int.TryParse(searchObject["nbHits"].ToString(), out val) ? val : 0;
            command.TotalPages = int.TryParse(searchObject["nbPages"].ToString(), out val) ? val : 0;

            return new PagedList<ProductOverviewModel>(products, command.PageIndex, command.PageSize, command.TotalItems);
        }

        public CatalogFilterings GetAlgoliaFilterings(string q, bool includeEkshopProducts = false)
        {
            var model = new CatalogFilterings();
             
            if (string.IsNullOrWhiteSpace(q) || q.Length < _algoliaSettings.MinimumQueryLength)
                return model;

            var index = GetIndex();
            JObject res = null;

            if (includeEkshopProducts)
            {
                res = index.Search(new Query(q)
                     .SetFacets(new List<string> { "Price", "Vendor.IdName", "Manufacturers.IdName", "Categories.IdName", "Specifications.IdOptionGroup", "Rating" })
                     .EnableFacetingAfterDistinct(true)
                     .SetFacetFilters(new List<string> { "Price", "Vendor.IdName", "Manufacturers.IdName", "Categories.IdName", "Specifications.IdOptionGroup", "Rating" }));
            }
            else
            {
                res = index.Search(new Query(q)
                     .SetFilters("EkshopOnly=0")
                     .SetFacets(new List<string> { "Price", "Vendor.IdName", "Manufacturers.IdName", "Categories.IdName", "Specifications.IdOptionGroup", "Rating", "EnableEmi" })
                     .EnableFacetingAfterDistinct(true)
                     .SetFacetFilters(new List<string> { "Price", "Vendor.IdName", "Manufacturers.IdName", "Categories.IdName", "Specifications.IdOptionGroup", "Rating", "EnableEmi" }));
            }

            if (_algoliaSettings.AllowPriceRangeFilter && res["facets"]["Price"] != null)
            {
                var values = JsonConvert.DeserializeObject<Dictionary<decimal, int>>(res["facets"]["Price"].ToString()).ToList();
                if (values != null)
                {
                    model.MaxPrice = values.Select(x => x.Key).Max(); 
                    model.MinPrice = values.Select(x => x.Key).Min();
                }
            }

            if (_algoliaSettings.AllowVendorFilter && res["facets"]["Vendor.IdName"] != null)
            {
                var values = JsonConvert.DeserializeObject<Dictionary<string, int>>(res["facets"]["Vendor.IdName"].ToString()).ToList();
                if (values != null)
                {
                    if (_algoliaSettings.MaximumVendorsShowInFilter != 0)
                        values = values.Take(_algoliaSettings.MaximumVendorsShowInFilter).ToList();

                    foreach (var item in values)
                    {
                        var str = item.Key.Split(new string[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
                        if (str.Length >= 2)
                        {
                            model.AvailableVendors.Add(new CatalogFilterings.SelectListItemDetails
                            {
                                Text = str[1],
                                Value = str[0],
                                Count = item.Value
                            });
                        }
                    }
                }
            }

            if (_algoliaSettings.AllowManufacturerFilter && res["facets"]["Manufacturers.IdName"] != null)
            {
                var values = JsonConvert.DeserializeObject<Dictionary<string, int>>(res["facets"]["Manufacturers.IdName"].ToString()).ToList();
                if (values != null)
                {
                    if (_algoliaSettings.MaximumManufacturersShowInFilter != 0)
                        values = values.Take(_algoliaSettings.MaximumManufacturersShowInFilter).ToList();

                    foreach (var item in values)
                    {
                        var str = item.Key.Split(new string[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
                        if (str.Length >= 2)
                        {
                            model.AvailableManufacturers.Add(new CatalogFilterings.SelectListItemDetails
                            {
                                Text = str[1],
                                Value = str[0],
                                Count = item.Value
                            });
                        }
                    }
                }
            }

            if (_algoliaSettings.AllowCategoryFilter && res["facets"]["Categories.IdName"] != null)
            {
                var values = JsonConvert.DeserializeObject<Dictionary<string, int>>(res["facets"]["Categories.IdName"].ToString()).ToList();
                if (values != null)
                {
                    if (_algoliaSettings.MaximumCategoriesShowInFilter != 0)
                        values = values.Take(_algoliaSettings.MaximumCategoriesShowInFilter).ToList();

                    foreach (var item in values)
                    {
                        var str = item.Key.Split(new string[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
                        if (str.Length >= 2)
                        {
                            model.AvailableCategories.Add(new CatalogFilterings.SelectListItemDetails
                            {
                                Text = str[1],
                                Value = str[0],
                                Count = item.Value
                            });
                        }
                    }
                }
            }

            if (_algoliaSettings.AllowSpecificationFilter && res["facets"]["Specifications.IdOptionGroup"] != null)
            {
                var values = JsonConvert.DeserializeObject<Dictionary<string, int>>(res["facets"]["Specifications.IdOptionGroup"].ToString()).ToList();
                if (values != null)
                {
                    foreach (var item in values)
                    {
                        var str = item.Key.Split(new string[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
                        if (str.Length > 2)
                        {
                            model.AvailableSpecifications.Add(new CatalogFilterings.SelectListItemDetails
                            {
                                Text = str[1],
                                Value = string.Format("{0}___{1}", str[0].Trim(), str[1].Trim()),
                                Count = item.Value,
                                GroupName = str[2]
                            });
                        }
                    }

                    if (_algoliaSettings.MaximumSpecificationsShowInFilter != 0)
                        model.AvailableSpecifications = model.AvailableSpecifications
                            .GroupBy(x => x.GroupName)
                            .SelectMany(g => g.Take(_algoliaSettings.MaximumSpecificationsShowInFilter))
                            .ToList();
                }
            }
            
            if (_algoliaSettings.AllowRatingFilter && res["facets"]["Rating"] != null)
            {
                var values = JsonConvert.DeserializeObject<Dictionary<string, int>>(res["facets"]["Rating"].ToString());
                
                for (int i = 5; i > 0; i--)
                {
                    if (values.TryGetValue(i.ToString(), out int v1))
                    {
                        model.AvailableRatings.Add(new CatalogFilterings.SelectListItemDetails
                        {
                            Text = i + " star",
                            Value = i.ToString(),
                            Count = v1
                        });
                    }
                }
            }

            if (_algoliaSettings.AllowEmiFilter && res["facets"]["EnableEmi"] != null)
            {
                var values = JsonConvert.DeserializeObject<Dictionary<bool, int>>(res["facets"]["EnableEmi"].ToString()).ToList();
                if (values != null)
                {
                    model.EmiProductsAvailable = values.Any(x => x.Key);
                }
            }

            return model;
        }

        public void UpdateAlgoliaModel()
        {
            var pageIndex = 0;

            try
            {
                var client = new AlgoliaClient(_algoliaSettings.ApplicationId, _algoliaSettings.AdminKey);
                var index = client.InitIndex("Products");

                while (true)
                {
                    var products = GetPagedProducts(pageIndex, 100, false);
                    if (products == null || products.Count == 0)
                    {
                        _logger.Information("Algolia add or update done.");
                        break;
                    }

                    var objects = new List<JObject>();

                    var modelList = PrepareAlgoliaUploadModel(products);

                    foreach (var algoliaModel in modelList)
                    {
                        try
                        {
                            var obj = new
                            {
                                algoliaModel.AutoCompleteImageUrl,
                                algoliaModel.Categories,
                                algoliaModel.DefaultPictureModel,
                                algoliaModel.Manufacturers,
                                algoliaModel.Name,
                                algoliaModel.MarkAsNew,
                                algoliaModel.Id,
                                algoliaModel.objectID,
                                algoliaModel.ProductPrice,
                                algoliaModel.ReviewOverviewModel,
                                algoliaModel.SeName,
                                algoliaModel.ShortDescription,
                                algoliaModel.Specifications,
                                algoliaModel.Vendor,
                                algoliaModel.OldPrice,
                                algoliaModel.Price,
                                algoliaModel.ProductType,
                                algoliaModel.FullDescription,
                                algoliaModel.CustomProperties,
                                algoliaModel.Rating,
                                algoliaModel.EnableEmi,
                                algoliaModel.Sku,
                                algoliaModel.SoldOut,
                                algoliaModel.EkshopOnly
                            };

                            var jobject = JObject.FromObject(obj);
                            objects.Add(jobject);

                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex.Message + ", Product Id = " + algoliaModel.Id, ex);
                            continue;
                        }
                    }

                    var res = index.PartialUpdateObjects(objects, true);
                    _logger.Information(products.Count + " items added or updated in Algolia. (" + string.Join(", ", products.Select(x => x.Id)) + ")");

                    pageIndex++;
                }

                pageIndex = 0;
                while (true)
                {
                    var products = GetPagedProducts(pageIndex, 100, true);
                    if (products == null || products.Count == 0)
                    {
                        _logger.Information("Algolia delete done.");
                        break;
                    }

                    index.DeleteObjects(products.Select(x => x.Id.ToString()));
                    pageIndex++;

                    _logger.Information(products.Count + " items deleted from Algolia. (" + string.Join(", ", products.Select(x => x.Id)) + ")");
                }

                _dbContext.ExecuteSqlCommand("Update Category set RecentlyUpdated = 0; Update Product set RecentlyUpdated = 0; Update Manufacturer set RecentlyUpdated = 0; Update Vendor set RecentlyUpdated = 0;");
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
            }
        }
        
        #endregion
    }
}
