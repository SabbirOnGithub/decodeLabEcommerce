using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Logging;
using Nop.Plugin.Misc.OnePageCheckOutAdmin.Infrastructure.Cache;
using Nop.Plugin.Misc.OnePageCheckOutAdmin.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Orders;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Common;
using Nop.Services.Payments;
using Nop.Plugin.Misc.OnePageCheckOutAdmin.Models.Checkout;
using Nop.Plugin.Misc.OnePageCheckOutAdmin.Models.Common;
using Nop.Services.Common;
using Nop.Core.Domain.Shipping;
using Nop.Services.Shipping;
using Nop.Core.Domain.Discounts;
using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Plugin.Misc.OnePageCheckOutAdmin.Models.ShoppingCart;
using Nop.Services.Discounts;
using Nop.Core.Domain.Media;
using System.Web;
using Nop.Core.Domain.Directory;
using System.Globalization;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Payments;
using Nop.Services.Configuration;
using Nop.Plugin.Misc.OnePageCheckOutAdmin.Helper;
using Nop.Admin.Models.Catalog;
using Nop.Admin.Helpers;
using Nop.Services.Vendors;
using Nop.Services;
using Nop.Admin.Models.Customers;
using Nop.Services.Affiliates;
using Nop.Services.Helpers;
using Nop.Services.Messages;
using Nop.Services.Authentication.External;
using Nop.Core.Domain.Messages;
using Nop.Plugin.Misc.OnePageCheckOutAdmin.Extensions;
using Nop.Plugin.Misc.OnePageCheckOutAdmin.Models.ProductModel;

using Nop.Web.Framework.Kendoui;

namespace Nop.Plugin.Misc.OnePageCheckOutAdmin.Controllers
{
    [NopHttpsRequirement(SslRequirement.NoMatter)]
    public class MiscOnePageCheckOutAdminController : BasePluginController
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly ICacheManager _cacheManager;
        private readonly CatalogSettings _catalogSettings;
        private readonly ICategoryService _categoryService;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ITaxService _taxService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IBSCustomerService _bsCustomerService;
       


        //checkout
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly AddressSettings _addressSettings;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly ICountryService _countryService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly ShippingSettings _shippingSettings;
        private readonly IShippingService _shippingService;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IPaymentService _paymentService;
        private readonly IWebHelper _webHelper;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;


        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IDiscountService _discountService;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly MediaSettings _mediaSettings;
        private readonly HttpContextBase _httpContext;
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICustomerService _customerService;
        private readonly TaxSettings _taxSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly IPluginFinder _pluginFinder;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        //private readonly IOrderTotalCalculationService _orderTotalCalculationService;

        private readonly IManufacturerService _manufacturerService;
        private readonly IStoreService _storeService;
        private readonly IVendorService _vendorService;
        private readonly ICustomerActivityService _customerActivityService;

        //customer
        private readonly IAffiliateService _affiliateService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly CustomerSettings _customerSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly IAddressService _addressService;
        private readonly IOrderProcessingServiceOnePageCheckOut _orderProcessingServiceOnePageCheckOut;

        private readonly int filterByCountryId = 186;
        #endregion

        #region Ctor

        public MiscOnePageCheckOutAdminController(IAclService aclService,
            ICacheManager cacheManager,
            CatalogSettings catalogSettings,
            ICategoryService categoryService,
            ICurrencyService currencyService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IProductService productService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            ITaxService taxService,
            IRewardPointService rewardPointService,
            IWorkContext workContext,
            OrderSettings orderSettings,
            IStateProvinceService stateProvinceService,
            IOrderTotalCalculationService orderTotalCalculationService,
            AddressSettings addressSettings,
            IAddressAttributeFormatter addressAttributeFormatter,
            IAddressAttributeService addressAttributeService,
            ICountryService countryService,
            IAddressAttributeParser addressAttributeParser,
            ShippingSettings shippingSettings,
            IShippingService shippingService,
            RewardPointsSettings rewardPointsSettings,
            IGenericAttributeService genericAttributeService,
            IPaymentService paymentService,
            IWebHelper webHelper,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IProductAttributeParser productAttributeParser,
            IProductAttributeFormatter productAttributeFormatter,
            ShoppingCartSettings shoppingCartSettings,
            IDiscountService discountService,
            ICheckoutAttributeService checkoutAttributeService,
            IShoppingCartService shoppingCartService,
            MediaSettings mediaSettings,
            HttpContextBase httpContext,
            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICustomerService customerService,
            TaxSettings taxSettings,
            PaymentSettings paymentSettings,
            IPluginFinder pluginFinder,
            ILogger logger,
            ISettingService settingService, 
            IManufacturerService manufacturerService,
            IStoreService storeService,
            IVendorService vendorService,
            ICustomerActivityService customerActivityService,
            IAffiliateService affiliateService,
            IDateTimeHelper dateTimeHelper,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            CustomerSettings customerSettings,
            DateTimeSettings dateTimeSettings,
            ICustomerAttributeService customerAttributeService,
            ICustomerAttributeParser customerAttributeParser,
            IOpenAuthenticationService openAuthenticationService,
            ICustomerRegistrationService customerRegistrationService,
            IBSCustomerService bsCustomerService,
            IAddressService addressService,
            IOrderProcessingServiceOnePageCheckOut orderProcessingServiceOnePageCheckOut
            )
        {
            this._aclService = aclService;
            this._cacheManager = cacheManager;
            this._catalogSettings = catalogSettings;
            this._categoryService = categoryService;
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._pictureService = pictureService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._productService = productService;
            this._storeContext = storeContext;
            this._storeMappingService = storeMappingService;
            this._taxService = taxService;
            this._rewardPointService = rewardPointService;


            this._workContext = workContext;
            this._orderSettings = orderSettings;
            this._stateProvinceService = stateProvinceService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._addressSettings = addressSettings;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._addressAttributeService=addressAttributeService;
            this._addressAttributeParser = addressAttributeParser;
            this._shippingSettings = shippingSettings;
            this._shippingService = shippingService;
            this._rewardPointsSettings = rewardPointsSettings;
            this._genericAttributeService = genericAttributeService;
            this._paymentService = paymentService;
            this._webHelper=webHelper;
            this._orderProcessingService=orderProcessingService;
            this._orderService=orderService;
            this._countryService = countryService;

            this._productAttributeParser = productAttributeParser;
            this._productAttributeFormatter = productAttributeFormatter;
            this._shoppingCartService = shoppingCartService;
            this._discountService = discountService;
            this._checkoutAttributeService = checkoutAttributeService;
            this._mediaSettings = mediaSettings;
            this._httpContext = httpContext;
            this._checkoutAttributeFormatter = checkoutAttributeFormatter;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._shoppingCartSettings = shoppingCartSettings;
            this._customerService = customerService;
            this._taxSettings = taxSettings;
            this._paymentSettings = paymentSettings;
            this._pluginFinder = pluginFinder;
            this._logger = logger;
            this._settingService = settingService;

            this._manufacturerService = manufacturerService;
            this._storeService = storeService;
            this._vendorService = vendorService;
            this._customerActivityService = customerActivityService;

            this._affiliateService = affiliateService;
            this._dateTimeHelper = dateTimeHelper;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._customerSettings = customerSettings;
            this._dateTimeSettings = dateTimeSettings;
            this._customerAttributeService = customerAttributeService;
            this._customerAttributeParser = customerAttributeParser;
            this._openAuthenticationService = openAuthenticationService;
            this._customerRegistrationService = customerRegistrationService;
            this._bsCustomerService = bsCustomerService;

            this._addressService = addressService;
            this._orderProcessingServiceOnePageCheckOut = orderProcessingServiceOnePageCheckOut;
        }

        #endregion

        #region Utilities
        [NonAction]
        protected virtual int GetBangladeshCountryId()
        {
            var countries = _countryService.GetAllCountriesForShipping();
            var countryId = 0;
            if (countries.Count > 0)
            {
                var bangladeshCountry = countries.FirstOrDefault(c => c.Name.ToLower().Contains("bangladesh"));
                if (bangladeshCountry != null)
                    countryId = bangladeshCountry.Id;
            }

            return countryId;
        }

        #region ShippingRate added 
        [HttpPost]
        public ActionResult GetDelivaryChargeByStateProvinceId(int stateProvinceId)
        {
            var customer = Utility.CustomerId != 0 ? _customerService.GetCustomerById(Utility.CustomerId) : _workContext.CurrentCustomer;

            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var countryId = 0;
            if (stateProvinceId > 0)
            {
                var stateProvince = _stateProvinceService.GetStateProvinceById(stateProvinceId);
                if (stateProvince != null)
                    countryId = stateProvince.CountryId;
            }

            decimal rate = decimal.Zero;
            var getShippingOptionResponse = _shippingService
                .GetShippingOptionsCustom(cart, null,
                    "", _storeContext.CurrentStore.Id,
                    countryId, stateProvinceId);
            if (getShippingOptionResponse.Success)
            {
                var getAllShippingOption = getShippingOptionResponse.ShippingOptions;
                foreach (var shippingOption in getShippingOptionResponse.ShippingOptions)
                {
                    List<Discount> appliedDiscount;
                    var shippingTotal = _orderTotalCalculationService.AdjustShippingRate(
                        shippingOption.Rate, cart, out appliedDiscount);

                    decimal rateBase = _taxService.GetShippingPrice(shippingTotal, _workContext.CurrentCustomer);
                    rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase,
                        _workContext.WorkingCurrency);
                }
            }

