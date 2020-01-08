using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Infrastructure.Cache;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.ShoppingCart;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Tax;
using Nop.Web.Models.Media;
using BS.Plugin.NopStation.MobileWebApi.PluginSettings;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class ShoppingCartController : WebApiController
    {
        #region Field

        private readonly IProductService _productService;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICurrencyService _currencyService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IDownloadService _downloadService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ITaxService _taxService;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly IPermissionService _permissionService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IPictureService _pictureService;
        private readonly IWebHelper _webHelper;
        private readonly ICacheManager _cacheManager;
        private readonly MediaSettings _mediaSettings;
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private readonly IGiftCardService _giftCardService;
        private readonly IDiscountService _discountService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPaymentService _paymentService;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly AddressSettings _addressSettings;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly ShippingSettings _shippingSettings;
        private readonly ApiSettings _apiSettings;
        private readonly IPurchaseOfferService _purchaseOfferService;
        #endregion

        #region Ctor
        public ShoppingCartController(IProductService productService,
            ShoppingCartSettings shoppingCartSettings,
            IWorkContext workContext,
            IStoreContext storeContext,
            ICurrencyService currencyService,
            IShoppingCartService shoppingCartService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IProductAttributeService productAttributeService,
            IProductAttributeParser productAttributeParser,
            IDownloadService downloadService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            ITaxService taxService,
            ICheckoutAttributeService checkoutAttributeService,
            IPermissionService permissionService,
            ICheckoutAttributeParser checkoutAttributeParser,
            IGenericAttributeService genericAttributeService,
            IProductAttributeFormatter productAttributeFormatter,
            IPictureService pictureService,
            IWebHelper webHelper,
             ICacheManager cacheManager,
            MediaSettings mediaSettings,
            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            IGiftCardService giftCardService,
            IDiscountService discountService,
            IOrderTotalCalculationService orderTotalCalculationService,
            //GenericAttributeServiceApi genericAttributeServiceApi,
            TaxSettings taxSettings,
            IPaymentService paymentService,
            RewardPointsSettings rewardPointsSettings,
            CatalogSettings catalogSettings,
            AddressSettings addressSettings,
            IAddressAttributeFormatter addressAttributeFormatter,
            ShippingSettings shippingSettings,
            ApiSettings apiSettings,
            IPurchaseOfferService purchaseOfferService)
        {
            this._productService = productService;
            this._shoppingCartSettings = shoppingCartSettings;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._currencyService = currencyService;
            this._shoppingCartService = shoppingCartService;
            this._customerService = customerService;
            this._localizationService = localizationService;
            this._productAttributeService = productAttributeService;
            this._downloadService = downloadService;
            this._productAttributeParser = productAttributeParser;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._taxService = taxService;
            this._checkoutAttributeService = checkoutAttributeService;
            this._permissionService = permissionService;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._genericAttributeService = genericAttributeService;
            this._productAttributeFormatter = productAttributeFormatter;
            this._pictureService = pictureService;
            this._webHelper = webHelper;
            this._cacheManager = cacheManager;
            this._mediaSettings = mediaSettings;
            this._checkoutAttributeFormatter = checkoutAttributeFormatter;
            this._giftCardService = giftCardService;
            this._discountService = discountService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._taxSettings = taxSettings;
            this._paymentService = paymentService;
            this._rewardPointsSettings = rewardPointsSettings;
            this._catalogSettings = catalogSettings;
            this._shippingSettings = shippingSettings;
            this._addressSettings = addressSettings;
            this._addressAttributeFormatter = addressAttributeFormatter;
            //this._genericAttributeServiceApi = genericAttributeServiceApi;
            this._apiSettings = apiSettings;
            this._purchaseOfferService = purchaseOfferService;
        }
        #endregion
        
        #region Utility

        [System.Web.Http.NonAction]
        protected virtual PictureModel PrepareCartItemPictureModel(ShoppingCartItem sci,
            int pictureSize, bool showDefaultPicture, string productName)
        {
            var pictureCacheKey = string.Format(ModelCacheEventConsumer.CART_PICTURE_MODEL_KEY, sci.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
            var model = _cacheManager.Get(pictureCacheKey,
                //as we cache per user (shopping cart item identifier)
                //let's cache just for 3 minutes
                3, () =>
                {
                    //shopping cart item picture
                    var sciPicture = sci.Product.GetProductPicture(sci.AttributesXml, _pictureService, _productAttributeParser);
                    return new PictureModel
                    {
                        ImageUrl = _pictureService.GetPictureUrl(sciPicture, pictureSize, showDefaultPicture),
                    };
                });
            return model;
        }
        /// <summary>
        /// Prepare shopping cart model
        /// </summary>
        /// <param name="model">Model instance</param>
        /// <param name="cart">Shopping cart</param>
        /// <param name="isEditable">A value indicating whether cart is editable</param>
        /// <param name="validateCheckoutAttributes">A value indicating whether we should validate checkout attributes when preparing the model</param>
        /// <param name="prepareEstimateShippingIfEnabled">A value indicating whether we should prepare "Estimate shipping" model</param>
        /// <param name="setEstimateShippingDefaultAddress">A value indicating whether we should prefill "Estimate shipping" model with the default customer address</param>
        /// <param name="prepareAndDisplayOrderReviewData">A value indicating whether we should prepare review data (such as billing/shipping address, payment or shipping data entered during checkout)</param>
        /// <returns>Model</returns>
        [System.Web.Http.NonAction]
        protected virtual void PrepareShoppingCartModel(ShoppingCartResponseModel model,
            IList<ShoppingCartItem> cart, bool isEditable = true,
            bool validateCheckoutAttributes = false,
            bool prepareEstimateShippingIfEnabled = true, bool setEstimateShippingDefaultAddress = true,
            bool prepareAndDisplayOrderReviewData = false)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (model == null)
                throw new ArgumentNullException("model");


            if (cart.Count == 0)
                return;

            var checkoutAttributesXml = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _genericAttributeService, _storeContext.CurrentStore.Id);
            model.CheckoutAttributeInfo = _checkoutAttributeFormatter.FormatAttributes(checkoutAttributesXml, _workContext.CurrentCustomer);
            #region Checkout attributes

            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, !cart.RequiresShipping());
            foreach (var attribute in checkoutAttributes)
            {
                var attributeModel = new ShoppingCartResponseModel.CheckoutAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.GetLocalized(x => x.Name),
                    TextPrompt = attribute.GetLocalized(x => x.TextPrompt),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    DefaultValue = attribute.DefaultValue
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
                    var attributeValues = _checkoutAttributeService.GetCheckoutAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new ShoppingCartResponseModel.CheckoutAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.GetLocalized(x => x.Name),
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb,
                            IsPreSelected = attributeValue.IsPreSelected,
                        };
                        attributeModel.Values.Add(attributeValueModel);

                        //display price if allowed
                        if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                        {
                            decimal priceAdjustmentBase = _taxService.GetCheckoutAttributePrice(attributeValue);
                            decimal priceAdjustment = _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, _workContext.WorkingCurrency);
                            if (priceAdjustmentBase > decimal.Zero)
                                attributeValueModel.PriceAdjustment = "+" + _priceFormatter.FormatPrice(priceAdjustment);
                            else if (priceAdjustmentBase < decimal.Zero)
                                attributeValueModel.PriceAdjustment = "-" + _priceFormatter.FormatPrice(-priceAdjustment);
                        }
                    }
                }



                //set already selected attributes
                var selectedCheckoutAttributes = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _genericAttributeService, _storeContext.CurrentStore.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                    case AttributeControlType.ColorSquares:
                        {
                            if (!String.IsNullOrEmpty(selectedCheckoutAttributes))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(selectedCheckoutAttributes);
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
                            if (!String.IsNullOrEmpty(selectedCheckoutAttributes))
                            {
                                var enteredText = _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
                                if (enteredText.Count > 0)
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            //keep in mind my that the code below works only in the current culture
                            var selectedDateStr = _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
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

                model.CheckoutAttributes.Add(attributeModel);
            }

            #endregion

            model.DiscountBox.Display = _shoppingCartSettings.ShowDiscountBox;
            var discountCouponCode = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.DiscountCouponCode);
            var discount = _discountService.GetDiscountByCouponCode(discountCouponCode);
            if (discount != null &&
                discount.RequiresCouponCode &&
                _discountService.ValidateDiscount(discount, _workContext.CurrentCustomer).IsValid)
                model.DiscountBox.CurrentCode = discount.CouponCode;

            #region Cart items

            foreach (var sci in cart)
            {
                var cartItemModel = new ShoppingCartResponseModel.ShoppingCartItemModel
                {
                    Id = sci.Id,
                    Sku = sci.Product.FormatSku(sci.AttributesXml, _productAttributeParser),
                    ProductId = sci.Product.Id,
                    ProductName = sci.Product.GetLocalized(x => x.Name),
                    ProductSeName = sci.Product.GetSeName(),
                    Quantity = sci.Quantity,
                    AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml, _workContext.CurrentCustomer, serapator: ","),
                };

                //allow editing?
                //1. setting enabled?
                //2. simple product?
                //3. has attribute or gift card?
                //4. visible individually?
                cartItemModel.AllowItemEditing = _shoppingCartSettings.AllowCartItemEditing &&
                    sci.Product.ProductType == ProductType.SimpleProduct &&
                    (!String.IsNullOrEmpty(cartItemModel.AttributeInfo) || sci.Product.IsGiftCard) &&
                    sci.Product.VisibleIndividually;

                //allowed quantities
                var allowedQuantities = sci.Product.ParseAllowedQuantities();
                foreach (var qty in allowedQuantities)
                {
                    cartItemModel.AllowedQuantities.Add(new SelectListItem
                    {
                        Text = qty.ToString(),
                        Value = qty.ToString(),
                        Selected = sci.Quantity == qty
                    });
                }

                //recurring info
                if (sci.Product.IsRecurring)
                    cartItemModel.RecurringInfo = string.Format(_localizationService.GetResource("ShoppingCart.RecurringPeriod"), sci.Product.RecurringCycleLength, sci.Product.RecurringCyclePeriod.GetLocalizedEnum(_localizationService, _workContext));

                //rental info
                if (sci.Product.IsRental)
                {
                    var rentalStartDate = sci.RentalStartDateUtc.HasValue ? sci.Product.FormatRentalDate(sci.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = sci.RentalEndDateUtc.HasValue ? sci.Product.FormatRentalDate(sci.RentalEndDateUtc.Value) : "";
                    cartItemModel.RentalInfo = string.Format(_localizationService.GetResource("ShoppingCart.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }

                //unit prices
                if (sci.Product.CallForPrice)
                {
                    cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    decimal taxRate;
                    decimal shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetUnitPrice(sci), out taxRate);
                    decimal shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
                }
                //subtotal, discount
                if (sci.Product.CallForPrice)
                {
                    cartItemModel.SubTotal = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    //sub total
                   // Discount scDiscount; //change 3.8
                    List<Discount> scDiscount = null;
                    decimal shoppingCartItemDiscountBase;
                    decimal taxRate;
                    decimal shoppingCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetSubTotal(sci, true, out shoppingCartItemDiscountBase, out scDiscount), out taxRate);
                    decimal shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.SubTotal = _priceFormatter.FormatPrice(shoppingCartItemSubTotalWithDiscount);

                    //display an applied discount amount
                    if (scDiscount != null)
                    {
                        shoppingCartItemDiscountBase = _taxService.GetProductPrice(sci.Product, shoppingCartItemDiscountBase, out taxRate);
                        if (shoppingCartItemDiscountBase > decimal.Zero)
                        {
                            decimal shoppingCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemDiscountBase, _workContext.WorkingCurrency);
                            cartItemModel.Discount = _priceFormatter.FormatPrice(shoppingCartItemDiscount);
                        }
                    }
                }

                //picture
                if (_shoppingCartSettings.ShowProductImagesOnShoppingCart)
                {
                    cartItemModel.Picture = PrepareCartItemPictureModel(sci,
                        _apiSettings.CartThumbPictureSize, true, cartItemModel.ProductName);
                }

                //item warnings
                var itemWarnings = _shoppingCartService.GetShoppingCartItemWarnings(
                    _workContext.CurrentCustomer,
                    sci.ShoppingCartType,
                    sci.Product,
                    sci.StoreId,
                    sci.AttributesXml,
                    sci.CustomerEnteredPrice,
                    sci.RentalStartDateUtc,
                    sci.RentalEndDateUtc,
                    sci.Quantity,
                    false);
                foreach (var warning in itemWarnings)
                    cartItemModel.Warnings.Add(warning);

                model.Items.Add(cartItemModel);
            }

            #endregion


            #region Order review data

            if (prepareAndDisplayOrderReviewData)
            {
                model.OrderReviewData.Display = true;

                //billing info
                var billingAddress = _workContext.CurrentCustomer.BillingAddress;
                if (billingAddress != null)
                    model.OrderReviewData.BillingAddress.PrepareModelApi(
                        address: billingAddress,
                        excludeProperties: false,
                        addressSettings: _addressSettings,
                        addressAttributeFormatter: _addressAttributeFormatter);

                //shipping info
                if (cart.RequiresShipping())
                {
                    model.OrderReviewData.IsShippable = true;

                    if (_shippingSettings.AllowPickUpInStore)
                    {
                        model.OrderReviewData.SelectedPickUpInStore = _workContext.CurrentCustomer.GetAttribute<bool>(SystemCustomerAttributeNames.SelectedPickupPoint, _storeContext.CurrentStore.Id);
                    }

                    if (!model.OrderReviewData.SelectedPickUpInStore)
                    {
                        var shippingAddress = _workContext.CurrentCustomer.ShippingAddress;
                        if (shippingAddress != null)
                        {
                            model.OrderReviewData.ShippingAddress.PrepareModelApi(
                                address: shippingAddress,
                                excludeProperties: false,
                                addressSettings: _addressSettings,
                                addressAttributeFormatter: _addressAttributeFormatter);
                        }
                    }

                    //selected shipping method
                    var shippingOption = _workContext.CurrentCustomer.GetAttribute<ShippingOption>(SystemCustomerAttributeNames.SelectedShippingOption, _storeContext.CurrentStore.Id);
                    if (shippingOption != null)
                        model.OrderReviewData.ShippingMethod = shippingOption.Name;
                }
                //payment info
                var selectedPaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod, _storeContext.CurrentStore.Id);
                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(selectedPaymentMethodSystemName);
                model.OrderReviewData.PaymentMethod = paymentMethod != null ? paymentMethod.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id) : "";

                //custom values
                
            }
            #endregion

            model.Count = cart.GetTotalProducts();
            model.OrderTotalResponseModel = PrepareOrderTotalsModel(cart, true);
        }

        /// <summary>
        /// Parse product attributes on the product details page
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="form">Form</param>
        /// <returns>Parsed attributes</returns>
        [System.Web.Http.NonAction]
        protected virtual string ParseProductAttributes(Product product, NameValueCollection form)
        {
            string attributesXml = "";

            #region Product attributes
            var productAttributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            foreach (var attribute in productAttributes)
            {
                string controlId = string.Format("product_attribute_{0}_{1}_{2}", attribute.ProductId, attribute.ProductAttributeId, attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                foreach (var item in ctrlAttributes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                string enteredText = ctrlAttributes.Trim();
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var day = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));
                            }
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            Guid downloadGuid;
                            Guid.TryParse(form[controlId], out downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, download.DownloadGuid.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            #endregion

            #region Gift cards

            if (product.IsGiftCard)
            {
                string recipientName = "";
                string recipientEmail = "";
                string senderName = "";
                string senderEmail = "";
                string giftCardMessage = "";
                foreach (string formKey in form.AllKeys)
                {
                    if (formKey.Equals(string.Format("giftcard_{0}.RecipientName", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.RecipientEmail", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.SenderName", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.SenderEmail", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.Message", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        giftCardMessage = form[formKey];
                        continue;
                    }
                }

                attributesXml = _productAttributeParser.AddGiftCardAttribute(attributesXml,
                    recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);
            }

            #endregion

            return attributesXml;
        }

        [System.Web.Http.NonAction]
        protected virtual void ParseAndSaveCheckoutAttributes(List<ShoppingCartItem> cart, NameValueCollection form)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (form == null)
                throw new ArgumentNullException("form");

            string attributesXml = "";
            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, !cart.RequiresShipping());
            foreach (var attribute in checkoutAttributes)
            {
                string controlId = string.Format("checkout_attribute_{0}", attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _checkoutAttributeService.GetCheckoutAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                string enteredText = ctrlAttributes.Trim();
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var date = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(date));
                            }
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            Guid downloadGuid;
                            Guid.TryParse(form[controlId], out downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                           attribute, download.DownloadGuid.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            //save checkout attributes
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.CheckoutAttributes, attributesXml, _storeContext.CurrentStore.Id);
        }

        /// <summary>
        /// Parse product rental dates on the product details page
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="form">Form</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        [System.Web.Http.NonAction]
        protected virtual void ParseRentalDates(Product product, NameValueCollection form,
            out DateTime? startDate, out DateTime? endDate)
        {
            startDate = null;
            endDate = null;

            string startControlId = string.Format("rental_start_date_{0}", product.Id);
            string endControlId = string.Format("rental_end_date_{0}", product.Id);
            var ctrlStartDate = form[startControlId];
            var ctrlEndDate = form[endControlId];
            try
            {
                //currenly we support only this format (as in the \Views\Product\_RentalInfo.cshtml file)
                const string datePickerFormat = "MM/dd/yyyy";
                startDate = DateTime.ParseExact(ctrlStartDate, datePickerFormat, CultureInfo.InvariantCulture);
                endDate = DateTime.ParseExact(ctrlEndDate, datePickerFormat, CultureInfo.InvariantCulture);
            }
            catch
            {
            }
        }


        [System.Web.Http.NonAction]
        protected virtual OrderTotalsResponseModel PrepareOrderTotalsModel(IList<ShoppingCartItem> cart, bool isEditable)
        {
            var model = new OrderTotalsResponseModel();
            model.IsEditable = isEditable;

            if (cart.Count > 0)
            {
                //subtotal
                decimal orderSubTotalDiscountAmountBase;
              //  Discount orderSubTotalAppliedDiscount;
                List<Discount> orderSubTotalAppliedDiscounts;  // change 3.8
                decimal subTotalWithoutDiscountBase;
                decimal subTotalWithDiscountBase;
                var subTotalIncludingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
                _orderTotalCalculationService.GetShoppingCartSubTotal(cart, subTotalIncludingTax,
                    out orderSubTotalDiscountAmountBase, out orderSubTotalAppliedDiscounts,
                    out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
                decimal subtotalBase = subTotalWithoutDiscountBase;
                decimal subtotal = _currencyService.ConvertFromPrimaryStoreCurrency(subtotalBase, _workContext.WorkingCurrency);
                model.SubTotal = _priceFormatter.FormatPrice(subtotal, true, _workContext.WorkingCurrency, _workContext.WorkingLanguage, subTotalIncludingTax);

                if (orderSubTotalDiscountAmountBase > decimal.Zero)
                {
                    decimal orderSubTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderSubTotalDiscountAmountBase, _workContext.WorkingCurrency);
                    model.SubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountAmount, true, _workContext.WorkingCurrency, _workContext.WorkingLanguage, subTotalIncludingTax);
                    //model.AllowRemovingSubTotalDiscount = orderSubTotalAppliedDiscount != null &&
                    //                                      orderSubTotalAppliedDiscount.RequiresCouponCode &&
                    //                                      !String.IsNullOrEmpty(orderSubTotalAppliedDiscount.CouponCode) &&
                    //                                      model.IsEditable;
                    model.AllowRemovingSubTotalDiscount = model.IsEditable &&
                       orderSubTotalAppliedDiscounts.Any(d => d.RequiresCouponCode && !String.IsNullOrEmpty(d.CouponCode));  //change 3.8
                }


                //shipping info
                model.RequiresShipping = cart.RequiresShipping();
                if (model.RequiresShipping)
                {
                    decimal? shoppingCartShippingBase = _orderTotalCalculationService.GetShoppingCartShippingTotal(cart);
                    if (shoppingCartShippingBase.HasValue)
                    {
                        decimal shoppingCartShipping = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartShippingBase.Value, _workContext.WorkingCurrency);
                        model.Shipping = _priceFormatter.FormatShippingPrice(shoppingCartShipping, true);

                        //selected shipping method
                        var shippingOption = _workContext.CurrentCustomer.GetAttribute<ShippingOption>(SystemCustomerAttributeNames.SelectedShippingOption, _storeContext.CurrentStore.Id);
                        if (shippingOption != null)
                            model.SelectedShippingMethod = shippingOption.Name;
                    }
                }

                //payment method fee
                var paymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod, _storeContext.CurrentStore.Id);
                decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, paymentMethodSystemName);
                decimal paymentMethodAdditionalFeeWithTaxBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, _workContext.CurrentCustomer);
                if (paymentMethodAdditionalFeeWithTaxBase > decimal.Zero)
                {
                    decimal paymentMethodAdditionalFeeWithTax = _currencyService.ConvertFromPrimaryStoreCurrency(paymentMethodAdditionalFeeWithTaxBase, _workContext.WorkingCurrency);
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeWithTax, true);
                }

                //tax
                bool displayTax = true;
                bool displayTaxRates = true;
                if (_taxSettings.HideTaxInOrderSummary && _workContext.TaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    SortedDictionary<decimal, decimal> taxRates;
                    decimal shoppingCartTaxBase = _orderTotalCalculationService.GetTaxTotal(cart, out taxRates);
                    decimal shoppingCartTax = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartTaxBase, _workContext.WorkingCurrency);

                    if (shoppingCartTaxBase == 0 && _taxSettings.HideZeroTax)
                    {
                        displayTax = false;
                        displayTaxRates = false;
                    }
                    else
                    {
                        displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Count > 0;
                        displayTax = !displayTaxRates;

                        model.Tax = _priceFormatter.FormatPrice(shoppingCartTax, true, false);
                        foreach (var tr in taxRates)
                        {
                            model.TaxRates.Add(new OrderTotalsResponseModel.TaxRate
                            {
                                Rate = _priceFormatter.FormatTaxRate(tr.Key),
                                Value = _priceFormatter.FormatPrice(_currencyService.ConvertFromPrimaryStoreCurrency(tr.Value, _workContext.WorkingCurrency), true, false),
                            });
                        }
                    }
                }
                model.DisplayTaxRates = displayTaxRates;
                model.DisplayTax = displayTax;

                //total
                decimal orderTotalDiscountAmountBase;
               // Discount orderTotalAppliedDiscount;
                List<Discount> orderTotalAppliedDiscounts;  //change 3.8
                List<AppliedGiftCard> appliedGiftCards;
                int redeemedRewardPoints;
                decimal redeemedRewardPointsAmount;
                decimal? shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotal(cart,
                    out orderTotalDiscountAmountBase, out orderTotalAppliedDiscounts,
                    out appliedGiftCards, out redeemedRewardPoints, out redeemedRewardPointsAmount);
                if (shoppingCartTotalBase.HasValue)
                {
                    decimal shoppingCartTotal = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartTotalBase.Value, _workContext.WorkingCurrency);
                    model.OrderTotal = _priceFormatter.FormatPrice(shoppingCartTotal, true, false);
                }

                //discount
                if (orderTotalDiscountAmountBase > decimal.Zero)
                {
                    decimal orderTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderTotalDiscountAmountBase, _workContext.WorkingCurrency);
                    model.OrderTotalDiscount = _priceFormatter.FormatPrice(-orderTotalDiscountAmount, true, false);
                    //model.AllowRemovingOrderTotalDiscount = orderTotalAppliedDiscount != null &&
                    //    orderTotalAppliedDiscount.RequiresCouponCode &&
                    //    !String.IsNullOrEmpty(orderTotalAppliedDiscount.CouponCode) &&
                    //    model.IsEditable;

                    model.AllowRemovingOrderTotalDiscount = model.IsEditable &&
                       orderTotalAppliedDiscounts.Any(d => d.RequiresCouponCode && !String.IsNullOrEmpty(d.CouponCode));  // change 3.8
                }

                //gift cards
                if (appliedGiftCards != null && appliedGiftCards.Count > 0)
                {
                    foreach (var appliedGiftCard in appliedGiftCards)
                    {
                        var gcModel = new OrderTotalsResponseModel.GiftCard
                        {
                            Id = appliedGiftCard.GiftCard.Id,
                            CouponCode = appliedGiftCard.GiftCard.GiftCardCouponCode,
                        };
                        decimal amountCanBeUsed = _currencyService.ConvertFromPrimaryStoreCurrency(appliedGiftCard.AmountCanBeUsed, _workContext.WorkingCurrency);
                        gcModel.Amount = _priceFormatter.FormatPrice(-amountCanBeUsed, true, false);

                        decimal remainingAmountBase = appliedGiftCard.GiftCard.GetGiftCardRemainingAmount() - appliedGiftCard.AmountCanBeUsed;
                        decimal remainingAmount = _currencyService.ConvertFromPrimaryStoreCurrency(remainingAmountBase, _workContext.WorkingCurrency);
                        gcModel.Remaining = _priceFormatter.FormatPrice(remainingAmount, true, false);

                        model.GiftCards.Add(gcModel);
                    }
                }

                //reward points to be spent (redeemed)
                if (redeemedRewardPointsAmount > decimal.Zero)
                {
                    decimal redeemedRewardPointsAmountInCustomerCurrency = _currencyService.ConvertFromPrimaryStoreCurrency(redeemedRewardPointsAmount, _workContext.WorkingCurrency);
                    model.RedeemedRewardPoints = redeemedRewardPoints;
                    model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-redeemedRewardPointsAmountInCustomerCurrency, true, false);
                }

                //reward points to be earned
                if (_rewardPointsSettings.Enabled &&
                    _rewardPointsSettings.DisplayHowMuchWillBeEarned &&
                    shoppingCartTotalBase.HasValue)
                {
                    model.WillEarnRewardPoints = _orderTotalCalculationService
                        .CalculateRewardPoints(_workContext.CurrentCustomer, shoppingCartTotalBase.Value);
                }

            }

            return model;
        }

        [System.Web.Http.NonAction]
        protected virtual void PrepareWishlistModel(WishlistResponseModel model,
            IList<ShoppingCartItem> cart, bool isEditable = true)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (model == null)
                throw new ArgumentNullException("model");

            model.EmailWishlistEnabled = _shoppingCartSettings.EmailWishlistEnabled;
            model.IsEditable = isEditable;
            model.DisplayAddToCart = _permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart);
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoWishlist;

            if (cart.Count == 0)
                return;

            #region Simple properties

            var customer = cart.GetCustomer();
            model.CustomerGuid = customer.CustomerGuid;
            model.CustomerFullname = customer.GetFullName();
            model.ShowProductImages = _shoppingCartSettings.ShowProductImagesOnShoppingCart;
            model.ShowSku = _catalogSettings.ShowProductSku;

            //cart warnings
            var cartWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, "", false);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);

            #endregion

            #region Cart items

            foreach (var sci in cart)
            {
                var cartItemModel = new WishlistResponseModel.ShoppingCartItemModel
                {
                    Id = sci.Id,
                    Sku = sci.Product.FormatSku(sci.AttributesXml, _productAttributeParser),
                    ProductId = sci.Product.Id,
                    ProductName = sci.Product.GetLocalized(x => x.Name),
                    ProductSeName = sci.Product.GetSeName(),
                    Quantity = sci.Quantity,
                    AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml, _workContext.CurrentCustomer, serapator: ","),
                };

                //allowed quantities
                var allowedQuantities = sci.Product.ParseAllowedQuantities();
                foreach (var qty in allowedQuantities)
                {
                    cartItemModel.AllowedQuantities.Add(new SelectListItem
                    {
                        Text = qty.ToString(),
                        Value = qty.ToString(),
                        Selected = sci.Quantity == qty
                    });
                }


                //recurring info
                if (sci.Product.IsRecurring)
                    cartItemModel.RecurringInfo = string.Format(_localizationService.GetResource("ShoppingCart.RecurringPeriod"), sci.Product.RecurringCycleLength, sci.Product.RecurringCyclePeriod.GetLocalizedEnum(_localizationService, _workContext));

                //rental info
                if (sci.Product.IsRental)
                {
                    var rentalStartDate = sci.RentalStartDateUtc.HasValue ? sci.Product.FormatRentalDate(sci.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = sci.RentalEndDateUtc.HasValue ? sci.Product.FormatRentalDate(sci.RentalEndDateUtc.Value) : "";
                    cartItemModel.RentalInfo = string.Format(_localizationService.GetResource("ShoppingCart.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }

                //unit prices
                if (sci.Product.CallForPrice)
                {
                    cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    decimal taxRate;
                    decimal shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetUnitPrice(sci), out taxRate);
                    decimal shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
                }
                //subtotal, discount
                if (sci.Product.CallForPrice)
                {
                    cartItemModel.SubTotal = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    //sub total
                   // Discount scDiscount;
                    List<Discount> scDiscounts;  // change 3.8
                    decimal shoppingCartItemDiscountBase;
                    decimal taxRate;
                    decimal shoppingCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetSubTotal(sci, true, out shoppingCartItemDiscountBase, out scDiscounts), out taxRate);
                    decimal shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.SubTotal = _priceFormatter.FormatPrice(shoppingCartItemSubTotalWithDiscount);

                    //display an applied discount amount
                    if (scDiscounts != null)
                    {
                        shoppingCartItemDiscountBase = _taxService.GetProductPrice(sci.Product, shoppingCartItemDiscountBase, out taxRate);
                        if (shoppingCartItemDiscountBase > decimal.Zero)
                        {
                            decimal shoppingCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemDiscountBase, _workContext.WorkingCurrency);
                            cartItemModel.Discount = _priceFormatter.FormatPrice(shoppingCartItemDiscount);
                        }
                    }
                }

                //picture
                if (_shoppingCartSettings.ShowProductImagesOnShoppingCart)
                {
                    cartItemModel.Picture = PrepareCartItemPictureModel(sci,
                        _mediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName);
                }

                //item warnings
                var itemWarnings = _shoppingCartService.GetShoppingCartItemWarnings(
                    _workContext.CurrentCustomer,
                    sci.ShoppingCartType,
                    sci.Product,
                    sci.StoreId,
                    sci.AttributesXml,
                    sci.CustomerEnteredPrice,
                    sci.RentalStartDateUtc,
                    sci.RentalEndDateUtc,
                    sci.Quantity,
                    false);
                foreach (var warning in itemWarnings)
                    cartItemModel.Warnings.Add(warning);

                model.Items.Add(cartItemModel);
            }

            #endregion
        }

       
        #endregion
        
        #region Action Method

        [System.Web.Http.Route("api/AddProductToCart/{productId}/{shoppingCartTypeId}")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult AddProductToCart_Details(int productId, int shoppingCartTypeId, List<KeyValueApi> formValues)
        {
            var form = new NameValueCollection();
            foreach (var values in formValues)
            {
                form.Add(values.Key, values.Value);
            }
            var errrorList = new List<string>();
            var result = new AddProductToCartResponseModel();
            var product = _productService.GetProductById(productId);
            if (product == null)
            {
                //return Json(new
                //{
                //    redirect = Url.RouteUrl("HomePage"),
                //});
                errrorList.Add("Product Not Found");
                result.ErrorList = errrorList;
                result.Success = false;
                result.StatusCode = (int)ErrorType.NotOk;
            }

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
            {
                //return Json(new
                //{
                //    success = false,
                //    message = "Only simple products could be added to the cart"
                //});
                errrorList.Add("Only simple products could be added to the cart");
                result.ErrorList = errrorList;
                result.Success = false;
                result.StatusCode = (int)ErrorType.NotOk;
            }

            #region Update existing shopping cart item?
            int updatecartitemid = 0;

            foreach (string formKey in form.AllKeys)
                if (formKey.Equals(string.Format("addtocart_{0}.UpdatedShoppingCartItemId", productId), StringComparison.InvariantCultureIgnoreCase))
                {
                    int.TryParse(form[formKey], out updatecartitemid);
                    break;
                }
            ShoppingCartItem updatecartitem = null;
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
                    //return Json(new
                    //{
                    //    success = false,
                    //    message = "No shopping cart item found to update"
                    //});
                    errrorList.Add("No shopping cart item found to update");
                    result.ErrorList = errrorList;
                    result.Success = false;
                    result.StatusCode = (int)ErrorType.NotOk;
                }
                //is it this product?
                if (product.Id != updatecartitem.ProductId)
                {
                    //return Json(new
                    //{
                    //    success = false,
                    //    message = "This product does not match a passed shopping cart item identifier"
                    //});
                    errrorList.Add("This product does not match a passed shopping cart item identifier");
                    result.ErrorList = errrorList;
                    result.Success = false;
                    result.StatusCode = (int)ErrorType.NotOk;
                }
            }
            #endregion

            #region Customer entered price
            decimal customerEnteredPriceConverted = decimal.Zero;
            if (product.CustomerEntersPrice)
            {

                foreach (string formKey in form.AllKeys)
                {
                    if (formKey.Equals(string.Format("addtocart_{0}.CustomerEnteredPrice", productId), StringComparison.InvariantCultureIgnoreCase))
                    {
                        decimal customerEnteredPrice;
                        if (decimal.TryParse(form[formKey], out customerEnteredPrice))
                            customerEnteredPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(customerEnteredPrice, _workContext.WorkingCurrency);
                        break;
                    }
                }
            }
            #endregion

            #region Quantity

            int quantity = 1;

            foreach (string formKey in form.AllKeys)
                if (formKey.Equals(string.Format("addtocart_{0}.EnteredQuantity", productId), StringComparison.InvariantCultureIgnoreCase))
                {
                    int.TryParse(form[formKey], out quantity);
                    break;
                }

            #endregion

            //product and gift card attributes
            string attributes = ParseProductAttributes(product, form);

            //rental attributes
            DateTime? rentalStartDate = null;
            DateTime? rentalEndDate = null;
            if (product.IsRental)
            {
                ParseRentalDates(product, form, out rentalStartDate, out rentalEndDate);
            }

            //save item
            var addToCartWarnings = new List<string>();
            var cartType = (ShoppingCartType)shoppingCartTypeId;
            if (updatecartitem == null)
            {
                //add to the cart
                addToCartWarnings.AddRange(_shoppingCartService.AddToCart(_workContext.CurrentCustomer,
                    product, cartType, _storeContext.CurrentStore.Id,
                    attributes, customerEnteredPriceConverted,
                    rentalStartDate, rentalEndDate, quantity, true));
            }
            else
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                var otherCartItemWithSameParameters = _shoppingCartService.FindShoppingCartItemInTheCart(
                    cart, cartType, product, attributes, customerEnteredPriceConverted,
                    rentalStartDate, rentalEndDate);
                if (otherCartItemWithSameParameters != null &&
                    otherCartItemWithSameParameters.Id == updatecartitem.Id)
                {
                    //ensure it's other shopping cart cart item
                    otherCartItemWithSameParameters = null;
                }
                //update existing item
                addToCartWarnings.AddRange(_shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                    updatecartitem.Id, attributes, customerEnteredPriceConverted,
                    rentalStartDate, rentalEndDate, quantity, true));
                if (otherCartItemWithSameParameters != null && addToCartWarnings.Count == 0)
                {
                    //delete the same shopping cart item (the other one)
                    _shoppingCartService.DeleteShoppingCartItem(otherCartItemWithSameParameters);
                }
            }

            #region Return result

            if (addToCartWarnings.Count > 0)
            {
                //cannot be added to the cart/wishlist
                //let's display warnings
                //return Json(new
                //{
                //    success = false,
                //    message = addToCartWarnings.ToArray()
                //});
                result.ErrorList = addToCartWarnings;
                result.Success = false;
                result.StatusCode = (int) ErrorType.NotOk;
            }
            else
            {
                //added to the cart/wishlist
                switch (cartType)
                {
                    case ShoppingCartType.Wishlist:
                    {
                        //activity log
                        //_customerActivityService.InsertActivity("PublicStore.AddToWishlist", _localizationService.GetResource("ActivityLog.PublicStore.AddToWishlist"), product.Name);

                        if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct)
                        {
                            //redirect to the wishlist page
                            //return Json(new
                            //{
                            //    redirect = Url.RouteUrl("Wishlist"),
                            //});
                            result.ForceRedirect = true;
                        }

                        ////display notification message and update appropriate blocks
                        //var updatetopwishlistsectionhtml = string.Format(_localizationService.GetResource("Wishlist.HeaderQuantity"),
                        //_workContext.CurrentCustomer.ShoppingCartItems
                        //.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                        //.LimitPerStore(_storeContext.CurrentStore.Id)
                        //.ToList()
                        //.GetTotalProducts());

                        //return Json(new
                        //{
                        //    success = true,
                        //    message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheWishlist.Link"), Url.RouteUrl("Wishlist")),
                        //    updatetopwishlistsectionhtml = updatetopwishlistsectionhtml,
                        //});
                        result.Success = true;
                        break;
                    }
                    case ShoppingCartType.ShoppingCart:
                    default:
                    {
                        //activity log
                        // _customerActivityService.InsertActivity("PublicStore.AddToShoppingCart", _localizationService.GetResource("ActivityLog.PublicStore.AddToShoppingCart"), product.Name);
                        var cart = _workContext.CurrentCustomer.ShoppingCartItems
                            .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
                            .LimitPerStore(_storeContext.CurrentStore.Id)
                            .ToList();
                        result.Count = _workContext.CurrentCustomer.ShoppingCartItems
                        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                        .LimitPerStore(_storeContext.CurrentStore.Id)
                        .ToList()
                        .GetTotalProducts();
                        if (_shoppingCartSettings.DisplayCartAfterAddingProduct)
                        {
                            //redirect to the shopping cart page
                            //return Json(new
                            //{
                            //    redirect = Url.RouteUrl("ShoppingCart"),
                            //});
                            result.ForceRedirect = true;
                            result.Success = true;
                        }

                        ////display notification message and update appropriate blocks
                        //var updatetopcartsectionhtml = string.Format(_localizationService.GetResource("ShoppingCart.HeaderQuantity"),
                        

                        //var updateflyoutcartsectionhtml = _shoppingCartSettings.MiniShoppingCartEnabled
                        //    ? this.RenderPartialViewToString("FlyoutShoppingCart", PrepareMiniShoppingCartModel())
                        //    : "";

                        //return Json(new
                        //{
                        //    success = true,
                        //    message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheCart.Link"), Url.RouteUrl("ShoppingCart")),
                        //    updatetopcartsectionhtml = updatetopcartsectionhtml,
                        //    updateflyoutcartsectionhtml = updateflyoutcartsectionhtml
                        //});
                        result.Success = true;
                        break;
                    }
                }
            }
            return Ok(result);

            #endregion
        }

        //handle product attribute selection event. this way we return new price, overridden gtin/sku/mpn
        //currently we use this method on the product details pages
        [System.Web.Http.Route("api/ProductDetailsPagePrice/{productId}")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult ProductDetails_AttributeChange(int productId, List<KeyValueApi> formValues)
        {
            var form = new NameValueCollection();
            foreach (var values in formValues)
            {
                form.Add(values.Key, values.Value);
            }
            var result = new ProductDetailPriceResponseModel();
            var errrorList = new List<string>();
            var product = _productService.GetProductById(productId);
            if (product == null)
            {
                errrorList.Add("Product Not Found");
                result.ErrorList = errrorList;
                result.StatusCode = (int)ErrorType.NotOk;
                return Ok(result);
            }

            string attributeXml = ParseProductAttributes(product, form);

            //rental attributes
            DateTime? rentalStartDate = null;
            DateTime? rentalEndDate = null;
            if (product.IsRental)
            {
                ParseRentalDates(product, form, out rentalStartDate, out rentalEndDate);
            }

            result.Sku = product.FormatSku(attributeXml, _productAttributeParser);
            result.Mpn = product.FormatMpn(attributeXml, _productAttributeParser);
            result.Gtin = product.FormatGtin(attributeXml, _productAttributeParser);


            string price = "";
            if (!product.CustomerEntersPrice)
            {
                //we do not calculate price of "customer enters price" option is enabled
               // Discount scDiscount;
                List<Discount> scDiscounts; // change 3.8
                decimal discountAmount;
                decimal finalPrice = _priceCalculationService.GetUnitPrice(product,
                    _workContext.CurrentCustomer,
                    ShoppingCartType.ShoppingCart,
                    1, attributeXml, 0,
                    rentalStartDate, rentalEndDate,
                    true, out discountAmount, out scDiscounts);
                decimal taxRate;
                decimal finalPriceWithDiscountBase = _taxService.GetProductPrice(product, finalPrice, out taxRate);
                decimal finalPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithDiscountBase, _workContext.WorkingCurrency);
                price = _priceFormatter.FormatPrice(finalPriceWithDiscount);
            }
            result.Price = price;
            return Ok(result);

        }

        [System.Web.Http.Route("api/ShoppingCart")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult Cart()
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
            //    return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new ShoppingCartResponseModel();

            //Task prepareShoppingCartModelTask = Task.Factory.StartNew(() => PrepareShoppingCartModel(model, cart));
            //Task<int> getTotalProductsTask = Task.Factory.StartNew<int>(() => cart.GetTotalProducts());
            //Task<OrderTotalsResponseModel> prepareOrderTotalsModelTask = Task.Factory.StartNew<OrderTotalsResponseModel>(() => PrepareOrderTotalsModel(cart, true));
            //Task.WaitAll(prepareShoppingCartModelTask, getTotalProductsTask);
            PrepareShoppingCartModel(model, cart);
            return Ok(model);
        }


        [System.Web.Http.Route("api/ShoppingCart/applycheckoutattribute")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult ApplyCheckoutAttribute(List<KeyValueApi> formValues)
        {
            var form = new NameValueCollection();
            foreach (var values in formValues)
            {
                form.Add(values.Key, values.Value);
            }

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            ParseAndSaveCheckoutAttributes(cart, form);

            var model = new GeneralResponseModel<bool>()
            {
                Data = true
            };
            return Ok(model);

        }

        [System.Web.Http.Route("api/ShoppingCart/UpdateCart")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult UpdateCart(List<KeyValueApi> formValues)
        {
            var form = new NameValueCollection();
            foreach (var values in formValues)
            {
                form.Add(values.Key, values.Value);
            }

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var allIdsToRemove = form["removefromcart"] != null ? form["removefromcart"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() : new List<int>();

            //current warnings <cart item identifier, warnings>
            var innerWarnings = new Dictionary<int, IList<string>>();
            foreach (var sci in cart)
            {
                bool remove = allIdsToRemove.Contains(sci.Id);
                if (remove)
                    _shoppingCartService.DeleteShoppingCartItem(sci, ensureOnlyActiveCheckoutAttributes: true);
                else
                {
                    foreach (string formKey in form.AllKeys)
                        if (formKey.Equals(string.Format("itemquantity{0}", sci.Id), StringComparison.InvariantCultureIgnoreCase))
                        {
                            int newQuantity;
                            if (int.TryParse(form[formKey], out newQuantity))
                            {
                                var currSciWarnings = _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                                    sci.Id, sci.AttributesXml, sci.CustomerEnteredPrice,
                                    sci.RentalStartDateUtc, sci.RentalEndDateUtc,
                                    newQuantity, true);
                                innerWarnings.Add(sci.Id, currSciWarnings);
                            }
                            break;
                        }
                }
            }

            //updated cart
            cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new ShoppingCartResponseModel(); 
            PrepareShoppingCartModel(model, cart);
            //update current warnings
            model.ErrorList = new List<string>();
            foreach (var kvp in innerWarnings)
            {
                //kvp = <cart item identifier, warnings>
                var sciId = kvp.Key;
                var warnings = kvp.Value;
                //find model
                var sciModel = model.Items.FirstOrDefault(x => x.Id == sciId);
                if (sciModel != null)
                    foreach (var w in warnings)
                        if (!model.ErrorList.Contains(w))
                            model.ErrorList.Add(w);


            }
            if (model.ErrorList.Count > 0)
            {
                model.StatusCode = (int)ErrorType.NotOk;
            }
            
            model.OrderTotalResponseModel = PrepareOrderTotalsModel(cart,true);
            return Ok(model);
        }

        [System.Web.Http.Route("api/ShoppingCart/ApplyGiftCard")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult ApplyGiftCard(SingleValue value)
        {
            string giftcardcouponcode = value.Value;
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            
            var model = new CoupontypeResponse();
            model.OrderTotalResponseModel = PrepareOrderTotalsModel(cart, true);
            var errorList = new List<string>();
            string error = string.Empty;
            //parse and save checkout attributes

            if (!cart.IsRecurring())
            {
                if (!String.IsNullOrWhiteSpace(giftcardcouponcode))
                {
                    var giftCard = _giftCardService.GetAllGiftCards(giftCardCouponCode: giftcardcouponcode).FirstOrDefault();
                    bool isGiftCardValid = giftCard != null && giftCard.IsGiftCardValid();
                    if (isGiftCardValid)
                    {
                        _workContext.CurrentCustomer.ApplyGiftCardCouponCode(giftcardcouponcode);
                        _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                        //model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.Applied");
                        model.Data = true;
                    }
                    else
                    {
                        error = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.WrongGiftCard");

                    }
                }
                else
                {
                    error = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.WrongGiftCard");

                }
            }
            else
            {
                error = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.DontWorkWithAutoshipProducts");

            }
            if (!string.IsNullOrEmpty(error))
            {
                errorList.Add(error);
                model.ErrorList = errorList;
                model.Data = false;
                model.StatusCode = (int)ErrorType.NotOk;
            }
            // PrepareShoppingCartModel(model, cart);
            return Ok(model);
        }

        [System.Web.Http.Route("api/ShoppingCart/RemoveGiftCard")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult RemoveGiftCardCode(SingleValue value)
        {

            var model = new CoupontypeResponse();
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            model.OrderTotalResponseModel = PrepareOrderTotalsModel(cart, true);
            //get gift card identifier
            int giftCardId = Convert.ToInt32(value.Value);
            var gc = _giftCardService.GetGiftCardById(giftCardId);
            if (gc != null)
            {
                _workContext.CurrentCustomer.RemoveGiftCardCouponCode(gc.GiftCardCouponCode);
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                model.Data = true;
            }
            else
            {
                model.StatusCode = (int)ErrorType.NotOk;
                model.Data = false;
            }

            return Ok(model);

        }

        [System.Web.Http.Route("api/ShoppingCart/ApplyDiscountCoupon")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult ApplyDiscountCoupon(SingleValue value)
        {
            string discountcouponcode = value.Value;
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //parse and save checkout attributes

            //var model = new CoupontypeResponse();
            var errorList = new List<string>();

            var model = new ShoppingCartResponseModel();
            if (!String.IsNullOrWhiteSpace(discountcouponcode))
            {
                //we find even hidden records here. this way we can display a user-friendly message if it's expired
                var discount = _discountService.GetDiscountByCouponCode(discountcouponcode, true);
                if (discount != null && discount.RequiresCouponCode)
                {
                    var validationResult = _discountService.ValidateDiscount(discount, _workContext.CurrentCustomer, discountcouponcode);
                    if (validationResult.IsValid)
                    {
                        //valid
                        _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.DiscountCouponCode, discountcouponcode);
                        model.DiscountBox.Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.Applied");
                        model.DiscountBox.IsApplied = true;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(validationResult.UserError))
                        {
                            //some user error
                            errorList.Add(validationResult.UserError);
                            model.DiscountBox.Message = validationResult.UserError;
                            model.DiscountBox.IsApplied = false;
                        }
                        else
                        {
                            //general error text
                            errorList.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount"));
                            model.DiscountBox.Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount");
                            model.DiscountBox.IsApplied = false;
                        }
                    }
                }
                else
                {
                    //discount cannot be found
                    errorList.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount"));
                    model.DiscountBox.Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount");
                    model.DiscountBox.IsApplied = false;
                }
            }
            else
            {
                errorList.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount"));
                model.DiscountBox.Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount");
                model.DiscountBox.IsApplied = false;
            }

            if (errorList.Any())
            {
                model.ErrorList.AddRange(errorList);
                model.StatusCode = (int)ErrorType.NotOk;
            }
            PrepareShoppingCartModel(model, cart);
            return Ok(model);
        }

        [System.Web.Http.Route("api/ShoppingCart/RemoveDiscountCoupon")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult RemoveDiscountCoupon()
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            //var id= _genericAttributeService.GetAttributeById(27);
            // _genericAttributeService.DeleteAttribute(id);

            _genericAttributeService.SaveAttribute<object>(_workContext.CurrentCustomer,
                SystemCustomerAttributeNames.DiscountCouponCode, null);

            var model = new CoupontypeResponse();
            model.OrderTotalResponseModel = PrepareOrderTotalsModel(cart, true);
            model.Data = true;
            return Ok(model);
        }

        [System.Web.Http.Route("api/ShoppingCart/OrderTotal")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult OrderTotals()
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = PrepareOrderTotalsModel(cart, true);
            return Ok(model);
        }

        [System.Web.Http.Route("api/shoppingCart/wishlist")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult Wishlist(Guid? customerGuid=null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            Customer customer = customerGuid.HasValue ?
                _customerService.GetCustomerByGuid(customerGuid.Value)
                : _workContext.CurrentCustomer;
            if (customer == null)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new WishlistResponseModel();
            PrepareWishlistModel(model, cart, !customerGuid.HasValue);
            return Ok(model);
        }

        [System.Web.Http.Route("api/ShoppingCart/UpdateWishlist")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult UpdateWishlist(List<KeyValueApi> formValues)
        {
            var form = new NameValueCollection();
            foreach (var values in formValues)
            {
                form.Add(values.Key, values.Value);
            }
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var allIdsToRemove = form["removefromcart"] != null
                ? form["removefromcart"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList()
                : new List<int>();

            //current warnings <cart item identifier, warnings>
            var innerWarnings = new Dictionary<int, IList<string>>();
            foreach (var sci in cart)
            {
                bool remove = allIdsToRemove.Contains(sci.Id);
                if (remove)
                    _shoppingCartService.DeleteShoppingCartItem(sci);
                else
                {
                    foreach (string formKey in form.AllKeys)
                        if (formKey.Equals(string.Format("itemquantity{0}", sci.Id), StringComparison.InvariantCultureIgnoreCase))
                        {
                            int newQuantity;
                            if (int.TryParse(form[formKey], out newQuantity))
                            {
                                var currSciWarnings = _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                                    sci.Id, sci.AttributesXml, sci.CustomerEnteredPrice,
                                    sci.RentalStartDateUtc, sci.RentalEndDateUtc,
                                    newQuantity, true);
                                innerWarnings.Add(sci.Id, currSciWarnings);
                            }
                            break;
                        }
                }
            }

            //updated wishlist
            cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new WishlistResponseModel();
            PrepareWishlistModel(model, cart);
            //update current warnings
            foreach (var kvp in innerWarnings)
            {
                //kvp = <cart item identifier, warnings>
                var sciId = kvp.Key;
                var warnings = kvp.Value;
                //find model
                var sciModel = model.Items.FirstOrDefault(x => x.Id == sciId);
                if (sciModel != null)
                    foreach (var w in warnings)
                        if (!model.ErrorList.Contains(w))
                            model.ErrorList.Add(w);


            }
            if (model.ErrorList.Count > 0)
            {
                model.StatusCode = (int)ErrorType.NotOk;
            }
            return Ok(model);
        }

        [System.Web.Http.Route("api/ShoppingCart/AddItemsToCartFromWishlist")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult AddItemsToCartFromWishlist(List<KeyValueApi> formValues, Guid? customerGuid = null)
        {
            Customer customer = customerGuid.HasValue ?
               _customerService.GetCustomerByGuid(customerGuid.Value)
               : _workContext.CurrentCustomer;
            var form = new NameValueCollection();
            foreach (var values in formValues)
            {
                form.Add(values.Key, values.Value);
            }
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            var response = new WishlistResponseModel();

            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            var pageCustomer = customerGuid.HasValue
                ? _customerService.GetCustomerByGuid(customerGuid.Value)
                : _workContext.CurrentCustomer;
            if (pageCustomer == null)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            var pageCart = pageCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var allWarnings = new List<string>();
            // var numberOfAddedItems = 0;
            var allIdsToAdd = form["addtocart"] != null
                ? form["addtocart"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList()
                : new List<int>();
            foreach (var sci in pageCart)
            {
                if (allIdsToAdd.Contains(sci.Id))
                {
                    var warnings = _shoppingCartService.AddToCart(_workContext.CurrentCustomer,
                        sci.Product, ShoppingCartType.ShoppingCart,
                        _storeContext.CurrentStore.Id,
                        sci.AttributesXml, sci.CustomerEnteredPrice,
                        sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, true);

                    if (_shoppingCartSettings.MoveItemsFromWishlistToCart && //settings enabled
                        !customerGuid.HasValue && //own wishlist
                        warnings.Count == 0) //no warnings ( already in the cart)
                    {
                        //let's remove the item from wishlist
                        _shoppingCartService.DeleteShoppingCartItem(sci);
                    }
                    response.ErrorList.AddRange(warnings);
                }
            }
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                 .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                 .ToList();
            response.Count = cart.GetTotalProducts();
            var wishlistCart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            PrepareWishlistModel(response, wishlistCart, !customerGuid.HasValue);

            if (response.ErrorList.Count > 0)
            {
                response.StatusCode = (int)ErrorType.NotOk;
            }
            return Ok(response);
        }

        [System.Web.Http.Route("api/shoppingcart/checkoutorderinformation")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult CheckoutOrderInformation()
        {
             var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
               .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new CheckoutInformationResponseModel();
            PrepareShoppingCartModel(model.ShoppingCartModel, cart, prepareAndDisplayOrderReviewData: true);
            model.OrderTotalModel = PrepareOrderTotalsModel(cart, true);
            return Ok(model);
        }

        [System.Web.Http.Route("api/purchase-offer")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult PurchaseOffer()
        {
            var model = new PurchaseOfferResponseModel();
            var purchaseOffer = _purchaseOfferService.GetCurrentPurchaseOffer(_workContext.CurrentCustomer,
                _workContext.TaxDisplayType, _workContext.WorkingCurrency);

            if (purchaseOffer != null)
            {
                var productPicture = _pictureService.GetPicturesByProductId(purchaseOffer.GiftProductId, 1).FirstOrDefault();
                model.GiftModel.ImageUrl = _pictureService.GetPictureUrl(productPicture, _mediaSettings.CartThumbPictureSize);
                model.GiftModel.GiftItemName = purchaseOffer.GiftProduct.Name;
                model.OfferActive = true;
                model.GiftAvailable = true;
            }

            return Ok(model);
        }

        #endregion
    }
}
