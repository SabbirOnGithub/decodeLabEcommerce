using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Infrastructure.Cache;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models.Product;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Product;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Product;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using Nop.Services.Topics;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class ProductController : WebApiController
    {
        #region Field

        private readonly IProductService _productService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICategoryService _categoryService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPictureService _pictureService;
        private readonly IWebHelper _webHelper;
        private readonly ICacheManager _cacheManager;
        private readonly CatalogSettings _catalogSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly IShippingService _shippingService;
        private readonly VendorSettings _vendorSettings;
        private readonly IVendorService _vendorService;
        private readonly IMeasureService _measureService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IProductTagService _productTagService;
        private readonly CustomerSettings _customerSettings;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly CaptchaSettings _captchaSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IEventPublisher _eventPublisher;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IProductServiceApi _productServiceApi;
        private readonly IBS_RecentlyViewedProductsApiService _bsRecentlyViewedProductsApi;
        private readonly ITopicService _topicService;

        #endregion

        #region Ctor
        public ProductController(IProductService productService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ICategoryService categoryService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ITaxService taxService,
            ICurrencyService currencyService,
            IPictureService pictureService,
            IWebHelper webHelper,
            ICacheManager cacheManager,
            CatalogSettings catalogSettings,
            MediaSettings mediaSettings,
            IShippingService shippingService,
            VendorSettings vendorSettings,
            IVendorService vendorService,
            IMeasureService measureService,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IManufacturerService manufacturerService,
            ISpecificationAttributeService specificationAttributeService,
            ShoppingCartSettings shoppingCartSettings,
            IProductTagService productTagService,
            CustomerSettings customerSettings,
            IDateTimeHelper dateTimeHelper,
            CaptchaSettings captchaSettings,
            ICustomerActivityService customerActivityService,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            IEventPublisher eventPublisher,
            IProductServiceApi productServiceApi,
            IBS_RecentlyViewedProductsApiService bsRecentlyViewedProductsApi,
             ITopicService topicService)
        {
            this._productService = productService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._categoryService = categoryService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._permissionService = permissionService;
            this._localizationService = localizationService;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._pictureService = pictureService;
            this._webHelper = webHelper;
            this._cacheManager = cacheManager;
            this._catalogSettings = catalogSettings;
            this._mediaSettings = mediaSettings;
            this._shippingService = shippingService;
            this._vendorService = vendorService;
            this._vendorSettings = vendorSettings;
            this._measureService = measureService;
            this._productAttributeParser = productAttributeParser;
            this._productAttributeService = productAttributeService;
            this._manufacturerService = manufacturerService;
            this._specificationAttributeService = specificationAttributeService;
            this._shoppingCartSettings = shoppingCartSettings;
            this._productTagService = productTagService;
            this._customerSettings = customerSettings;
            this._captchaSettings = captchaSettings;
            this._dateTimeHelper = dateTimeHelper;
            this._customerActivityService = customerActivityService;
            this._workflowMessageService = workflowMessageService;
            this._localizationSettings = localizationSettings;
            this._eventPublisher = eventPublisher;
            this._productServiceApi = productServiceApi;
            this._bsRecentlyViewedProductsApi = bsRecentlyViewedProductsApi;
            this._topicService = topicService;
        }
        #endregion     

        #region Utility

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
                productThumbPictureSize, prepareSpecificationAttributes,
                forceRedirectionAfterAddingToCart);
        }


        [System.Web.Http.NonAction]
        protected virtual ProductDetailsModelApi PrepareProductDetailsPageModel(Product product,
            ShoppingCartItem updatecartitem = null, bool isAssociatedProduct = false)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            #region Standard properties

            var model = new ProductDetailsModelApi
            {
                Id = product.Id,
                Name = product.GetLocalized(x => x.Name),
                Sku = product.Sku,
                Url = _webHelper.GetStoreLocation() + product.GetSeName(),
                ShortDescription = product.GetLocalized(x => x.ShortDescription),
                FullDescription = product.GetLocalized(x => x.FullDescription),
                ShowManufacturerPartNumber = _catalogSettings.ShowManufacturerPartNumber,
                FreeShippingNotificationEnabled = _catalogSettings.ShowFreeShippingNotification,
                ManufacturerPartNumber = product.ManufacturerPartNumber,
                StockAvailability = product.FormatStockMessage("", _localizationService, _productAttributeParser),
                HasSampleDownload = product.IsDownload && product.HasSampleDownload,
                Confident = product.Confident,
                VerifiedByShoppers = product.VerifiedByShoppers,
                FasterShipping = product.FasterShipping,
                WarrantyDuration = product.WarrantyDuration,
                IsFreeShipping = product.IsFreeShipping
            };

            if (product.TopicId > 0)
            {
                var topic = _topicService.GetTopicById(product.TopicId);
                if (topic != null)
                {
                    model.TopicModel.Id = topic.Id;
                    model.TopicModel.Body = topic.Body;
                    model.TopicModel.IsPasswordProtected = topic.IsPasswordProtected;
                    model.TopicModel.MetaDescription = topic.MetaDescription;
                    model.TopicModel.MetaKeywords = topic.MetaKeywords;
                    model.TopicModel.MetaTitle = topic.MetaTitle;
                    model.TopicModel.SystemName = topic.SystemName;
                    model.TopicModel.Title = topic.Title;
                    model.TopicModel.TopicTemplateId = topic.TopicTemplateId;
                }
            }

            //shipping info
            model.IsShipEnabled = product.IsShipEnabled;
            if (product.IsShipEnabled)
            {
                model.IsFreeShipping = product.IsFreeShipping;
                //delivery date
                var deliveryDate = _shippingService.GetDeliveryDateById(product.DeliveryDateId);
                if (deliveryDate != null)
                {
                    model.DeliveryDate = deliveryDate.GetLocalized(dd => dd.Name);
                }
            }

            //email a friend
            model.EmailAFriendEnabled = _catalogSettings.EmailAFriendEnabled;
            //compare products
            model.CompareProductsEnabled = _catalogSettings.CompareProductsEnabled;

            #endregion

            #region Vendor details

            //vendor
            if (_vendorSettings.ShowVendorOnProductDetailsPage)
            {
                var vendor = _vendorService.GetVendorById(product.VendorId);
                if (vendor != null && !vendor.Deleted && vendor.Active)
                {
                    model.ShowVendor = true;

                    model.VendorModel = new VendorBriefInfoModel
                    {
                        Id = vendor.Id,
                        Name = vendor.GetLocalized(x => x.Name),
                        SeName = vendor.GetSeName(),
                    };
                }
            }

            #endregion


            #region Back in stock subscriptions

            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.BackorderMode == BackorderMode.NoBackorders &&
                product.AllowBackInStockSubscriptions &&
                product.GetTotalStockQuantity() <= 0)
            {
                //out of stock
                model.DisplayBackInStockSubscription = true;
            }

            #endregion

            #region Breadcrumb

            //do not prepare this model for the associated products. anyway it's not used
            if (_catalogSettings.CategoryBreadcrumbEnabled && !isAssociatedProduct)
            {
                var breadcrumbCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_BREADCRUMB_MODEL_KEY,
                    product.Id,
                    _workContext.WorkingLanguage.Id,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                    _storeContext.CurrentStore.Id);
                model.Breadcrumb = _cacheManager.Get(breadcrumbCacheKey, () =>
                {
                    var breadcrumbModel = new ProductDetailsModelApi.ProductBreadcrumbModel
                    {
                        Enabled = _catalogSettings.CategoryBreadcrumbEnabled,
                        ProductId = product.Id,
                        ProductName = product.GetLocalized(x => x.Name)
                    };
                    var productCategories = _categoryService.GetProductCategoriesByProductId(product.Id);
                    if (productCategories.Count > 0)
                    {
                        var category = productCategories[0].Category;
                        if (category != null)
                        {
                            foreach (var catBr in category.GetCategoryBreadCrumb(_categoryService, _aclService, _storeMappingService))
                            {
                                breadcrumbModel.CategoryBreadcrumb.Add(new CategorySimpleModel
                                {
                                    Id = catBr.Id,
                                    Name = catBr.GetLocalized(x => x.Name),
                                    IncludeInTopMenu = catBr.IncludeInTopMenu
                                });
                            }
                        }
                    }
                    return breadcrumbModel;
                });
            }

            #endregion

            #region Product tags

            //do not prepare this model for the associated products. anyway it's not used
            if (!isAssociatedProduct)
            {
                var productTagsCacheKey = string.Format(ModelCacheEventConsumer.PRODUCTTAG_BY_PRODUCT_MODEL_KEY, product.Id, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id);
                model.ProductTags = _cacheManager.Get(productTagsCacheKey, () =>
                    product.ProductTags
                    //filter by store
                    .Where(x => _productTagService.GetProductCount(x.Id, _storeContext.CurrentStore.Id) > 0)
                    .Select(x => new ProductTagModel
                    {
                        Id = x.Id,
                        Name = x.GetLocalized(y => y.Name),
                        SeName = x.GetSeName(),
                        ProductCount = _productTagService.GetProductCount(x.Id, _storeContext.CurrentStore.Id)
                    })
                    .ToList());
            }

            #endregion

            #region Pictures

            model.DefaultPictureZoomEnabled = _mediaSettings.DefaultPictureZoomEnabled;
            //default picture
            var defaultPictureSize = isAssociatedProduct ?
                _mediaSettings.AssociatedProductPictureSize :
                _mediaSettings.ProductDetailsPictureSize;
            //prepare picture models
            var productPicturesCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_DETAILS_PICTURES_MODEL_KEY, product.Id, defaultPictureSize, isAssociatedProduct, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
            var cachedPictures = _cacheManager.Get(productPicturesCacheKey, () =>
            {
                var pictures = _pictureService.GetPicturesByProductId(product.Id);
                var defaultPicture = pictures.FirstOrDefault();
                var defaultPictureModel = new PictureModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(defaultPicture, defaultPictureSize, !isAssociatedProduct),
                    FullSizeImageUrl = _pictureService.GetPictureUrl(defaultPicture, 800)
                };


                //all pictures
                var pictureModels = new List<PictureModel>();
                foreach (var picture in pictures)
                {
                    var pictureModel = new PictureModel
                    {
                        //ImageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ProductThumbPictureSizeOnProductDetailsPage),
                        ImageUrl = _pictureService.GetPictureUrl(picture, 400),
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture, 800)
                    };


                    pictureModels.Add(pictureModel);
                }

                return new { DefaultPictureModel = defaultPictureModel, PictureModels = pictureModels };
            });
            model.DefaultPictureModel = cachedPictures.DefaultPictureModel;
            model.PictureModels = cachedPictures.PictureModels;

            #endregion

            #region Product price

            model.ProductPrice.ProductId = product.Id;
            if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
            {
                model.ProductPrice.HidePrices = false;
                if (product.CustomerEntersPrice)
                {
                    model.ProductPrice.CustomerEntersPrice = true;
                }
                else
                {
                    if (product.CallForPrice)
                    {
                        model.ProductPrice.CallForPrice = true;
                    }
                    else
                    {
                        decimal taxRate;
                        decimal oldPriceBase = _taxService.GetProductPrice(product, product.OldPrice, out taxRate);
                        decimal finalPriceWithoutDiscountBase = _taxService.GetProductPrice(product, _priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer, includeDiscounts: false), out taxRate);
                        decimal finalPriceWithDiscountBase = _taxService.GetProductPrice(product, _priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer, includeDiscounts: true), out taxRate);

                        decimal oldPrice = _currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, _workContext.WorkingCurrency);
                        decimal finalPriceWithoutDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithoutDiscountBase, _workContext.WorkingCurrency);
                        decimal finalPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithDiscountBase, _workContext.WorkingCurrency);

                        if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
                            model.ProductPrice.OldPrice = _priceFormatter.FormatPrice(oldPrice);

                        model.ProductPrice.Price = _priceFormatter.FormatPrice(finalPriceWithoutDiscount);

                        if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                            model.ProductPrice.PriceWithDiscount = _priceFormatter.FormatPrice(finalPriceWithDiscount);

                        model.ProductPrice.PriceValue = finalPriceWithoutDiscount;
                        model.ProductPrice.PriceWithDiscountValue = finalPriceWithDiscount;

                        if (model.ProductPrice.PriceValue > model.ProductPrice.PriceWithDiscountValue)
                        {
                            var disc = model.ProductPrice.PriceValue - model.ProductPrice.PriceWithDiscountValue;
                            model.ProductPrice.PriceDiscountPercent = Math.Round(disc / model.ProductPrice.PriceValue * 100);
                        }

                        //property for German market
                        //we display tax/shipping info only with "shipping enabled" for this product
                        //we also ensure this it's not free shipping
                        model.ProductPrice.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoProductDetailsPage
                            && product.IsShipEnabled &&
                            !product.IsFreeShipping;

                        //PAngV baseprice (used in Germany)
                        model.ProductPrice.BasePricePAngV = product.FormatBasePrice(finalPriceWithDiscountBase,
                            _localizationService, _measureService, _currencyService, _workContext, _priceFormatter);

                        //currency code
                        model.ProductPrice.CurrencyCode = _workContext.WorkingCurrency.CurrencyCode;

                        //rental
                        if (product.IsRental)
                        {
                            model.ProductPrice.IsRental = true;
                            var priceStr = _priceFormatter.FormatPrice(finalPriceWithDiscount);
                            model.ProductPrice.RentalPrice = _priceFormatter.FormatRentalProductPeriod(product, priceStr);
                        }
                    }
                }
            }
            else
            {
                model.ProductPrice.HidePrices = true;
                model.ProductPrice.OldPrice = null;
                model.ProductPrice.Price = null;
            }
            #endregion

            #region Quantity
            model.Quantity.OrderMinimumQuantity = product.OrderMinimumQuantity;
            model.Quantity.OrderMaximumQuantity = product.OrderMaximumQuantity;
            model.Quantity.StockQuantity = product.StockQuantity;
            #endregion

            #region 'Add to cart' model

            model.AddToCart.ProductId = product.Id;
            model.AddToCart.UpdatedShoppingCartItemId = updatecartitem != null ? updatecartitem.Id : 0;

            //quantity
            model.AddToCart.EnteredQuantity = updatecartitem != null ? updatecartitem.Quantity : product.OrderMinimumQuantity;

            //'add to cart', 'add to wishlist' buttons
            model.AddToCart.DisableBuyButton = product.DisableBuyButton || !_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart);
            model.AddToCart.DisableWishlistButton = product.DisableWishlistButton || !_permissionService.Authorize(StandardPermissionProvider.EnableWishlist);
            if (!_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
            {
                model.AddToCart.DisableBuyButton = true;
                model.AddToCart.DisableWishlistButton = true;
            }
            //pre-order
            if (product.AvailableForPreOrder)
            {
                model.AddToCart.AvailableForPreOrder = !product.PreOrderAvailabilityStartDateTimeUtc.HasValue ||
                    product.PreOrderAvailabilityStartDateTimeUtc.Value >= DateTime.UtcNow;
                model.AddToCart.PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc;
            }
            //rental
            model.AddToCart.IsRental = product.IsRental;

            //customer entered price
            model.AddToCart.CustomerEntersPrice = product.CustomerEntersPrice;
            if (model.AddToCart.CustomerEntersPrice)
            {
                decimal minimumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(product.MinimumCustomerEnteredPrice, _workContext.WorkingCurrency);
                decimal maximumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(product.MaximumCustomerEnteredPrice, _workContext.WorkingCurrency);

                model.AddToCart.CustomerEnteredPrice = updatecartitem != null ? updatecartitem.CustomerEnteredPrice : minimumCustomerEnteredPrice;
                model.AddToCart.CustomerEnteredPriceRange = string.Format(_localizationService.GetResource("Products.EnterProductPrice.Range"),
                    _priceFormatter.FormatPrice(minimumCustomerEnteredPrice, false, false),
                    _priceFormatter.FormatPrice(maximumCustomerEnteredPrice, false, false));
            }
            //allowed quantities
            var allowedQuantities = product.ParseAllowedQuantities();
            foreach (var qty in allowedQuantities)
            {
                model.AddToCart.AllowedQuantities.Add(new SelectListItem
                {
                    Text = qty.ToString(),
                    Value = qty.ToString(),
                    Selected = updatecartitem != null && updatecartitem.Quantity == qty
                });
            }

            #endregion

            #region Gift card

            model.GiftCard.IsGiftCard = product.IsGiftCard;
            if (model.GiftCard.IsGiftCard)
            {
                model.GiftCard.GiftCardType = product.GiftCardType;

                if (updatecartitem == null)
                {
                    model.GiftCard.SenderName = _workContext.CurrentCustomer.GetFullName();
                    model.GiftCard.SenderEmail = _workContext.CurrentCustomer.Email;
                }
                else
                {
                    string giftCardRecipientName, giftCardRecipientEmail, giftCardSenderName, giftCardSenderEmail, giftCardMessage;
                    _productAttributeParser.GetGiftCardAttribute(updatecartitem.AttributesXml,
                        out giftCardRecipientName, out giftCardRecipientEmail,
                        out giftCardSenderName, out giftCardSenderEmail, out giftCardMessage);

                    model.GiftCard.RecipientName = giftCardRecipientName;
                    model.GiftCard.RecipientEmail = giftCardRecipientEmail;
                    model.GiftCard.SenderName = giftCardSenderName;
                    model.GiftCard.SenderEmail = giftCardSenderEmail;
                    model.GiftCard.Message = giftCardMessage;
                }
            }

            #endregion

            #region Product attributes

            //performance optimization
            //We cache a value indicating whether a product has attributes
            IList<ProductAttributeMapping> productAttributeMapping = null;
            string cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_HAS_PRODUCT_ATTRIBUTES_KEY, product.Id);
            var hasProductAttributesCache = _cacheManager.Get<bool?>(cacheKey);
            if (!hasProductAttributesCache.HasValue)
            {
                //no value in the cache yet
                //let's load attributes and cache the result (true/false)
                productAttributeMapping = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
                hasProductAttributesCache = productAttributeMapping.Count > 0;
                _cacheManager.Set(cacheKey, hasProductAttributesCache, 60);
            }
            if (hasProductAttributesCache.Value && productAttributeMapping == null)
            {
                //cache indicates that the product has attributes
                //let's load them
                productAttributeMapping = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            }
            if (productAttributeMapping == null)
            {
                productAttributeMapping = new List<ProductAttributeMapping>();
            }
            foreach (var attribute in productAttributeMapping)
            {
                var attributeModel = new ProductDetailsModelApi.ProductAttributeModel
                {
                    Id = attribute.Id,
                    ProductId = product.Id,
                    ProductAttributeId = attribute.ProductAttributeId,
                    Name = attribute.ProductAttribute.GetLocalized(x => x.Name),
                    Description = attribute.ProductAttribute.GetLocalized(x => x.Description),
                    TextPrompt = attribute.TextPrompt,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    DefaultValue = updatecartitem != null ? null : attribute.DefaultValue,
                };
                if (!String.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
                {
                    attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var valueModel = new ProductDetailsModelApi.ProductAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.GetLocalized(x => x.Name),
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb, //used with "Color squares" attribute type
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(valueModel);

                        //display price if allowed
                        if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                        {
                            decimal taxRate;
                            decimal attributeValuePriceAdjustment = _priceCalculationService.GetProductAttributeValuePriceAdjustment(attributeValue);
                            decimal priceAdjustmentBase = _taxService.GetProductPrice(product, attributeValuePriceAdjustment, out taxRate);
                            decimal priceAdjustment = _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, _workContext.WorkingCurrency);
                            if (priceAdjustmentBase > decimal.Zero)
                                valueModel.PriceAdjustment = "+" + _priceFormatter.FormatPrice(priceAdjustment, false, false);
                            else if (priceAdjustmentBase < decimal.Zero)
                                valueModel.PriceAdjustment = "-" + _priceFormatter.FormatPrice(-priceAdjustment, false, false);

                            valueModel.PriceAdjustmentValue = priceAdjustment;
                        }

                        //picture
                        if (attributeValue.PictureId > 0)
                        {
                            var productAttributePictureCacheKey = string.Format(ModelCacheEventConsumer.PRODUCTATTRIBUTE_PICTURE_MODEL_KEY,
                                attributeValue.PictureId,
                                _webHelper.IsCurrentConnectionSecured(),
                                _storeContext.CurrentStore.Id);

                            valueModel.PictureModel = _cacheManager.Get(productAttributePictureCacheKey, () =>
                            {
                                var valuePicture = _pictureService.GetPictureById(attributeValue.PictureId);
                                if (valuePicture != null)
                                {
                                    return new PictureModel
                                    {
                                        ImageUrl = _pictureService.GetPictureUrl(valuePicture, defaultPictureSize)
                                    };
                                }
                                return new PictureModel();
                            });
                        }
                    }
                }

                //set already selected attributes (if we're going to update the existing shopping cart item)
                if (updatecartitem != null)
                {
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.Checkboxes:
                        case AttributeControlType.ColorSquares:
                            {
                                if (!String.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    //clear default selection
                                    foreach (var item in attributeModel.Values)
                                        item.IsPreSelected = false;

                                    //select new values
                                    var selectedValues = _productAttributeParser.ParseProductAttributeValues(updatecartitem.AttributesXml);
                                    foreach (var attributeValue in selectedValues)
                                        foreach (var item in attributeModel.Values)
                                            if (attributeValue.Id == item.Id)
                                                item.IsPreSelected = true;
                                }
                            }
                            break;
                        case AttributeControlType.ReadonlyCheckboxes:
                            {
                                //do nothing
                                //values are already pre-set
                            }
                            break;
                        case AttributeControlType.TextBox:
                        case AttributeControlType.MultilineTextbox:
                            {
                                if (!String.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    var enteredText = _productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
                                    if (enteredText.Count > 0)
                                        attributeModel.DefaultValue = enteredText[0];
                                }
                            }
                            break;
                        case AttributeControlType.Datepicker:
                            {
                                //keep in mind my that the code below works only in the current culture
                                var selectedDateStr = _productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
                                if (selectedDateStr.Count > 0)
                                {
                                    DateTime selectedDate;
                                    if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture,
                                                           DateTimeStyles.None, out selectedDate))
                                    {
                                        //successfully parsed
                                        attributeModel.SelectedDay = selectedDate.Day;
                                        attributeModel.SelectedMonth = selectedDate.Month;
                                        attributeModel.SelectedYear = selectedDate.Year;
                                    }
                                }

                            }
                            break;
                        default:
                            break;
                    }
                }

                model.ProductAttributes.Add(attributeModel);
            }

            #endregion

            #region Product specifications

            //do not prepare this model for the associated products. any it's not used
            if (!isAssociatedProduct)
            {
                model.ProductSpecifications = product.PrepareProductSpecificationModel(_workContext,
                    _specificationAttributeService,
                    _cacheManager);
            }

            #endregion

            #region Product review overview

            model.ProductReviewOverview = new ProductOverViewModelApi.ProductReviewOverviewModel
            {
                ProductId = product.Id,
                RatingSum = product.ApprovedRatingSum,
                TotalReviews = product.ApprovedTotalReviews,
                AllowCustomerReviews = product.AllowCustomerReviews
            };

            #endregion

            #region Tier prices

            if (product.HasTierPrices && _permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
            {
                model.TierPrices = product.TierPrices
                    .OrderBy(x => x.Quantity)
                    .ToList()
                    .FilterByStore(_storeContext.CurrentStore.Id)
                    .FilterForCustomer(_workContext.CurrentCustomer)
                    .RemoveDuplicatedQuantities()
                    .Select(tierPrice =>
                    {
                        var m = new ProductDetailsModelApi.TierPriceModel
                        {
                            Quantity = tierPrice.Quantity,
                        };
                        decimal taxRate;
                        decimal priceBase = _taxService.GetProductPrice(product, _priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer, decimal.Zero, _catalogSettings.DisplayTierPricesWithDiscounts, tierPrice.Quantity), out taxRate);
                        decimal price = _currencyService.ConvertFromPrimaryStoreCurrency(priceBase, _workContext.WorkingCurrency);
                        m.Price = _priceFormatter.FormatPrice(price, false, false);
                        return m;
                    })
                    .ToList();
            }

            #endregion

            #region Manufacturers

            //do not prepare this model for the associated products. any it's not used
            if (!isAssociatedProduct)
            {
                string manufacturersCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_MANUFACTURERS_MODEL_KEY,
                    product.Id,
                    _workContext.WorkingLanguage.Id,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                    _storeContext.CurrentStore.Id);
                model.ProductManufacturers = _cacheManager.Get(manufacturersCacheKey, () =>
                    _manufacturerService.GetProductManufacturersByProductId(product.Id)
                    .Select(x => x.Manufacturer.MapTo<Manufacturer, MenuFacturerModelShortDetailApi>())
                    .ToList()
                    );
            }
            #endregion

            #region Rental products

            if (product.IsRental)
            {
                model.IsRental = true;
                //set already entered dates attributes (if we're going to update the existing shopping cart item)
                if (updatecartitem != null)
                {
                    model.RentalStartDate = updatecartitem.RentalStartDateUtc;
                    model.RentalEndDate = updatecartitem.RentalEndDateUtc;
                }
            }

            #endregion

            #region Associated products

            if (product.ProductType == ProductType.GroupedProduct)
            {
                //ensure no circular references
                if (!isAssociatedProduct)
                {
                    var associatedProducts = _productService.GetAssociatedProducts(product.Id, _storeContext.CurrentStore.Id);
                    foreach (var associatedProduct in associatedProducts)
                        model.AssociatedProducts.Add(PrepareProductDetailsPageModel(associatedProduct, null, true));
                }
            }

            #endregion

            return model;
        }



        [System.Web.Http.NonAction]
        protected virtual void PrepareProductReviewsModel(ProductReviewsResponseModel model, Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            if (model == null)
                throw new ArgumentNullException("model");

            model.ProductId = product.Id;
            model.ProductName = product.GetLocalized(x => x.Name);
            model.ProductSeName = product.GetSeName();
            model.IsFreeShipping = product.IsFreeShipping;

            var productReviews = product.ProductReviews.Where(pr => pr.IsApproved).OrderBy(pr => pr.CreatedOnUtc);
            foreach (var pr in productReviews)
            {
                var customer = pr.Customer;
                model.Items.Add(new ProductReviewModel
                {
                    Id = pr.Id,
                    CustomerId = pr.CustomerId,
                    CustomerName = customer.FormatUserName(),
                    AllowViewingProfiles = _customerSettings.AllowViewingProfiles && customer != null && !customer.IsGuest(),
                    Title = pr.Title,
                    ReviewText = pr.ReviewText,
                    Rating = pr.Rating,
                    Helpfulness = new ProductReviewHelpfulnessModel
                    {
                        ProductReviewId = pr.Id,
                        HelpfulYesTotal = pr.HelpfulYesTotal,
                        HelpfulNoTotal = pr.HelpfulNoTotal,
                    },
                    WrittenOnStr = _dateTimeHelper.ConvertToUserTime(pr.CreatedOnUtc, DateTimeKind.Utc).ToString("g"),
                });
            }

            model.AddProductReview.CanCurrentCustomerLeaveReview = _catalogSettings.AllowAnonymousUsersToReviewProduct || !_workContext.CurrentCustomer.IsGuest();
            model.AddProductReview.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnProductReviewPage;
        }

        #endregion

        #region Action Method
        [System.Web.Http.Route("api/homepageproducts")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult HomepageProducts(int? thumbPictureSize = null)
        {
            if (!thumbPictureSize.HasValue)
            {
                thumbPictureSize = _mediaSettings.ProductThumbPictureSize;
            }
            var products = _productService.GetAllProductsDisplayedOnHomePage();
            // Mobile Featured Products
            var featuredProducts = _productServiceApi.GetFeaturedProducts();

            foreach (var item in featuredProducts)
            {
                var prod = _productService.GetProductById(item.ProductId);
                if (!products.Contains(prod))
                {
                    products.Add(prod);
                }
            }
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            var model = PrepareProductOverviewModels(products, true, true, thumbPictureSize).ToList();
            var result = new GeneralResponseModel<IList<ProductOverViewModelApi>>()
            {
                Data = model
            };
            return Ok(result);
        }

        [System.Web.Http.Route("api/productdetails/{productId}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult ProductDetails(int productId, int updatecartitemid = 0)
        {
            ShoppingCartItem updatecartitem = null;

            var product = _productService.GetProductById(productId);

            if (product == null || product.Deleted)
                return Unauthorized();

            IList<int> productIds = new List<int>();
            var lastOrDefault = product.ProductCategories.LastOrDefault();
            if (lastOrDefault != null)
            {
                productIds =
                   _productServiceApi.GetPreviousAndNextProducts(lastOrDefault.CategoryId,
                       productId);
            }

            //Is published?
            //Check whether the current user has a "Manage catalog" permission
            //It allows him to preview a product before publishing
            if (!product.Published && !_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return Unauthorized();

            //ACL (access control list)
            if (!_aclService.Authorize(product))
                return Unauthorized();

            //Store mapping
            if (!_storeMappingService.Authorize(product))
                return Unauthorized();

            //availability dates
            if (!product.IsAvailable())
                return Unauthorized();

            //visible individually?
            if (!product.VisibleIndividually)
            {
                //is this one an associated products?
                var parentGroupedProduct = _productService.GetProductById(product.ParentGroupedProductId);
                if (parentGroupedProduct != null)
                    product = parentGroupedProduct;
                else
                {
                    return Unauthorized();
                }
                //return RedirectToRoute("Product", new { SeName = parentGroupedProduct.GetSeName() });
            }

            //update existing shopping cart item?
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found?
                if (updatecartitem == null)
                {
                    product = _productService.GetProductById(productId);
                }
                //is it this product?
                if (product.Id != updatecartitem.ProductId)
                {
                    product = _productService.GetProductById(productId);
                }
            }

            //prepare the model
            var model = PrepareProductDetailsPageModel(product, updatecartitem, false);
            if (productIds.Count >= 2)
            {
                model.NextProduct = productIds[0];
                model.PreviousProduct = productIds[1];
            }

            //save as recently viewed
            _bsRecentlyViewedProductsApi.AddProductToRecentlyViewedList(product.Id);

            var result = new GeneralResponseModel<ProductDetailsModelApi>()
            {
                Data = model
            };
            return Ok(result);
        }

        [System.Web.Http.Route("api/relatedproducts/{productId}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult RelatedProducts(int productId, int thumbPictureSize = 400)
        {
            //load and cache report
            var productIds = _cacheManager.Get(string.Format(ModelCacheEventConsumer.PRODUCTS_RELATED_IDS_KEY, productId, _storeContext.CurrentStore.Id),
                () =>
                    _productService.GetRelatedProductsByProductId1(productId).Select(x => x.ProductId2).ToArray()
                    );

            //load products
            var products = _productService.GetProductsByIds(productIds);
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            var model = new List<ProductOverViewModelApi>();

            if (products.Count > 0)
                model = PrepareProductOverviewModels(products, true, true, thumbPictureSize).ToList();

            var result = new GeneralResponseModel<IList<ProductOverViewModelApi>>()
            {
                Data = model
            };

            return Ok(result);
        }

        [System.Web.Http.Route("api/product/productreviews/{productId}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult ProductReviews(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            var model = new ProductReviewsResponseModel();
            PrepareProductReviewsModel(model, product);
            //only registered users can leave reviews
            if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
                //ModelState.AddModelError("", _localizationService.GetResource("Reviews.OnlyRegisteredUsersCanWriteReviews"));
                model.AddProductReview.CanCurrentCustomerLeaveReview = false;
            //default value
            model.AddProductReview.Rating = _catalogSettings.DefaultProductRatingValue;
            return Ok(model);
        }

        [System.Web.Http.Route("api/product/productreviewsadd/{productId}")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult ProductReviewsAdd(int productId, ProductReviewQueryModel model)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            var response = new GeneralResponseModel<bool>();
            ValidationExtension.WriteReviewValidator(ModelState, model, _localizationService);
            if (ModelState.IsValid)
            {
                //save review
                int rating = model.Rating;
                if (rating < 1 || rating > 5)
                    rating = _catalogSettings.DefaultProductRatingValue;
                bool isApproved = !_catalogSettings.ProductReviewsMustBeApproved;

                var productReview = new ProductReview
                {
                    ProductId = product.Id,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    Title = model.Title,
                    ReviewText = model.ReviewText,
                    Rating = rating,
                    HelpfulYesTotal = 0,
                    HelpfulNoTotal = 0,
                    IsApproved = isApproved,
                    CreatedOnUtc = DateTime.UtcNow,
                };
                product.ProductReviews.Add(productReview);
                _productService.UpdateProduct(product);

                //update product totals
                _productService.UpdateProductReviewTotals(product);

                //notify store owner
                if (_catalogSettings.NotifyStoreOwnerAboutNewProductReviews)
                    _workflowMessageService.SendProductReviewNotificationMessage(productReview, _localizationSettings.DefaultAdminLanguageId);

                //activity log
                _customerActivityService.InsertActivity("PublicStore.AddProductReview", _localizationService.GetResource("ActivityLog.PublicStore.AddProductReview"), product.Name);

                //raise event
                if (productReview.IsApproved)
                    _eventPublisher.Publish(new ProductReviewApprovedEvent(productReview));

                response.Data = true;
                if (!isApproved)
                    response.SuccessMessage = _localizationService.GetResource("Reviews.SeeAfterApproving");
                else
                    response.SuccessMessage = _localizationService.GetResource("Reviews.SuccessfullyAdded");

                return Ok(response);
            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        response.ErrorList.Add(error.ErrorMessage);
                    }
                }
                response.StatusCode = (int)ErrorType.NotOk;
                response.Data = false;
                return Ok(response);
            }
            //If we got this far, something failed, redisplay form
            //PrepareProductReviewsModel(model, product);
        }

        [System.Web.Http.Route("api/recentlyviewedproducts/")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult RecentlyViewedProducts(int thumbPictureSize = 400)
        {
            var result = new GeneralResponseModel<IList<ProductOverViewModelApi>>();

            if (!_catalogSettings.RecentlyViewedProductsEnabled)
            {
                result.StatusCode = (int)ErrorType.NotOk;
                result.ErrorList.Add("Recently viewed products setting is not enable.");
            }

            var products = _bsRecentlyViewedProductsApi.GetRecentlyViewedProducts(_catalogSettings.RecentlyViewedProductsNumber);

            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            IList<ProductOverViewModelApi> model;
            if (products.Count > 0)
            {
                model = PrepareProductOverviewModels(products, true, true, thumbPictureSize).ToList();
            }
            else
            {
                model = null;
            }

            result.Data = model;
            return Ok(result);
        }

        #endregion
    }
}
