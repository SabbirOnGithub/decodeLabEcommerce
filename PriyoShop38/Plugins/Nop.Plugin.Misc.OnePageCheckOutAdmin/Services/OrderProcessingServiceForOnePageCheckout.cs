using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;

using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.Misc.OnePageCheckOutAdmin.Services;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;

using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Services.Vendors;

namespace Nop.Plugin.Misc.OnePageCheckOutAdmin.Orders
{
    /// <summary>
    /// Order processing service
    /// </summary>
    public partial class OrderProcessingServiceForOnePageCheckout : OrderProcessingService, IOrderProcessingServiceOnePageCheckOut
    {
        #region Fields

        private readonly IOrderService _orderService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly IProductService _productService;
        private readonly IPaymentService _paymentService;
        private readonly ILogger _logger;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IGiftCardService _giftCardService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private readonly IShippingService _shippingService;
        private readonly IShipmentService _shipmentService;
        private readonly ITaxService _taxService;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IEncryptionService _encryptionService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IVendorService _vendorService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICurrencyService _currencyService;
        private readonly IAffiliateService _affiliateService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPdfService _pdfService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICountryService _countryService;
        private readonly IStoreContext _storeContext;        
        private readonly ShippingSettings _shippingSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly OrderSettings _orderSettings;
        private readonly TaxSettings _taxSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="orderService">Order service</param>
        /// <param name="webHelper">Web helper</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="languageService">Language service</param>
        /// <param name="productService">Product service</param>
        /// <param name="paymentService">Payment service</param>
        /// <param name="logger">Logger</param>
        /// <param name="orderTotalCalculationService">Order total calculationservice</param>
        /// <param name="priceCalculationService">Price calculation service</param>
        /// <param name="priceFormatter">Price formatter</param>
        /// <param name="productAttributeParser">Product attribute parser</param>
        /// <param name="productAttributeFormatter">Product attribute formatter</param>
        /// <param name="giftCardService">Gift card service</param>
        /// <param name="shoppingCartService">Shopping cart service</param>
        /// <param name="checkoutAttributeFormatter">Checkout attribute service</param>
        /// <param name="shippingService">Shipping service</param>
        /// <param name="shipmentService">Shipment service</param>
        /// <param name="taxService">Tax service</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="discountService">Discount service</param>
        /// <param name="encryptionService">Encryption service</param>
        /// <param name="workContext">Work context</param>
        /// <param name="workflowMessageService">Workflow message service</param>
        /// <param name="vendorService">Vendor service</param>
        /// <param name="customerActivityService">Customer activity service</param>
        /// <param name="currencyService">Currency service</param>
        /// <param name="affiliateService">Affiliate service</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="pdfService">PDF service</param>
        /// <param name="rewardPointService">Reward point service</param>
        /// <param name="genericAttributeService">Generic attribute service</param>
        /// <param name="countryService">Country service</param>
        /// <param name="paymentSettings">Payment settings</param>
        /// <param name="shippingSettings">Shipping settings</param>
        /// <param name="rewardPointsSettings">Reward points settings</param>
        /// <param name="orderSettings">Order settings</param>
        /// <param name="taxSettings">Tax settings</param>
        /// <param name="localizationSettings">Localization settings</param>
        /// <param name="currencySettings">Currency settings</param>
        ///  <param name="storeContext">Store Context</param>
        public OrderProcessingServiceForOnePageCheckout(IOrderService orderService,
            IWebHelper webHelper,
            ILocalizationService localizationService,
            ILanguageService languageService,
            IProductService productService,
            IPaymentService paymentService,
            ILogger logger,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeFormatter productAttributeFormatter,
            IGiftCardService giftCardService,
            IShoppingCartService shoppingCartService,
            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            IShippingService shippingService,
            IShipmentService shipmentService,
            ITaxService taxService,
            ICustomerService customerService,
            IDiscountService discountService,
            IEncryptionService encryptionService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            IVendorService vendorService,
            ICustomerActivityService customerActivityService,
            ICurrencyService currencyService,
            IAffiliateService affiliateService,
            IEventPublisher eventPublisher,
            IPdfService pdfService,
            IRewardPointService rewardPointService,
            IGenericAttributeService genericAttributeService,
            ICountryService countryService,
            ShippingSettings shippingSettings,
            PaymentSettings paymentSettings,
            RewardPointsSettings rewardPointsSettings,
            OrderSettings orderSettings,
            TaxSettings taxSettings,
            LocalizationSettings localizationSettings,
            CurrencySettings currencySettings)
            : base(orderService,
             webHelper,
             localizationService,
             languageService,
             productService,
             paymentService,
             logger,
             orderTotalCalculationService,
             priceCalculationService,
             priceFormatter,
             productAttributeParser,
             productAttributeFormatter,
             giftCardService,
             shoppingCartService,
             checkoutAttributeFormatter,
             shippingService,
             shipmentService,
             taxService,
             customerService,
             discountService,
             encryptionService,
             workContext,
             workflowMessageService,
             vendorService,
             customerActivityService,
             currencyService,
             affiliateService,
             eventPublisher,
             pdfService,
             rewardPointService,
             genericAttributeService,
             countryService,
             shippingSettings,
             paymentSettings,
             rewardPointsSettings,
             orderSettings,
             taxSettings,
             localizationSettings,
             currencySettings
            )
        {
            this._orderService = orderService;
            this._webHelper = webHelper;
            this._localizationService = localizationService;
            this._languageService = languageService;
            this._productService = productService;
            this._paymentService = paymentService;
            this._logger = logger;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._productAttributeParser = productAttributeParser;
            this._productAttributeFormatter = productAttributeFormatter;
            this._giftCardService = giftCardService;
            this._shoppingCartService = shoppingCartService;
            this._checkoutAttributeFormatter = checkoutAttributeFormatter;
            this._workContext = workContext;
            this._workflowMessageService = workflowMessageService;
            this._vendorService = vendorService;
            this._shippingService = shippingService;
            this._shipmentService = shipmentService;
            this._taxService = taxService;
            this._customerService = customerService;
            this._discountService = discountService;
            this._encryptionService = encryptionService;
            this._customerActivityService = customerActivityService;
            this._currencyService = currencyService;
            this._affiliateService = affiliateService;
            this._eventPublisher = eventPublisher;
            this._pdfService = pdfService;
            this._rewardPointService = rewardPointService;
            this._genericAttributeService = genericAttributeService;
            this._countryService = countryService;

            this._paymentSettings = paymentSettings;
            this._shippingSettings = shippingSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._orderSettings = orderSettings;
            this._taxSettings = taxSettings;
            this._localizationSettings = localizationSettings;
            this._currencySettings = currencySettings;
            //this._storeContext = storeContext;
            
            //this._settingService = settingService;
        }

