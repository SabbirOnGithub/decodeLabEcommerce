//using Microsoft.Owin.Logging;
//using Nop.Core;
//using Nop.Core.Caching;
//using Nop.Core.Domain.Catalog;
//using Nop.Core.Domain.Media;
//using Nop.Core.Domain.Orders;
//using Nop.Plugin.Widgets.AlgoliaSearch.Models.AlgoliaProduct;
//using Nop.Services.Catalog;
//using Nop.Services.Directory;
//using Nop.Services.Localization;
//using Nop.Services.Media;
//using Nop.Services.Security;
//using Nop.Services.Seo;
//using Nop.Services.Tax;
//using Nop.Web.Infrastructure.Cache;
//using Nop.Web.Models.Catalog;
//using Nop.Web.Models.Media;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web;

//namespace Nop.Plugin.Widgets.AlgoliaSearch
//{
//    public static class ProductModelConverterExtensions
//    {
//        public static IList<ProductSpecificationModel> PrepareProductSpecificationModel(this Product product,
//            IWorkContext workContext,
//            ISpecificationAttributeService specificationAttributeService,
//            ICacheManager cacheManager)
//        {
//            if (product == null)
//                throw new ArgumentNullException("product");

//            string cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_SPECS_MODEL_KEY, product.Id, workContext.WorkingLanguage.Id);
//            return cacheManager.Get(cacheKey, () =>
//                specificationAttributeService.GetProductSpecificationAttributes(product.Id, 0, null, true)
//                .Select(psa =>
//                {
//                    var m = new ProductSpecificationModel
//                    {
//                        SpecificationAttributeId = psa.SpecificationAttributeOption.SpecificationAttributeId,
//                        SpecificationAttributeName = psa.SpecificationAttributeOption.SpecificationAttribute.GetLocalized(x => x.Name),
//                        ColorSquaresRgb = psa.SpecificationAttributeOption.ColorSquaresRgb
//                    };

//                    switch (psa.AttributeType)
//                    {
//                        case SpecificationAttributeType.Option:
//                            m.ValueRaw = HttpUtility.HtmlEncode(psa.SpecificationAttributeOption.GetLocalized(x => x.Name));
//                            break;
//                        case SpecificationAttributeType.CustomText:
//                            m.ValueRaw = HttpUtility.HtmlEncode(psa.CustomValue);
//                            break;
//                        case SpecificationAttributeType.CustomHtmlText:
//                            m.ValueRaw = psa.CustomValue;
//                            break;
//                        case SpecificationAttributeType.Hyperlink:
//                            m.ValueRaw = string.Format("<a href='{0}' target='_blank'>{0}</a>", psa.CustomValue);
//                            break;
//                        default:
//                            break;
//                    }
//                    return m;
//                }).ToList()
//            );
//        }

//        //public static IList<ProductDetailsModel.ProductAttributeModel> PrepareProductAttributeModel(this Product product,
//        //    ICacheManager cacheManager,
//        //    IProductAttributeService productAttributeService,
//        //    IProductAttributeParser productAttributeParser,
//        //    IPermissionService permissionService,
//        //    IPriceCalculationService priceCalculationService,
//        //    ITaxService taxService,
//        //    IPriceFormatter priceFormatter,
//        //    ICurrencyService currencyService,
//        //    IWorkContext workContext,
//        //    IWebHelper webHelper,
//        //    IStoreContext storeContext,
//        //    IPictureService pictureService,
//        //    MediaSettings mediaSettings,
//        //    IDownloadService downloadService)
//        //{
//        //    ShoppingCartItem updatecartitem = null;
//        //    var productAttributes = new List<ProductDetailsModel.ProductAttributeModel>();

