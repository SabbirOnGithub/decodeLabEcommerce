using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Plugin.Widgets.AlgoliaSearch;
using Nop.Plugin.Widgets.EkShopA2I.Domain;
using Nop.Plugin.Widgets.EkShopA2I.Extensions;
using Nop.Plugin.Widgets.EkShopA2I.Models;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using System.Web;

namespace Nop.Plugin.Widgets.EkShopA2I.Services
{
    public class EkshopEpService : IEkshopEpService
    {
        #region Fields
        const SslProtocols _Tls12 = (SslProtocols)0x00000C00;
        const SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;


        private readonly ILogger _logger;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ICustomerService _customerService;
        private readonly ICountryService _countryService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IPictureService _pictureService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IConfigureService _esConfigureService;
        private readonly MediaSettings _mediaSettings;
        private readonly ITaxService _taxService;
        private readonly TaxSettings _taxSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IProductService _productService;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ILanguageService _languageService;
        private readonly OrderSettings _orderSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly IPaymentService _paymentService;
        private readonly IWebHelper _webHelper;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderService _orderService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IGiftCardService _giftCardService;
        private readonly IDiscountService _discountService;
        private readonly IPdfService _pdfService;
        private readonly ICategoryService _categoryService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly PaymentSettings _paymentSettings;
        private readonly ICacheManager _cacheManager;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IShippingService _shippingService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IRepository<EsOrder> _esOrderRepo;
        private readonly IRepository<EsUdcCommissionRate> _udcCommissionRateRepository;
        private readonly EkshopSettings _ekShopA2ISettings;
        private readonly AlgoliaSettings _algoliaSettings;
        #endregion

        #region Ctor
        public EkshopEpService(ISettingService settingService, IWorkContext workContext,
            IConfigureService esConfigureService, IRepository<EsOrder> esOrderRepo,
            IPictureService pictureService, IOrderProcessingService orderProcessingService,
            ITaxService taxService, IPriceCalculationService priceCalculationService,
            ICurrencyService currencyService, MediaSettings mediaSettings,
            IProductService productService, ShoppingCartSettings shoppingCartSettings,
            IShoppingCartService shoppingCartService, IStoreContext storeContext,
            TaxSettings taxSettings, IOrderTotalCalculationService orderTotalCalculationService,
            ILocalizationService localizationService, ICustomerService customerService,
            ICheckoutAttributeFormatter checkoutAttributeFormatter, IWebHelper webHelper,
            OrderSettings orderSettings, CurrencySettings currencySettings,
            ICountryService countryService, IPaymentService paymentService,
            ShippingSettings shippingSettings, IPriceFormatter priceFormatter,
            IEventPublisher eventPublisher, ILanguageService languageService,
            IOrderService orderService, IProductAttributeParser productAttributeParser,
            IProductAttributeFormatter productAttributeFormatter, IGiftCardService giftCardService,
            IGenericAttributeService genericAttributeService, IPdfService pdfService,
            IRewardPointService rewardPointService, IDiscountService discountService,
            PaymentSettings paymentSettings, RewardPointsSettings rewardPointsSettings,
            LocalizationSettings localizationSettings, IShippingService shippingService,
            ICustomerActivityService customerActivityService, ILogger logger,
            ICacheManager cacheManager, IRepository<EsUdcCommissionRate> udcCommissionRateRepository,
            ICategoryService categoryService, EkshopSettings ekShopA2ISettings,
            AlgoliaSettings algoliaSettings)
        {
            _cacheManager = cacheManager;
            _logger = logger;
            _esOrderRepo = esOrderRepo;
            _shippingService = shippingService;
            _localizationSettings = localizationSettings;
            _rewardPointsSettings = rewardPointsSettings;
            _paymentSettings = paymentSettings;
            _genericAttributeService = genericAttributeService;
            _rewardPointService = rewardPointService;
            _discountService = discountService;
            _pdfService = pdfService;
            _productAttributeFormatter = productAttributeFormatter;
            _giftCardService = giftCardService;
            _productAttributeParser = productAttributeParser;
            _orderService = orderService;
            _eventPublisher = eventPublisher;
            _webHelper = webHelper;
            _priceFormatter = priceFormatter;
            _paymentService = paymentService;
            _orderSettings = orderSettings;
            _checkoutAttributeFormatter = checkoutAttributeFormatter;
            _localizationService = localizationService;
            _settingService = settingService;
            _workContext = workContext;
            _esConfigureService = esConfigureService;
            _pictureService = pictureService;
            _mediaSettings = mediaSettings;
            _storeContext = storeContext;
            _priceCalculationService = priceCalculationService;
            _currencyService = currencyService;
            _taxService = taxService;
            _productService = productService;
            _shoppingCartSettings = shoppingCartSettings;
            _shoppingCartService = shoppingCartService;
            _orderProcessingService = orderProcessingService;
            _taxSettings = taxSettings;
            _orderTotalCalculationService = orderTotalCalculationService;
            _customerService = customerService;
            _languageService = languageService;
            _currencySettings = currencySettings;
            _countryService = countryService;
            _shippingSettings = shippingSettings;
            _customerActivityService = customerActivityService;
            _udcCommissionRateRepository = udcCommissionRateRepository;
            _categoryService = categoryService;
            _ekShopA2ISettings = ekShopA2ISettings;
            _algoliaSettings = algoliaSettings;
        }
        #endregion

