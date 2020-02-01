using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Extensions.Authorize.Net;
using BS.Plugin.NopStation.MobileWebApi.Extensions.Paypal;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Payment;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.PayPal;
using BS.Plugin.NopStation.MobileWebApi.PluginSettings;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Checkout;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Models.Checkout;
using Nop.Web.Models.Common;
using Nop.Services.Configuration;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class CheckoutController : WebApiController
    {
        #region field

        private readonly IWorkContext _workContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ILocalizationService _localizationService;
        private readonly AddressSettings _addressSettings;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerService _customerService;
        private readonly ISettingService _settingService;
        private readonly ILogger _logger;
        private readonly IShippingService _shippingService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ShippingSettings _shippingSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly IPaymentService _paymentService;
        private readonly IWebHelper _webHelper;
        private readonly OrderSettings _orderSettings;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPluginFinder _pluginFinder;
        private readonly AuthorizeNetPaymentSettings _authorizeNetPaymentSettings;
        private readonly CurrencySettings _currencySettings;

        #endregion

        #region Ctor
        public CheckoutController(IWorkContext workContext,
            IStoreMappingService storeMappingService, IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService, IAddressAttributeFormatter addressAttributeFormatter,
            ICountryService countryService, IStateProvinceService stateProvinceService,
            ILocalizationService localizationService, AddressSettings addressSettings,
            IStoreContext storeContext, ICustomerService customerService,
            ILogger logger, IShippingService shippingService,
            IGenericAttributeService genericAttributeService, 
            IOrderTotalCalculationService orderTotalCalculationService,
            ITaxService taxService, 
            ICurrencyService currencyService,
            IPriceFormatter priceFormatter, 
            ShippingSettings shippingSettings,
            PaymentSettings paymentSettings, 
            IPaymentService paymentService,
            IWebHelper webHelper, 
            RewardPointsSettings rewardPointsSettings,
            OrderSettings orderSettings, 
            IOrderService orderService,
            IOrderProcessingService orderProcessingService, 
            IPluginFinder pluginFinder,
            AuthorizeNetPaymentSettings authorizeNetPaymentSettings,
            CurrencySettings currencySettings, IRewardPointService rewardPointService,
            ISettingService settingService)
        {
            this._workContext = workContext;
            this._storeMappingService = storeMappingService;
            this._addressAttributeParser = addressAttributeParser;
            this._addressAttributeService = addressAttributeService;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._localizationService = localizationService;
            this._addressSettings = addressSettings;
            this._storeContext = storeContext;
            this._customerService = customerService;
            this._logger = logger;
            this._shippingService = shippingService;
            this._genericAttributeService = genericAttributeService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._shippingSettings = shippingSettings;
            //this._genericAttributeServiceApi = genericAttributeServiceApi;
            this._paymentSettings = paymentSettings;
            this._paymentService = paymentService;
            this._webHelper = webHelper;
            this._rewardPointsSettings = rewardPointsSettings;
            this._orderSettings = orderSettings;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._pluginFinder = pluginFinder;
            this._authorizeNetPaymentSettings = authorizeNetPaymentSettings;
            this._currencySettings = currencySettings;
            this._rewardPointService = rewardPointService;
            _settingService = settingService;

        }
        #endregion

        #region Utility

        [NonAction]
        protected virtual CheckoutBillingAddressResponseModel PrepareBillingAddressModel(int? selectedCountryId = null,
            bool prePopulateNewAddressWithCustomerFields = false)
        {
             var model = new CheckoutBillingAddressResponseModel();
            //existing addresses
            var addresses = _workContext.CurrentCustomer.Addresses
                //allow billing
                //.Where(a => a.Country == null || a.Country.AllowsBilling)
                //enabled for the current store
                .Where(a => a.Country != null && a.Country.Published)
                //enabled for the current store
                .Where(a => a.StateProvince != null && a.StateProvince.Published)
                .ToList();
            foreach (var address in addresses)
            {
                var addressModel = new AddressModel();
                addressModel.PrepareModelApi(
                    address: address,
                    excludeProperties: false,
                    addressSettings: _addressSettings,
                    localizationService: _localizationService,
                    addressAttributeFormatter: _addressAttributeFormatter);
                model.ExistingAddresses.Add(addressModel);
            }

            //new address
            model.NewAddress.CountryId = selectedCountryId;
            model.NewAddress.PrepareModel(address:
                null,
                excludeProperties: false,
                addressSettings: _addressSettings,
                localizationService: _localizationService,
                stateProvinceService: _stateProvinceService,
                addressAttributeService: _addressAttributeService,
                addressAttributeParser: _addressAttributeParser,
                loadCountries: () => _countryService.GetAllCountriesForBilling(),
                prePopulateWithCustomerFields: prePopulateNewAddressWithCustomerFields,
                customer: _workContext.CurrentCustomer);
            return model;
        }

        [NonAction]
        protected virtual CheckoutShippingMethodResponseModel PrepareShippingMethodModel(IList<ShoppingCartItem> cart = null, Address shippingAddress = null)
        {
            var model = new CheckoutShippingMethodResponseModel();

            var getShippingOptionResponse = _shippingService
                .GetShippingOptions(cart, shippingAddress,
                "", _storeContext.CurrentStore.Id);

            if (getShippingOptionResponse.Success)
            {
                //performance optimization. cache returned shipping options.
                //we'll use them later (after a customer has selected an option).
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                                                       SystemCustomerAttributeNames.OfferedShippingOptions,
                                                       getShippingOptionResponse.ShippingOptions,
                                                       _storeContext.CurrentStore.Id);

                foreach (var shippingOption in getShippingOptionResponse.ShippingOptions)
                {
                    var soModel = new CheckoutShippingMethodResponseModel.ShippingMethodModel
                    {
                        Name = shippingOption.Name,
                        Description = shippingOption.Description,
                        ShippingRateComputationMethodSystemName = shippingOption.ShippingRateComputationMethodSystemName,
                        ShippingOption = shippingOption,
                    };

                    //adjust rate
                    List<Discount> appliedDiscounts;
                    var shippingTotal = _orderTotalCalculationService.AdjustShippingRate(
                        shippingOption.Rate, cart, out appliedDiscounts);

                    decimal rateBase = _taxService.GetShippingPrice(shippingTotal, _workContext.CurrentCustomer);
                    decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                    soModel.Fee = _priceFormatter.FormatShippingPrice(rate, true);

                    model.ShippingMethods.Add(soModel);
                }

                if (model.ShippingMethods.Count >= 1)
                {
                    var shippingOption = model.ShippingMethods.FirstOrDefault().ShippingOption;
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedShippingOption, shippingOption, _storeContext.CurrentStore.Id);
                }

                //find a selected (previously) shipping method
                var selectedShippingOption = _workContext.CurrentCustomer.GetAttribute<ShippingOption>(
                        SystemCustomerAttributeNames.SelectedShippingOption, _storeContext.CurrentStore.Id);

                if (selectedShippingOption != null)
                {
                    var shippingOptionToSelect = model.ShippingMethods.ToList()
                        .Find(so =>
                            !String.IsNullOrEmpty(so.Name) &&
                            so.Name.Equals(selectedShippingOption.Name, StringComparison.InvariantCultureIgnoreCase) &&
                            !String.IsNullOrEmpty(so.ShippingRateComputationMethodSystemName) &&
                            so.ShippingRateComputationMethodSystemName.Equals(selectedShippingOption.ShippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase));
                    if (shippingOptionToSelect != null)
                    {
                        shippingOptionToSelect.Selected = true;
                    }
                }
                //if no option has been selected, let's do it for the first one
                if (model.ShippingMethods.FirstOrDefault(so => so.Selected) == null)
                {
                    var shippingOptionToSelect = model.ShippingMethods.FirstOrDefault();
                    if (shippingOptionToSelect != null)
                    {
                        shippingOptionToSelect.Selected = true;
                    }
                }

                //notify about shipping from multiple locations
                if (_shippingSettings.NotifyCustomerAboutShippingFromMultipleLocations)
                {
                    model.NotifyCustomerAboutShippingFromMultipleLocations = getShippingOptionResponse.ShippingFromMultipleLocations;
                }
            }
            else
            {
                foreach (var error in getShippingOptionResponse.Errors)
                    model.Warnings.Add(error);
            }

            return model;
        }

        [NonAction]
        protected virtual CheckoutPaymentMethodResponseModel PreparePaymentMethodModel(IList<ShoppingCartItem> cart, int filterByCountryId)
        {
            var model = new CheckoutPaymentMethodResponseModel();

            //reward points
            if (_rewardPointsSettings.Enabled && !cart.IsRecurring())
            {
                int rewardPointsBalance = _rewardPointService.GetRewardPointsBalance(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
                decimal rewardPointsAmountBase = _orderTotalCalculationService.ConvertRewardPointsToAmount(rewardPointsBalance);
                decimal rewardPointsAmount = _currencyService.ConvertFromPrimaryStoreCurrency(rewardPointsAmountBase, _workContext.WorkingCurrency);
                if (rewardPointsAmount > decimal.Zero &&
                    _orderTotalCalculationService.CheckMinimumRewardPointsToUseRequirement(rewardPointsBalance))
                {
                    model.DisplayRewardPoints = true;
                    model.RewardPointsAmount = _priceFormatter.FormatPrice(rewardPointsAmount, true, false);
                    model.RewardPointsBalance = rewardPointsBalance;
                }
            }

            //filter by country
            var paymentMethods = _paymentService
                .LoadActivePaymentMethods(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id, filterByCountryId)
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Standard || pm.PaymentMethodType == PaymentMethodType.Redirection)
                .Where(pm => !pm.HidePaymentMethod(cart))
                .ToList();
            foreach (var pm in paymentMethods)
            {
                if (cart.IsRecurring() && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                    continue;

                var pmModel = new CheckoutPaymentMethodResponseModel.PaymentMethodModel
                {
                    Name = pm.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id),
                    PaymentMethodSystemName = pm.PluginDescriptor.SystemName,
                    LogoUrl = pm.PluginDescriptor.GetLogoUrl(_webHelper)
                };
                //payment method additional fee
                decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, pm.PluginDescriptor.SystemName);
                decimal rateBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, _workContext.CurrentCustomer);
                decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                if (rate > decimal.Zero)
                    pmModel.Fee = _priceFormatter.FormatPaymentMethodAdditionalFee(rate, true);

                #region Cod

                var paymentMethodList = new List<string>();
                paymentMethodList.Add("Payments.CashOnDelivery");
                paymentMethodList.Add("Payments.Bkash");

                if (paymentMethodList.Any(x => x.Equals(pmModel.PaymentMethodSystemName)) || pm.PaymentMethodType == PaymentMethodType.Redirection)
                {
                    model.PaymentMethods.Add(pmModel);
                }


                #endregion


            }

            //find a selected (previously) payment method
            var selectedPaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                SystemCustomerAttributeNames.SelectedPaymentMethod,
                _genericAttributeService, _storeContext.CurrentStore.Id);
            if (!String.IsNullOrEmpty(selectedPaymentMethodSystemName))
            {
                var paymentMethodToSelect = model.PaymentMethods.ToList()
                    .Find(pm => pm.PaymentMethodSystemName.Equals(selectedPaymentMethodSystemName, StringComparison.InvariantCultureIgnoreCase));
                if (paymentMethodToSelect != null)
                    paymentMethodToSelect.Selected = true;
            }
            //if no option has been selected, let's do it for the first one
            if (model.PaymentMethods.FirstOrDefault(so => so.Selected) == null)
            {
                var paymentMethodToSelect = model.PaymentMethods.FirstOrDefault();
                if (paymentMethodToSelect != null)
                    paymentMethodToSelect.Selected = true;
            }

            var paymentMethodModel = new CheckoutPaymentMethodResponseModel.PaymentMethodModel();
            paymentMethodModel.Name = "Partial Payment";
            paymentMethodModel.PaymentMethodSystemName = "Payments.PartialPayment";
            paymentMethodModel.LogoUrl = "http://localhost:15536/plugins/Payments.CashOnDelivery/logo.png";
            paymentMethodModel.Selected = false;

            model.PaymentMethods.Add(paymentMethodModel);

            return model;
        }

        [NonAction]
        protected virtual bool IsMinimumOrderPlacementIntervalValid(Customer customer)
        {
            //prevent 2 orders being placed within an X seconds time frame
            if (_orderSettings.MinimumOrderPlacementInterval == 0)
                return true;

            var lastOrder = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1)
                .FirstOrDefault();
            if (lastOrder == null)
                return true;

            var interval = DateTime.UtcNow - lastOrder.CreatedOnUtc;
            return interval.TotalSeconds > _orderSettings.MinimumOrderPlacementInterval;
        }
        
        #endregion

        #region Action Method

        [Route("api/checkout/opccheckoutforguest")]
        [HttpGet]
        public IHttpActionResult OpcCheckoutForGuest()
        {
            var model = new GeneralResponseModel<bool>();
            if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                model.Data = false;
            else
            {
                model.Data = true;
            }
            return Ok(model);
            //return PartialView("OpcBillingAddress", billingAddressModel);
        }

        [Route("api/checkout/billingform")]
        [HttpGet]
        public IHttpActionResult OpcBillingForm()
        {
            var billingAddressModel = PrepareBillingAddressModel(prePopulateNewAddressWithCustomerFields: true);
            return Ok(billingAddressModel);
            //return PartialView("OpcBillingAddress", billingAddressModel);
        }


        [Route("api/checkout/checkoutsaveadressid/{addressType}")]
        [HttpPost]
        public IHttpActionResult CheckoutSaveAddressId(int addressType, SingleValue value)
        {
            int addressId;
            int.TryParse(value.Value, out addressId);
            var result = new GeneralResponseModel<bool>();
            if (addressId > 0)
            {
                //existing address
                var address = _workContext.CurrentCustomer.Addresses.FirstOrDefault(a => a.Id == addressId);

                if (address == null)
                    throw new Exception("Address can't be loaded");

                if (address.StateProvince == null ||
                    !address.StateProvince.Published)
                {
                    _workContext.CurrentCustomer.RemoveAddress(address);
                }
                else
                {
                    _workContext.CurrentCustomer.BillingAddress = address;
                    _workContext.CurrentCustomer.ShippingAddress = address;
                }

                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                result.Data = true;

                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                        .LimitPerStore(_storeContext.CurrentStore.Id)
                        .ToList();

                PrepareShippingMethodModel(cart, _workContext.CurrentCustomer.ShippingAddress);
            }
            else
            {
                result.StatusCode = (int)ErrorType.NotOk;
                result.Data = false;
                result.ErrorList = new List<string>
                {
                    "Address can't be loaded"
                };
            }
            return Ok(result);
        }

        [Route("api/checkout/checkoutsaveadress/{addressType}")]
        [HttpPost]
        public IHttpActionResult CheckoutSaveAddress(int addressType, List<KeyValueApi> formValues)
        {
            var result = new GeneralResponseModel<bool>();
            NameValueCollection form = formValues.ToNameValueCollection();
            //////custom address attributes
            var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            var aT = (AddressType)addressType;
            string prefix = HelperExtension.GetEnumDescription((AddressType)addressType);
            Address address = form.AddressFromToModel(prefix);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }
            ValidationExtension.AddressValidator(ModelState, address, _localizationService, _addressSettings, _stateProvinceService);
            if (ModelState.IsValid)
            {
                var mainAddress = _workContext.CurrentCustomer.Addresses.ToList().FindAddress(
                    address.FirstName, address.LastName, address.PhoneNumber,
                    address.Email, address.FaxNumber, address.Company,
                    address.Address1, address.Address2, address.City,
                    address.StateProvinceId, address.ZipPostalCode,
                    address.CountryId, customAttributes);

                if (mainAddress == null)
                {
                    //address is not found. let's create a new one
                    address.CustomAttributes = customAttributes;
                    address.CreatedOnUtc = DateTime.UtcNow;
                    //some validation
                    if (address.CountryId == 0)
                        address.CountryId = null;
                    if (address.StateProvinceId == 0)
                        address.StateProvinceId = null;
                    if (address.CountryId.HasValue && address.CountryId.Value > 0)
                    {
                        address.Country = _countryService.GetCountryById(address.CountryId.Value);
                    }
                    _workContext.CurrentCustomer.Addresses.Add(address);
                }

                _workContext.CurrentCustomer.BillingAddress = mainAddress ?? address;
                _workContext.CurrentCustomer.ShippingAddress = mainAddress ?? address;

                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                result.Data = true;

                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                        .LimitPerStore(_storeContext.CurrentStore.Id)
                        .ToList();

                PrepareShippingMethodModel(cart, _workContext.CurrentCustomer.ShippingAddress);
            }
            //try to find an address with the same values (don't duplicate records)
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        result.ErrorList.Add(error.ErrorMessage);
                    }
                }

                result.Data = false;
                result.StatusCode = (int)ErrorType.NotOk;
            }
            return Ok(result);
        }

        [Route("api/checkout/checkoutgetshippingmethods")]
        [HttpGet]
        public IHttpActionResult CheckoutGetShippingMethods()
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();

            var shippingMethodModel = PrepareShippingMethodModel(cart, _workContext.CurrentCustomer.ShippingAddress);
            return Ok(shippingMethodModel);
            //return PartialView("OpcBillingAddress", billingAddressModel);
        }


        [Route("api/checkout/checkoutsetshippingmethod")]
        [HttpPost]
        public IHttpActionResult CheckoutSetShippingMethods(SingleValue value)
        {
            var result = new GeneralResponseModel<bool>();
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
            string shippingoption = value.Value;
            if (String.IsNullOrEmpty(shippingoption))
                throw new Exception("Selected shipping method can't be parsed");
            var splittedOption = shippingoption.Split(new[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
            if (splittedOption.Length != 2)
                throw new Exception("Selected shipping method can't be parsed");
            string selectedName = splittedOption[0];
            string shippingRateComputationMethodSystemName = splittedOption[1];

            //find it
            //performance optimization. try cache first
            var shippingOptions = _workContext.CurrentCustomer.GetAttribute<List<ShippingOption>>(SystemCustomerAttributeNames.OfferedShippingOptions, _storeContext.CurrentStore.Id);
            if (shippingOptions == null || shippingOptions.Count == 0)
            {
                //not found? let's load them using shipping service
                shippingOptions = _shippingService
                    .GetShippingOptions(cart, _workContext.CurrentCustomer.ShippingAddress, shippingRateComputationMethodSystemName, _storeContext.CurrentStore.Id)
                    .ShippingOptions
                    .ToList();
            }
            else
            {
                //loaded cached results. let's filter result by a chosen shipping rate computation method
                shippingOptions = shippingOptions.Where(so => so.ShippingRateComputationMethodSystemName.Equals(shippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }

            var shippingOption = shippingOptions
                .Find(so => !String.IsNullOrEmpty(so.Name) && so.Name.Equals(selectedName, StringComparison.InvariantCultureIgnoreCase));
            if (shippingOption == null)
                throw new Exception("Selected shipping method can't be loaded");

            //save
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedShippingOption, shippingOption, _storeContext.CurrentStore.Id);
            result.Data = true;
            return Ok(result);
        }

        [Route("api/checkout/checkoutgetpaymentmethod")]
        [HttpGet]
        public IHttpActionResult CheckoutGetPaymentMethods()
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                     .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                     .LimitPerStore(_storeContext.CurrentStore.Id)
                     .ToList();
            int filterByCountryId = 0;
            if (_addressSettings.CountryEnabled &&
                _workContext.CurrentCustomer.BillingAddress != null &&
                _workContext.CurrentCustomer.BillingAddress.Country != null)
            {
                filterByCountryId = _workContext.CurrentCustomer.BillingAddress.Country.Id;
            }

            //payment is required
            var paymentMethodModel = PreparePaymentMethodModel(cart, filterByCountryId);

            return Ok(paymentMethodModel);
        }

        [Route("api/checkout/checkoutsavepaymentmethod")]
        [HttpPost]
        public IHttpActionResult OpcSavePaymentMethod(SingleValue value)
        {
            try
            {
                //validation
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                if (cart.Count == 0)
                    throw new Exception("Your cart is empty");

                //if (!_orderSettings.OnePageCheckoutEnabled)
                //    throw new Exception("One page checkout is disabled");

                //if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                //    throw new Exception("Anonymous checkout is not allowed");

                string paymentmethod = value.Value;
                //payment method 
                if (String.IsNullOrEmpty(paymentmethod))
                    throw new Exception("Selected payment method can't be parsed");


                var model = new CheckoutPaymentMethodModel();
                //TryUpdateModel(model);

                //reward points
                if (_rewardPointsSettings.Enabled)
                {
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.UseRewardPointsDuringCheckout, model.UseRewardPoints,
                        _storeContext.CurrentStore.Id);
                }

                ////Check whether payment workflow is required
                //bool isPaymentWorkflowRequired = IsPaymentWorkflowRequired(cart);
                //if (!isPaymentWorkflowRequired)
                //{
                //    //payment is not required
                //    _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
                //        SystemCustomerAttributeNames.SelectedPaymentMethod, null, _storeContext.CurrentStore.Id);

                //    var confirmOrderModel = PrepareConfirmOrderModel(cart);
                //    return Json(new
                //    {
                //        update_section = new UpdateSectionJsonModel
                //        {
                //            name = "confirm-order",
                //            html = this.RenderPartialViewToString("OpcConfirmOrder", confirmOrderModel)
                //        },
                //        goto_section = "confirm_order"
                //    });
                //}

                var paymentMethodInst = _paymentService.LoadPaymentMethodBySystemName(paymentmethod);
                if (paymentMethodInst == null || !paymentMethodInst.IsPaymentMethodActive(_paymentSettings) ||
                    !_pluginFinder.AuthenticateStore(paymentMethodInst.PluginDescriptor, _storeContext.CurrentStore.Id))
                    throw new Exception("Selected payment method can't be parsed");

                //save
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                    SystemCustomerAttributeNames.SelectedPaymentMethod, paymentmethod, _storeContext.CurrentStore.Id);

                var response = new GeneralResponseModel<bool>()
                {
                    Data = true
                };

                return Ok(response);
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return Json(new { error = 1, message = exc.Message });
            }
        }

        [Route("api/checkout/checkoutcomplete")]
        [HttpGet]
        public IHttpActionResult Complete()
        {
            var result = new CompleteResponseModel();
            result.ErrorList = new List<string>();
            try
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                   .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                   .LimitPerStore(_storeContext.CurrentStore.Id)
                   .ToList();
                if (cart.Count == 0)
                    throw new Exception("Your cart is empty");

                //if (!_orderSettings.OnePageCheckoutEnabled)
                //    throw new Exception("One page checkout is disabled");

                //if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                //    throw new Exception("Anonymous checkout is not allowed");

                //prevent 2 orders being placed within an X seconds time frame
                if (!IsMinimumOrderPlacementIntervalValid(_workContext.CurrentCustomer))
                    throw new Exception(_localizationService.GetResource("Checkout.MinOrderPlacementInterval"));

                //place order

                var paymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod,
                    _genericAttributeService, _storeContext.CurrentStore.Id);

                var paymentMethodget = _paymentService.LoadPaymentMethodBySystemName(paymentMethodSystemName);
                if (paymentMethodget == null)
                    throw new Exception("Payment method is not selected");

                var processPaymentRequest = new ProcessPaymentRequest
                {
                    StoreId = _storeContext.CurrentStore.Id,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    PaymentMethodSystemName = paymentMethodSystemName
                };


                decimal walletPayAmount = 0;
                var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
                if (customer != null)
                {
                    if (customer.WalletPayAmount != null)
                    {
                        if (customer.PayFromWallet == true)
                        {
                            walletPayAmount = (decimal)customer.WalletPayAmount;
                        }

                    }
                }
                

                var placeOrderResult = _orderProcessingService.PlaceOrder(processPaymentRequest, walletPayAmount);
                if (placeOrderResult.Success)
                {

                    customer.PayFromWallet = false;
                    customer.WalletPayAmount = 0;
                    _customerService.UpdateCustomer(customer);

                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = placeOrderResult.PlacedOrder
                    };

                    var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(placeOrderResult.PlacedOrder.PaymentMethodSystemName);
                    if (paymentMethod == null)
                        //payment method could be null if order total is 0
                        //success
                        throw new Exception("OrderTotal 0");

                    // _paymentService.PostProcessPayment(postProcessPaymentRequest);
                    //success
                    //result.Data = postProcessPaymentRequest.Order.Id;
                    if (paymentMethodSystemName == "Payments.PayPalStandard")
                    {
                        result.CompleteOrder = false;
                        result.OrderId = postProcessPaymentRequest.Order.Id;
                        result.PayPal = new CompleteResponseModel.PaypalModel();
                        result.PayPal.ClientId = PayPalExtension.ClientId;
                        result.PaymentType = (int)PaymentType.PayPal;

                        var conversionRateSetting = _settingService.GetSetting("paypalstandardpaymentsettings.conversionrate");
                        if (conversionRateSetting != null)
                        {
                            decimal rate = 0;
                            result.ConversionRate = decimal.TryParse(conversionRateSetting.Value, out rate) ? rate : 80;
                        }
                        else
                            result.ConversionRate = 80;

                    }
                    else if (paymentMethodSystemName == "Payments.bKashAdvance")
                    {
                        var orderId = postProcessPaymentRequest.Order.Id;
                        var currentStore = _storeContext.CurrentStore;
                        result.CompleteOrder = false;
                        result.OrderId = orderId;
                        result.PaymentType = (int)PaymentType.BkashAdvance;
                        result.ProcessUrl = new Uri(currentStore.SslEnabled ? currentStore.SecureUrl : currentStore.Url)
                            .Append("api/bkash/pay/" + orderId)
                            .AbsoluteUri;
                    }
                    else if (paymentMethodSystemName == "Payments.AuthorizeNet")
                    {
                        result.CompleteOrder = false;
                        result.OrderId = postProcessPaymentRequest.Order.Id;
                        result.PaymentType = (int)PaymentType.AuthorizeDotNet;
                    }
                    else if (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection)
                    {
                        result.CompleteOrder = false;
                        result.OrderId = postProcessPaymentRequest.Order.Id;
                        result.PaymentType = (int)PaymentType.ReDirectType;
                    }
                    else
                    {
                        result.CompleteOrder = true;
                        result.OrderId = postProcessPaymentRequest.Order.Id;
                        result.PaymentType = (int)PaymentType.CashOnDelivery;
                    }
                    return Ok(result);
                }

                //error
                result.StatusCode = (int)ErrorType.NotOk;

                foreach (var error in placeOrderResult.Errors)
                    result.ErrorList.Add(error);

                return Ok(result);
            }
            catch (Exception e)
            {
                result.StatusCode = (int)ErrorType.NotOk;
                result.ErrorList.Add(e.Message);
                return Ok(result);
            }
        }

        [Route("api/checkout/saveWalletInformation")]
        [HttpPost]
        public IHttpActionResult SaveWalletInformation()
        {
            var result = new GeneralResponseModel<bool>();
            decimal payAmount = (decimal)0.0;
            var customerId = 0;
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["walletPayAmount"]))
            {
                payAmount = Convert.ToDecimal(HttpContext.Current.Request["walletPayAmount"]);
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["customerId"]))
            {
                customerId = Convert.ToInt32(HttpContext.Current.Request["customerId"]);
            }

            if (customerId == 0)
            {
                result.Data = false;
                result.StatusCode = 400;
                result.SuccessMessage = "customerId missing";
                return Ok(result);
            }

            
            var customer = _customerService.GetCustomerById(customerId);

            if (customer != null)
            {
                if (payAmount > (decimal)0.0)
                {
                    customer.PayFromWallet = true;
                    customer.WalletPayAmount = payAmount;
                }
                else
                {
                    customer.PayFromWallet = false;
                    customer.WalletPayAmount = payAmount;
                }

                _customerService.UpdateCustomer(customer);
                result.Data = true;
                result.StatusCode = 200;
                result.SuccessMessage = "Wallet Amount Saved Successfully";

            }
            else{
                
                    result.Data = false;
                    result.StatusCode = 400;
                    result.SuccessMessage = "customerId missing";
            }

            return Ok(result);
        }

        [Route("api/checkout/savePartialPayment")]
        [HttpPost]
        public IHttpActionResult SavePartialPayment()
        {
            var result = new GeneralResponseModel<bool>();
            var orderId = 0;
            decimal partialAmount = (decimal) 0.0;
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["orderId"]))
            {
                orderId = Convert.ToInt32(HttpContext.Current.Request["orderId"]);
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["partialAmount"]))
            {
                partialAmount = Convert.ToDecimal(HttpContext.Current.Request["partialAmount"]);
            }

            if (orderId > 0)
            {
                var order = _orderService.GetOrderById(orderId: orderId);
                if (order != null)
                {
                    order.PartialPayAmount = partialAmount;
                    _orderService.UpdateOrder(order);
                    result.Data = true;
                    result.StatusCode = 200;
                    result.SuccessMessage = "Partial Payment Done";
                }
            }
            else
            {
                result.StatusCode = 400;
            }

            return Ok(result);
        }


        [Route("api/checkout/checkpaypalaccount")]
        [HttpPost]
        public IHttpActionResult CheckPayPalAccount(PayPalResponseModel payPal)
        {
            string paymentId = payPal.PaymentId;
            int id = payPal.OrderId;
            var order = _orderService.GetOrderById(payPal.OrderId);

            var payDetail = PayPalExtension.GetAmount(paymentId);
            var result = new CompleteResponseModel();
            result.StatusCode = (int)ErrorType.NotOk;
            result.OrderId = id;
            result.CompleteOrder = false;
            if (payDetail != null)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Paypal PDT: From Mobile");
                sb.AppendLine("mc_gross: " + payDetail.Total);
                sb.AppendLine("Payment status: " + payDetail.PaymentStatus);
                sb.AppendLine("mc_currency: " + payDetail.Currency);
                sb.AppendLine("payer_id: " + payDetail.PayeeId);
                sb.AppendLine("Done From Mobile");
                decimal total = Convert.ToDecimal(payDetail.Total);
                //order note
                order.OrderNotes.Add(new OrderNote
                {
                    Note = sb.ToString(),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow,
                });

                _orderService.UpdateOrder(order);
                if (Math.Round(total).Equals(Math.Round(order.OrderTotal, 2)))
                {
                    if (_orderProcessingService.CanMarkOrderAsPaid(order))
                    {
                        order.AuthorizationTransactionId = paymentId;
                        _orderService.UpdateOrder(order);

                        _orderProcessingService.MarkOrderAsPaid(order);
                        result.StatusCode = (int)ErrorType.Ok;
                        result.CompleteOrder = true;
                    }
                    else
                    {
                        result.ErrorList.Add("Order Already Paid");
                    }
                }
                result.ErrorList.Add("Total not match");
            }
            return Ok(result);
        }


        [Route("api/checkout/checkauthorizepayment")]
        [HttpPost]
        public IHttpActionResult CheckAuthorizePayment(AuthorizeQueryModel authorizeNet)
        {
            var result = new CompleteResponseModel();
            result.StatusCode = (int)ErrorType.NotOk;
            result.OrderId = authorizeNet.OrderId;
            var order = _orderService.GetOrderById(authorizeNet.OrderId);
            result.CompleteOrder = false;
            var excuteResult = AuthorizeNetExtention.ExcuteTransaction(authorizeNet, _authorizeNetPaymentSettings, _currencyService,
                 _currencySettings, _workContext.CurrentCustomer, _orderService);
            if (excuteResult.Errors.Count == 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Authorize.net PDT: From Mobile");
                sb.AppendLine("mc_gross: " + order.OrderTotal);
                sb.AppendLine("Payment status: " + excuteResult.NewPaymentStatus);
                sb.AppendLine("payer_id: " + excuteResult.AuthorizationTransactionId);
                sb.AppendLine("Done From Mobile");
                //order note
                order.OrderNotes.Add(new OrderNote
                {
                    Note = sb.ToString(),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow,

                });
                _orderService.UpdateOrder(order);
                if (_orderProcessingService.CanMarkOrderAsPaid(order))
                {

                    order.AuthorizationTransactionId = excuteResult.AuthorizationTransactionId;
                    _orderService.UpdateOrder(order);
                    if (excuteResult.NewPaymentStatus == PaymentStatus.Authorized)
                    {
                        if (_orderProcessingService.CanMarkOrderAsAuthorized(order))
                        {
                            _orderProcessingService.MarkAsAuthorized(order);
                            result.StatusCode = (int)ErrorType.Ok;
                            result.CompleteOrder = true;
                        }
                    }
                    else if (excuteResult.NewPaymentStatus == PaymentStatus.Paid)
                    {
                        _orderProcessingService.MarkOrderAsPaid(order);
                        result.StatusCode = (int)ErrorType.Ok;
                        result.CompleteOrder = true;
                    }

                }
                else
                {
                    result.ErrorList.Add("Order Already Paid");
                }
            }
            else
            {
                result.ErrorList.AddRange(excuteResult.Errors);
            }
            return Ok(result);
        }

        #endregion
    }
}