        #endregion


        Order SaveOrderDetailsOnepAgeCheckOut(ProcessPaymentRequest processPaymentRequest,
            ProcessPaymentResult processPaymentResult, PlaceOrderContainter details,Customer wcustomer)
        {
            var order = new Order
            {
                StoreId = processPaymentRequest.StoreId,
                OrderGuid = processPaymentRequest.OrderGuid,
                CustomerId = details.Customer.Id,
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
                AllowStoringCreditCardNumber = processPaymentResult.AllowStoringCreditCardNumber,
                CardType = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardType) : string.Empty,
                CardName = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardName) : string.Empty,
                CardNumber = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardNumber) : string.Empty,
                MaskedCreditCardNumber = _encryptionService.EncryptText(_paymentService.GetMaskedCreditCardNumber(processPaymentRequest.CreditCardNumber)),
                CardCvv2 = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardCvv2) : string.Empty,
                CardExpirationMonth = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardExpireMonth.ToString()) : string.Empty,
                CardExpirationYear = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardExpireYear.ToString()) : string.Empty,
                PaymentMethodSystemName = processPaymentRequest.PaymentMethodSystemName,
                AuthorizationTransactionId = processPaymentResult.AuthorizationTransactionId,
                AuthorizationTransactionCode = processPaymentResult.AuthorizationTransactionCode,
                AuthorizationTransactionResult = processPaymentResult.AuthorizationTransactionResult,
                CaptureTransactionId = processPaymentResult.CaptureTransactionId,
                CaptureTransactionResult = processPaymentResult.CaptureTransactionResult,
                SubscriptionTransactionId = processPaymentResult.SubscriptionTransactionId,
                PaymentStatus = processPaymentResult.NewPaymentStatus,
                PaidDateUtc = null,
                BillingAddress = details.BillingAddress,
                ShippingAddress = details.ShippingAddress,
                ShippingStatus = details.ShippingStatus,
                ShippingMethod = details.ShippingMethodName,
                PickUpInStore = details.PickUpInStore,
                PickupAddress = details.PickupAddress,
                ShippingRateComputationMethodSystemName = details.ShippingRateComputationMethodSystemName,
                CustomValuesXml = processPaymentRequest.SerializeCustomValues(),
                VatNumber = details.VatNumber,
                CreatedOnUtc = DateTime.UtcNow
            };
            _orderService.InsertOrder(order);

            
            #region saved money total for customer
            //bs-23 edit
            var customer = order.Customer;            
            _customerService.UpdateCustomer(customer);
            #endregion

            //reward points history
            if (details.RedeemedRewardPointsAmount > decimal.Zero)
            {
                _rewardPointService.AddRewardPointsHistoryEntry(details.Customer, -details.RedeemedRewardPoints, order.StoreId,
                    string.Format(_localizationService.GetResource("RewardPoints.Message.RedeemedForOrder", order.CustomerLanguageId), order.Id),
                    order, details.RedeemedRewardPointsAmount);
                _customerService.UpdateCustomer(details.Customer);
            }

            return order;
        }
        

        #region Methods

        public virtual PlaceOrderResult PlaceOrder(ProcessPaymentRequest processPaymentRequest,Customer wcustomer)
        {
            if (processPaymentRequest == null)
                throw new ArgumentNullException("processPaymentRequest");

            var result = new PlaceOrderResult();
            try
            {
                if (processPaymentRequest.OrderGuid == Guid.Empty)
                    processPaymentRequest.OrderGuid = Guid.NewGuid();

                //prepare order details
                var details = PreparePlaceOrderDetails(processPaymentRequest);

                #region Payment workflow


                //process payment
                ProcessPaymentResult processPaymentResult = null;
                //skip payment workflow if order total equals zero
                var skipPaymentWorkflow = details.OrderTotal == decimal.Zero;
                if (!skipPaymentWorkflow)
                {
                    var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(processPaymentRequest.PaymentMethodSystemName);
                    if (paymentMethod == null)
                        throw new NopException("Payment method couldn't be loaded");

                    //ensure that payment method is active
                    if (!paymentMethod.IsPaymentMethodActive(_paymentSettings))
                        throw new NopException("Payment method is not active");

                    if (details.IsRecurringShoppingCart)
                    {
                        //recurring cart
                        switch (_paymentService.GetRecurringPaymentType(processPaymentRequest.PaymentMethodSystemName))
                        {
                            case RecurringPaymentType.NotSupported:
                                throw new NopException("Recurring payments are not supported by selected payment method");
                            case RecurringPaymentType.Manual:
                            case RecurringPaymentType.Automatic:
                                processPaymentResult = _paymentService.ProcessRecurringPayment(processPaymentRequest);
                                break;
                            default:
                                throw new NopException("Not supported recurring payment type");
                        }
                    }
                    else
                        //standard cart
                        processPaymentResult = _paymentService.ProcessPayment(processPaymentRequest);
                }
                else
                    //payment is not required
                    processPaymentResult = new ProcessPaymentResult { NewPaymentStatus = PaymentStatus.Paid };

                if (processPaymentResult == null)
                    throw new NopException("processPaymentResult is not available");

                #endregion
                
                if (processPaymentResult.Success)
                {
                    #region Save order details

                    var order = SaveOrderDetailsOnepAgeCheckOut(processPaymentRequest, processPaymentResult, details,wcustomer);
                    result.PlacedOrder = order;
                    //bs-23 edit
                    var customer = order.Customer;
                    //move shopping cart items to order items
                    foreach (var sc in details.Cart)
                    {
                        //prices
                        decimal taxRate;
                        List<Discount> scDiscounts;
                        decimal discountAmount;
                        var scUnitPrice = _priceCalculationService.GetUnitPrice(sc);
                        var scSubTotal = _priceCalculationService.GetSubTotal(sc, true, out discountAmount, out scDiscounts);
                        var scUnitPriceInclTax = _taxService.GetProductPrice(sc.Product, scUnitPrice, true, details.Customer, out taxRate);
                        var scUnitPriceExclTax = _taxService.GetProductPrice(sc.Product, scUnitPrice, false, details.Customer, out taxRate);
                        var scSubTotalInclTax = _taxService.GetProductPrice(sc.Product, scSubTotal, true, details.Customer, out taxRate);
                        var scSubTotalExclTax = _taxService.GetProductPrice(sc.Product, scSubTotal, false, details.Customer, out taxRate);
                        var discountAmountInclTax = _taxService.GetProductPrice(sc.Product, discountAmount, true, details.Customer, out taxRate);
                        var discountAmountExclTax = _taxService.GetProductPrice(sc.Product, discountAmount, false, details.Customer, out taxRate);
                        foreach (var disc in scDiscounts)
                            if (!details.AppliedDiscounts.ContainsDiscount(disc))
                                details.AppliedDiscounts.Add(disc);

                        //attributes
                        var attributeDescription = _productAttributeFormatter.FormatAttributes(sc.Product, sc.AttributesXml, details.Customer);

                        var itemWeight = _shippingService.GetShoppingCartItemWeight(sc);

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
                    //bs-23 edit
                    _customerService.UpdateCustomer(customer);
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

                    //recurring orders
                    if (details.IsRecurringShoppingCart)
                    {
                        //create recurring payment (the first payment)
                        var rp = new RecurringPayment
                        {
                            CycleLength = processPaymentRequest.RecurringCycleLength,
                            CyclePeriod = processPaymentRequest.RecurringCyclePeriod,
                            TotalCycles = processPaymentRequest.RecurringTotalCycles,
                            StartDateUtc = DateTime.UtcNow,
                            IsActive = true,
                            CreatedOnUtc = DateTime.UtcNow,
                            InitialOrder = order,
                        };
                        _orderService.InsertRecurringPayment(rp);

                        switch (_paymentService.GetRecurringPaymentType(processPaymentRequest.PaymentMethodSystemName))
                        {
                            case RecurringPaymentType.NotSupported:
                                //not supported
                                break;
                            case RecurringPaymentType.Manual:
                                rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory
                                {
                                    RecurringPayment = rp,
                                    CreatedOnUtc = DateTime.UtcNow,
                                    OrderId = order.Id,
                                });
                                _orderService.UpdateRecurringPayment(rp);
                                break;
                            case RecurringPaymentType.Automatic:
                                //will be created later (process is automated)
                                break;
                            default:
                                break;
                        }
                    }

                    #endregion

                    //notifications
                    SendNotificationsAndSaveNotes(order);

                    //reset checkout data
                    _customerService.ResetCheckoutData(details.Customer, processPaymentRequest.StoreId, clearCouponCodes: true, clearCheckoutAttributes: true);
                    _customerActivityService.InsertActivity("PublicStore.PlaceOrder", _localizationService.GetResource("ActivityLog.PublicStore.PlaceOrder"), order.Id);

                    //check order status
                    CheckOrderStatus(order);

                    //raise event       
                    _eventPublisher.Publish(new OrderPlacedEvent(order));

                    if (order.PaymentStatus == PaymentStatus.Paid)
                        ProcessOrderPaid(order);
                }
                else
                    foreach (var paymentError in processPaymentResult.Errors)
                        result.AddError(string.Format(_localizationService.GetResource("Checkout.PaymentError"), paymentError));
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

        #endregion
    }
}