        #region Utilities

        private void GetParentCategory(int categoryId, List<int> ids)
        {
            int id = _categoryService.GetCategoryById(categoryId).ParentCategoryId;
            if (id == 0)
                return;
            ids.Add(id);
            GetParentCategory(id, ids);
        }

        #endregion

        #region Methods

        #region EP Session

        public bool TryCreateSession(string auth_token, string ekshop_api_base_url)
        {
            try
            {
                if (_ekShopA2ISettings.EnableLog)
                    _logger.Information("Auth token: " + auth_token);

                var tokenInfo = new TokenInfo()
                {
                    auth_token = auth_token,
                    access_token = _ekShopA2ISettings.AccessToken
                };

                var tokenInfoJson = HttpUtility.UrlEncode(JsonConvert.SerializeObject(tokenInfo));
                var requestUrl = new Uri(ekshop_api_base_url).Append("api/create-ep-session").AbsoluteUri;

                ServicePointManager.SecurityProtocol = Tls12;
                var request = (HttpWebRequest)WebRequest.Create(requestUrl);

                if (_ekShopA2ISettings.EnableLog)
                    _logger.Information("Auth request url; " + requestUrl);

                var postData = string.Format("token_info={0}", tokenInfoJson);
                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = " application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                request.AuthenticationLevel = AuthenticationLevel.MutualAuthRequired;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                if (_ekShopA2ISettings.EnableLog)
                    _logger.Information("Auth response string: " + responseString);

                var model = JsonConvert.DeserializeObject<ResponseModel>(responseString);

                if (model.meta.status == 200)
                {
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.EkshopSessionToken, model.response.session_token);
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.EkshopBaseUrl, ekshop_api_base_url);
                    return true;
                }
                else
                    _logger.Error(model.response.message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return false;
        }

        #endregion

        #region Set Cart

        public bool TrySetCart(out string redirectUrl)
        {
            redirectUrl = "";
            try
            {
                var sessionToken = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.EkshopSessionToken);
                if (!string.IsNullOrWhiteSpace(sessionToken))
                {
                    var cartModel = PrepareCartList();
                    string cartModelJson = HttpUtility.UrlEncode(JsonConvert.SerializeObject(cartModel));

                    ServicePointManager.SecurityProtocol = Tls12;
                    var requestUrl = new Uri(_workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.EkshopBaseUrl)).Append("api/set-cart").AbsoluteUri;
                    var request = (HttpWebRequest)WebRequest.Create(requestUrl);

                    var postData = string.Format("cart_detail={0}&session_token={1}&access_token={2}", cartModelJson,
                        HttpUtility.UrlEncode(sessionToken), HttpUtility.UrlEncode(_ekShopA2ISettings.AccessToken));

                    var data = Encoding.ASCII.GetBytes(postData);
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = data.Length;

                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    var response = (HttpWebResponse)request.GetResponse();
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    var model = JsonConvert.DeserializeObject<ResponseModel>(responseString);

                    if (model.meta.status == 200)
                    {
                        redirectUrl = model.response.redirect_url;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return false;
        }

        private CartDetailsModel PrepareCartList()
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                        .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
                        .LimitPerStore(_storeContext.CurrentStore.Id)
                        .ToList();

            var model = new CartDetailsModel();
            model.cart_options.cart_id = _workContext.CurrentCustomer.Id;
            model.cart_options.other_required_data = null;

            _orderTotalCalculationService.GetShoppingCartSubTotal(cart, true, out decimal subTotalDiscountAmount,
                out _, out decimal subTotalWithoutDiscountBase, out _);

            if (_ekShopA2ISettings.EnableFreeShipping && _ekShopA2ISettings.MinimumCartValue <= subTotalWithoutDiscountBase)
            {
                model.shipping_cost = 0;
            }
            else
            {
                model.shipping_cost = _ekShopA2ISettings.ShippingCharge;
            }

            int pictureSize = _mediaSettings.ProductThumbPictureSize;
            var baseUrl = _storeContext.CurrentStore.Url;

            foreach (var item in cart)
            {
                if (!IsVendorRestricted(item.Product))
                {
                    var picture = _pictureService.GetPicturesByProductId(item.ProductId).FirstOrDefault();

                    decimal taxRate = decimal.Zero;
                    List<Discount> scDiscounts;
                    decimal shoppingCartItemDiscountBase = decimal.Zero;
                    decimal shoppingCartItemDiscount = decimal.Zero;

                    decimal shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(item.Product, _priceCalculationService.GetUnitPrice(item), out taxRate);
                    decimal shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);

                    decimal shoppingCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(item.Product, _priceCalculationService.GetSubTotal(item, true, out shoppingCartItemDiscountBase, out scDiscounts), out taxRate);
                    decimal shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, _workContext.WorkingCurrency);


                    if (shoppingCartItemDiscountBase > decimal.Zero)
                    {
                        shoppingCartItemDiscountBase = _taxService.GetProductPrice(item.Product, shoppingCartItemDiscountBase, out taxRate);
                        if (shoppingCartItemDiscountBase > decimal.Zero)
                        {
                            shoppingCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemDiscountBase, _workContext.WorkingCurrency);
                        }
                    }