//        //    //performance optimization
//        //    //We cache a value indicating whether a product has attributes
//        //    IList <ProductAttributeMapping> productAttributeMapping = null;
//        //    string cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_HAS_PRODUCT_ATTRIBUTES_KEY, product.Id);
//        //    var hasProductAttributesCache = cacheManager.Get<bool?>(cacheKey);
//        //    if (!hasProductAttributesCache.HasValue)
//        //    {
//        //        //no value in the cache yet
//        //        //let's load attributes and cache the result (true/false)
//        //        productAttributeMapping = productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
//        //        hasProductAttributesCache = productAttributeMapping.Any();
//        //        cacheManager.Set(cacheKey, hasProductAttributesCache, 60);
//        //    }
//        //    if (hasProductAttributesCache.Value && productAttributeMapping == null)
//        //    {
//        //        //cache indicates that the product has attributes
//        //        //let's load them
//        //        productAttributeMapping = productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
//        //    }
//        //    if (productAttributeMapping == null)
//        //    {
//        //        productAttributeMapping = new List<ProductAttributeMapping>();
//        //    }
//        //    foreach (var attribute in productAttributeMapping)
//        //    {
//        //        var attributeModel = new ProductDetailsModel.ProductAttributeModel
//        //        {
//        //            Id = attribute.Id,
//        //            ProductId = product.Id,
//        //            ProductAttributeId = attribute.ProductAttributeId,
//        //            Name = attribute.ProductAttribute.GetLocalized(x => x.Name),
//        //            Description = attribute.ProductAttribute.GetLocalized(x => x.Description),
//        //            TextPrompt = attribute.TextPrompt,
//        //            IsRequired = attribute.IsRequired,
//        //            AttributeControlType = attribute.AttributeControlType,
//        //            DefaultValue = updatecartitem != null ? null : attribute.DefaultValue,
//        //            HasCondition = !String.IsNullOrEmpty(attribute.ConditionAttributeXml)
//        //        };
//        //        if (!String.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
//        //        {
//        //            attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
//        //                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
//        //                .ToList();
//        //        }

//        //        if (attribute.ShouldHaveValues())
//        //        {
//        //            //values
//        //            var attributeValues = productAttributeService.GetProductAttributeValues(attribute.Id);
//        //            foreach (var attributeValue in attributeValues)
//        //            {
//        //                var valueModel = new ProductDetailsModel.ProductAttributeValueModel
//        //                {
//        //                    Id = attributeValue.Id,
//        //                    Name = attributeValue.GetLocalized(x => x.Name),
//        //                    ColorSquaresRgb = attributeValue.ColorSquaresRgb, //used with "Color squares" attribute type
//        //                    IsPreSelected = attributeValue.IsPreSelected
//        //                };
//        //                attributeModel.Values.Add(valueModel);

//        //                //display price if allowed
//        //                if (permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
//        //                {
//        //                    decimal taxRate;
//        //                    decimal attributeValuePriceAdjustment = priceCalculationService.GetProductAttributeValuePriceAdjustment(attributeValue);
//        //                    decimal priceAdjustmentBase = taxService.GetProductPrice(product, attributeValuePriceAdjustment, out taxRate);
//        //                    decimal priceAdjustment = currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, workContext.WorkingCurrency);
//        //                    if (priceAdjustmentBase > decimal.Zero)
//        //                        valueModel.PriceAdjustment = "+" + priceFormatter.FormatPrice(priceAdjustment, false, false);
//        //                    else if (priceAdjustmentBase < decimal.Zero)
//        //                        valueModel.PriceAdjustment = "-" + priceFormatter.FormatPrice(-priceAdjustment, false, false);

//        //                    valueModel.PriceAdjustmentValue = priceAdjustment;
//        //                }

//        //                //"image square" picture (with with "image squares" attribute type only)
//        //                if (attributeValue.ImageSquaresPictureId > 0)
//        //                {
//        //                    var productAttributeImageSquarePictureCacheKey = string.Format(ModelCacheEventConsumer.PRODUCTATTRIBUTE_IMAGESQUARE_PICTURE_MODEL_KEY,
//        //                           attributeValue.ImageSquaresPictureId,
//        //                           webHelper.IsCurrentConnectionSecured(),
//        //                           storeContext.CurrentStore.Id);
//        //                    valueModel.ImageSquaresPictureModel = cacheManager.Get(productAttributeImageSquarePictureCacheKey, () =>
//        //                    {
//        //                        var imageSquaresPicture = pictureService.GetPictureById(attributeValue.ImageSquaresPictureId);
//        //                        if (imageSquaresPicture != null)
//        //                        {
//        //                            return new PictureModel
//        //                            {
//        //                                FullSizeImageUrl = pictureService.GetPictureUrl(imageSquaresPicture),
//        //                                ImageUrl = pictureService.GetPictureUrl(imageSquaresPicture, mediaSettings.ImageSquarePictureSize)
//        //                            };
//        //                        }
//        //                        return new PictureModel();
//        //                    });
//        //                }

//        //                //picture of a product attribute value
//        //                valueModel.PictureId = attributeValue.PictureId;
//        //            }

//        //        }