            return Json(new
            {
                rate
            });
        }
        #endregion

        [NonAction]
        protected virtual bool IsPaymentWorkflowRequired(IList<ShoppingCartItem> cart, bool ignoreRewardPoints = false,
            int countryId = 0, int stateProvinenceId = 0)
        {
            bool result = true;

            //check whether order total equals zero
            decimal? shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotalCustom(cart, ignoreRewardPoints,
                countryId: countryId, stateProvinenceId: stateProvinenceId);
            if (shoppingCartTotalBase.HasValue && shoppingCartTotalBase.Value == decimal.Zero)
                result = false;
            return result;
        }

        [NonAction]
        protected virtual CheckoutBillingAddressModel PrepareBillingAddressModel(int? selectedCountryId = null, 
            bool prePopulateNewAddressWithCustomerFields = false)
        {
            var customer = Utility.CustomerId != 0 ? _customerService.GetCustomerById(Utility.CustomerId) : _workContext.CurrentCustomer;

            var model = new CheckoutBillingAddressModel();
            //existing addresses
            var addresses = customer.Addresses
                //allow billing
                .Where(a => a.Country == null || a.Country.AllowsBilling)
                //enabled for the current store
                .Where(a => a.Country == null || _storeMappingService.Authorize(a.Country))
                .ToList();
            foreach (var address in addresses)
            {
                var addressModel = new AddressModel();
                addressModel.PrepareModel(
                    address: address, 
                    excludeProperties: false, 
                    addressSettings: _addressSettings,
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
                customer: customer);
            return model;
        }

        [NonAction]
        protected virtual CheckoutShippingAddressModel PrepareShippingAddressModel(int customerId = 0, int? selectedCountryId = null, 
            bool prePopulateNewAddressWithCustomerFields = false)
        {
            var customer = Utility.CustomerId != 0 ? _customerService.GetCustomerById(Utility.CustomerId) : _workContext.CurrentCustomer;

            var model = new CheckoutShippingAddressModel();
            //allow pickup in store?
            model.AllowPickUpInStore = _shippingSettings.AllowPickUpInStore;
            
            //if (model.AllowPickUpInStore && _shippingSettings.PickUpInStoreFee > 0)
            //{
            //    decimal shippingTotal = _shippingSettings.PickUpInStoreFee;
            //    decimal rateBase = _taxService.GetShippingPrice(shippingTotal, _workContext.CurrentCustomer);
            //    decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);     //  Change 3.8
            //    model.PickUpInStoreFee = _priceFormatter.FormatShippingPrice(rate, true);
            //}
            if (model.AllowPickUpInStore)
            {
                model.DisplayPickupPointsOnMap = _shippingSettings.DisplayPickupPointsOnMap;
                model.GoogleMapsApiKey = _shippingSettings.GoogleMapsApiKey;
                var pickupPointProviders = _shippingService.LoadActivePickupPointProviders(_storeContext.CurrentStore.Id);
                if (pickupPointProviders.Any())
                {
                    var pickupPointsResponse = _shippingService.GetPickupPoints(customer.BillingAddress, null, _storeContext.CurrentStore.Id);
                    if (pickupPointsResponse.Success)
                        model.PickupPoints = pickupPointsResponse.PickupPoints.Select(x =>
                        {
                            var country = _countryService.GetCountryByTwoLetterIsoCode(x.CountryCode);
                            var pickupPointModel = new CheckoutPickupPointModel
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Description = x.Description,
                                ProviderSystemName = x.ProviderSystemName,
                                Address = x.Address,
                                City = x.City,
                                CountryName = country != null ? country.Name : string.Empty,
                                ZipPostalCode = x.ZipPostalCode,
                                Latitude = x.Latitude,
                                Longitude = x.Longitude,
                                OpeningHours = x.OpeningHours
                            };
                            if (x.PickupFee > 0)
                            {
                                var amount = _taxService.GetShippingPrice(x.PickupFee, customer);
                                amount = _currencyService.ConvertFromPrimaryStoreCurrency(amount, _workContext.WorkingCurrency);
                                pickupPointModel.PickupFee = _priceFormatter.FormatShippingPrice(amount, true);
                            }

                            return pickupPointModel;
                        }).ToList();
                    else
                        foreach (var error in pickupPointsResponse.Errors)
                            model.Warnings.Add(error);
                }

                //only available pickup points
                if (!_shippingService.LoadActiveShippingRateComputationMethods(_storeContext.CurrentStore.Id).Any())
                {
                    if (!pickupPointProviders.Any())
                    {
                        model.Warnings.Add(_localizationService.GetResource("Checkout.ShippingIsNotAllowed"));
                        model.Warnings.Add(_localizationService.GetResource("Checkout.PickupPoints.NotAvailable"));
                    }
                    model.PickUpInStoreOnly = true;
                    model.PickUpInStore = true;
                    return model;
                }
            }
            //existing addresses
            var addresses = customer.Addresses
                //allow shipping
                .Where(a => a.Country == null || a.Country.AllowsShipping)
                //enabled for the current store
                .Where(a => a.Country == null || _storeMappingService.Authorize(a.Country))
                .ToList();
            if (customerId != 0)
            {
                addresses = _customerService.GetCustomerById(customerId).Addresses
                    //allow shipping
                    .Where(a => a.Country == null || a.Country.AllowsShipping)
                    //enabled for the current store
                    .Where(a => a.Country == null || _storeMappingService.Authorize(a.Country))
                    .ToList();
            } 

            foreach (var address in addresses)
            {
                var addressModel = new AddressModel();
                addressModel.PrepareModel(
                    address: address,
                    excludeProperties: false,
                    addressSettings: _addressSettings,
                    addressAttributeFormatter: _addressAttributeFormatter);
                model.ExistingAddresses.Add(addressModel);
            }

            //new address
            model.NewAddress.CountryId = selectedCountryId;
            model.NewAddress.PrepareModel(
                address: null,
                excludeProperties: false,
                addressSettings: _addressSettings,
                localizationService: _localizationService,
                stateProvinceService: _stateProvinceService,
                addressAttributeService: _addressAttributeService,
                addressAttributeParser: _addressAttributeParser,
                loadCountries: () => _countryService.GetAllCountriesForShipping(),
                prePopulateWithCustomerFields: prePopulateNewAddressWithCustomerFields,
                customer: customer);

            //states
            if (customerId > 0)
            {
                var countryId = GetBangladeshCountryId();
                var states = _stateProvinceService.GetStateProvincesByCountryId(countryId);
                if (states.Count > 0)
                {
                    model.NewAddress.AvailableStates.Add(new SelectListItem { Text = "Please Select Delivery Zone", Value = "0" });

                    foreach (var s in states)
                    {
                        model.NewAddress.AvailableStates.Add(new SelectListItem
                        {
                            Text = s.GetLocalized(x => x.Name),
                            Value = s.Id.ToString(),
                            Selected = (s.Id == model.NewAddress.StateProvinceId)
                        });
                    }
                }
            }


            //customize code for selectedshipping address
            if (customer.ShippingAddress != null)
            {
                model.SelectedShippingAdressId = customer.ShippingAddress.Id;
            }
            
            return model;
        }

        [NonAction]
        protected virtual CheckoutShippingMethodModel PrepareShippingMethodModel(IList<ShoppingCartItem> cart, Address shippingAddress)
        {
            var customer = Utility.CustomerId != 0 ? _customerService.GetCustomerById(Utility.CustomerId) : _workContext.CurrentCustomer;

            var model = new CheckoutShippingMethodModel();

            var getShippingOptionResponse = _shippingService
                .GetShippingOptions(cart, shippingAddress,
                "", _storeContext.CurrentStore.Id);
            if (getShippingOptionResponse.Success)
            {
                //performance optimization. cache returned shipping options.
                //we'll use them later (after a customer has selected an option).
                _genericAttributeService.SaveAttribute(customer,
                                                       SystemCustomerAttributeNames.OfferedShippingOptions,
                                                       getShippingOptionResponse.ShippingOptions,
                                                       _storeContext.CurrentStore.Id);
                var addressCurrentCustomerShipping = customer.ShippingAddress;
                //string stateProvinc = addressCurrentCustomerShipping.StateProvince.Name;
                //_logger.InsertLog(Nop.Core.Domain.Logging.LogLevel.Warning, "count", getShippingOptionResponse.ShippingOptions.Count.ToString());

                //#region Modifying code for state providence
                //var getAllShippingOption = getShippingOptionResponse.ShippingOptions;
                //var shippingOption = getAllShippingOption.Where(x => x.Name == stateProvinc).FirstOrDefault();
                //if (shippingOption == null)
                //{
                //    shippingOption = getAllShippingOption.Where(x => x.Name == "Outside Lagos").FirstOrDefault();
                //}


                //var soModel = new CheckoutShippingMethodModel.ShippingMethodModel
                //{
                //    Name = shippingOption.Name,
                //    Description = shippingOption.Description,
                //    ShippingRateComputationMethodSystemName = shippingOption.ShippingRateComputationMethodSystemName,
                //    ShippingOption = shippingOption,
                //};
                //List<Discount> appliedDiscount;
                //var shippingTotal = _orderTotalCalculationService.AdjustShippingRate(
                //    shippingOption.Rate, cart, out appliedDiscount);

                //decimal rateBase = _taxService.GetShippingPrice(shippingTotal, _workContext.CurrentCustomer);
                //decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase,
                //                                                                _workContext.WorkingCurrency);
                //soModel.Fee = _priceFormatter.FormatShippingPrice(rate, true);
                ////save
                //_logger.InsertLog(Nop.Core.Domain.Logging.LogLevel.Warning, "shipping method", soModel.Name);
                ////Obeezi
                //model.ShippingMethods.Add(soModel);
                //_genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedShippingOption, shippingOption, _storeContext.CurrentStore.Id);
                //_logger.InsertLog(Nop.Core.Domain.Logging.LogLevel.Warning, soModel.Fee);


                //#endregion

                #region Previous Code

                foreach (var shippingOption in getShippingOptionResponse.ShippingOptions)
                {
                    var soModel = new CheckoutShippingMethodModel.ShippingMethodModel
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

                    decimal rateBase = _taxService.GetShippingPrice(shippingTotal, customer);
                    decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                    soModel.Fee = _priceFormatter.FormatShippingPrice(rate, true);

                    model.ShippingMethods.Add(soModel);
                }

                if (model.ShippingMethods.Count == 1)
                {
                    var shippingOption = model.ShippingMethods.FirstOrDefault().ShippingOption;
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.SelectedShippingOption, shippingOption, _storeContext.CurrentStore.Id);
                }
                #endregion

                //find a selected (previously) shipping method
                var selectedShippingOption = customer.GetAttribute<ShippingOption>(
                        SystemCustomerAttributeNames.SelectedShippingOption, _storeContext.CurrentStore.Id);
                if (selectedShippingOption != null)
                {
                    var shippingOptionToSelect = model.ShippingMethods.ToList()
                        .Find( so =>
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
        protected virtual CheckoutPaymentMethodModel PreparePaymentMethodModel(IList<ShoppingCartItem> cart, int filterByCountryId)
        {
            var customer = Utility.CustomerId != 0 ? _customerService.GetCustomerById(Utility.CustomerId) : _workContext.CurrentCustomer;

            var model = new CheckoutPaymentMethodModel();

            //reward points
            if (_rewardPointsSettings.Enabled && !cart.IsRecurring())
            {
                int rewardPointsBalance = _rewardPointService.GetRewardPointsBalance(customer.Id, _storeContext.CurrentStore.Id);
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
                .LoadActivePaymentMethods(customer.Id, _storeContext.CurrentStore.Id, filterByCountryId)
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Standard || pm.PaymentMethodType == PaymentMethodType.Redirection)
                .Where(pm => !pm.HidePaymentMethod(cart))
                .ToList();
            foreach (var pm in paymentMethods)
            {
                if (cart.IsRecurring() && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                    continue;

                var pmModel = new CheckoutPaymentMethodModel.PaymentMethodModel
                {
                    Name = pm.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id),
                    PaymentMethodSystemName = pm.PluginDescriptor.SystemName,
                    LogoUrl = pm.PluginDescriptor.GetLogoUrl(_webHelper)
                };
                //payment method additional fee
                decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, pm.PluginDescriptor.SystemName);
                decimal rateBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, customer);
                decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                if (rate > decimal.Zero)
                    pmModel.Fee = _priceFormatter.FormatPaymentMethodAdditionalFee(rate, true);

                model.PaymentMethods.Add(pmModel);
            }
            
            //find a selected (previously) payment method
            var selectedPaymentMethodSystemName = customer.GetAttribute<string>(
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

            return model;
        }

        [NonAction]
        protected virtual CheckoutPaymentInfoModel PreparePaymentInfoModel(IPaymentMethod paymentMethod)
        {
            var model = new CheckoutPaymentInfoModel();
            string actionName;
            string controllerName;
            RouteValueDictionary routeValues;
            paymentMethod.GetPaymentInfoRoute(out actionName, out controllerName, out routeValues);
            model.PaymentInfoActionName = actionName;
            model.PaymentInfoControllerName = controllerName;
            model.PaymentInfoRouteValues = routeValues;
            model.DisplayOrderTotals = _orderSettings.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab;
            return model;
        }

        [NonAction]
        protected virtual CheckoutConfirmModel PrepareConfirmOrderModel(IList<ShoppingCartItem> cart)
        {
            var model = new CheckoutConfirmModel();
            //terms of service
            model.TermsOfServiceOnOrderConfirmPage = _orderSettings.TermsOfServiceOnOrderConfirmPage;
            //min order amount validation
            bool minOrderTotalAmountOk = _orderProcessingService.ValidateMinOrderTotalAmount(cart);
            if (!minOrderTotalAmountOk)
            {
                decimal minOrderTotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderTotalAmount, _workContext.WorkingCurrency);
                model.MinOrderTotalWarning = string.Format(_localizationService.GetResource("Checkout.MinOrderTotalAmount"), _priceFormatter.FormatPrice(minOrderTotalAmount, true, false));
            }
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

        [NonAction]
        protected virtual void PrepareShoppingCartModel(ShoppingCartModel model,
            IList<ShoppingCartItem> cart, bool isEditable = true,
            bool validateCheckoutAttributes = false,
            bool prepareEstimateShippingIfEnabled = true, bool setEstimateShippingDefaultAddress = true,
            bool prepareAndDisplayOrderReviewData = false)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (model == null)
                throw new ArgumentNullException("model");

            var customer = Utility.CustomerId != 0 ? _customerService.GetCustomerById(Utility.CustomerId) : _workContext.CurrentCustomer;

            model.OnePageCheckoutEnabled = _orderSettings.OnePageCheckoutEnabled;

            if (cart.Count == 0)
                return;

            #region Simple properties

            model.IsEditable = isEditable;
            model.ShowProductImages = _shoppingCartSettings.ShowProductImagesOnShoppingCart;
            model.ShowSku = _catalogSettings.ShowProductSku;
            var checkoutAttributesXml = customer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _genericAttributeService, _storeContext.CurrentStore.Id);
            model.CheckoutAttributeInfo = _checkoutAttributeFormatter.FormatAttributes(checkoutAttributesXml, customer);
            bool minOrderSubtotalAmountOk = _orderProcessingService.ValidateMinOrderSubtotalAmount(cart);
            if (!minOrderSubtotalAmountOk)
            {
                decimal minOrderSubtotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderSubtotalAmount, _workContext.WorkingCurrency);
                model.MinOrderSubtotalWarning = string.Format(_localizationService.GetResource("Checkout.MinOrderSubtotalAmount"), _priceFormatter.FormatPrice(minOrderSubtotalAmount, true, false));
            }
            model.TermsOfServiceOnShoppingCartPage = _orderSettings.TermsOfServiceOnShoppingCartPage;
            model.TermsOfServiceOnOrderConfirmPage = _orderSettings.TermsOfServiceOnOrderConfirmPage;

            //gift card and gift card boxes
            model.DiscountBox.Display = _shoppingCartSettings.ShowDiscountBox;
            var discountCouponCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.DiscountCouponCode);
            var discount = _discountService.GetDiscountByCouponCode(discountCouponCode);
            if (discount != null &&
                discount.RequiresCouponCode &&
               _discountService.ValidateDiscount(discount, customer).IsValid)
                model.DiscountBox.CurrentCode = discount.CouponCode;
            model.GiftCardBox.Display = _shoppingCartSettings.ShowGiftCardBox;

            //cart warnings
            var cartWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, checkoutAttributesXml, validateCheckoutAttributes);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);

            #endregion

            #region Checkout attributes

            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, !cart.RequiresShipping());
            foreach (var attribute in checkoutAttributes)
            {
                var attributeModel = new ShoppingCartModel.CheckoutAttributeModel
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
                        var attributeValueModel = new ShoppingCartModel.CheckoutAttributeValueModel
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
                var selectedCheckoutAttributes = customer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _genericAttributeService, _storeContext.CurrentStore.Id);
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

            #region Estimate shipping

            if (prepareEstimateShippingIfEnabled)
            {
                model.EstimateShipping.Enabled = cart.Count > 0 && cart.RequiresShipping() && _shippingSettings.EstimateShippingEnabled;
                if (model.EstimateShipping.Enabled)
                {
                    //countries
                    int? defaultEstimateCountryId = (setEstimateShippingDefaultAddress && customer.ShippingAddress != null) ? customer.ShippingAddress.CountryId : model.EstimateShipping.CountryId;
                    model.EstimateShipping.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                    foreach (var c in _countryService.GetAllCountriesForShipping())
                        model.EstimateShipping.AvailableCountries.Add(new SelectListItem
                        {
                            Text = c.GetLocalized(x => x.Name),
                            Value = c.Id.ToString(),
                            Selected = c.Id == defaultEstimateCountryId
                        });
                    //states
                    int? defaultEstimateStateId = (setEstimateShippingDefaultAddress && customer.ShippingAddress != null) ? customer.ShippingAddress.StateProvinceId : model.EstimateShipping.StateProvinceId;
                    var states = defaultEstimateCountryId.HasValue ? _stateProvinceService.GetStateProvincesByCountryId(defaultEstimateCountryId.Value).ToList() : new List<StateProvince>();
                    if (states.Count > 0)
                        foreach (var s in states)
                            model.EstimateShipping.AvailableStates.Add(new SelectListItem
                            {
                                Text = s.GetLocalized(x => x.Name),
                                Value = s.Id.ToString(),
                                Selected = s.Id == defaultEstimateStateId
                            });
                    else
                        model.EstimateShipping.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Address.OtherNonUS"), Value = "0" });

                    if (setEstimateShippingDefaultAddress && customer.ShippingAddress != null)
                        model.EstimateShipping.ZipPostalCode = customer.ShippingAddress.ZipPostalCode;
                }
            }

            #endregion

            #region Cart items

            foreach (var sci in cart)
            {
                var cartItemModel = new ShoppingCartModel.ShoppingCartItemModel
                {
                    Id = sci.Id,
                    Sku = sci.Product.FormatSku(sci.AttributesXml, _productAttributeParser),
                    ProductId = sci.Product.Id,
                    ProductName = sci.Product.GetLocalized(x => x.Name),
                    ProductSeName = sci.Product.GetSeName(),
                    Quantity = sci.Quantity,
                    AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml),
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
                    decimal shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetUnitPrice(sci), customer, out taxRate);
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
                    List<Discount> scDiscount;
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
                        _mediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName);
                }

                //item warnings
                var itemWarnings = _shoppingCartService.GetShoppingCartItemWarnings(
                    customer,
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

            #region Button payment methods

            var paymentMethods = _paymentService
                .LoadActivePaymentMethods(customer.Id, _storeContext.CurrentStore.Id)
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Button)
                .Where(pm => !pm.HidePaymentMethod(cart))
                .ToList();
            foreach (var pm in paymentMethods)
            {
                if (cart.IsRecurring() && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                    continue;

                string actionName;
                string controllerName;
                RouteValueDictionary routeValues;
                pm.GetPaymentInfoRoute(out actionName, out controllerName, out routeValues);

                model.ButtonPaymentMethodActionNames.Add(actionName);
                model.ButtonPaymentMethodControllerNames.Add(controllerName);
                model.ButtonPaymentMethodRouteValues.Add(routeValues);
            }

            #endregion

            #region Order review data

            if (prepareAndDisplayOrderReviewData)
            {
                model.OrderReviewData.Display = true;

                //billing info
                var billingAddress = customer.BillingAddress;
                if (billingAddress != null)
                    model.OrderReviewData.BillingAddress.PrepareModel(
                        address: billingAddress,
                        excludeProperties: false,
                        addressSettings: _addressSettings,
                        addressAttributeFormatter: _addressAttributeFormatter);

                //shipping info
                if (cart.RequiresShipping())
                {
                    //model.OrderReviewData.IsShippable = true;

                    //if (_shippingSettings.AllowPickUpInStore)
                    //{
                    //    model.OrderReviewData.SelectedPickUpInStore = _workContext.CurrentCustomer.GetAttribute<bool>(SystemCustomerAttributeNames.SelectedPickUpInStore, _storeContext.CurrentStore.Id);
                    //}

                    //if (!model.OrderReviewData.SelectedPickUpInStore)
                    //{
                    //    var shippingAddress = _workContext.CurrentCustomer.ShippingAddress;         // Change 3.8
                    //    if (shippingAddress != null)
                    //    {
                    //        model.OrderReviewData.ShippingAddress.PrepareModel(
                    //            address: shippingAddress,
                    //            excludeProperties: false,
                    //            addressSettings: _addressSettings,
                    //            addressAttributeFormatter: _addressAttributeFormatter);
                    //    }
                    //}
                    model.OrderReviewData.IsShippable = true;

                    var pickupPoint = customer
                        .GetAttribute<PickupPoint>(SystemCustomerAttributeNames.SelectedPickupPoint, _storeContext.CurrentStore.Id);
                    model.OrderReviewData.SelectedPickUpInStore = _shippingSettings.AllowPickUpInStore && pickupPoint != null;
                    if (!model.OrderReviewData.SelectedPickUpInStore)
                    {
                        if (customer.ShippingAddress != null)
                        {
                            model.OrderReviewData.ShippingAddress.PrepareModel(
                                address: customer.ShippingAddress,
                                excludeProperties: false,
                                addressSettings: _addressSettings,
                                addressAttributeFormatter: _addressAttributeFormatter);
                        }
                    }
                    else
                    {
                        var country = _countryService.GetCountryByTwoLetterIsoCode(pickupPoint.CountryCode);
                        model.OrderReviewData.PickupAddress = new AddressModel
                        {
                            Address1 = pickupPoint.Address,
                            City = pickupPoint.City,
                            CountryName = country != null ? country.Name : string.Empty,
                            ZipPostalCode = pickupPoint.ZipPostalCode
                        };
                    }

                    //selected shipping method
                    var shippingOption = customer.GetAttribute<ShippingOption>(SystemCustomerAttributeNames.SelectedShippingOption, _storeContext.CurrentStore.Id);
                    if (shippingOption != null)
                        model.OrderReviewData.ShippingMethod = shippingOption.Name;
                }
                //payment info
                var selectedPaymentMethodSystemName = customer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod, _storeContext.CurrentStore.Id);
                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(selectedPaymentMethodSystemName);
                model.OrderReviewData.PaymentMethod = paymentMethod != null ? paymentMethod.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id) : "";

                //custom values
                var processPaymentRequest = _httpContext.Session["OrderPaymentInfo"] as ProcessPaymentRequest;
                if (processPaymentRequest != null)
                {
                    model.OrderReviewData.CustomValues = processPaymentRequest.CustomValues;
                }
            }
            #endregion
        }

        [NonAction]
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
                        Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), productName),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), productName),
                    };
                });
            return model;
        }

        [NonAction]
        protected virtual OrderTotalsModel PrepareOrderTotalsModel(IList<ShoppingCartItem> cart, bool isEditable, string stProvinceName = "")
        {
            var customer = Utility.CustomerId != 0 ? _customerService.GetCustomerById(Utility.CustomerId) : _workContext.CurrentCustomer;

            var model = new OrderTotalsModel();
            model.IsEditable = isEditable;

            var countryId = GetBangladeshCountryId();
            var stateProvinenceId = 0;
            if (!string.IsNullOrEmpty(stProvinceName))
            {
                var states = _stateProvinceService.GetStateProvincesByCountryId(countryId);
                if (states.Any())
                {
                    var stateProvince = states.FirstOrDefault(s => s.Name.ToLower() == stProvinceName.ToLower());
                    if (stateProvince != null)
                        stateProvinenceId = stateProvince.Id;
                }
            }

            if (cart.Count > 0)
            {
                //subtotal
                decimal orderSubTotalDiscountAmountBase;
                List<Discount> orderSubTotalAppliedDiscount;
                decimal subTotalWithoutDiscountBase;
                decimal subTotalWithDiscountBase;
                var subTotalIncludingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
                _orderTotalCalculationService.GetShoppingCartSubTotal(cart, subTotalIncludingTax,
                    out orderSubTotalDiscountAmountBase, out orderSubTotalAppliedDiscount,
                    out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
                decimal subtotalBase = subTotalWithoutDiscountBase;
                decimal subtotal = _currencyService.ConvertFromPrimaryStoreCurrency(subtotalBase, _workContext.WorkingCurrency);
                model.SubTotal = _priceFormatter.FormatPrice(subtotal, true, _workContext.WorkingCurrency, _workContext.WorkingLanguage, subTotalIncludingTax);

                if (orderSubTotalDiscountAmountBase > decimal.Zero)
                {

                    decimal orderSubTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderSubTotalDiscountAmountBase, _workContext.WorkingCurrency);
                    model.SubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountAmount, true, _workContext.WorkingCurrency, _workContext.WorkingLanguage, subTotalIncludingTax);
                    model.AllowRemovingSubTotalDiscount = model.IsEditable &&
                        orderSubTotalAppliedDiscount.Any(d => d.RequiresCouponCode && !String.IsNullOrEmpty(d.CouponCode));
                }


                //shipping info
                model.RequiresShipping = cart.RequiresShipping();
                if (model.RequiresShipping)
                {
                    decimal? shoppingCartShippingBase = _orderTotalCalculationService.GetShoppingCartShippingTotalCustom(cart, countryId, stateProvinenceId);
                    if (shoppingCartShippingBase.HasValue)
                    {
                        decimal shoppingCartShipping = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartShippingBase.Value, _workContext.WorkingCurrency);
                        model.Shipping = _priceFormatter.FormatShippingPrice(shoppingCartShipping, true);

                        //selected shipping method
                        var shippingOption = customer.GetAttribute<ShippingOption>(SystemCustomerAttributeNames.SelectedShippingOption, _storeContext.CurrentStore.Id);
                        if (shippingOption != null)
                            model.SelectedShippingMethod = shippingOption.Name;
                    }
                }

                //payment method fee
                var paymentMethodSystemName = customer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod, _storeContext.CurrentStore.Id);
                decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, paymentMethodSystemName);
                decimal paymentMethodAdditionalFeeWithTaxBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, customer);
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
                            model.TaxRates.Add(new OrderTotalsModel.TaxRate
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
                List<Discount> orderTotalAppliedDiscount;
                List<AppliedGiftCard> appliedGiftCards;
                int redeemedRewardPoints;
                decimal redeemedRewardPointsAmount;
                decimal? shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotalCustom(cart,
                    out orderTotalDiscountAmountBase, out orderTotalAppliedDiscount,
                    out appliedGiftCards, out redeemedRewardPoints, out redeemedRewardPointsAmount,
                    countryId: countryId, stateProvinenceId: stateProvinenceId);
                if (shoppingCartTotalBase.HasValue)
                {
                    decimal shoppingCartTotal = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartTotalBase.Value, _workContext.WorkingCurrency);
                    model.OrderTotal = _priceFormatter.FormatPrice(shoppingCartTotal, true, false);
                }

                //discount
                if (orderTotalDiscountAmountBase > decimal.Zero)
                {
                    //decimal orderTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderTotalDiscountAmountBase, _workContext.WorkingCurrency);
                    //model.OrderTotalDiscount = _priceFormatter.FormatPrice(-orderTotalDiscountAmount, true, false);
                    //model.AllowRemovingOrderTotalDiscount = orderTotalAppliedDiscount != null &&
                    //    orderTotalAppliedDiscount.RequiresCouponCode &&
                    //    !String.IsNullOrEmpty(orderTotalAppliedDiscount.CouponCode) &&
                    //    model.IsEditable;

                    //Change 3.8//
                    decimal orderTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderTotalDiscountAmountBase, _workContext.WorkingCurrency);
                    model.OrderTotalDiscount = _priceFormatter.FormatPrice(-orderTotalDiscountAmount, true, false);
                    model.AllowRemovingOrderTotalDiscount = model.IsEditable &&
                        orderTotalAppliedDiscount.Any(d => d.RequiresCouponCode && !String.IsNullOrEmpty(d.CouponCode));
                }

                //gift cards
                if (appliedGiftCards != null && appliedGiftCards.Count > 0)
                {
                    foreach (var appliedGiftCard in appliedGiftCards)
                    {
                        var gcModel = new OrderTotalsModel.GiftCard
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
                        .CalculateRewardPoints(customer, shoppingCartTotalBase.Value);
                }

            }

            return model;
        }

        [NonAction]
        protected JsonResult OpcLoadStepAfterPaymentMethod(IPaymentMethod paymentMethod, List<ShoppingCartItem> cart)
        {
            if (paymentMethod.SkipPaymentInfo)
            {
                //skip payment info page
                var paymentInfo = new ProcessPaymentRequest();
                //session save
                _httpContext.Session["OrderPaymentInfo"] = paymentInfo;

                var confirmOrderModel = PrepareConfirmOrderModel(cart);
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "confirm-order",
                        PaymentInfoHtml = this.RenderPartialViewToString("OpcConfirmOrder", confirmOrderModel)
                    },
                    goto_section = "confirm_order"
                });
            }


            //return payment info page
            var paymenInfoModel = PreparePaymentInfoModel(paymentMethod);
            return Json(new
            {
                update_section = new UpdateSectionJsonModel
                {
                    name = "payment-info",
                    //PaymentInfoHtml = this.RenderPartialViewToString("OpcPaymentInfo", paymenInfoModel)
                    PaymentInfoHtml = ""
                },
                goto_section = "payment_info"
            });
        }

        [NonAction]
        public IList<string> AddProductToShoppingCart(Customer customer, int productId, int shoppingCartTypeId,
            int quantity)
        {
            var cartType = (ShoppingCartType)shoppingCartTypeId;

            var product = _productService.GetProductById(productId);
            if (product == null)
            {
                //no product found
            }

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
            {
                //return Json(new
                //{
                //    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                //});
            }

            //products with "minimum order quantity" more than a specified qty
            if (product.OrderMinimumQuantity > quantity)
            {
                quantity = product.OrderMinimumQuantity;
                //we cannot add to the cart such products from category pages
                //it can confuse customers. That's why we redirect customers to the product details page
                //return Json(new
                //{
                //    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                //});
            }

            if (product.CustomerEntersPrice)
            {
                //cannot be added to the cart (requires a customer to enter price)
                //return Json(new
                //{
                //    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                //});
            }

            if (product.IsRental)
            {
                //rental products require start/end dates to be entered
                //return Json(new
                //{
                //    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                //});
            }

            var allowedQuantities = product.ParseAllowedQuantities();
            if (allowedQuantities.Length > 0)
            {
                //cannot be added to the cart (requires a customer to select a quantity from dropdownlist)
                //return Json(new
                //{
                //    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                //});
            }

            if (product.ProductAttributeMappings.Any())
            {
                //product has some attributes. let a customer see them
                //return Json(new
                //{
                //    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                //});
            }

            //get standard warnings without attribute validations
            //first, try to find existing shopping cart item
            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == cartType)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var shoppingCartItem = _shoppingCartService.FindShoppingCartItemInTheCart(cart, cartType, product);
            //if we already have the same product in the cart, then use the total quantity to validate
            var quantityToValidate = shoppingCartItem != null ? shoppingCartItem.Quantity + quantity : quantity;
            var addToCartWarnings = _shoppingCartService
                .GetShoppingCartItemWarnings(customer, cartType,
                product, _storeContext.CurrentStore.Id, string.Empty,
                decimal.Zero, null, null, quantityToValidate, false, true, false, false, false);
            if (!addToCartWarnings.Any())
            {
                addToCartWarnings = _shoppingCartService.AddToCart(customer: customer,
                product: product,
                shoppingCartType: cartType,
                storeId: _storeContext.CurrentStore.Id,
                quantity: quantity);
                if (addToCartWarnings.Any())
                {
                    //cannot be added to the cart
                    //but we do not display attribute and gift card warnings here. let's do it on the product details page
                    //return Json(new
                    //{
                    //    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                    //});
                }
            }

            //now let's try adding product to the cart (now including product attribute validation, etc)

            var warnings = new List<string>();
            foreach (var warning in addToCartWarnings)
            {
                warnings.Add(product.Name + ":" + warning);
            }
            return warnings;
        }

        [NonAction]
        protected virtual void PrepareVendorsModel(CustomerModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableVendors.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Customers.Customers.Fields.Vendor.None"),
                Value = "0"
            });
            var vendors = _vendorService.GetAllVendors(showHidden: true);
            foreach (var vendor in vendors)
            {
                model.AvailableVendors.Add(new SelectListItem
                {
                    Text = vendor.Name,
                    Value = vendor.Id.ToString()
                });
            }
        }

        [NonAction]
        protected virtual void PrepareCustomerAttributeModel(CustomerModel model, Customer customer)
        {
            var customerAttributes = _customerAttributeService.GetAllCustomerAttributes();
            foreach (var attribute in customerAttributes)
            {
                var attributeModel = new CustomerModel.CustomerAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.Name,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _customerAttributeService.GetCustomerAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new CustomerModel.CustomerAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.Name,
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }


                //set already selected attributes
                if (customer != null)
                {
                    var selectedCustomerAttributes = customer.GetAttribute<string>(SystemCustomerAttributeNames.CustomCustomerAttributes, _genericAttributeService);
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.Checkboxes:
                            {
                                if (!String.IsNullOrEmpty(selectedCustomerAttributes))
                                {
                                    //clear default selection
                                    foreach (var item in attributeModel.Values)
                                        item.IsPreSelected = false;

                                    //select new values
                                    var selectedValues = _customerAttributeParser.ParseCustomerAttributeValues(selectedCustomerAttributes);
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
                                if (!String.IsNullOrEmpty(selectedCustomerAttributes))
                                {
                                    var enteredText = _customerAttributeParser.ParseValues(selectedCustomerAttributes, attribute.Id);
                                    if (enteredText.Any())
                                        attributeModel.DefaultValue = enteredText[0];
                                }
                            }
                            break;
                        case AttributeControlType.Datepicker:
                        case AttributeControlType.ColorSquares:
                        case AttributeControlType.ImageSquares:
                        case AttributeControlType.FileUpload:
                        default:
                            //not supported attribute control types
                            break;
                    }
                }

                model.CustomerAttributes.Add(attributeModel);
            }
        }

        [NonAction]
        protected virtual IList<CustomerModel.AssociatedExternalAuthModel> GetAssociatedExternalAuthRecords(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var result = new List<CustomerModel.AssociatedExternalAuthModel>();
            foreach (var record in _openAuthenticationService.GetExternalIdentifiersFor(customer))
            {
                var method = _openAuthenticationService.LoadExternalAuthenticationMethodBySystemName(record.ProviderSystemName);
                if (method == null)
                    continue;

                result.Add(new CustomerModel.AssociatedExternalAuthModel
                {
                    Id = record.Id,
                    Email = record.Email,
                    ExternalIdentifier = record.ExternalIdentifier,
                    AuthMethodName = method.PluginDescriptor.FriendlyName
                });
            }

            return result;
        }

        [NonAction]
        protected virtual void PrepareCustomerModel(CustomerModel model, Customer customer, bool excludeProperties)
        {
            var allStores = _storeService.GetAllStores();
            if (customer != null)
            {
                model.Id = customer.Id;
                if (!excludeProperties)
                {
                    model.Email = customer.Email;
                    model.Username = customer.Username;
                    model.VendorId = customer.VendorId;
                    model.AdminComment = customer.AdminComment;
                    model.IsTaxExempt = customer.IsTaxExempt;
                    model.Active = customer.Active;

                    var affiliate = _affiliateService.GetAffiliateById(customer.AffiliateId);
                    if (affiliate != null)
                    {
                        model.AffiliateId = affiliate.Id;
                        model.AffiliateName = affiliate.GetFullName();
                    }

                    model.TimeZoneId = customer.GetAttribute<string>(SystemCustomerAttributeNames.TimeZoneId);
                    model.VatNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.VatNumber);
                    model.VatNumberStatusNote = ((VatNumberStatus)customer.GetAttribute<int>(SystemCustomerAttributeNames.VatNumberStatusId))
                        .GetLocalizedEnum(_localizationService, _workContext);
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(customer.CreatedOnUtc, DateTimeKind.Utc);
                    model.LastActivityDate = _dateTimeHelper.ConvertToUserTime(customer.LastActivityDateUtc, DateTimeKind.Utc);
                    model.LastIpAddress = customer.LastIpAddress;
                    model.LastVisitedPage = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastVisitedPage);

                    model.SelectedCustomerRoleIds = customer.CustomerRoles.Select(cr => cr.Id).ToList();

                    //newsletter subscriptions
                    if (!String.IsNullOrEmpty(customer.Email))
                    {
                        var newsletterSubscriptionStoreIds = new List<int>();
                        foreach (var store in allStores)
                        {
                            var newsletterSubscription = _newsLetterSubscriptionService
                                .GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, store.Id);
                            if (newsletterSubscription != null)
                                newsletterSubscriptionStoreIds.Add(store.Id);
                            model.SelectedNewsletterSubscriptionStoreIds = newsletterSubscriptionStoreIds.ToArray();
                        }
                    }

                    //form fields
                    model.FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName);
                    model.LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName);
                    model.Gender = customer.GetAttribute<string>(SystemCustomerAttributeNames.Gender);
                    model.DateOfBirth = customer.GetAttribute<DateTime?>(SystemCustomerAttributeNames.DateOfBirth);
                    model.Company = customer.GetAttribute<string>(SystemCustomerAttributeNames.Company);
                    model.StreetAddress = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress);
                    model.StreetAddress2 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress2);
                    model.ZipPostalCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.ZipPostalCode);
                    model.City = customer.GetAttribute<string>(SystemCustomerAttributeNames.City);
                    model.CountryId = customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId);
                    model.StateProvinceId = customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId);
                    model.Phone = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                    model.Fax = customer.GetAttribute<string>(SystemCustomerAttributeNames.Fax);
                }
            }

            model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            model.AllowUsersToChangeUsernames = _customerSettings.AllowUsersToChangeUsernames;
            model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == model.TimeZoneId) });
            if (customer != null)
            {
                model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            }
            else
            {
                model.DisplayVatNumber = false;
            }

            //vendors
            PrepareVendorsModel(model);
            //customer attributes
            PrepareCustomerAttributeModel(model, customer);

            model.GenderEnabled = _customerSettings.GenderEnabled;
            model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _customerSettings.CompanyEnabled;
            model.StreetAddressEnabled = _customerSettings.StreetAddressEnabled;
            model.StreetAddress2Enabled = _customerSettings.StreetAddress2Enabled;
            model.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;
            model.CityEnabled = _customerSettings.CityEnabled;
            model.CountryEnabled = _customerSettings.CountryEnabled;
            model.StateProvinceEnabled = _customerSettings.StateProvinceEnabled;
            model.PhoneEnabled = _customerSettings.PhoneEnabled;
            model.FaxEnabled = _customerSettings.FaxEnabled;

            //countries and states
            if (_customerSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (_customerSettings.StateProvinceEnabled)
                {
                    //states
                    var states = _stateProvinceService.GetStateProvincesByCountryId(model.CountryId).ToList();
                    if (states.Any())
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                        }
                    }
                    else
                    {
                        bool anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);

                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = _localizationService.GetResource(anyCountrySelected ? "Admin.Address.OtherNonUS" : "Admin.Address.SelectState"),
                            Value = "0"
                        });
                    }
                }
            }

            //newsletter subscriptions
            model.AvailableNewsletterSubscriptionStores = allStores
                .Select(s => new CustomerModel.StoreModel() {Id = s.Id, Name = s.Name })
                .ToList();

            //customer roles
            var allRoles = _customerService.GetAllCustomerRoles(true);
            var adminRole = allRoles.FirstOrDefault(c => c.SystemName == SystemCustomerRoleNames.Registered);
            //precheck Registered Role as a default role while creating a new customer through admin
            if (customer == null && adminRole != null)
            {
                model.SelectedCustomerRoleIds.Add(adminRole.Id);
            }
            foreach (var role in allRoles)
            {
                model.AvailableCustomerRoles.Add(new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Id.ToString(),
                    Selected = model.SelectedCustomerRoleIds.Contains(role.Id)
                });
            }

            //reward points history
            if (customer != null)
            {
                model.DisplayRewardPointsHistory = _rewardPointsSettings.Enabled;
                model.AddRewardPointsValue = 0;
                model.AddRewardPointsMessage = "Some comment here...";

                //stores
                foreach (var store in allStores)
                {
                    model.RewardPointsAvailableStores.Add(new SelectListItem
                    {
                        Text = store.Name,
                        Value = store.Id.ToString(),
                        Selected = (store.Id == _storeContext.CurrentStore.Id)
                    });
                }
            }
            else
            {
                model.DisplayRewardPointsHistory = false;
            }
            //external authentication records
            if (customer != null)
            {
                model.AssociatedExternalAuthRecords = GetAssociatedExternalAuthRecords(customer);
            }
            //sending of the welcome message:
            //1. "admin approval" registration method
            //2. already created customer
            //3. registered
            model.AllowSendingOfWelcomeMessage = _customerSettings.UserRegistrationType == UserRegistrationType.AdminApproval &&
                customer != null &&
                customer.IsRegistered();
            //sending of the activation message
            //1. "email validation" registration method
            //2. already created customer
            //3. registered
            //4. not active
            model.AllowReSendingOfActivationMessage = _customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation &&
                customer != null &&
                customer.IsRegistered() &&
                !customer.Active;
        }

        [NonAction]
        protected virtual string ValidateCustomerRoles(IList<CustomerRole> customerRoles)
        {
            if (customerRoles == null)
                throw new ArgumentNullException("customerRoles");

            //ensure a customer is not added to both 'Guests' and 'Registered' customer roles
            //ensure that a customer is in at least one required role ('Guests' and 'Registered')
            bool isInGuestsRole = customerRoles.FirstOrDefault(cr => cr.SystemName == SystemCustomerRoleNames.Guests) != null;
            bool isInRegisteredRole = customerRoles.FirstOrDefault(cr => cr.SystemName == SystemCustomerRoleNames.Registered) != null;
            if (isInGuestsRole && isInRegisteredRole)
                return "The customer cannot be in both 'Guests' and 'Registered' customer roles";
            if (!isInGuestsRole && !isInRegisteredRole)
                return "Add the customer to 'Guests' or 'Registered' customer role";

            //no errors
            return "";
        }

        [NonAction]
        protected virtual string ParseCustomCustomerAttributes(Customer customer, FormCollection form)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (form == null)
                throw new ArgumentNullException("form");

            string attributesXml = "";
            var customerAttributes = _customerAttributeService.GetAllCustomerAttributes();
            foreach (var attribute in customerAttributes)
            {
                string controlId = string.Format("customer_attribute_{0}", attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
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
                                        attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _customerAttributeService.GetCustomerAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
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
                                attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.FileUpload:
                    //not supported customer attributes
                    default:
                        break;
                }
            }

            return attributesXml;
        }
        #endregion

        #region Methods

        [ChildActionOnly]
        [AdminAuthorize]
        public ActionResult Configure()
        {
            return View("~/Plugins/Misc.OnePageCheckOutAdmin/Views/MiscOnePageCheckOutAdmin/Configure.cshtml");
        }

        public ActionResult OnePageCheckOutAdmin()
        {
            //validation                        
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id).ToList();
                            
            //if (cart.Count == 0)
            //    return RedirectToRoute("ShoppingCart");

            //if (!_orderSettings.OnePageCheckoutEnabled)
            //return RedirectToRoute("Checkout");


            Utility.CustomerId = 0;
            var shoppingCartModel = new ShoppingCartModel();
            PrepareShoppingCartModel(shoppingCartModel, cart, false);
            CheckoutShippingMethodModel shippingMethodModel = null;
            if (_workContext.CurrentCustomer.ShippingAddress != null)
            {
                shippingMethodModel = PrepareShippingMethodModel(cart, _workContext.CurrentCustomer.ShippingAddress);
            }
            var OrderTotalModel = PrepareOrderTotalsModel(cart, true);
            var paymentMethodModel = PreparePaymentMethodModel(cart, filterByCountryId);
            var model = new OnePageCheckoutModel
            {
                ShippingRequired = cart.RequiresShipping(),
                DisableBillingAddressCheckoutStep = _orderSettings.DisableBillingAddressCheckoutStep,
                ShippingAddresses = PrepareShippingAddressModel(prePopulateNewAddressWithCustomerFields: true),
                BillingAddresses = PrepareBillingAddressModel(prePopulateNewAddressWithCustomerFields: true),
                ShoppingCart = shoppingCartModel,
                CheckoutShippingMethod = shippingMethodModel,
                CheckoutPaymentMethod = paymentMethodModel,
                AllOrderTotalsModel = OrderTotalModel
            };

            //states
            var countryId = GetBangladeshCountryId();
            var states = _stateProvinceService.GetStateProvincesByCountryId(countryId);
            if (states.Count > 0)
            {
                model.ShippingAddresses.NewAddress.AvailableStates.Add(new SelectListItem { Text = "Please Select Delivery Zone", Value = "0" });

                foreach (var s in states)
                {
                    model.ShippingAddresses.NewAddress.AvailableStates.Add(new SelectListItem
                    {
                        Text = s.GetLocalized(x => x.Name),
                        Value = s.Id.ToString(),
                        Selected = (s.Id == model.ShippingAddresses.NewAddress.StateProvinceId)
                    });
                }
            }

            if (_workContext.CurrentCustomer.ShippingAddress != null)
            {
                model.SelectedShippingAdressId = _workContext.CurrentCustomer.ShippingAddress.Id;
            }

            //var customers = _customerService.GetAllCustomers().ToList();
            var customers = _customerService.GetAllCustomers().Where(c => !c.IsSystemAccount).ToList();            
            model.Customers = customers;
            model.SearchTermMinimumLength = _catalogSettings.ProductSearchTermMinimumLength;
            //if (customers.Any())
            //{
            //    foreach (var customer in customers)
            //    {
            //        model.Customers.Add(new SelectListItem
            //        {
            //            Value = customer.Id.ToString(),
            //            Text = customer.GetFullName(),
            //        });
            //    }
            //}

            // = PrepareBillingAddressModel(prePopulateNewAddressWithCustomerFields: true);
            //var shippingAddressModel = PrepareShippingAddressModel(prePopulateNewAddressWithCustomerFields: true);
            //return View("~/Plugins/Misc.OnePageCheckOut/Views/Checkout/OnePageCheckout.cshtml", model);
            //return View(model);
            return View("~/Plugins/Misc.OnePageCheckOutAdmin/Views/MiscOnePageCheckOutAdmin/OnePageCheckoutAdmin.cshtml", model);
        }

        public ActionResult ProductAddToCartPopup()
        {
            var model = new SeachProductModel();
            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var m in _manufacturerService.GetAllManufacturers(showHidden: true))
                model.AvailableManufacturers.Add(new SelectListItem { Text = m.Name, Value = m.Id.ToString() });

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var v in _vendorService.GetAllVendors(showHidden: true))
                model.AvailableVendors.Add(new SelectListItem { Text = v.Name, Value = v.Id.ToString() });

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public ActionResult ProductAddPopupList(DataSourceRequest command, SeachProductModel model)
        {
            

            var gridModel = new DataSourceResult();
            IPagedList<Product> products;
            if (!string.IsNullOrEmpty(model.SearchProductSku))
            {
                
                var product = _productService.GetProductBySku(model.SearchProductSku);
                products = new PagedList<Product>(new List<Product>() {product}, 0, 1, 1);
            }
            else
            {
                products = _productService.SearchProducts(
                    includeEkshopProducts: true,
                    activeVendorOnly: false,
                    categoryIds: new List<int> { model.SearchCategoryId },
                    manufacturerId: model.SearchManufacturerId,
                    storeId: model.SearchStoreId,
                    vendorId: model.SearchVendorId,
                    productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                    keywords: model.SearchProductName,
                    pageIndex: command.Page - 1,
                    pageSize: command.PageSize,
                    showHidden: true
                    );
            }
             
            gridModel.Data = products.Select(x => 
                new
                {
                    x.Id,
                    x.Name,
                    x.Published

                });
            gridModel.Total = products.TotalCount;

            return Json(gridModel);
        }
        [HttpPost]
        public ActionResult ProductAddToCartPopup(string btnId, string formId, SeachProductModel model)
        {

            var customer = Utility.CustomerId != 0 ? _customerService.GetCustomerById(Utility.CustomerId) : _workContext.CurrentCustomer;

            var cart = customer.ShoppingCartItems
                            .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                            .LimitPerStore(_storeContext.CurrentStore.Id)
                            .ToList();            
            var addtocartwarnings= new List<string>();
            if (model.SelectedProductIds != null)
            {
                
                foreach (int id in model.SelectedProductIds)
                {
                    var product = _productService.GetProductById(id);
                    if(product != null)
                    {

                        //_shoppingCartService.AddToCart(customer, product,
                        //                           ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id,
                        //                           null, 0,
                        //                           null, null,
                        //                           1, false);
                        //        addToCartWarnings.AddRange(_shoppingCartService.AddToCart(customer,
                        //product, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id,
                        //null, 0,
                        //null, null, 1, true));

                        addtocartwarnings.AddRange(AddProductToShoppingCart(customer, id, 1, 1));                        
                    }
                }
            }

            
            if (addtocartwarnings.Count > 0)
            {
                //categories
                model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
                var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
                foreach (var c in categories)
                    model.AvailableCategories.Add(c);

                //manufacturers
                model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
                foreach (var m in _manufacturerService.GetAllManufacturers(showHidden: true))
                    model.AvailableManufacturers.Add(new SelectListItem { Text = m.Name, Value = m.Id.ToString() });

                //stores
                model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
                foreach (var s in _storeService.GetAllStores())
                    model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

                //vendors
                model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
                foreach (var v in _vendorService.GetAllVendors(showHidden: true))
                    model.AvailableVendors.Add(new SelectListItem { Text = v.Name, Value = v.Id.ToString() });

                //product types
                model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
                model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

                model.Warnings = addtocartwarnings;

            }
            else
            {
                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
            }
            
            return View(model);
        }

        [HttpPost]   
        public ActionResult reloadAllPartialView(int customerId)
        {
            //var model = new OnePageCheckoutModel
            //{                
            //    ShippingAddresses = PrepareShippingAddressModel(prePopulateNewAddressWithCustomerFields: true),
            //    BillingAddresses = PrepareBillingAddressModel(prePopulateNewAddressWithCustomerFields: true),                
            //};
            //nizam changes            
            Utility.CustomerId = customerId;

            var customer = Utility.CustomerId != 0 ? _customerService.GetCustomerById(Utility.CustomerId) : _workContext.CurrentCustomer;
            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var shoppingCartModel = new ShoppingCartModel();
            PrepareShoppingCartModel(shoppingCartModel, cart, false);

            var ShippingAddresses = PrepareShippingAddressModel(customerId, prePopulateNewAddressWithCustomerFields: true);
            var BillingAddresses = PrepareBillingAddressModel(prePopulateNewAddressWithCustomerFields: true);
            var ShippingMethodModel = PrepareShippingMethodModel(cart, customer.ShippingAddress);
            var OrderTotalModel = PrepareOrderTotalsModel(cart, true);

            return Json(new
            {
                update_section = new UpdateSectionJsonModel
                {
                    name = "one-page-checkout-info",
                    ShoppigCartHtml = this.RenderPartialViewToString("AdminOPCOrderSummary", shoppingCartModel),
                    ShippingAdressUpdateHtml = this.RenderPartialViewToString("AdminOPCShippingAddress", ShippingAddresses),
                    ShippingMethodUpdateHtml = this.RenderPartialViewToString("AdminOpcShippingMethods", ShippingMethodModel),
                    OrderTotalHtml = this.RenderPartialViewToString("AdminOPCOrderTotals", OrderTotalModel),
                    DiscountCodeHtml = this.RenderPartialViewToString("_DiscountBox", shoppingCartModel.DiscountBox)
                },
                success = true
            });            

            //return View("~/Views/MiscOnePageCheckOutAdmin/OnePageCheckoutAdmin.cshtml", model);
        }

        [HttpPost]
        public ActionResult SaveSelectedShippingAddressAjax(int shippingAddressId)
        {
            var customer = Utility.CustomerId != 0 ? _customerService.GetCustomerById(Utility.CustomerId) : _workContext.CurrentCustomer;

            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            if (cart.Count == 0)
                return RedirectToRoute("ShoppingCart");            

            if (shippingAddressId > 0)
            {
                //existing address
                
                //var address = customer.Addresses.FirstOrDefault(a => a.Id == shippingAddressId);
                var address = customer.Addresses.FirstOrDefault(a => a.Id == shippingAddressId);
                if (address == null)
                    throw new Exception("Address can't be loaded");

                customer.ShippingAddress = address;
                customer.BillingAddress = address;
                _customerService.UpdateCustomer(customer);
            }

            //return shipping address info page
            var ShippingAddresses = PrepareShippingAddressModel();
            var ShippingMethodModel = PrepareShippingMethodModel(cart, customer.ShippingAddress);
            var OrderTotalModel = PrepareOrderTotalsModel(cart, true);
            return Json(new
            {
                update_section = new UpdateSectionJsonModel
                {
                    name = "one-page-checkout-info",
                    ShippingAdressUpdateHtml = this.RenderPartialViewToString("AdminOPCShippingAddress", ShippingAddresses),
                    ShippingMethodUpdateHtml = this.RenderPartialViewToString("AdminOpcShippingMethods", ShippingMethodModel),
                    OrderTotalHtml = this.RenderPartialViewToString("AdminOPCOrderTotals", OrderTotalModel)
                },
                success = true
            });
        }

        [HttpPost, ActionName("ShippingAddress")]
        [ValidateInput(false)]
        public ActionResult NewShippingAddress(CheckoutShippingAddressModel model, FormCollection form)
        {
            //validation
            var customer = Utility.CustomerId != 0 ? _customerService.GetCustomerById(Utility.CustomerId) : _workContext.CurrentCustomer;
            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            if (cart.Count == 0)
            {
                //return RedirectToRoute("ShoppingCart");
                return Json(new { success = false, errorMsg = "Cart is empty" });
            }
            model.NewAddress.Email = customer.Email;

            //countryId by stateId
            var countryId = 0;
            if (model.NewAddress.StateProvinceId > 0)
            {
                var state = _stateProvinceService.GetStateProvinceById(model.NewAddress.StateProvinceId != null ?
                    (int)model.NewAddress.StateProvinceId : 0);
                if (state != null)
                    countryId = state.CountryId;
            }
            model.NewAddress.CountryId = countryId;

            //custom address attributes
            var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
            
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                //try to find an address with the same values (don't duplicate records)
                var address = customer.Addresses.ToList().FindAddress(
                    model.NewAddress.FirstName, model.NewAddress.LastName, model.NewAddress.PhoneNumber,
                    model.NewAddress.Email, model.NewAddress.FaxNumber, model.NewAddress.Company,
                    model.NewAddress.Address1, model.NewAddress.Address2, model.NewAddress.City,
                    model.NewAddress.StateProvinceId, model.NewAddress.ZipPostalCode,
                    model.NewAddress.CountryId, customAttributes);
                if (address == null)
                {
                    address = model.NewAddress.ToEntity();
                    address.CustomAttributes = customAttributes;
                    address.CreatedOnUtc = DateTime.UtcNow;
                    //some validation
                    if (address.CountryId == 0)
                        address.CountryId = null;
                    if (address.StateProvinceId == 0)
                        address.StateProvinceId = null;
                    customer.Addresses.Add(address);
                }
                customer.ShippingAddress = address;
                customer.BillingAddress = address;
                _customerService.UpdateCustomer(customer);


                //return shipping address info page
                var ShippingAddresses = PrepareShippingAddressModel();
                var ShippingMethodModel = PrepareShippingMethodModel(cart, customer.ShippingAddress);
                var OrderTotalModel = PrepareOrderTotalsModel(cart, true);
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "shipping-info"
                        //ShippingAdressUpdateHtml = this.RenderPartialViewToString("ShippingAddress", ShippingAddresses),
                        //ShippingMethodUpdateHtml = this.RenderPartialViewToString("OpcShippingMethods", ShippingMethodModel),
                        //OrderTotalHtml = this.RenderPartialViewToString("OrderTotals", OrderTotalModel)
                    },
                    success = true
                });
               
            }


            //If we got this far, something failed, redisplay form
            return Json( new {success = false});
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SavePaymentMethod(FormCollection form)
        {
            try
            {
                //validation

                var customer = Utility.CustomerId != 0 ? _customerService.GetCustomerById(Utility.CustomerId) : _workContext.CurrentCustomer;
                var cart = customer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();

                if (cart.Count == 0)
                    throw new Exception("Cart is empty");
               
                if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                    throw new Exception("Anonymous checkout is not allowed");

                string paymentmethod = form["paymentmethod"];
                
                //payment method 
                if (String.IsNullOrEmpty(paymentmethod))
                    throw new Exception("Selected payment method can't be parsed");


                var model = new CheckoutPaymentMethodModel();
                TryUpdateModel(model);

                //reward points
                if (_rewardPointsSettings.Enabled)
                {
                    _genericAttributeService.SaveAttribute(customer,
                        SystemCustomerAttributeNames.UseRewardPointsDuringCheckout, model.UseRewardPoints,
                        _storeContext.CurrentStore.Id);
                }

                //Check whether payment workflow is required
                bool isPaymentWorkflowRequired = IsPaymentWorkflowRequired(cart);
                if (!isPaymentWorkflowRequired)
                {
                    //payment is not required
                    _genericAttributeService.SaveAttribute<string>(customer,
                        SystemCustomerAttributeNames.SelectedPaymentMethod, null, _storeContext.CurrentStore.Id);

                    var confirmOrderModel = PrepareConfirmOrderModel(cart);
                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "confirm-order",
                            PaymentInfoHtml = this.RenderPartialViewToString("AdminOpcConfirmOrder", confirmOrderModel)
                        },
                        goto_section = "confirm_order"
                    });
                }

                var paymentMethodInst = _paymentService.LoadPaymentMethodBySystemName(paymentmethod);
                if (paymentMethodInst == null ||
                    !paymentMethodInst.IsPaymentMethodActive(_paymentSettings) ||
                    !_pluginFinder.AuthenticateStore(paymentMethodInst.PluginDescriptor, _storeContext.CurrentStore.Id))
                    throw new Exception("Selected payment method can't be parsed");

                //save
                _genericAttributeService.SaveAttribute(customer,
                    SystemCustomerAttributeNames.SelectedPaymentMethod, paymentmethod, _storeContext.CurrentStore.Id);

                return OpcLoadStepAfterPaymentMethod(paymentMethodInst, cart);
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return Json(new { error = 1, message = exc.Message });
            }
        }

        [ValidateInput(false)]
        public ActionResult SavePaymentInformation(FormCollection form)
        {
            try
            {
                //validation
                var customer = Utility.CustomerId != 0 ? _customerService.GetCustomerById(Utility.CustomerId) : _workContext.CurrentCustomer;
                var cart = customer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();

                if (cart.Count == 0)
                    throw new Exception("Cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                    throw new Exception("Anonymous checkout is not allowed");

                var paymentMethodSystemName = customer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod,
                    _genericAttributeService, _storeContext.CurrentStore.Id);
                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(paymentMethodSystemName);
                if (paymentMethod == null)
                    throw new Exception("Payment method is not selected");

                var paymentControllerType = paymentMethod.GetControllerType();
                var paymentController = DependencyResolver.Current.GetService(paymentControllerType) as BasePaymentController;
                if (paymentController == null)
                    throw new Exception("Payment controller cannot be loaded");

                var warnings = paymentController.ValidatePaymentForm(form);
                foreach (var warning in warnings)
                    ModelState.AddModelError("", warning);
                if (ModelState.IsValid)
                {
                    //get payment info
                    var paymentInfo = paymentController.GetPaymentInfo(form);
                    //session save
                    _httpContext.Session["OrderPaymentInfo"] = paymentInfo;

                    var confirmOrderModel = PrepareConfirmOrderModel(cart);
                    bool warings = Convert.ToBoolean(confirmOrderModel.Warnings.Count);
                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "confirm-order",
                            PaymentInfoHtml = this.RenderPartialViewToString("AdminOpcConfirmOrder", confirmOrderModel),
                            warning = warings
                        },
                        goto_section = "confirm_order"
                    });
                }

                //If we got this far, something failed, redisplay form
                var paymenInfoModel = PreparePaymentInfoModel(paymentMethod);
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "payment-info",
                        PaymentInfoHtml = this.RenderPartialViewToString("AdminOpcPaymentInfo", paymenInfoModel)
                    }
                });
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return Json(new { error = 1, message = exc.Message });
            }
        }
        
        [ValidateInput(false)]
        public ActionResult SaveConfirmOrder()
        {
            try
            {
                //validation
                var customer = Utility.CustomerId != 0 ? _customerService.GetCustomerById(Utility.CustomerId) : _workContext.CurrentCustomer;
                var cart = customer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                if (cart.Count == 0)
                    throw new Exception("Cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                    throw new Exception("Anonymous checkout is not allowed");

                //prevent 2 orders being placed within an X seconds time frame
                if (!IsMinimumOrderPlacementIntervalValid(customer))
                    throw new Exception(_localizationService.GetResource("Checkout.MinOrderPlacementInterval"));

                //place order
                var processPaymentRequest = _httpContext.Session["OrderPaymentInfo"] as ProcessPaymentRequest;
                if (processPaymentRequest == null)
                {
                    //Check whether payment workflow is required
                    if (IsPaymentWorkflowRequired(cart))
                    {
                        throw new Exception("Payment information is not entered");
                    }
                    else
                        processPaymentRequest = new ProcessPaymentRequest();
                }

                processPaymentRequest.StoreId = _storeContext.CurrentStore.Id;
                processPaymentRequest.CustomerId = Utility.CustomerId;
                processPaymentRequest.PaymentMethodSystemName = customer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod,
                    _genericAttributeService, _storeContext.CurrentStore.Id);         

                var placeOrderResult = _orderProcessingServiceOnePageCheckOut.PlaceOrder(processPaymentRequest,customer);
                _logger.InsertLog(LogLevel.Warning, shortMessage: "Error", fullMessage:placeOrderResult.ToString());
                if (placeOrderResult.Success)
                {
                    _httpContext.Session["OrderPaymentInfo"] = null;
                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = placeOrderResult.PlacedOrder
                    };

                    var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(placeOrderResult.PlacedOrder.PaymentMethodSystemName);
                    if (paymentMethod == null)
                        //payment method could be null if order total is 0
                        //success
                        return Json(new { success = 1 });

                    if (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection)
                    {
                        //Redirection will not work because it's AJAX request.
                        //That's why we don't process it here (we redirect a user to another page where he'll be redirected)

                        //redirect
                        return Json(new
                        {
                            success = 2 ,
                            redirect = string.Format("{0}checkout/OpcCompleteRedirectionPayment", _webHelper.GetStoreLocation())
                        }, JsonRequestBehavior.AllowGet);
                    }

                    _paymentService.PostProcessPayment(postProcessPaymentRequest);
                    //success
                    return Json(new { success = 1 },JsonRequestBehavior.AllowGet);
                    //return RedirectToRoute("CheckoutCompleted", new { orderId = placeOrderResult.PlacedOrder.Id });
                }

                //error
                var confirmOrderModel = new CheckoutConfirmModel();
                foreach (var error in placeOrderResult.Errors)
                    confirmOrderModel.Warnings.Add(error);

                return Json(new
                {
                    success = 0,
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "confirm-order",
                        PaymentInfoHtml = this.RenderPartialViewToString("AdminOpcConfirmOrder", confirmOrderModel)
                    },
                    goto_section = "confirm_order"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return Json(new { error = 1, message = exc.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult UpdateCart(FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("HomePage");

            //var cart = _workContext.CurrentCustomer.ShoppingCartItems
            //    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
            //    .LimitPerStore(_storeContext.CurrentStore.Id)
            //    .ToList();
            
            var customer = Utility.CustomerId != 0 ? _customerService.GetCustomerById(Utility.CustomerId) : _workContext.CurrentCustomer;
            
            var cart = customer.ShoppingCartItems
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
                                var currSciWarnings = _shoppingCartService.UpdateShoppingCartItem(customer,
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
            cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var shoppingCartModel = new ShoppingCartModel();

            PrepareShoppingCartModel(shoppingCartModel, cart,false);
            //update current warnings
            foreach (var kvp in innerWarnings)
            {
                //kvp = <cart item identifier, warnings>
                var sciId = kvp.Key;
                var warnings = kvp.Value;
                //find model
                var sciModel = shoppingCartModel.Items.FirstOrDefault(x => x.Id == sciId);
                if (sciModel != null)
                    foreach (var w in warnings)
                        if (!sciModel.Warnings.Contains(w))
                            sciModel.Warnings.Add(w);
            }
            PrepareShippingMethodModel(cart, customer.ShippingAddress);
            var OrderTotalModel = PrepareOrderTotalsModel(cart, true);

            //if (cart.Count > 0)
            //{
                return Json(new
                {
                    success = 1,
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "shopping-cart-info",
                        ShoppigCartHtml = this.RenderPartialViewToString("AdminOPCOrderSummary", shoppingCartModel),
                        OrderTotalHtml = this.RenderPartialViewToString("AdminOPCOrderTotals", OrderTotalModel)

                    }
                });
            //}
            //else
            //{
            //    return Json(new
            //    {
            //        success = 0
            //    });  
            //}
            //return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult getShoppingCartHtml()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("HomePage");

            var customer = Utility.CustomerId != 0 ? _customerService.GetCustomerById(Utility.CustomerId) : _workContext.CurrentCustomer;
            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();                      

            //updated cart
            
            var shoppingCartModel = new ShoppingCartModel();

            PrepareShoppingCartModel(shoppingCartModel, cart, false);
       
            PrepareShippingMethodModel(cart, customer.ShippingAddress);
            var OrderTotalModel = PrepareOrderTotalsModel(cart, true);

            if (cart.Count > 0)
            {
                return Json(new
                {
                    success = 1,
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "shopping-cart-info",
                        ShoppigCartHtml = this.RenderPartialViewToString("AdminOPCOrderSummary", shoppingCartModel),
                        OrderTotalHtml = this.RenderPartialViewToString("AdminOPCOrderTotals", OrderTotalModel)
                    }
                });
            }
            else
            {
                return Json(new
                {
                    success = 0
                });
            }
            //return View(model);
        }

        public ActionResult AddNewCustomerPopup()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return new HttpUnauthorizedResult();

            var model = new CustomerModel();
            PrepareCustomerModel(model, null, false);
            //default value
            model.Active = true;
            return View("~/Plugins/Misc.OnePageCheckOutAdmin/Views/MiscOnePageCheckOutAdmin/AddNewCustomerPopup.cshtml", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddNewCustomerPopup(CustomerModel model, bool? continueEditing, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return new HttpUnauthorizedResult();

            if (!String.IsNullOrWhiteSpace(model.Email))
            {
                var cust2 = _customerService.GetCustomerByEmail(model.Email);
                if (cust2 != null)
                    ModelState.AddModelError("", "Email is already registered");
            }
            if (!String.IsNullOrWhiteSpace(model.Username) & _customerSettings.UsernamesEnabled)
            {
                var cust2 = _customerService.GetCustomerByUsername(model.Username);
                if (cust2 != null)
                    ModelState.AddModelError("", "Username is already registered");
            }

            //validate customer roles
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            var newCustomerRoles = new List<CustomerRole>();
            model.SelectedCustomerRoleIds = new List<int>() {3};
            foreach (var customerRole in allCustomerRoles)
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                    newCustomerRoles.Add(customerRole);
            var customerRolesError = ValidateCustomerRoles(newCustomerRoles);
            if (!String.IsNullOrEmpty(customerRolesError))
            {
                ModelState.AddModelError("", customerRolesError);
                ErrorNotification(customerRolesError, false);
            }

            // Ensure that valid email address is entered if Registered role is checked to avoid registered customers with empty email address
            if (newCustomerRoles.Any() && newCustomerRoles.FirstOrDefault(c => c.SystemName == SystemCustomerRoleNames.Registered) != null && !CommonHelper.IsValidEmail(model.Email))
            {
                ModelState.AddModelError("", "Valid Email is required for customer to be in 'Registered' role");
                ErrorNotification("Valid Email is required for customer to be in 'Registered' role", false);
            }

            if (ModelState.IsValid)
            {
                var customer = new Customer
                {
                    CustomerGuid = Guid.NewGuid(),
                    Email = model.Email,
                    Username = model.Username,
                    VendorId = model.VendorId,
                    AdminComment = model.AdminComment,
                    IsTaxExempt = model.IsTaxExempt,
                    Active = model.Active,
                    CreatedOnUtc = DateTime.UtcNow,
                    LastActivityDateUtc = DateTime.UtcNow,
                };
                _customerService.InsertCustomer(customer);

                //form fields
                if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.TimeZoneId, model.TimeZoneId);
                if (_customerSettings.GenderEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Gender, model.Gender);
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
                if (_customerSettings.DateOfBirthEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.DateOfBirth, model.DateOfBirth);
                if (_customerSettings.CompanyEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Company, model.Company);
                if (_customerSettings.StreetAddressEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress, model.StreetAddress);
                if (_customerSettings.StreetAddress2Enabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress2, model.StreetAddress2);
                if (_customerSettings.ZipPostalCodeEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.ZipPostalCode, model.ZipPostalCode);
                if (_customerSettings.CityEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.City, model.City);
                if (_customerSettings.CountryEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CountryId, model.CountryId);
                if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StateProvinceId, model.StateProvinceId);
                if (_customerSettings.PhoneEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Phone, model.Phone);
                if (_customerSettings.FaxEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Fax, model.Fax);

                //custom customer attributes
                var customerAttributes = ParseCustomCustomerAttributes(customer, form);
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CustomCustomerAttributes, customerAttributes);

                var defaultAddress = new Address
                {
                    FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName),
                    LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName),
                    Email = customer.Email,
                    Company = customer.GetAttribute<string>(SystemCustomerAttributeNames.Company),
                    CountryId = customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId) > 0 ?
                        (int?)customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId) : null,
                    StateProvinceId = customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId) > 0 ?
                        (int?)customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId) : null,
                    City = customer.GetAttribute<string>(SystemCustomerAttributeNames.City),
                    Address1 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress),
                    Address2 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress2),
                    ZipPostalCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.ZipPostalCode),
                    PhoneNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone),
                    FaxNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Fax),
                    CreatedOnUtc = customer.CreatedOnUtc
                };
                if (this._addressService.IsAddressValid(defaultAddress))
                {
                    //some validation
                    if (defaultAddress.CountryId == 0)
                        defaultAddress.CountryId = null;
                    if (defaultAddress.StateProvinceId == 0)
                        defaultAddress.StateProvinceId = null;
                    //set default address
                    customer.Addresses.Add(defaultAddress);
                    customer.BillingAddress = defaultAddress;
                    customer.ShippingAddress = defaultAddress;
                    _customerService.UpdateCustomer(customer);
                }

                //newsletter subscriptions
                if (!String.IsNullOrEmpty(customer.Email))
                {
                    var allStores = _storeService.GetAllStores();
                    foreach (var store in allStores)
                    {
                        var newsletterSubscription = _newsLetterSubscriptionService
                            .GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, store.Id);
                        if (model.SelectedNewsletterSubscriptionStoreIds != null &&
                            model.SelectedNewsletterSubscriptionStoreIds.Contains(store.Id))
                        {
                            //subscribed
                            if (newsletterSubscription == null)
                            {
                                _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = customer.Email,
                                    Active = true,
                                    StoreId = store.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                            }
                        }
                        else
                        {
                            //not subscribed
                            if (newsletterSubscription != null)
                            {
                                _newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletterSubscription);
                            }
                        }
                    }
                }

                //password
                if (!String.IsNullOrWhiteSpace(model.Password))
                {
                    var changePassRequest = new ChangePasswordRequest(model.Email, false, _customerSettings.DefaultPasswordFormat, model.Password);
                    var changePassResult = _customerRegistrationService.ChangePassword(changePassRequest);
                    if (!changePassResult.Success)
                    {
                        foreach (var changePassError in changePassResult.Errors)
                            ErrorNotification(changePassError);
                    }
                }

                //customer roles
                foreach (var customerRole in newCustomerRoles)
                {
                    //ensure that the current customer cannot add to "Administrators" system role if he's not an admin himself
                    if (customerRole.SystemName == SystemCustomerRoleNames.Administrators &&
                        !_workContext.CurrentCustomer.IsAdmin())
                        continue;

                    customer.CustomerRoles.Add(customerRole);
                }
                _customerService.UpdateCustomer(customer);


                //ensure that a customer with a vendor associated is not in "Administrators" role
                //otherwise, he won't have access to other functionality in admin area
                if (customer.IsAdmin() && customer.VendorId > 0)
                {
                    customer.VendorId = 0;
                    _customerService.UpdateCustomer(customer);
                    ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.AdminCouldNotbeVendor"));
                }

                //ensure that a customer in the Vendors role has a vendor account associated.
                //otherwise, he will have access to ALL products
                if (customer.IsVendor() && customer.VendorId == 0)
                {
                    var vendorRole = customer
                        .CustomerRoles
                        .FirstOrDefault(x => x.SystemName == SystemCustomerRoleNames.Vendors);
                    customer.CustomerRoles.Remove(vendorRole);
                    _customerService.UpdateCustomer(customer);
                    ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.CannotBeInVendoRoleWithoutVendorAssociated"));
                }

                //activity log
                _customerActivityService.InsertActivity("AddNewCustomer", _localizationService.GetResource("ActivityLog.AddNewCustomer"), customer.Id);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Added"));

                
                //return RedirectToAction("List");
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            PrepareCustomerModel(model, null, true);
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]        
        public ActionResult ApplyDiscountCoupon(string discountcouponcode)
        {
            //trim
            if (discountcouponcode != null)
                discountcouponcode = discountcouponcode.Trim();

            //cart
            //var cart = _workContext.CurrentCustomer.ShoppingCartItems
            //    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
            //    .LimitPerStore(_storeContext.CurrentStore.Id)
            //    .ToList();

            var customer = Utility.CustomerId != 0 ? _customerService.GetCustomerById(Utility.CustomerId) : _workContext.CurrentCustomer;

            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //parse and save checkout attributes
            //ParseAndSaveCheckoutAttributes(cart, form);

            var shoppingCartModel = new ShoppingCartModel();
            if (!String.IsNullOrWhiteSpace(discountcouponcode))
            {
                //we find even hidden records here. this way we can display a user-friendly message if it's expired
                var discount = _discountService.GetDiscountByCouponCode(discountcouponcode, true);
                if (discount != null && discount.RequiresCouponCode)
                {
                    var validationResult = _discountService.ValidateDiscount(discount, customer, discountcouponcode);
                    if (validationResult.IsValid)
                    {
                        //valid
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.DiscountCouponCode, discountcouponcode);
                        shoppingCartModel.DiscountBox.Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.Applied");
                        shoppingCartModel.DiscountBox.IsApplied = true;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(validationResult.UserError))
                        {
                            //some user error
                            shoppingCartModel.DiscountBox.Message = validationResult.UserError;
                            shoppingCartModel.DiscountBox.IsApplied = false;
                        }
                        else
                        {
                            //general error text
                            shoppingCartModel.DiscountBox.Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount");
                            shoppingCartModel.DiscountBox.IsApplied = false;
                        }
                    }
                }
                else
                {
                    //discount cannot be found
                    shoppingCartModel.DiscountBox.Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount");
                    shoppingCartModel.DiscountBox.IsApplied = false;
                }
            }
            else
            {
                //empty coupon code
                shoppingCartModel.DiscountBox.Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount");
                shoppingCartModel.DiscountBox.IsApplied = false;
            }                        

            PrepareShoppingCartModel(shoppingCartModel, cart, false);
            cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var OrderTotalModel = PrepareOrderTotalsModel(cart, true);

            //if (cart.Count > 0)
            //{
            return Json(new
            {
                success = 1,
                update_section = new UpdateSectionJsonModel
                {
                    name = "shopping-cart-info",
                    ShoppigCartHtml = this.RenderPartialViewToString("AdminOPCOrderSummary", shoppingCartModel),
                    OrderTotalHtml = this.RenderPartialViewToString("AdminOPCOrderTotals", OrderTotalModel),
                    DiscountCodeHtml = this.RenderPartialViewToString("_DiscountBox", shoppingCartModel.DiscountBox)
                }
            });
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult RemoveDiscountCoupon()
        {
            var customer = Utility.CustomerId != 0 ? _customerService.GetCustomerById(Utility.CustomerId) : _workContext.CurrentCustomer;

            var cart = customer.ShoppingCartItems
               .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
               .LimitPerStore(_storeContext.CurrentStore.Id)
               .ToList();

            var shoppingCartModel = new ShoppingCartModel();

            _genericAttributeService.SaveAttribute<string>(customer,
                SystemCustomerAttributeNames.DiscountCouponCode, null);

            PrepareShoppingCartModel(shoppingCartModel, cart);

            var OrderTotalModel = PrepareOrderTotalsModel(cart, true);

            return Json(new
            {
                success = 1,
                update_section = new UpdateSectionJsonModel
                {
                    name = "shopping-cart-info",                    
                    OrderTotalHtml = this.RenderPartialViewToString("AdminOPCOrderTotals", OrderTotalModel),
                    DiscountCodeHtml = this.RenderPartialViewToString("_DiscountBox", shoppingCartModel.DiscountBox)
                }
            });
        }
        #endregion

        #region customerautosearch
        public ActionResult CustomerSearchAutoComplete(string term)
        {
            if (String.IsNullOrWhiteSpace(term) || term.Length < _catalogSettings.ProductSearchTermMinimumLength)
                return Content("");

            //products
            var productNumber = _catalogSettings.ProductSearchAutoCompleteNumberOfProducts > 0 ?
                _catalogSettings.ProductSearchAutoCompleteNumberOfProducts : 10;

            var customers = _bsCustomerService.GetAllCustomers(
                keywords: term,
                pageSize: productNumber).ToList();
            var result = (from p in customers
                          select new
                          {
                              label = p.Email,
                              phone = p.GetAttribute<string>(SystemCustomerAttributeNames.Phone),
                              name = p.GetAttribute<string>(SystemCustomerAttributeNames.FirstName)+ p.GetAttribute<string>(SystemCustomerAttributeNames.LastName),
                              id = p.Id
                          })
                          .ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}   