                    model.cart.Add(new CartListModel()
                    {
                        image = _pictureService.GetPictureUrl(picture, pictureSize),
                        name = item.Product.Name,
                        id = item.ProductId,
                        price = shoppingCartItemSubTotalWithDiscount,
                        unit_price = shoppingCartUnitPriceWithDiscount,
                        qty = item.Quantity,
                        commission = shoppingCartItemSubTotalWithDiscount * GetCommissionRate(item.Product) / 100,
                        product_url = new Uri(new Uri(baseUrl), item.Product.GetSeName()).AbsoluteUri
                    });
                }
            }

            if (model.cart.Count > 0 && subTotalDiscountAmount > decimal.Zero)
            {
                model.cart_options.discount_details.discount_amount = subTotalDiscountAmount;

                //model.cart.Add(
                //    new CartListModel
                //    {
                //        image = "",
                //        name = "SubTotal Discount",
                //        id = -1,
                //        price = -subTotalDiscountAmount,
                //        unit_price = -subTotalDiscountAmount,
                //        qty = 1,
                //        commission = 0,
                //        product_url = ""
                //    }
                //);
            }

            return model;
        }

        private bool IsVendorRestricted(Product product)
        {
            if (product == null)
                throw new Exception("Product not found.");

            var vendors = _esConfigureService.GetRestrictedVendors();

            if (vendors == null)
                return true;

            return vendors.Contains(product.VendorId);
        }

        public bool IsVendorRestricted(int productId)
        {
            try
            {
                var product = _productService.GetProductById(productId);
                return IsVendorRestricted(product);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return false;
            }
        }

        public decimal GetCommissionRate(int productId)
        {
            try
            {
                var product = _productService.GetProductById(productId);
                return GetCommissionRate(product);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return 0;
            }
        }

        private decimal GetCommissionRate(Product product)
        {
            var rates = _udcCommissionRateRepository.Table;
            var vendorRate = rates.FirstOrDefault(x => x.Type == EntityType.Vendor &&
                x.EntityId == product.VendorId);

            if (vendorRate != null)
                return vendorRate.CommissionRate;

            foreach (var category in product.ProductCategories)
            {
                var ids = new List<int>();
                ids.Add(category.CategoryId);
                GetParentCategory(category.CategoryId, ids);
                if (rates.Any(x => ids.Contains(x.EntityId) && x.Type == EntityType.Category))
                {
                    return rates.FirstOrDefault(x => ids.Contains(x.EntityId) &&
                        x.Type == EntityType.Category).CommissionRate;
                }
            }

            return _ekShopA2ISettings.UdcCommission;
        }

        #endregion

        #region Placeorder

        public ResponseModel PlaceEkShopOrder(PlaceOrderRootModel model)
        {
            if (_ekShopA2ISettings.EnableLog)
                _logger.Information("place ekshop order called");

            var responseModel = new ResponseModel();
            try
            {
                var orderModel = JsonConvert.DeserializeObject<PlaceOrderModel>(model.ekom_order);

                if (_ekShopA2ISettings.EnableLog)
                    _logger.Information("order model: " + model.ekom_order);

                if (orderModel != null)
                {
                    var totalCommission = decimal.Zero;

                    var processPaymentRequest = new ProcessPaymentRequest()
                    {
                        CustomerId = orderModel.order.cart_options.cart_id,
                        PaymentMethodSystemName = "EkShop.Manual",
                        StoreId = _storeContext.CurrentStore.Id
                    };

                    var result = PlaceOrder(processPaymentRequest, orderModel, out totalCommission);

                    var esOrder = new EsOrder()
                    {
                        DeliveryCharge = orderModel.order.lp_delivery_charge,
                        DeliveryDuration = orderModel.order.delivery_duration,
                        LpCode = orderModel.order.lp_code,
                        LpContactNumber = orderModel.order.lp_contact_number,
                        LpContactPerson = orderModel.order.lp_contact_person,
                        LpLocation = orderModel.order.lp_location,
                        LpName = orderModel.order.lp_name,
                        OrderCode = orderModel.order.order_code,
                        OtherRequiredData = orderModel.order.cart_options.other_required_data,
                        OrderId = result.PlacedOrder.Id,
                        PaymentMethod = orderModel.order.payment_method,
                        Total = orderModel.order.total,
                        CreatedOn = DateTime.UtcNow,
                        UdcCommission = totalCommission
                    };
                    _esOrderRepo.Insert(esOrder);

                    responseModel.meta.status = 200;
                    responseModel.response.order_id = result.PlacedOrder.Id;
                }
                else
                {
                    responseModel.response.message = "Post model is empty.";
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                responseModel.response.message = ex.Message;
            }

            if (_ekShopA2ISettings.EnableLog)
                _logger.Information("response status: " + responseModel.meta.status);

            return responseModel;
        }

        public virtual PlaceOrderResult PlaceOrder(ProcessPaymentRequest processPaymentRequest,
            PlaceOrderModel orderModel, out decimal totalCommission)
        {
            if (_ekShopA2ISettings.EnableLog)
                _logger.Information("placing order");

            totalCommission = 0;
            if (processPaymentRequest == null)
                throw new ArgumentNullException("processPaymentRequest");

            var result = new PlaceOrderResult();
            try
            {
                if (processPaymentRequest.OrderGuid == Guid.Empty)
                    processPaymentRequest.OrderGuid = Guid.NewGuid();

                //prepare order details
                var details = PreparePlaceOrderDetails(processPaymentRequest, orderModel);

                #region Save order details

                var order = SaveOrderDetails(processPaymentRequest, details);
                result.PlacedOrder = order;
                totalCommission = decimal.Zero;

                //move shopping cart items to order items
                foreach (var sc in details.Cart)
                {
                    //prices
                    decimal taxRate;
                    List<Discount> scDiscounts;
                    decimal discountAmount;
                    var scUnitPrice = _priceCalculationService.GetUnitPrice(sc);
                    var scSubTotal = _priceCalculationService.GetSubTotal(sc, true, out discountAmount, out scDiscounts);
                    var scUnitPriceInclTax = _taxService.GetProductPrice(sc.Product, scUnitPrice, true, details.UdcCustomer, out taxRate);
                    var scUnitPriceExclTax = _taxService.GetProductPrice(sc.Product, scUnitPrice, false, details.UdcCustomer, out taxRate);
                    var scSubTotalInclTax = _taxService.GetProductPrice(sc.Product, scSubTotal, true, details.UdcCustomer, out taxRate);
                    var scSubTotalExclTax = _taxService.GetProductPrice(sc.Product, scSubTotal, false, details.UdcCustomer, out taxRate);
                    var discountAmountInclTax = _taxService.GetProductPrice(sc.Product, discountAmount, true, details.UdcCustomer, out taxRate);
                    var discountAmountExclTax = _taxService.GetProductPrice(sc.Product, discountAmount, false, details.UdcCustomer, out taxRate);
                    foreach (var disc in scDiscounts)
                        if (!details.AppliedDiscounts.ContainsDiscount(disc))
                            details.AppliedDiscounts.Add(disc);

                    //attributes
                    var attributeDescription = _productAttributeFormatter.FormatAttributes(sc.Product, sc.AttributesXml, details.UdcCustomer);

                    var itemWeight = _shippingService.GetShoppingCartItemWeight(sc);

                    totalCommission += scSubTotalInclTax * GetCommissionRate(sc.ProductId) / 100;

                    //save order item
                    var orderItem = new OrderItem
                    {
                        OrderItemGuid = Guid.NewGuid(),
                        Order = order,
                        ProductId = sc.ProductId,
                        UnitPriceInclTax = scUnitPriceInclTax,
                        UnitPriceExclTax = scUnitPriceExclTax,
                        PriceInclTax = scSubTotalInclTax,
                        PriceExclTax = scSubTotalExclTax,
                        OriginalProductCost = _priceCalculationService.GetProductCost(sc.Product, sc.AttributesXml),
                        AttributeDescription = attributeDescription,
                        AttributesXml = sc.AttributesXml,
                        Quantity = sc.Quantity,
                        DiscountAmountInclTax = discountAmountInclTax,
                        DiscountAmountExclTax = discountAmountExclTax,
                        DownloadCount = 0,
                        IsDownloadActivated = false,
                        LicenseDownloadId = 0,
                        ItemWeight = itemWeight,
                        RentalStartDateUtc = sc.RentalStartDateUtc,
                        RentalEndDateUtc = sc.RentalEndDateUtc
                    };

                    order.OrderItems.Add(orderItem);
                    _orderService.UpdateOrder(order);

                    //gift cards
                    if (sc.Product.IsGiftCard)
                    {
                        string giftCardRecipientName;
                        string giftCardRecipientEmail;
                        string giftCardSenderName;
                        string giftCardSenderEmail;
                        string giftCardMessage;
                        _productAttributeParser.GetGiftCardAttribute(sc.AttributesXml, out giftCardRecipientName,
                            out giftCardRecipientEmail, out giftCardSenderName, out giftCardSenderEmail, out giftCardMessage);

                        for (var i = 0; i < sc.Quantity; i++)
                        {
                            _giftCardService.InsertGiftCard(new GiftCard
                            {
                                GiftCardType = sc.Product.GiftCardType,
                                PurchasedWithOrderItem = orderItem,
                                Amount = sc.Product.OverriddenGiftCardAmount.HasValue ? sc.Product.OverriddenGiftCardAmount.Value : scUnitPriceExclTax,
                                IsGiftCardActivated = false,
                                GiftCardCouponCode = _giftCardService.GenerateGiftCardCode(),
                                RecipientName = giftCardRecipientName,
                                RecipientEmail = giftCardRecipientEmail,
                                SenderName = giftCardSenderName,
                                SenderEmail = giftCardSenderEmail,
                                Message = giftCardMessage,
                                IsRecipientNotified = false,
                                CreatedOnUtc = DateTime.UtcNow
                            });
                        }
                    }

                    //inventory
                    _productService.AdjustInventory(sc.Product, -sc.Quantity, sc.AttributesXml);
                }

                //clear shopping cart
                details.Cart.ToList().ForEach(sci => _shoppingCartService.DeleteShoppingCartItem(sci, false));

                //discount usage history
                foreach (var discount in details.AppliedDiscounts)
                {
                    _discountService.InsertDiscountUsageHistory(new DiscountUsageHistory
                    {
                        Discount = discount,
                        Order = order,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                }

                //gift card usage history
                if (details.AppliedGiftCards != null)
                    foreach (var agc in details.AppliedGiftCards)
                    {
                        agc.GiftCard.GiftCardUsageHistory.Add(new GiftCardUsageHistory
                        {
                            GiftCard = agc.GiftCard,
                            UsedWithOrder = order,
                            UsedValue = agc.AmountCanBeUsed,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                        _giftCardService.UpdateGiftCard(agc.GiftCard);
                    }

                #endregion

                //reset checkout data
                _customerService.ResetCheckoutData(details.UdcCustomer, processPaymentRequest.StoreId, clearCouponCodes: true, clearCheckoutAttributes: true);
                _customerActivityService.InsertActivity("PublicStore.PlaceOrder", _localizationService.GetResource("ActivityLog.PublicStore.PlaceOrder"), order.Id);

                _genericAttributeService.SaveAttribute<ShippingOption>(details.UdcCustomer, SystemCustomerAttributeNames.EkshopBaseUrl, null);
                _genericAttributeService.SaveAttribute<ShippingOption>(details.UdcCustomer, SystemCustomerAttributeNames.EkshopSessionToken, null);

                //raise event       
                _eventPublisher.Publish(new OrderPlacedEvent(order));
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc);
                result.AddError(exc.Message);
            }

            #region Process errors

            if (!result.Success)
            {
                //log errors
                var logError = result.Errors.Aggregate("Error while placing order. ",
                    (current, next) => string.Format("{0}Error {1}: {2}. ", current, result.Errors.IndexOf(next) + 1, next));
                var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
                _logger.Error(logError, customer: customer);
            }

            #endregion

            return result;
        }

        protected virtual PlaceOrderContainter PreparePlaceOrderDetails(ProcessPaymentRequest processPaymentRequest, PlaceOrderModel rootModel)
        {
            if (_ekShopA2ISettings.EnableLog)
                _logger.Information("preparing order details model");

            var details = new PlaceOrderContainter();

            //customer
            details.UdcCustomer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            details.A2iCustomer = _esConfigureService.GetA2iCustomer(_ekShopA2ISettings.ApiKey);
            if (details.UdcCustomer == null)
                throw new ArgumentException("Customer is not set");

            //customer currency
            var currencyTmp = _currencyService.GetCurrencyById(
                details.A2iCustomer.GetAttribute<int>(SystemCustomerAttributeNames.CurrencyId, processPaymentRequest.StoreId));
            var customerCurrency = (currencyTmp != null && currencyTmp.Published) ? currencyTmp : _workContext.WorkingCurrency;
            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            details.CustomerCurrencyCode = customerCurrency.CurrencyCode;
            details.CustomerCurrencyRate = customerCurrency.Rate / primaryStoreCurrency.Rate;

            //customer language
            details.CustomerLanguage = _languageService.GetLanguageById(
                details.A2iCustomer.GetAttribute<int>(SystemCustomerAttributeNames.LanguageId, processPaymentRequest.StoreId));
            if (details.CustomerLanguage == null || !details.CustomerLanguage.Published)
                details.CustomerLanguage = _workContext.WorkingLanguage;

            var address = new Address()
            {
                Country = _countryService.GetCountryByTwoLetterIsoCode("BD"),
                City = rootModel.order.recipient_district,
                Email = rootModel.order.recipient_email,
                FirstName = rootModel.order.recipient_name,
                PhoneNumber = rootModel.order.recipient_mobile,
                Address1 = rootModel.order.recipient_union + ", " + rootModel.order.recipient_upazila + ", " + rootModel.order.recipient_division,
                CreatedOnUtc = DateTime.UtcNow
            };

            details.BillingAddress = address;
            details.ShippingAddress = address;

            //checkout attributes
            details.CheckoutAttributesXml = details.A2iCustomer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, processPaymentRequest.StoreId);
            details.CheckoutAttributeDescription = _checkoutAttributeFormatter.FormatAttributes(details.CheckoutAttributesXml, details.A2iCustomer);

            //load shopping cart
            details.Cart = details.UdcCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(processPaymentRequest.StoreId).ToList();

            if (!details.Cart.Any())
                throw new NopException("Cart is empty");

            //validate the entire shopping cart
            var warnings = _shoppingCartService.GetShoppingCartWarnings(details.Cart, details.CheckoutAttributesXml, true);
            if (warnings.Any())
                throw new NopException(warnings.Aggregate(string.Empty, (current, next) => string.Format("{0}{1};", current, next)));

            //validate individual cart items
            //var isConfident = false;

            foreach (var sci in details.Cart)
            {
                var sciWarnings = _shoppingCartService.GetShoppingCartItemWarnings(details.UdcCustomer,
                    sci.ShoppingCartType, sci.Product, processPaymentRequest.StoreId, sci.AttributesXml,
                    sci.CustomerEnteredPrice, sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity,
                    false, true);

                if (sciWarnings.Any())
                    throw new NopException(sciWarnings.Aggregate(string.Empty, (current, next) => string.Format("{0}{1};", current, next)));
            }
            //details.Confident = isConfident;

            //min totals validation
            if (!ValidateMinOrderSubtotalAmount(details.Cart))
            {
                var minOrderSubtotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderSubtotalAmount, _workContext.WorkingCurrency);
                throw new NopException(string.Format(_localizationService.GetResource("Checkout.MinOrderSubtotalAmount"),
                    _priceFormatter.FormatPrice(minOrderSubtotalAmount, true, false)));
            }

            if (!ValidateMinOrderTotalAmount(details.Cart))
            {
                var minOrderTotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderTotalAmount, _workContext.WorkingCurrency);
                throw new NopException(string.Format(_localizationService.GetResource("Checkout.MinOrderTotalAmount"),
                    _priceFormatter.FormatPrice(minOrderTotalAmount, true, false)));
            }

            //tax display type
            if (_taxSettings.AllowCustomersToSelectTaxDisplayType)
                details.CustomerTaxDisplayType = (TaxDisplayType)details.A2iCustomer.GetAttribute<int>(SystemCustomerAttributeNames.TaxDisplayTypeId, processPaymentRequest.StoreId);
            else
                details.CustomerTaxDisplayType = _taxSettings.TaxDisplayType;

            //sub total (incl tax)
            decimal orderSubTotalDiscountAmount;
            List<Discount> orderSubTotalAppliedDiscounts;
            decimal subTotalWithoutDiscountBase;
            decimal subTotalWithDiscountBase;
            _orderTotalCalculationService.GetShoppingCartSubTotal(details.Cart, true, out orderSubTotalDiscountAmount,
                out orderSubTotalAppliedDiscounts, out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
            details.OrderSubTotalInclTax = subTotalWithoutDiscountBase;
            details.OrderSubTotalDiscountInclTax = orderSubTotalDiscountAmount;

            //discount history
            foreach (var disc in orderSubTotalAppliedDiscounts)
                if (!details.AppliedDiscounts.ContainsDiscount(disc))
                    details.AppliedDiscounts.Add(disc);

            //sub total (excl tax)
            _orderTotalCalculationService.GetShoppingCartSubTotal(details.Cart, false, out orderSubTotalDiscountAmount,
                out orderSubTotalAppliedDiscounts, out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
            details.OrderSubTotalExclTax = subTotalWithoutDiscountBase;
            details.OrderSubTotalDiscountExclTax = orderSubTotalDiscountAmount;

            //shipping info
            details.ShippingStatus = ShippingStatus.NotYetShipped;

            //shipping total
            if (_ekShopA2ISettings.EnableFreeShipping && _ekShopA2ISettings.MinimumCartValue <= details.OrderSubTotalInclTax)
            {
                details.OrderShippingTotalInclTax = 0;
                details.OrderShippingTotalExclTax = 0;
            }
            else
            {
                details.OrderShippingTotalInclTax = _ekShopA2ISettings.ShippingCharge;
                details.OrderShippingTotalExclTax = _ekShopA2ISettings.ShippingCharge;
            }

            //payment total
            var paymentAdditionalFee = _paymentService.GetAdditionalHandlingFee(details.Cart, processPaymentRequest.PaymentMethodSystemName);
            details.PaymentAdditionalFeeInclTax = _taxService.GetPaymentMethodAdditionalFee(paymentAdditionalFee, true, details.A2iCustomer);
            details.PaymentAdditionalFeeExclTax = _taxService.GetPaymentMethodAdditionalFee(paymentAdditionalFee, false, details.A2iCustomer);

            //tax amount
            SortedDictionary<decimal, decimal> taxRatesDictionary;
            details.OrderTaxTotal = _orderTotalCalculationService.GetTaxTotal(details.Cart, out taxRatesDictionary);

            //VAT number
            var customerVatStatus = (VatNumberStatus)details.A2iCustomer.GetAttribute<int>(SystemCustomerAttributeNames.VatNumberStatusId);
            if (_taxSettings.EuVatEnabled && customerVatStatus == VatNumberStatus.Valid)
                details.VatNumber = details.A2iCustomer.GetAttribute<string>(SystemCustomerAttributeNames.VatNumber);

            //tax rates
            details.TaxRates = taxRatesDictionary.Aggregate(string.Empty, (current, next) =>
                string.Format("{0}{1}:{2};   ", current, next.Key.ToString(CultureInfo.InvariantCulture), next.Value.ToString(CultureInfo.InvariantCulture)));

            var orderTotal = details.OrderSubTotalInclTax + details.OrderShippingTotalInclTax;

            details.OrderTotal = orderTotal;

            processPaymentRequest.OrderTotal = details.OrderTotal;

            //recurring or standard shopping cart?
            details.IsRecurringShoppingCart = details.Cart.IsRecurring();
            if (details.IsRecurringShoppingCart)
            {
                int recurringCycleLength;
                RecurringProductCyclePeriod recurringCyclePeriod;
                int recurringTotalCycles;
                var recurringCyclesError = details.Cart.GetRecurringCycleInfo(_localizationService,
                    out recurringCycleLength, out recurringCyclePeriod, out recurringTotalCycles);
                if (!string.IsNullOrEmpty(recurringCyclesError))
                    throw new NopException(recurringCyclesError);

                processPaymentRequest.RecurringCycleLength = recurringCycleLength;
                processPaymentRequest.RecurringCyclePeriod = recurringCyclePeriod;
                processPaymentRequest.RecurringTotalCycles = recurringTotalCycles;
            }

            return details;
        }

        protected virtual Order SaveOrderDetails(ProcessPaymentRequest processPaymentRequest, PlaceOrderContainter details)
        {
            var order = new Order
            {
                StoreId = processPaymentRequest.StoreId,
                OrderGuid = processPaymentRequest.OrderGuid,
                CustomerId = details.A2iCustomer.Id,
                CustomerLanguageId = details.CustomerLanguage.Id,
                CustomerTaxDisplayType = details.CustomerTaxDisplayType,
                CustomerIp = _webHelper.GetCurrentIpAddress(),
                OrderSubtotalInclTax = details.OrderSubTotalInclTax,
                OrderSubtotalExclTax = details.OrderSubTotalExclTax,
                OrderSubTotalDiscountInclTax = details.OrderSubTotalDiscountInclTax,
                OrderSubTotalDiscountExclTax = details.OrderSubTotalDiscountExclTax,
                OrderShippingInclTax = details.OrderShippingTotalInclTax,
                OrderShippingExclTax = details.OrderShippingTotalExclTax,
                PaymentMethodAdditionalFeeInclTax = details.PaymentAdditionalFeeInclTax,
                PaymentMethodAdditionalFeeExclTax = details.PaymentAdditionalFeeExclTax,
                TaxRates = details.TaxRates,
                OrderTax = details.OrderTaxTotal,
                OrderTotal = details.OrderTotal,
                RefundedAmount = decimal.Zero,
                OrderDiscount = details.OrderDiscountAmount,
                CheckoutAttributeDescription = details.CheckoutAttributeDescription,
                CheckoutAttributesXml = details.CheckoutAttributesXml,
                CustomerCurrencyCode = details.CustomerCurrencyCode,
                CurrencyRate = details.CustomerCurrencyRate,
                AffiliateId = details.AffiliateId,
                OrderStatus = OrderStatus.Pending,
                AllowStoringCreditCardNumber = false,
                PaymentMethodSystemName = processPaymentRequest.PaymentMethodSystemName,
                PaymentStatus = PaymentStatus.Pending,
                PaidDateUtc = null,
                BillingAddress = details.BillingAddress,
                ShippingStatus = details.ShippingStatus,
                ShippingMethod = details.ShippingMethodName,
                PickUpInStore = details.PickUpInStore,
                PickupAddress = details.PickupAddress,
                ShippingRateComputationMethodSystemName = details.ShippingRateComputationMethodSystemName,
                CustomValuesXml = processPaymentRequest.SerializeCustomValues(),
                VatNumber = details.VatNumber,
                CreatedOnUtc = DateTime.UtcNow,
                UserAgentTypeId = (int)UserAgentType.Ekshop,
                ShippingAddress = details.ShippingAddress
            };
            _orderService.InsertOrder(order);

            //reward points history
            if (details.RedeemedRewardPointsAmount > decimal.Zero)
            {
                _rewardPointService.AddRewardPointsHistoryEntry(details.UdcCustomer, -details.RedeemedRewardPoints, order.StoreId,
                    string.Format(_localizationService.GetResource("RewardPoints.Message.RedeemedForOrder", order.CustomerLanguageId), order.Id),
                    order, details.RedeemedRewardPointsAmount);
                _customerService.UpdateCustomer(details.UdcCustomer);
            }

            return order;
        }

        public bool ValidateMinOrderSubtotalAmount(IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            //min order amount sub-total validation
            if (cart.Any() && _orderSettings.MinOrderSubtotalAmount > decimal.Zero)
            {
                //subtotal
                decimal orderSubTotalDiscountAmountBase;
                List<Discount> orderSubTotalAppliedDiscounts;
                decimal subTotalWithoutDiscountBase;
                decimal subTotalWithDiscountBase;
                _orderTotalCalculationService.GetShoppingCartSubTotal(cart, _orderSettings.MinOrderSubtotalAmountIncludingTax,
                    out orderSubTotalDiscountAmountBase, out orderSubTotalAppliedDiscounts,
                    out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);

                if (subTotalWithoutDiscountBase < _orderSettings.MinOrderSubtotalAmount)
                    return false;
            }

            return true;
        }

        public virtual bool ValidateMinOrderTotalAmount(IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (cart.Any() && _orderSettings.MinOrderTotalAmount > decimal.Zero)
            {
                decimal? shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotal(cart);
                if (shoppingCartTotalBase.HasValue && shoppingCartTotalBase.Value < _orderSettings.MinOrderTotalAmount)
                    return false;
            }

            return true;
        }

        #endregion

        #endregion
    }
}