//        //        //set already selected attributes (if we're going to update the existing shopping cart item)
//        //        if (updatecartitem != null)
//        //        {
//        //            switch (attribute.AttributeControlType)
//        //            {
//        //                case AttributeControlType.DropdownList:
//        //                case AttributeControlType.RadioList:
//        //                case AttributeControlType.Checkboxes:
//        //                case AttributeControlType.ColorSquares:
//        //                case AttributeControlType.ImageSquares:
//        //                    {
//        //                        if (!String.IsNullOrEmpty(updatecartitem.AttributesXml))
//        //                        {
//        //                            //clear default selection
//        //                            foreach (var item in attributeModel.Values)
//        //                                item.IsPreSelected = false;

//        //                            //select new values
//        //                            var selectedValues = productAttributeParser.ParseProductAttributeValues(updatecartitem.AttributesXml);
//        //                            foreach (var attributeValue in selectedValues)
//        //                                foreach (var item in attributeModel.Values)
//        //                                    if (attributeValue.Id == item.Id)
//        //                                        item.IsPreSelected = true;
//        //                        }
//        //                    }
//        //                    break;
//        //                case AttributeControlType.ReadonlyCheckboxes:
//        //                    {
//        //                        //do nothing
//        //                        //values are already pre-set
//        //                    }
//        //                    break;
//        //                case AttributeControlType.TextBox:
//        //                case AttributeControlType.MultilineTextbox:
//        //                    {
//        //                        if (!String.IsNullOrEmpty(updatecartitem.AttributesXml))
//        //                        {
//        //                            var enteredText = productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
//        //                            if (enteredText.Any())
//        //                                attributeModel.DefaultValue = enteredText[0];
//        //                        }
//        //                    }
//        //                    break;
//        //                case AttributeControlType.Datepicker:
//        //                    {
//        //                        //keep in mind my that the code below works only in the current culture
//        //                        var selectedDateStr = productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
//        //                        if (selectedDateStr.Any())
//        //                        {
//        //                            DateTime selectedDate;
//        //                            if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture,
//        //                                                   DateTimeStyles.None, out selectedDate))
//        //                            {
//        //                                //successfully parsed
//        //                                attributeModel.SelectedDay = selectedDate.Day;
//        //                                attributeModel.SelectedMonth = selectedDate.Month;
//        //                                attributeModel.SelectedYear = selectedDate.Year;
//        //                            }
//        //                        }

//        //                    }
//        //                    break;
//        //                case AttributeControlType.FileUpload:
//        //                    {
//        //                        if (!String.IsNullOrEmpty(updatecartitem.AttributesXml))
//        //                        {
//        //                            var downloadGuidStr = productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id).FirstOrDefault();
//        //                            Guid downloadGuid;
//        //                            Guid.TryParse(downloadGuidStr, out downloadGuid);
//        //                            var download = downloadService.GetDownloadByGuid(downloadGuid);
//        //                            if (download != null)
//        //                                attributeModel.DefaultValue = download.DownloadGuid.ToString();
//        //                        }
//        //                    }
//        //                    break;
//        //                default:
//        //                    break;
//        //            }
//        //        }

//        //        productAttributes.Add(attributeModel);
//        //    }

//        //    return productAttributes;
//        //}

//        public static IEnumerable<ProductOverviewModel> PrepareProductOverviewModels(IPagedList<Product> products,
//            IWorkContext workContext,
//            IStoreContext storeContext,
//            ICategoryService categoryService,
//            IProductService productService,
//            ISpecificationAttributeService specificationAttributeService,
//            IPriceCalculationService priceCalculationService,
//            IPriceFormatter priceFormatter,
//            IPermissionService permissionService,
//            ILocalizationService localizationService,
//            ITaxService taxService,
//            ICurrencyService currencyService,
//            IPictureService pictureService,
//            IMeasureService measureService,
//            IWebHelper webHelper,
//            ICacheManager cacheManager,
//            CatalogSettings catalogSettings,
//            MediaSettings mediaSettings,
//            bool preparePriceModel = true, bool preparePictureModel = true,
//            int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
//            bool forceRedirectionAfterAddingToCart = false)
//        {
//            if (products == null)
//                throw new ArgumentNullException("products");

//            var models = new List<ProductOverviewModel>();
//            foreach (var product in products)
//            {
//                var model = new ProductOverviewModel
//                {
//                    Id = product.Id,
//                    Name = product.GetLocalized(x => x.Name),
//                    ShortDescription = product.GetLocalized(x => x.ShortDescription),
//                    FullDescription = product.GetLocalized(x => x.FullDescription),
//                    SeName = product.GetSeName(),
//                    ProductType = product.ProductType,
//                    MarkAsNew = product.MarkAsNew &&
//                        (!product.MarkAsNewStartDateTimeUtc.HasValue || product.MarkAsNewStartDateTimeUtc.Value < DateTime.UtcNow) &&
//                        (!product.MarkAsNewEndDateTimeUtc.HasValue || product.MarkAsNewEndDateTimeUtc.Value > DateTime.UtcNow)
//                };
//                //price
//                if (preparePriceModel)
//                {
//                    #region Prepare product price

//                    var priceModel = new ProductOverviewModel.ProductPriceModel
//                    {
//                        ForceRedirectionAfterAddingToCart = forceRedirectionAfterAddingToCart
//                    };

//                    switch (product.ProductType)
//                    {
//                        case ProductType.GroupedProduct:
//                            {
//                                #region Grouped product

//                                var associatedProducts = productService.GetAssociatedProducts(product.Id, storeContext.CurrentStore.Id);

//                                //add to cart button (ignore "DisableBuyButton" property for grouped products)
//                                priceModel.DisableBuyButton = !permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart) ||
//                                    !permissionService.Authorize(StandardPermissionProvider.DisplayPrices);

//                                //add to wishlist button (ignore "DisableWishlistButton" property for grouped products)
//                                priceModel.DisableWishlistButton = !permissionService.Authorize(StandardPermissionProvider.EnableWishlist) ||
//                                    !permissionService.Authorize(StandardPermissionProvider.DisplayPrices);

//                                //compare products
//                                priceModel.DisableAddToCompareListButton = !catalogSettings.CompareProductsEnabled;
//                                switch (associatedProducts.Count)
//                                {
//                                    case 0:
//                                        {
//                                            //no associated products
//                                        }
//                                        break;
//                                    default:
//                                        {
//                                            //we have at least one associated product
//                                            //compare products
//                                            priceModel.DisableAddToCompareListButton = !catalogSettings.CompareProductsEnabled;
//                                            //priceModel.AvailableForPreOrder = false;

//                                            if (permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
//                                            {
//                                                //find a minimum possible price
//                                                decimal? minPossiblePrice = null;
//                                                Product minPriceProduct = null;
//                                                foreach (var associatedProduct in associatedProducts)
//                                                {
//                                                    //calculate for the maximum quantity (in case if we have tier prices)
//                                                    var tmpPrice = priceCalculationService.GetFinalPrice(associatedProduct,
//                                                        workContext.CurrentCustomer, decimal.Zero, true, int.MaxValue);
//                                                    if (!minPossiblePrice.HasValue || tmpPrice < minPossiblePrice.Value)
//                                                    {
//                                                        minPriceProduct = associatedProduct;
//                                                        minPossiblePrice = tmpPrice;
//                                                    }
//                                                }
//                                                if (minPriceProduct != null && !minPriceProduct.CustomerEntersPrice)
//                                                {
//                                                    if (minPriceProduct.CallForPrice)
//                                                    {
//                                                        priceModel.OldPrice = null;
//                                                        priceModel.Price = localizationService.GetResource("Products.CallForPrice");
//                                                    }
//                                                    else if (minPossiblePrice.HasValue)
//                                                    {
//                                                        //calculate prices
//                                                        decimal taxRate;
//                                                        decimal finalPriceBase = taxService.GetProductPrice(minPriceProduct, minPossiblePrice.Value, out taxRate);
//                                                        decimal finalPrice = currencyService.ConvertFromPrimaryStoreCurrency(finalPriceBase, workContext.WorkingCurrency);

//                                                        priceModel.OldPrice = null;
//                                                        priceModel.Price = String.Format(localizationService.GetResource("Products.PriceRangeFrom"), priceFormatter.FormatPrice(finalPrice));
//                                                        priceModel.PriceValue = finalPrice;

//                                                        //PAngV baseprice (used in Germany)
//                                                        priceModel.BasePricePAngV = product.FormatBasePrice(finalPrice,
//                                                            localizationService, measureService, currencyService, workContext, priceFormatter);
//                                                    }
//                                                    else
//                                                    {
//                                                        //Actually it's not possible (we presume that minimalPrice always has a value)
//                                                        //We never should get here
//                                                        Debug.WriteLine("Cannot calculate minPrice for product #{0}", product.Id);
//                                                    }
//                                                }
//                                            }
//                                            else
//                                            {
//                                                //hide prices
//                                                priceModel.OldPrice = null;
//                                                priceModel.Price = null;
//                                            }
//                                        }
//                                        break;
//                                }

//                                #endregion
//                            }
//                            break;
//                        case ProductType.SimpleProduct:
//                        default:
//                            {
//                                #region Simple product

//                                //add to cart button
//                                priceModel.DisableBuyButton = product.DisableBuyButton ||
//                                    !permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart) ||
//                                    !permissionService.Authorize(StandardPermissionProvider.DisplayPrices);

//                                //add to wishlist button
//                                priceModel.DisableWishlistButton = product.DisableWishlistButton ||
//                                    !permissionService.Authorize(StandardPermissionProvider.EnableWishlist) ||
//                                    !permissionService.Authorize(StandardPermissionProvider.DisplayPrices);
//                                //compare products
//                                priceModel.DisableAddToCompareListButton = !catalogSettings.CompareProductsEnabled;

//                                //rental
//                                priceModel.IsRental = product.IsRental;

//                                //pre-order
//                                if (product.AvailableForPreOrder)
//                                {
//                                    priceModel.AvailableForPreOrder = !product.PreOrderAvailabilityStartDateTimeUtc.HasValue ||
//                                        product.PreOrderAvailabilityStartDateTimeUtc.Value >= DateTime.UtcNow;
//                                    priceModel.PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc;
//                                }

//                                //prices
//                                if (permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
//                                {
//                                    if (!product.CustomerEntersPrice)
//                                    {
//                                        if (product.CallForPrice)
//                                        {
//                                            //call for price
//                                            priceModel.OldPrice = null;
//                                            priceModel.Price = localizationService.GetResource("Products.CallForPrice");
//                                        }
//                                        else
//                                        {
//                                            //prices

//                                            //calculate for the maximum quantity (in case if we have tier prices)
//                                            decimal minPossiblePrice = priceCalculationService.GetFinalPrice(product,
//                                                workContext.CurrentCustomer, decimal.Zero, true, int.MaxValue);

//                                            decimal taxRate;
//                                            decimal oldPriceBase = taxService.GetProductPrice(product, product.OldPrice, out taxRate);
//                                            decimal finalPriceBase = taxService.GetProductPrice(product, minPossiblePrice, out taxRate);

//                                            decimal oldPrice = currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, workContext.WorkingCurrency);
//                                            decimal finalPrice = currencyService.ConvertFromPrimaryStoreCurrency(finalPriceBase, workContext.WorkingCurrency);

//                                            //do we have tier prices configured?
//                                            var tierPrices = new List<TierPrice>();
//                                            if (product.HasTierPrices)
//                                            {
//                                                tierPrices.AddRange(product.TierPrices
//                                                    .OrderBy(tp => tp.Quantity)
//                                                    .ToList()
//                                                    .FilterByStore(storeContext.CurrentStore.Id)
//                                                    .FilterForCustomer(workContext.CurrentCustomer)
//                                                    .RemoveDuplicatedQuantities());
//                                            }
//                                            //When there is just one tier (with  qty 1), 
//                                            //there are no actual savings in the list.
//                                            bool displayFromMessage = tierPrices.Any() &&
//                                                !(tierPrices.Count == 1 && tierPrices[0].Quantity <= 1);
//                                            if (displayFromMessage)
//                                            {
//                                                priceModel.OldPrice = null;
//                                                priceModel.Price = String.Format(localizationService.GetResource("Products.PriceRangeFrom"), priceFormatter.FormatPrice(finalPrice));
//                                                priceModel.PriceValue = finalPrice;
//                                            }
//                                            else
//                                            {
//                                                if (finalPriceBase != oldPriceBase && oldPriceBase != decimal.Zero)
//                                                {
//                                                    priceModel.OldPrice = priceFormatter.FormatPrice(oldPrice);
//                                                    priceModel.Price = priceFormatter.FormatPrice(finalPrice);
//                                                    priceModel.PriceValue = finalPrice;
//                                                }
//                                                else
//                                                {
//                                                    priceModel.OldPrice = null;
//                                                    priceModel.Price = priceFormatter.FormatPrice(finalPrice);
//                                                    priceModel.PriceValue = finalPrice;
//                                                }
//                                            }
//                                            if (product.IsRental)
//                                            {
//                                                //rental product
//                                                priceModel.OldPrice = priceFormatter.FormatRentalProductPeriod(product, priceModel.OldPrice);
//                                                priceModel.Price = priceFormatter.FormatRentalProductPeriod(product, priceModel.Price);
//                                            }


//                                            //property for German market
//                                            //we display tax/shipping info only with "shipping enabled" for this product
//                                            //we also ensure this it's not free shipping
//                                            priceModel.DisplayTaxShippingInfo = catalogSettings.DisplayTaxShippingInfoProductBoxes
//                                                && product.IsShipEnabled &&
//                                                !product.IsFreeShipping;


//                                            //PAngV baseprice (used in Germany)
//                                            priceModel.BasePricePAngV = product.FormatBasePrice(finalPrice,
//                                                localizationService, measureService, currencyService, workContext, priceFormatter);
//                                        }
//                                    }
//                                }
//                                else
//                                {
//                                    //hide prices
//                                    priceModel.OldPrice = null;
//                                    priceModel.Price = null;
//                                }

//                                #endregion
//                            }
//                            break;
//                    }

//                    model.ProductPrice = priceModel;

//                    #endregion
//                }

//                //picture
//                if (preparePictureModel)
//                {
//                    #region Prepare product picture

//                    //If a size has been set in the view, we use it in priority
//                    int pictureSize = productThumbPictureSize.HasValue ? productThumbPictureSize.Value : mediaSettings.ProductThumbPictureSize;
//                    //prepare picture model
//                    var defaultProductPictureCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_DEFAULTPICTURE_MODEL_KEY, product.Id, pictureSize, true, workContext.WorkingLanguage.Id, webHelper.IsCurrentConnectionSecured(), storeContext.CurrentStore.Id);
//                    model.DefaultPictureModel = cacheManager.Get(defaultProductPictureCacheKey, () =>
//                    {
//                        var picture = pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();
//                        var pictureModel = new PictureModel
//                        {
//                            ImageUrl = pictureService.GetPictureUrl(picture, pictureSize),
//                            FullSizeImageUrl = pictureService.GetPictureUrl(picture)
//                        };
//                        //"title" attribute
//                        pictureModel.Title = (picture != null && !string.IsNullOrEmpty(picture.TitleAttribute)) ?
//                            picture.TitleAttribute :
//                            string.Format(localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), model.Name);
//                        //"alt" attribute
//                        pictureModel.AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute)) ?
//                            picture.AltAttribute :
//                            string.Format(localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), model.Name);

//                        return pictureModel;
//                    });

//                    #endregion
//                }

//                //specs
//                if (prepareSpecificationAttributes)
//                {
//                    model.SpecificationAttributeModels = PrepareProductSpecificationModel(product, workContext,
//                         specificationAttributeService, cacheManager);
//                }

//                //reviews
//                model.ReviewOverviewModel = product.PrepareProductReviewOverviewModel(storeContext, catalogSettings, cacheManager);

//                models.Add(model);
//            }
//            return models;
//        }

//        public static ProductReviewOverviewModel PrepareProductReviewOverviewModel(this Product product,
//            IStoreContext storeContext,
//            CatalogSettings catalogSettings,
//            ICacheManager cacheManager)
//        {
//            ProductReviewOverviewModel productReview;

//            if (catalogSettings.ShowProductReviewsPerStore)
//            {
//                string cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_REVIEWS_MODEL_KEY, product.Id, storeContext.CurrentStore.Id);

//                productReview = cacheManager.Get(cacheKey, () =>
//                {
//                    return new ProductReviewOverviewModel
//                    {
//                        RatingSum = product.ProductReviews
//                                .Where(pr => pr.IsApproved && pr.StoreId == storeContext.CurrentStore.Id)
//                                .Sum(pr => pr.Rating),
//                        TotalReviews = product
//                                .ProductReviews
//                                .Count(pr => pr.IsApproved && pr.StoreId == storeContext.CurrentStore.Id)
//                    };
//                });
//            }
//            else
//            {
//                productReview = new ProductReviewOverviewModel()
//                {
//                    RatingSum = product.ApprovedRatingSum,
//                    TotalReviews = product.ApprovedTotalReviews
//                };
//            }
//            if (productReview != null)
//            {
//                productReview.ProductId = product.Id;
//                productReview.AllowCustomerReviews = product.AllowCustomerReviews;
//            }
//            return productReview;
//        }
//    }
//}
