using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Logging;
using Nop.Plugin.Misc.OnePageCheckOut.Infrastructure.Cache;
using Nop.Plugin.Misc.OnePageCheckOut.Models;
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
using Nop.Plugin.Misc.OnePageCheckOut.Models.Checkout;
using Nop.Plugin.Misc.OnePageCheckOut.Models.Common;
using Nop.Services.Common;
using Nop.Core.Domain.Shipping;
using Nop.Services.Shipping;
using Nop.Core.Domain.Discounts;
using System.Web.Routing;
using Nop.Plugin.Misc.OnePageCheckOut.Extensions;
using Nop.Core.Plugins;
using Nop.Plugin.Misc.OnePageCheckOut.Models.ShoppingCart;
using Nop.Services.Discounts;
using Nop.Core.Domain.Media;
using System.Web;
using Nop.Core.Domain.Directory;
using System.Globalization;
using System.Text.RegularExpressions;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.SSLCommerzEmi;
using Nop.Services.Configuration;

namespace Nop.Plugin.Misc.OnePageCheckOut.Controllers
{
    [NopHttpsRequirement(SslRequirement.NoMatter)]
    public class MiscOnePageCheckOutController : BasePluginController
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
        private readonly ICustomerLedgerMasterService _customerLedgerMasterService;
        private readonly ICustomerLedgerDetailService _customerLedgerDetailService;



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
        private readonly IStoreService _storeService;
        //private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly int filterByCountryId = 186;
        #endregion

        #region Ctor

        public MiscOnePageCheckOutController(IAclService aclService,
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
            IStoreService storeService,
            ICustomerLedgerMasterService customerLedgerMasterService,
            ICustomerLedgerDetailService customerLedgerDetailService
            )
        {
            _aclService = aclService;
            _cacheManager = cacheManager;
            _catalogSettings = catalogSettings;
            _categoryService = categoryService;
            _currencyService = currencyService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _priceCalculationService = priceCalculationService;
            _priceFormatter = priceFormatter;
            _productService = productService;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _taxService = taxService;
            _rewardPointService = rewardPointService;


            _workContext = workContext;
            _orderSettings = orderSettings;
            _stateProvinceService = stateProvinceService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _addressSettings = addressSettings;
            _addressAttributeFormatter = addressAttributeFormatter;
            _addressAttributeService = addressAttributeService;
            _addressAttributeParser = addressAttributeParser;
            _shippingSettings = shippingSettings;
            _shippingService = shippingService;
            _rewardPointsSettings = rewardPointsSettings;
            _genericAttributeService = genericAttributeService;
            _paymentService = paymentService;
            _webHelper = webHelper;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _countryService = countryService;

            _productAttributeParser = productAttributeParser;
            _productAttributeFormatter = productAttributeFormatter;
            _shoppingCartService = shoppingCartService;
            _discountService = discountService;
            _checkoutAttributeService = checkoutAttributeService;
            _mediaSettings = mediaSettings;
            _httpContext = httpContext;
            _checkoutAttributeFormatter = checkoutAttributeFormatter;
            _checkoutAttributeParser = checkoutAttributeParser;
            _shoppingCartSettings = shoppingCartSettings;
            _customerService = customerService;
            _taxSettings = taxSettings;
            _paymentSettings = paymentSettings;
            _pluginFinder = pluginFinder;
            _logger = logger;
            _settingService = settingService;
            _storeService = storeService;

            _customerLedgerMasterService = customerLedgerMasterService;
            _customerLedgerDetailService = customerLedgerDetailService;
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
        public ActionResult GetDelivaryChargeByStateProvinceId(int stProvinceId)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var countryId = GetBangladeshCountryId();
            //if (!string.IsNullOrEmpty(stProvinceName))
            //{
            //    var states = _stateProvinceService.GetStateProvincesByCountryId(countryId);
            //    if (states.Any())
            //    {
            //        var stateProvince = states.FirstOrDefault(s => s.Id == da);
            //        if (stateProvince != null)
            //            stateProvinceId = stateProvince.Id;
            //    }

            //}

            decimal rate = decimal.Zero;
            var getShippingOptionResponse = _shippingService
                 .GetShippingOptionsCustom(cart, null,
                 "", _storeContext.CurrentStore.Id,
                    countryId, stProvinceId);
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

                    _genericAttributeService.SaveAttribute<ShippingOption>(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedShippingOption, shippingOption, _storeContext.CurrentStore.Id);
                }
            }

            //emi values
            //TODO:: this code need to refactor
            var paymentMethods = _paymentService
                .LoadActivePaymentMethods(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id, filterByCountryId)
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Standard || pm.PaymentMethodType == PaymentMethodType.Redirection)
                .Where(pm => !pm.HidePaymentMethod(cart))
                .ToList();

            string emiOption3Amount = "";
            string emiOption6Amount = "";
            string emiOption9Amount = "";
            string emiOption12Amount = "";
            string emiOption18Amount = "";
            string emiOption24Amount = "";
            bool hasEMIPaymentMethod = false;

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
                if (pm.PluginDescriptor.SystemName == "Payments.SSLCommerzEmi")
                {
                    PrepareSSLEmiPaymentMethodModel(pm, cart, pmModel, countryId, stProvinceId);

                    emiOption3Amount = pmModel.EmiThreeMonthFee;
                    emiOption6Amount = pmModel.EmiSixMonthFee;
                    emiOption9Amount = pmModel.EmiNineMonthFee;
                    emiOption12Amount = pmModel.EmiTwelveMonthFee;
                    emiOption18Amount = pmModel.EmiEighteenMonthFee;
                    emiOption24Amount = pmModel.EmiTwentyFourMonthFee;

                    hasEMIPaymentMethod = true;

                    break;
                }
            }

            if (cart.Count > 0)
            {
                return Json(new
                {
                    success = 1,
                    rate,
                    hasEMIPaymentMethod,
                    emiOption3Amount,
                    emiOption6Amount,
                    emiOption9Amount,
                    emiOption12Amount,
                    emiOption18Amount,
                    emiOption24Amount
                });
            }
            else
            {
                return Json(new
                {
                    success = 0
                });
            }
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

            #region wallet payment
            decimal totalPayableFromWallet = 0;

            if (_httpContext.Session["OrderWalletPaymentInfo"] is Dictionary<string, object> OrderWalletPaymentInfo && OrderWalletPaymentInfo.ContainsKey("willDeduct"))
            {
                if (Convert.ToBoolean(OrderWalletPaymentInfo["willDeduct"]))
                {
                    totalPayableFromWallet = Convert.ToDecimal(OrderWalletPaymentInfo["walletPayAmount"]);
                }
            }

            #endregion

            if (shoppingCartTotalBase.HasValue && shoppingCartTotalBase.Value - totalPayableFromWallet == decimal.Zero)
                result = false;

            return result;
        }

        [NonAction]
        protected virtual CheckoutBillingAddressModel PrepareBillingAddressModel(int? selectedCountryId = null,
            bool prePopulateNewAddressWithCustomerFields = false)
        {
            var model = new CheckoutBillingAddressModel();
            //existing addresses
            var addresses = _workContext.CurrentCustomer.Addresses
                //allow billing
                .Where(a => a.Country == null || a.Country.AllowsBilling)
                //enabled for the current store
                .Where(a => a.StateProvince != null && a.StateProvince.Published)
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
                customer: _workContext.CurrentCustomer);
            return model;
        }

        [NonAction]
        protected virtual CheckoutShippingAddressModel PrepareShippingAddressModel(int? selectedCountryId = null,
            bool prePopulateNewAddressWithCustomerFields = false)
        {
            var model = new CheckoutShippingAddressModel();
            //allow pickup in store?
            model.AllowPickUpInStore = _shippingSettings.AllowPickUpInStore;

            if (model.AllowPickUpInStore)
            {
                model.DisplayPickupPointsOnMap = _shippingSettings.DisplayPickupPointsOnMap;
                model.GoogleMapsApiKey = _shippingSettings.GoogleMapsApiKey;
                var pickupPointProviders = _shippingService.LoadActivePickupPointProviders(_storeContext.CurrentStore.Id);
                if (pickupPointProviders.Any())
                {
                    var pickupPointsResponse = _shippingService.GetPickupPoints(_workContext.CurrentCustomer.BillingAddress, null, _storeContext.CurrentStore.Id);
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
                                var amount = _taxService.GetShippingPrice(x.PickupFee, _workContext.CurrentCustomer);
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
            var addresses = _workContext.CurrentCustomer.Addresses
                //allow shipping
                .Where(a => a.Country == null || a.Country.AllowsShipping)
                //enabled for the current store
                .Where(a => a.StateProvince != null && a.StateProvince.Published)
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
                customer: _workContext.CurrentCustomer);


            //customize code for selectedshipping address
            if (_workContext.CurrentCustomer.ShippingAddress != null)
            {
                if (_workContext.CurrentCustomer.ShippingAddress.StateProvince == null ||
                    !_workContext.CurrentCustomer.ShippingAddress.StateProvince.Published)
                {
                    _workContext.CurrentCustomer.RemoveAddress(_workContext.CurrentCustomer.ShippingAddress);
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                }
                else
                {
                    model.SelectedShippingAdressId = _workContext.CurrentCustomer.ShippingAddress.Id;
                }
            }

            return model;
        }

        [NonAction]
        protected virtual CheckoutShippingMethodModel PrepareShippingMethodModel(IList<ShoppingCartItem> cart, Address shippingAddress)
        {
            var model = new CheckoutShippingMethodModel();

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
                var addressCurrentCustomerShipping = _workContext.CurrentCustomer.ShippingAddress;
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
                #endregion

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
        protected virtual CheckoutPaymentMethodModel PreparePaymentMethodModel(IList<ShoppingCartItem> cart, int filterByCountryId)
        {
            var model = new CheckoutPaymentMethodModel();

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

                var pmModel = new CheckoutPaymentMethodModel.PaymentMethodModel
                {
                    Name = pm.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id),
                    PaymentMethodSystemName = pm.PluginDescriptor.SystemName,
                    LogoUrl = pm.PluginDescriptor.GetLogoUrl(_webHelper)
                };
                //payment method additional fee


                decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, pm.PluginDescriptor.SystemName);
                if (pm.PluginDescriptor.SystemName == "Payments.SSLCommerzEmi")
                    PrepareSSLEmiPaymentMethodModel(pm, cart, pmModel);
                decimal rateBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, _workContext.CurrentCustomer);
                decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                if (rate > decimal.Zero)
                    pmModel.Fee = _priceFormatter.FormatPaymentMethodAdditionalFee(rate, true);

                model.PaymentMethods.Add(pmModel);
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

            return model;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterByCountryId"></param>
        /// <returns></returns>
        [NonAction]
        protected virtual CheckoutPaymentMethodModel PreparePaymentMethodForMakePaymetModel(int filterByCountryId)
        {
            var model = new CheckoutPaymentMethodModel();

            //filter by country
            var paymentMethods = _paymentService
                .LoadActivePaymentMethods(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id, filterByCountryId)
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Standard || pm.PaymentMethodType == PaymentMethodType.Redirection)

                .ToList();
            foreach (var pm in paymentMethods)
            {

                //if (pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                //    continue;
                if (pm.PluginDescriptor.SystemName.ToUpper() == "PAYMENTS.CASHONDELIVERY")
                {
                }
                else if (pm.PluginDescriptor.SystemName.ToUpper() == "PAYMENTS.NEXUSPAY")
                {
                }
                else
                {

                    var pmModel = new CheckoutPaymentMethodModel.PaymentMethodModel
                    {
                        Name = pm.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id),
                        PaymentMethodSystemName = pm.PluginDescriptor.SystemName,
                        LogoUrl = pm.PluginDescriptor.GetLogoUrl(_webHelper)
                    };

                  

                    model.PaymentMethods.Add(pmModel);
                }
                
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

            return model;
        }


        protected virtual void PrepareSSLEmiPaymentMethodModel(IPaymentMethod pm, IList<ShoppingCartItem> cart,
            CheckoutPaymentMethodModel.PaymentMethodModel pmModel, int countryId = 0, int stateProvinenceId = 0)
        {
            //TODO:: These code need to be optimized

            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var sSlCommerzEmiPaymentSettings = _settingService.LoadSetting<SSLCommerzEmiPaymentSettings>(storeScope);
            var orderTotalWithoutPaymentFee = _orderTotalCalculationService.GetShoppingCartTotalCustom(cart, usePaymentMethodAdditionalFee: false,
                countryId: countryId, stateProvinenceId: stateProvinenceId);
            var result3 = pm.CalculateAdditionalFee(_orderTotalCalculationService, cart, sSlCommerzEmiPaymentSettings.AdditionalFeeThreeMonthOption, true);
            var finalresult3 = (result3 + orderTotalWithoutPaymentFee) / 3;
            if (finalresult3 != null)
                result3 = (decimal)finalresult3;
            decimal rateBase3 = _taxService.GetPaymentMethodAdditionalFee(result3, _workContext.CurrentCustomer);
            decimal rate3 = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase3, _workContext.WorkingCurrency);
            if (rate3 > decimal.Zero)
                pmModel.EmiThreeMonthFee = _priceFormatter.FormatPaymentMethodAdditionalFee(rate3, true);
            var result6 = pm.CalculateAdditionalFee(_orderTotalCalculationService, cart, sSlCommerzEmiPaymentSettings.AdditionalFeeSixMonthOption, true);
            var finalresult6 = (result6 + orderTotalWithoutPaymentFee) / 6;
            if (finalresult6 != null)
                result6 = (decimal)finalresult6;
            decimal rateBase6 = _taxService.GetPaymentMethodAdditionalFee(result6, _workContext.CurrentCustomer);
            decimal rate6 = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase6, _workContext.WorkingCurrency);
            if (rate6 > decimal.Zero)
                pmModel.EmiSixMonthFee = _priceFormatter.FormatPaymentMethodAdditionalFee(rate6, true);

            var result9 = pm.CalculateAdditionalFee(_orderTotalCalculationService, cart, sSlCommerzEmiPaymentSettings.AdditionalFeeNineMonthOption, true);
            var finalresult9 = (result9 + orderTotalWithoutPaymentFee) / 9;
            if (finalresult9 != null)
                result9 = (decimal)finalresult9;
            decimal rateBase9 = _taxService.GetPaymentMethodAdditionalFee(result9, _workContext.CurrentCustomer);
            decimal rate9 = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase9, _workContext.WorkingCurrency);
            if (rate9 > decimal.Zero)
                pmModel.EmiNineMonthFee = _priceFormatter.FormatPaymentMethodAdditionalFee(rate9, true);

            var result12 = pm.CalculateAdditionalFee(_orderTotalCalculationService, cart, sSlCommerzEmiPaymentSettings.AdditionalFeeTwelveMonthOption, true);
            var finalresult12 = (result12 + orderTotalWithoutPaymentFee) / 12;
            if (finalresult12 != null)
                result12 = (decimal)finalresult12;
            decimal rateBase12 = _taxService.GetPaymentMethodAdditionalFee(result12, _workContext.CurrentCustomer);
            decimal rate12 = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase12, _workContext.WorkingCurrency);
            if (rate12 > decimal.Zero)
                pmModel.EmiTwelveMonthFee = _priceFormatter.FormatPaymentMethodAdditionalFee(rate12, true);

            var result18 = pm.CalculateAdditionalFee(_orderTotalCalculationService, cart, sSlCommerzEmiPaymentSettings.AdditionalFeeEighteenMonthOption, true);
            var finalresult18 = (result18 + orderTotalWithoutPaymentFee) / 18;
            if (finalresult18 != null)
                result18 = (decimal)finalresult18;
            decimal rateBase18 = _taxService.GetPaymentMethodAdditionalFee(result18, _workContext.CurrentCustomer);
            decimal rate18 = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase18, _workContext.WorkingCurrency);
            if (rate18 > decimal.Zero)
                pmModel.EmiEighteenMonthFee = _priceFormatter.FormatPaymentMethodAdditionalFee(rate18, true);

            var result24 = pm.CalculateAdditionalFee(_orderTotalCalculationService, cart, sSlCommerzEmiPaymentSettings.AdditionalFeeTwentyFourMonthOption, true);
            var finalresult24 = (result24 + orderTotalWithoutPaymentFee) / 24;
            if (finalresult24 != null)
                result24 = (decimal)finalresult24;
            decimal rateBase24 = _taxService.GetPaymentMethodAdditionalFee(result24, _workContext.CurrentCustomer);
            decimal rate24 = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase24, _workContext.WorkingCurrency);
            if (rate24 > decimal.Zero)
                pmModel.EmiTwentyFourMonthFee = _priceFormatter.FormatPaymentMethodAdditionalFee(rate24, true);
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
        protected virtual CheckoutConfirmModel PrepareConfirmMakePaymentModel()
        {
            var model = new CheckoutConfirmModel();
            //terms of service
            model.TermsOfServiceOnOrderConfirmPage = _orderSettings.TermsOfServiceOnOrderConfirmPage;
            bool minTotalAmountOk = false;
            if (_workContext.CurrentCustomer.MakeAmount > 5)
            {
                minTotalAmountOk = true;
            }
            //min order amount validation
            bool minOrderTotalAmountOk = minTotalAmountOk; //_orderProcessingService.ValidateMinOrderTotalAmount(cart);
            if (!minOrderTotalAmountOk)
            {
                decimal minOrderTotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderTotalAmount, _workContext.WorkingCurrency);
                model.MinOrderTotalWarning = string.Format(_localizationService.GetResource("Checkout.MinOrderTotalAmount"), _priceFormatter.FormatPrice(minOrderTotalAmount, true, false));
            }
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

            model.OnePageCheckoutEnabled = _orderSettings.OnePageCheckoutEnabled;

            if (cart.Count == 0)
                return;

            #region Simple properties

            model.IsEditable = isEditable;
            model.ShowProductImages = _shoppingCartSettings.ShowProductImagesOnShoppingCart;
            model.ShowSku = _catalogSettings.ShowProductSku;
            var checkoutAttributesXml = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _genericAttributeService, _storeContext.CurrentStore.Id);
            model.CheckoutAttributeInfo = _checkoutAttributeFormatter.FormatAttributes(checkoutAttributesXml, _workContext.CurrentCustomer);
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
            var discountCouponCode = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.DiscountCouponCode);
            var discount = _discountService.GetDiscountByCouponCode(discountCouponCode);
            if (discount != null &&
                discount.RequiresCouponCode &&
               _discountService.ValidateDiscount(discount, _workContext.CurrentCustomer).IsValid)
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

            #region Estimate shipping

            if (prepareEstimateShippingIfEnabled)
            {
                model.EstimateShipping.Enabled = cart.Count > 0 && cart.RequiresShipping() && _shippingSettings.EstimateShippingEnabled;
                if (model.EstimateShipping.Enabled)
                {
                    //countries
                    int? defaultEstimateCountryId = setEstimateShippingDefaultAddress && _workContext.CurrentCustomer.ShippingAddress != null ? _workContext.CurrentCustomer.ShippingAddress.CountryId : model.EstimateShipping.CountryId;
                    model.EstimateShipping.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                    foreach (var c in _countryService.GetAllCountriesForShipping())
                        model.EstimateShipping.AvailableCountries.Add(new SelectListItem
                        {
                            Text = c.GetLocalized(x => x.Name),
                            Value = c.Id.ToString(),
                            Selected = c.Id == defaultEstimateCountryId
                        });
                    //states
                    int? defaultEstimateStateId = setEstimateShippingDefaultAddress && _workContext.CurrentCustomer.ShippingAddress != null ? _workContext.CurrentCustomer.ShippingAddress.StateProvinceId : model.EstimateShipping.StateProvinceId;
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

                    if (setEstimateShippingDefaultAddress && _workContext.CurrentCustomer.ShippingAddress != null)
                        model.EstimateShipping.ZipPostalCode = _workContext.CurrentCustomer.ShippingAddress.ZipPostalCode;
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

            #region Button payment methods

            var paymentMethods = _paymentService
                .LoadActivePaymentMethods(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id)
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
                var billingAddress = _workContext.CurrentCustomer.BillingAddress;
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

                    var pickupPoint = _workContext.CurrentCustomer
                        .GetAttribute<PickupPoint>(SystemCustomerAttributeNames.SelectedPickupPoint, _storeContext.CurrentStore.Id);
                    model.OrderReviewData.SelectedPickUpInStore = _shippingSettings.AllowPickUpInStore && pickupPoint != null;
                    if (!model.OrderReviewData.SelectedPickUpInStore)
                    {
                        if (_workContext.CurrentCustomer.ShippingAddress != null)
                        {
                            model.OrderReviewData.ShippingAddress.PrepareModel(
                                address: _workContext.CurrentCustomer.ShippingAddress,
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
        protected virtual OrderTotalsModel PrepareMakeApymentTotalsModel(bool isEditable, string stProvinceName = "")
        {
            var model = new OrderTotalsModel();
            model.IsEditable = isEditable;


            //payment method fee
            var paymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod, _storeContext.CurrentStore.Id);

            model.PaymentMethodSystemName = paymentMethodSystemName;

            model.SubTotal = _workContext.CurrentCustomer.MakeAmount.ToString();
            model.OrderTotal = _workContext.CurrentCustomer.MakeAmount.ToString();


            return model;
        }


        [NonAction]
        protected virtual OrderTotalsModel PrepareOrderTotalsModel(IList<ShoppingCartItem> cart, bool isEditable, string stProvinceName = "")
        {
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
                    model.PaymentMethodSystemName = paymentMethodSystemName;
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
                        .CalculateRewardPoints(_workContext.CurrentCustomer, shoppingCartTotalBase.Value);
                }
                #region BuyOneGetOne
                if (_orderSettings.BuyOneGetOneEnable)
                {
                    if (_orderSettings.BuyOneGetOneCategoryId > 0 && _workContext.CurrentCustomer != null)
                    {
                        var categoryProductIds = _categoryService.GetProductCategoriesByCategoryId(_orderSettings.BuyOneGetOneCategoryId).Select(x => x.ProductId);
                        var cartAvilableProducts = 0;
                        decimal firstProductprice = 0;
                        bool isalreadyDeduct = false;
                        int cartitemCount = 0;
                        foreach (var categoryProductId in categoryProductIds)
                        {
                            foreach (var cartItem in cart)
                            {
                                if (cartItem.ProductId == categoryProductId)
                                {
                                    cartitemCount += 1;
                                    if (cartitemCount == 1)
                                    {
                                        firstProductprice = cartItem.Product.Price;
                                    }
                                    cartAvilableProducts += cartItem.Quantity;
                                    if (cartAvilableProducts >= 2)
                                    {
                                        if (firstProductprice == cartItem.Product.Price)
                                        {
                                            model.IsActivatedBuyOneGetOne = true;
                                            model.BuyOneGetOneDiscountAmount =
                                                _priceFormatter.FormatPrice(cartItem.Product.Price);
                                            isalreadyDeduct = true;
                                            break;
                                        }
                                    }

                                }
                                if (isalreadyDeduct)
                                {
                                    break;
                                }

                            }
                            if (isalreadyDeduct)
                                break;
                        }

                    }

                }

                #endregion BuyoneGetOne
            }

            return model;
        }

        [NonAction]
        protected JsonResult OpcLoadStepAfterPaymentMethodForMakePayment(IPaymentMethod paymentMethod)
        {
            if (paymentMethod.SkipPaymentInfo)
            {
                //skip payment info page
                var paymentInfo = new ProcessPaymentRequest();
                //session save
                _httpContext.Session["OrderPaymentInfo"] = paymentInfo;

                var confirmOrderModel = PrepareConfirmMakePaymentModel();
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "confirm-order",
                        PaymentInfoHtml = RenderPartialViewToString("OpcConfirmOrder", confirmOrderModel)
                    },
                    goto_section = "confirm_order",
                    isPaymentWorkflowRequired = true
                });
            }


            //return payment info page
            var paymenInfoModel = PreparePaymentInfoModel(paymentMethod);
            return Json(new
            {
                update_section = new UpdateSectionJsonModel
                {
                    name = "payment-info",

                    PaymentInfoHtml = ""
                },
                goto_section = "payment_info",
                isPaymentWorkflowRequired = true
            });
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
                        PaymentInfoHtml = RenderPartialViewToString("OpcConfirmOrder", confirmOrderModel)
                    },
                    goto_section = "confirm_order",
                    isPaymentWorkflowRequired = true
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
                goto_section = "payment_info",
                isPaymentWorkflowRequired = true
            });
        }
        #endregion

        #region Methods

        [ChildActionOnly]
        [AdminAuthorize]
        public ActionResult Configure()
        {
            return View("~/Plugins/Misc.FacebookShop/Views/MiscFacebookShop/Configure.cshtml");
        }

        #region MakePayment
        public ActionResult MakePaymentCheckout()
        {
            var model = new OnePageCheckoutModel(); 

            _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
            SystemCustomerAttributeNames.SelectedEmiOption, null, _storeContext.CurrentStore.Id);

            if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                return new HttpUnauthorizedResult();
            var OrderTotalModel = PrepareMakeApymentTotalsModel(true);
            var paymentMethodModel = PreparePaymentMethodForMakePaymetModel(filterByCountryId);
            model = new OnePageCheckoutModel
            {
                CheckoutPaymentMethod = paymentMethodModel,
                AllOrderTotalsModel = OrderTotalModel
            };

            //clear payment method if selected for initial load            
            _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.SelectedPaymentMethod, null, _storeContext.CurrentStore.Id);
            return View("~/Themes/Pavilion/Views/MiscOnePageCheckOut/MakePayment.cshtml", model);


        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SavePaymentMethodForMakePayment(FormCollection form)
        {
            try
            {

                if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                string paymentmethod = form["paymentmethod"];
                string paymentmethodemioption = form["emiOption"];
                //payment method 
                if (String.IsNullOrEmpty(paymentmethod))
                    throw new Exception("Selected payment method can't be parsed");


                var model = new CheckoutPaymentMethodModel();
                TryUpdateModel(model);

                //Check whether payment workflow is required
                bool isPaymentWorkflowRequired = true;
                if (!isPaymentWorkflowRequired)
                {
                    //payment is not required
                    _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.SelectedPaymentMethod, null, _storeContext.CurrentStore.Id);
                    //bs-23
                    _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.SelectedEmiOption, null, _storeContext.CurrentStore.Id);

                    var confirmOrderModel = PrepareConfirmMakePaymentModel();

                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "confirm-order",
                            PaymentInfoHtml = RenderPartialViewToString("OpcConfirmMakePayment", confirmOrderModel)
                        },
                        goto_section = "confirm_order",
                        isPaymentWorkflowRequired
                    });
                }

                var paymentMethodInst = _paymentService.LoadPaymentMethodBySystemName(paymentmethod);
                if (paymentMethodInst == null ||
                    !paymentMethodInst.IsPaymentMethodActive(_paymentSettings) ||
                    !_pluginFinder.AuthenticateStore(paymentMethodInst.PluginDescriptor, _storeContext.CurrentStore.Id))
                    throw new Exception("Selected payment method can't be parsed");

                //save
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                    SystemCustomerAttributeNames.SelectedPaymentMethod, paymentmethod, _storeContext.CurrentStore.Id);
                //bs-23
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                    SystemCustomerAttributeNames.SelectedEmiOption, paymentmethodemioption, _storeContext.CurrentStore.Id);
                return OpcLoadStepAfterPaymentMethodForMakePayment(paymentMethodInst);
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return Json(new { error = 1, message = exc.Message });
            }
        }


        [ValidateInput(false)]
        public ActionResult SavePaymentInformationForMakePayment(FormCollection form)
        {
            try
            {


                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                var paymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
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

                    var confirmOrderModel = PrepareConfirmMakePaymentModel();//PrepareConfirmOrderModel(cart);
                    bool warings = Convert.ToBoolean(confirmOrderModel.Warnings.Count);
                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "confirm-order",
                            PaymentInfoHtml = RenderPartialViewToString("OpcConfirmMakePayment", confirmOrderModel),
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
                        PaymentInfoHtml = RenderPartialViewToString("OpcPaymentInfo", paymenInfoModel)
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
        public ActionResult SaveConfirmPayment()
        {
            try
            {


                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                //prevent 2 orders being placed within an X seconds time frame
                if (!IsMinimumOrderPlacementIntervalValid(_workContext.CurrentCustomer))
                    throw new Exception(_localizationService.GetResource("Checkout.MinOrderPlacementInterval"));

                //place order
                var processPaymentRequest = _httpContext.Session["OrderPaymentInfo"] as ProcessPaymentRequest;
                if (processPaymentRequest == null)
                {
                    processPaymentRequest = new ProcessPaymentRequest();
                }

                processPaymentRequest.StoreId = _storeContext.CurrentStore.Id;
                processPaymentRequest.CustomerId = _workContext.CurrentCustomer.Id;
                processPaymentRequest.PaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod,
                    _genericAttributeService, _storeContext.CurrentStore.Id);



                var placeOrderResult = _orderProcessingService.PlaceMakepayment(processPaymentRequest, _workContext.CurrentCustomer.MakeAmount);

                if (placeOrderResult.Success)
                {
                    _httpContext.Session["OrderPaymentInfo"] = null;

                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = placeOrderResult.PlacedOrder
                    };

                    var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(placeOrderResult.PlacedOrder.PaymentMethodSystemName);
                    if (paymentMethod == null || placeOrderResult.PlacedOrder.PaymentStatus == PaymentStatus.Paid)
                    {
                        //payment method could be null if order total is 0
                        //success
                        return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
                    }

                    if (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection)
                    {
                        //Redirection will not work because it's AJAX request.
                        //That's why we don't process it here (we redirect a user to another page where he'll be redirected)

                        //redirect
                        return Json(new
                        {
                            success = 2
                            //redirect = string.Format("{0}checkout/OpcCompleteRedirectionPayment", _webHelper.GetStoreLocation())
                        }, JsonRequestBehavior.AllowGet);
                    }


                    _paymentService.PostProcessPayment(postProcessPaymentRequest);
                    //success
                    _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.SelectedEmiOption, null, _storeContext.CurrentStore.Id);
                    return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
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
                        PaymentInfoHtml = RenderPartialViewToString("OpcConfirmMakePayment", confirmOrderModel)
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

        [HttpPost]
        public ActionResult UpdateMakePaymentTotal(string paymentMethod, string stProvinceName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("HomePage");

            try
            {


                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                    SystemCustomerAttributeNames.SelectedPaymentMethod, paymentMethod, _storeContext.CurrentStore.Id);

                var OrderTotalModel = PrepareMakeApymentTotalsModel(true, stProvinceName);
                // var OrderTotalModel = PrepareOrderTotalsModel(cart,true, stProvinceName);
                // var showWalletPay = false;
                if (!string.IsNullOrEmpty(_workContext.CurrentCustomer.MakeAmount.ToString()))
                {
                    var orderTotal = Convert.ToDecimal(_workContext.CurrentCustomer.MakeAmount.ToString());

                    var orderSubTotal = Convert.ToDecimal(orderTotal);
                    OrderTotalModel.OrderTotal = _priceFormatter.FormatPrice(orderTotal, true, false);
                }



                return Json(new
                {
                    success = 1,
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "shopping-cart-info",
                        OrderTotalHtml = RenderPartialViewToString("MakePaymentTotals", OrderTotalModel)

                    },
                    // showWalletPay
                });

            }
            catch (Exception ex)
            {
                _logger.Information("Inside Wallet Region -> Update Total Order: \n" + ex.StackTrace, ex);
            }

            return Json(new
            {
                success = 0
                //success = 1,
            });
        }



        #endregion


        public ActionResult OnePageCheckout()
        {
            var model = new OnePageCheckoutModel();

            //validation
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
            SystemCustomerAttributeNames.SelectedEmiOption, null, _storeContext.CurrentStore.Id);


            if (cart.Count == 0)
                return RedirectToRoute("ShoppingCart");

            if (!_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("Checkout");

            if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                return new HttpUnauthorizedResult();


            var shoppingCartModel = new ShoppingCartModel();
            PrepareShoppingCartModel(shoppingCartModel, cart, false);
            CheckoutShippingMethodModel shippingMethodModel = null;
            if (_workContext.CurrentCustomer.ShippingAddress != null &&
                _workContext.CurrentCustomer.ShippingAddress.StateProvince != null &&
                _workContext.CurrentCustomer.ShippingAddress.StateProvince.Published)
            {
                shippingMethodModel = PrepareShippingMethodModel(cart, _workContext.CurrentCustomer.ShippingAddress);
            }
            else
            {
                _genericAttributeService.SaveAttribute<ShippingOption>(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedShippingOption, null, _storeContext.CurrentStore.Id);
            }
            var OrderTotalModel = PrepareOrderTotalsModel(cart, true);
            var paymentMethodModel = PreparePaymentMethodModel(cart, filterByCountryId);
            model = new OnePageCheckoutModel
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

            //cities
            //model.ShippingAddresses.NewAddress.AvailableCities = new List<SelectListItem>();
            //model.ShippingAddresses.NewAddress.AvailableCities.Add(new SelectListItem { Text = "Please Select Delivery Zone", Value = "" });         

            //var cityList = new string[] { "Dhaka", "Gazipur", "Savar", "Narayanganj", "Keraniganj", "Outside Dhaka" };
            //foreach (var c in cityList)
            //{
            //    model.ShippingAddresses.NewAddress.AvailableCities.Add(new SelectListItem { Text = c, Value = c });
            //}

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
                        Selected = s.Id == model.ShippingAddresses.NewAddress.StateProvinceId
                    });
                }
            }

            if (_workContext.CurrentCustomer.ShippingAddress != null)
            {
                model.SelectedShippingAdressId = _workContext.CurrentCustomer.ShippingAddress.Id;
            }

            //clear payment method if selected for initial load            
            _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.SelectedPaymentMethod, null, _storeContext.CurrentStore.Id);
            //bs-23


            // = PrepareBillingAddressModel(prePopulateNewAddressWithCustomerFields: true);
            //var shippingAddressModel = PrepareShippingAddressModel(prePopulateNewAddressWithCustomerFields: true);
            //return View("~/Plugins/Misc.OnePageCheckOut/Views/Checkout/OnePageCheckout.cshtml", model);

            try
            {
                #region customer wallet
                var CustomerAccountInformation = _customerLedgerMasterService.GetCustomerWalletAccountByCustomerId(_workContext.CurrentCustomer.Id);

                long Phone = 0;

                if (CustomerAccountInformation != null)
                {
                    Phone = CustomerAccountInformation.ContactNo;
                }

                //var longExtractor = new Regex("[^0-9]");
                //var PhoneP = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);

                //if (string.IsNullOrEmpty(PhoneP))
                //{
                //    PhoneP = "-1";
                //}

                //var Phone = longExtractor.Replace(PhoneP, "");
                var customerLedgerMaster = _customerLedgerMasterService.GetCustomerLedgerMasterByContactNo(Phone);

                var decimalExtractor = new Regex("[^0-9.-]");

                var orderTotalD = model.AllOrderTotalsModel.OrderTotal;
                if (string.IsNullOrEmpty(orderTotalD))
                {
                    orderTotalD = "";
                }

                var shippingD = model.AllOrderTotalsModel.Shipping;
                if (string.IsNullOrEmpty(shippingD))
                {
                    shippingD = "";
                }

                var orderTotal = decimalExtractor.Replace(orderTotalD, "");
                var shipping = decimalExtractor.Replace(shippingD, "");

                var totalPayable = string.IsNullOrEmpty(orderTotal) ?
                    Convert.ToDecimal(decimalExtractor.Replace(model.AllOrderTotalsModel.SubTotal, "")) + Convert.ToDecimal(string.IsNullOrEmpty(shipping) ? "0" : shipping) :
                    Convert.ToDecimal(decimalExtractor.Replace(model.AllOrderTotalsModel.OrderTotal, ""));

                var totalPayableFromWallet = customerLedgerMaster != null ? totalPayable <= customerLedgerMaster.TotalBalance ? totalPayable : customerLedgerMaster.TotalBalance : 0;

                model.AllOrderTotalsModel.TotalPayableFromWallet = _priceFormatter.FormatPrice(totalPayableFromWallet, true, false);

                #endregion
            }
            catch (Exception ex)
            {
                _logger.Information("Inside Wallet Region: \n" + ex.StackTrace, ex);
            }

            return View(model);
        }

        public ActionResult MakePaymentMethod(OnePageCheckoutModel model)
        {


            return View(model);
        }
        [HttpPost]
        public ActionResult SaveSelectedShippingAddressAjax(int shippingAddressId)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            if (cart.Count == 0)
                return RedirectToRoute("ShoppingCart");
            var customer = _workContext.CurrentCustomer;

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

            var showWalletPay = false;

            if (cart.Count > 0)
            {
                #region wallet payment
                var decimalExtractor = new Regex("[^0-9.-]");
                var OrderTotalD = string.IsNullOrEmpty(OrderTotalModel.OrderTotal) ? string.Empty : OrderTotalModel.OrderTotal;
                var SubTotalD = string.IsNullOrEmpty(OrderTotalModel.SubTotal) ? string.Empty : OrderTotalModel.SubTotal;

                string orderTotalString = decimalExtractor.Replace(OrderTotalD, ""),
                    orderSubTotalString = decimalExtractor.Replace(SubTotalD, "");

                if (_httpContext.Session["OrderWalletPaymentInfo"] is Dictionary<string, object> OrderWalletPaymentInfo && OrderWalletPaymentInfo.ContainsKey("willDeduct"))
                {
                    if (Convert.ToBoolean(OrderWalletPaymentInfo["willDeduct"]))
                    {
                        showWalletPay = true;

                        if (!string.IsNullOrEmpty(orderTotalString))
                        {
                            var orderTotal = Convert.ToDecimal(orderTotalString);

                            orderTotal -= Convert.ToDecimal(OrderWalletPaymentInfo["walletPayAmount"]);
                            OrderTotalModel.OrderTotal = _priceFormatter.FormatPrice(orderTotal, true, false);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(orderSubTotalString))
                            {
                                var orderSubTotal = Convert.ToDecimal(orderSubTotalString);
                                orderSubTotal -= Convert.ToDecimal(OrderWalletPaymentInfo["walletPayAmount"]);
                                OrderTotalModel.SubTotal = _priceFormatter.FormatPrice(orderSubTotal, true, false);
                            }
                        }
                    }
                }

                var ShippingD = string.IsNullOrEmpty(OrderTotalModel.Shipping) ? string.Empty : OrderTotalModel.Shipping;
                var shipping = decimalExtractor.Replace(ShippingD, "");

                var totalPayable = string.IsNullOrEmpty(orderTotalString) ?
                    Convert.ToDecimal(string.IsNullOrEmpty(orderSubTotalString) ? "0" : orderSubTotalString) + Convert.ToDecimal(string.IsNullOrEmpty(shipping) ? "0" : shipping) :
                    Convert.ToDecimal(string.IsNullOrEmpty(orderTotalString) ? "0" : orderTotalString);

                var CustomerAccountInformation = _customerLedgerMasterService.GetCustomerWalletAccountByCustomerId(_workContext.CurrentCustomer.Id);

                long Phone = 0;

                if (CustomerAccountInformation != null)
                {
                    Phone = CustomerAccountInformation.ContactNo;
                }

                var customerLedgerMaster = _customerLedgerMasterService.GetCustomerLedgerMasterByContactNo(Phone);
                var totalPayableFromWallet = customerLedgerMaster != null ? totalPayable <= customerLedgerMaster.TotalBalance ? totalPayable : customerLedgerMaster.TotalBalance : 0;

                OrderTotalModel.TotalPayableFromWallet = _priceFormatter.FormatPrice(totalPayableFromWallet, true, false);
                #endregion

                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "one-page-checkout-info",
                        //ShippingAdressUpdateHtml = this.RenderPartialViewToString("ShippingAddress", ShippingAddresses),
                        //ShippingMethodUpdateHtml = this.RenderPartialViewToString("OpcShippingMethods", ShippingMethodModel),
                        OrderTotalHtml = RenderPartialViewToString("OrderTotals", OrderTotalModel)
                    },
                    success = true,
                    showWalletPay
                });

                //return Json(new
                //{
                //    success = 1,
                //    update_section = new UpdateSectionJsonModel
                //    {
                //        name = "shopping-cart-info",
                //        OrderTotalHtml = RenderPartialViewToString("OrderTotals", OrderTotalModel)
                //    },
                //    showWalletPay
                //});
            }

            return Json(new
            {
                success = 0
            });
        }


        [HttpPost, ActionName("ShippingAddress")]
        [ValidateInput(false)]
        public ActionResult NewShippingAddress(CheckoutShippingAddressModel model, FormCollection form)
        {
            //validation
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            if (cart.Count == 0)
                return RedirectToRoute("ShoppingCart");

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
                var address = _workContext.CurrentCustomer.Addresses.ToList().FindAddress(
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
                    _workContext.CurrentCustomer.Addresses.Add(address);
                }
                _workContext.CurrentCustomer.ShippingAddress = address;
                _workContext.CurrentCustomer.BillingAddress = address;
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);


                //return shipping address info page
                var ShippingAddresses = PrepareShippingAddressModel();
                var ShippingMethodModel = PrepareShippingMethodModel(cart, _workContext.CurrentCustomer.ShippingAddress);
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
            return Json(new { success = false });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SavePaymentMethod(FormCollection form)
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


                if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                string paymentmethod = form["paymentmethod"];
                string paymentmethodemioption = form["emiOption"];
                //payment method 
                if (String.IsNullOrEmpty(paymentmethod))
                    throw new Exception("Selected payment method can't be parsed");


                var model = new CheckoutPaymentMethodModel();
                TryUpdateModel(model);

                //reward points
                if (_rewardPointsSettings.Enabled)
                {
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.UseRewardPointsDuringCheckout, model.UseRewardPoints,
                        _storeContext.CurrentStore.Id);
                }

                //Check whether payment workflow is required
                bool isPaymentWorkflowRequired = IsPaymentWorkflowRequired(cart);
                if (!isPaymentWorkflowRequired)
                {
                    //payment is not required
                    _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.SelectedPaymentMethod, null, _storeContext.CurrentStore.Id);
                    //bs-23
                    _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.SelectedEmiOption, null, _storeContext.CurrentStore.Id);

                    var confirmOrderModel = PrepareConfirmOrderModel(cart);

                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "confirm-order",
                            PaymentInfoHtml = RenderPartialViewToString("OpcConfirmOrder", confirmOrderModel)
                        },
                        goto_section = "confirm_order",
                        isPaymentWorkflowRequired
                    });
                }

                var paymentMethodInst = _paymentService.LoadPaymentMethodBySystemName(paymentmethod);
                if (paymentMethodInst == null ||
                    !paymentMethodInst.IsPaymentMethodActive(_paymentSettings) ||
                    !_pluginFinder.AuthenticateStore(paymentMethodInst.PluginDescriptor, _storeContext.CurrentStore.Id))
                    throw new Exception("Selected payment method can't be parsed");

                //save
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                    SystemCustomerAttributeNames.SelectedPaymentMethod, paymentmethod, _storeContext.CurrentStore.Id);
                //bs-23
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                    SystemCustomerAttributeNames.SelectedEmiOption, paymentmethodemioption, _storeContext.CurrentStore.Id);

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
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                if (cart.Count == 0)
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                var paymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
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
                            PaymentInfoHtml = RenderPartialViewToString("OpcConfirmOrder", confirmOrderModel),
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
                        PaymentInfoHtml = RenderPartialViewToString("OpcPaymentInfo", paymenInfoModel)
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
        public ActionResult MakePaymentMethod()
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

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                //prevent 2 orders being placed within an X seconds time frame
                if (!IsMinimumOrderPlacementIntervalValid(_workContext.CurrentCustomer))
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
                processPaymentRequest.CustomerId = _workContext.CurrentCustomer.Id;
                processPaymentRequest.PaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod,
                    _genericAttributeService, _storeContext.CurrentStore.Id);

                #region wallet payment
                decimal totalPayableFromWallet = 0;

                if (_httpContext.Session["OrderWalletPaymentInfo"] is Dictionary<string, object> OrderWalletPaymentInfo && OrderWalletPaymentInfo.ContainsKey("willDeduct"))
                {
                    if (Convert.ToBoolean(OrderWalletPaymentInfo["willDeduct"]))
                    {
                        totalPayableFromWallet = Convert.ToDecimal(OrderWalletPaymentInfo["walletPayAmount"]);
                    }
                }

                #endregion

                var placeOrderResult = _orderProcessingService.PlaceOrder(processPaymentRequest, totalPayableFromWallet);

                if (placeOrderResult.Success)
                {
                    _httpContext.Session["OrderPaymentInfo"] = null;

                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = placeOrderResult.PlacedOrder
                    };

                    var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(placeOrderResult.PlacedOrder.PaymentMethodSystemName);
                    if (paymentMethod == null || placeOrderResult.PlacedOrder.PaymentStatus == PaymentStatus.Paid)
                    {
                        //payment method could be null if order total is 0
                        //success
                        return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
                    }

                    if (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection)
                    {
                        //Redirection will not work because it's AJAX request.
                        //That's why we don't process it here (we redirect a user to another page where he'll be redirected)

                        //redirect
                        return Json(new
                        {
                            success = 2
                            //redirect = string.Format("{0}checkout/OpcCompleteRedirectionPayment", _webHelper.GetStoreLocation())
                        }, JsonRequestBehavior.AllowGet);
                    }

                    //if (postProcessPaymentRequest.Order.PaymentMethodSystemName != "Payments.bKashAdvance")
                    //{
                    //    _httpContext.Session["OrderWalletPaymentInfo"] = null;
                    //}

                    _paymentService.PostProcessPayment(postProcessPaymentRequest);
                    //success
                    _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.SelectedEmiOption, null, _storeContext.CurrentStore.Id);
                    return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
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
                        PaymentInfoHtml = RenderPartialViewToString("OpcConfirmOrder", confirmOrderModel)
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
        public ActionResult SaveConfirmOrder()
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

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                //prevent 2 orders being placed within an X seconds time frame
                if (!IsMinimumOrderPlacementIntervalValid(_workContext.CurrentCustomer))
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
                processPaymentRequest.CustomerId = _workContext.CurrentCustomer.Id;
                processPaymentRequest.PaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod,
                    _genericAttributeService, _storeContext.CurrentStore.Id);

                #region wallet payment
                decimal totalPayableFromWallet = 0;

                if (_httpContext.Session["OrderWalletPaymentInfo"] is Dictionary<string, object> OrderWalletPaymentInfo && OrderWalletPaymentInfo.ContainsKey("willDeduct"))
                {
                    if (Convert.ToBoolean(OrderWalletPaymentInfo["willDeduct"]))
                    {
                        totalPayableFromWallet = Convert.ToDecimal(OrderWalletPaymentInfo["walletPayAmount"]);
                    }
                }

                #endregion

                var placeOrderResult = _orderProcessingService.PlaceOrder(processPaymentRequest, totalPayableFromWallet);

                if (placeOrderResult.Success)
                {
                    _httpContext.Session["OrderPaymentInfo"] = null;

                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = placeOrderResult.PlacedOrder
                    };

                    var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(placeOrderResult.PlacedOrder.PaymentMethodSystemName);
                    if (paymentMethod == null || placeOrderResult.PlacedOrder.PaymentStatus == PaymentStatus.Paid)
                    {
                        //payment method could be null if order total is 0
                        //success
                        return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
                    }

                    if (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection)
                    {
                        //Redirection will not work because it's AJAX request.
                        //That's why we don't process it here (we redirect a user to another page where he'll be redirected)

                        //redirect
                        return Json(new
                        {
                            success = 2
                            //redirect = string.Format("{0}checkout/OpcCompleteRedirectionPayment", _webHelper.GetStoreLocation())
                        }, JsonRequestBehavior.AllowGet);
                    }

                    //if (postProcessPaymentRequest.Order.PaymentMethodSystemName != "Payments.bKashAdvance")
                    //{
                    //    _httpContext.Session["OrderWalletPaymentInfo"] = null;
                    //}

                    _paymentService.PostProcessPayment(postProcessPaymentRequest);
                    //success
                    _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.SelectedEmiOption, null, _storeContext.CurrentStore.Id);
                    return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
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
                        PaymentInfoHtml = RenderPartialViewToString("OpcConfirmOrder", confirmOrderModel)
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
            var shoppingCartModel = new ShoppingCartModel();

            PrepareShoppingCartModel(shoppingCartModel, cart, false);
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
            PrepareShippingMethodModel(cart, _workContext.CurrentCustomer.ShippingAddress);
            var OrderTotalModel = PrepareOrderTotalsModel(cart, true);

            if (cart.Count > 0)
            {
                return Json(new
                {
                    success = 1,
                    update_section = new UpdateSectionJsonModel
                    {



                        name = "shopping-cart-info",
                        ShoppigCartHtml = RenderPartialViewToString("OrderSummary", shoppingCartModel),
                        OrderTotalHtml = RenderPartialViewToString("OrderTotals", OrderTotalModel)

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


        [HttpPost]
        public ActionResult UpdateOrderTotal(string paymentMethod, string stProvinceName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("HomePage");

            try
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();

                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                    SystemCustomerAttributeNames.SelectedPaymentMethod, paymentMethod, _storeContext.CurrentStore.Id);

                var OrderTotalModel = PrepareOrderTotalsModel(cart, true, stProvinceName);

                var showWalletPay = false;

                if (cart.Count > 0)
                {
                    #region wallet payment
                    var decimalExtractor = new Regex("[^0-9.-]");
                    var OrderTotalD = string.IsNullOrEmpty(OrderTotalModel.OrderTotal) ? string.Empty : OrderTotalModel.OrderTotal;
                    var SubTotalD = string.IsNullOrEmpty(OrderTotalModel.SubTotal) ? string.Empty : OrderTotalModel.SubTotal;

                    string orderTotalString = decimalExtractor.Replace(OrderTotalD, ""),
                        orderSubTotalString = decimalExtractor.Replace(SubTotalD, "");

                    if (_httpContext.Session["OrderWalletPaymentInfo"] is Dictionary<string, object> OrderWalletPaymentInfo && OrderWalletPaymentInfo.ContainsKey("willDeduct"))
                    {
                        if (Convert.ToBoolean(OrderWalletPaymentInfo["willDeduct"]))
                        {
                            showWalletPay = true;

                            if (!string.IsNullOrEmpty(orderTotalString))
                            {
                                var orderTotal = Convert.ToDecimal(orderTotalString);

                                orderTotal -= Convert.ToDecimal(OrderWalletPaymentInfo["walletPayAmount"]);
                                OrderTotalModel.OrderTotal = _priceFormatter.FormatPrice(orderTotal, true, false);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(orderSubTotalString))
                                {
                                    var orderSubTotal = Convert.ToDecimal(orderSubTotalString);
                                    orderSubTotal -= Convert.ToDecimal(OrderWalletPaymentInfo["walletPayAmount"]);
                                    OrderTotalModel.SubTotal = _priceFormatter.FormatPrice(orderSubTotal, true, false);
                                }
                            }
                        }
                    }

                    var ShippingD = string.IsNullOrEmpty(OrderTotalModel.Shipping) ? string.Empty : OrderTotalModel.Shipping;
                    var shipping = decimalExtractor.Replace(ShippingD, "");

                    var totalPayable = string.IsNullOrEmpty(orderTotalString) ?
                        Convert.ToDecimal(string.IsNullOrEmpty(orderSubTotalString) ? "0" : orderSubTotalString) + Convert.ToDecimal(string.IsNullOrEmpty(shipping) ? "0" : shipping) :
                        Convert.ToDecimal(string.IsNullOrEmpty(orderTotalString) ? "0" : orderTotalString);

                    var CustomerAccountInformation = _customerLedgerMasterService.GetCustomerWalletAccountByCustomerId(_workContext.CurrentCustomer.Id);

                    long Phone = 0;

                    if (CustomerAccountInformation != null)
                    {
                        Phone = CustomerAccountInformation.ContactNo;
                    }

                    //var longExtractor = new Regex("[^0-9]");
                    //var PhoneP = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);

                    //if (string.IsNullOrEmpty(PhoneP))
                    //{
                    //    PhoneP = "-1";
                    //}

                    //var Phone = longExtractor.Replace(PhoneP, "");

                    var customerLedgerMaster = _customerLedgerMasterService.GetCustomerLedgerMasterByContactNo(Phone);
                    var totalPayableFromWallet = customerLedgerMaster != null ? totalPayable <= customerLedgerMaster.TotalBalance ? totalPayable : customerLedgerMaster.TotalBalance : 0;

                    OrderTotalModel.TotalPayableFromWallet = _priceFormatter.FormatPrice(totalPayableFromWallet, true, false);
                    #endregion

                    return Json(new
                    {
                        success = 1,
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "shopping-cart-info",
                            OrderTotalHtml = RenderPartialViewToString("OrderTotals", OrderTotalModel)
                        },
                        showWalletPay
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Information("Inside Wallet Region -> Update Total Order: \n" + ex.StackTrace, ex);
            }

            return Json(new
            {
                success = 0
            });
        }

        [HttpPost]
        public ActionResult UpdateOrderTotalWithWalletPayAmount(decimal walletPayAmount, bool willDeduct)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var OrderTotalModel = PrepareOrderTotalsModel(cart, true);

            var decimalExtractor = new Regex("[^0-9.-]");

            var orderTotalString = decimalExtractor.Replace(OrderTotalModel.OrderTotal, "");

            if (!string.IsNullOrEmpty(orderTotalString))
            {
                var orderTotal = Convert.ToDecimal(orderTotalString);

                orderTotal -= willDeduct ? walletPayAmount : 0;
                OrderTotalModel.OrderTotal = _priceFormatter.FormatPrice(orderTotal, true, false);
            }
            else
            {
                var orderSubTotalString = decimalExtractor.Replace(OrderTotalModel.SubTotal, "");
                if (!string.IsNullOrEmpty(orderSubTotalString))
                {
                    var orderSubTotal = Convert.ToDecimal(orderSubTotalString);
                    orderSubTotal -= willDeduct ? walletPayAmount : 0;
                    OrderTotalModel.SubTotal = _priceFormatter.FormatPrice(orderSubTotal, true, false);
                }
            }

            OrderTotalModel.TotalPayableFromWallet = _priceFormatter.FormatPrice(walletPayAmount, true, false);

            if (cart.Count > 0)
            {
                if (willDeduct)
                {
                    _httpContext.Session["OrderWalletPaymentInfo"] = new Dictionary<string, object> {
                        {"willDeduct", true},
                        {"walletPayAmount", walletPayAmount}
                    };
                }
                else
                {
                    _httpContext.Session["OrderWalletPaymentInfo"] = null;
                }

                return Json(new
                {
                    success = 1,
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "shopping-cart-info",
                        OrderTotalHtml = RenderPartialViewToString("OrderTotals", OrderTotalModel)
                    }
                });
            }

            return Json(new
            {
                success = 0
            });
        }

        [HttpPost]
        public ActionResult UpdateOrderTotalWithEmi(string paymentMethod, string paymentEmioption, string stProvinceName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //save
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                SystemCustomerAttributeNames.SelectedPaymentMethod, paymentMethod, _storeContext.CurrentStore.Id);

            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                SystemCustomerAttributeNames.SelectedEmiOption, paymentEmioption, _storeContext.CurrentStore.Id);

            //current warnings <cart item identifier, warnings>
            var innerWarnings = new Dictionary<int, IList<string>>();

            var OrderTotalModel = PrepareOrderTotalsModel(cart, true, stProvinceName);

            var showWalletPay = false;

            var decimalExtractor = new Regex("[^0-9.-]");
            string orderTotalString = decimalExtractor.Replace(OrderTotalModel.OrderTotal, ""),
                orderSubTotalString = decimalExtractor.Replace(OrderTotalModel.SubTotal, "");

            #region wallet payment
            if (_httpContext.Session["OrderWalletPaymentInfo"] is Dictionary<string, object> OrderWalletPaymentInfo && OrderWalletPaymentInfo.ContainsKey("willDeduct"))
            {
                if (Convert.ToBoolean(OrderWalletPaymentInfo["willDeduct"]))
                {
                    showWalletPay = true;

                    if (!string.IsNullOrEmpty(orderTotalString))
                    {
                        var orderTotal = Convert.ToDecimal(orderTotalString);

                        orderTotal -= Convert.ToDecimal(OrderWalletPaymentInfo["walletPayAmount"]);
                        OrderTotalModel.OrderTotal = _priceFormatter.FormatPrice(orderTotal, true, false);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(orderSubTotalString))
                        {
                            var orderSubTotal = Convert.ToDecimal(orderSubTotalString);
                            orderSubTotal -= Convert.ToDecimal(OrderWalletPaymentInfo["walletPayAmount"]);
                            OrderTotalModel.SubTotal = _priceFormatter.FormatPrice(orderSubTotal, true, false);
                        }
                    }
                }
            }

            var shipping = decimalExtractor.Replace(OrderTotalModel.Shipping, "");

            var totalPayable = string.IsNullOrEmpty(orderTotalString) ?
                Convert.ToDecimal(string.IsNullOrEmpty(orderSubTotalString) ? "0" : orderSubTotalString) + Convert.ToDecimal(string.IsNullOrEmpty(shipping) ? "0" : shipping) :
                Convert.ToDecimal(string.IsNullOrEmpty(orderTotalString) ? "0" : orderTotalString);

            var customerLedgerMaster = _customerLedgerMasterService.GetCustomerLedgerMasterByContactNo(Convert.ToInt64(_workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.Phone)));
            var totalPayableFromWallet = totalPayable <= customerLedgerMaster.TotalBalance ? totalPayable : customerLedgerMaster.TotalBalance;

            OrderTotalModel.TotalPayableFromWallet = _priceFormatter.FormatPrice(totalPayableFromWallet, true, false);
            #endregion

            //emi values
            //TODO:: this code need to refactor
            var countryId = GetBangladeshCountryId();
            var stateProvinceId = 0;
            if (!string.IsNullOrEmpty(stProvinceName))
            {
                var states = _stateProvinceService.GetStateProvincesByCountryId(countryId);
                if (states.Any())
                {
                    var stateProvince = states.FirstOrDefault(s => s.Name.ToLower() == stProvinceName.ToLower());
                    if (stateProvince != null)
                        stateProvinceId = stateProvince.Id;
                }
            }

            var paymentMethods = _paymentService
                .LoadActivePaymentMethods(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id, filterByCountryId)
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Standard || pm.PaymentMethodType == PaymentMethodType.Redirection)
                .Where(pm => !pm.HidePaymentMethod(cart))
                .ToList();

            string emiOption3Amount = "";
            string emiOption6Amount = "";
            string emiOption9Amount = "";
            string emiOption12Amount = "";
            string emiOption18Amount = "";
            string emiOption24Amount = "";
            bool hasEMIPaymentMethod = false;

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
                if (pm.PluginDescriptor.SystemName == "Payments.SSLCommerzEmi")
                {
                    PrepareSSLEmiPaymentMethodModel(pm, cart, pmModel, countryId, stateProvinceId);

                    emiOption3Amount = pmModel.EmiThreeMonthFee;
                    emiOption6Amount = pmModel.EmiSixMonthFee;
                    emiOption9Amount = pmModel.EmiNineMonthFee;
                    emiOption12Amount = pmModel.EmiTwelveMonthFee;
                    emiOption18Amount = pmModel.EmiEighteenMonthFee;
                    emiOption24Amount = pmModel.EmiTwentyFourMonthFee;

                    hasEMIPaymentMethod = true;

                    break;
                }
            }

            if (cart.Count > 0)
            {
                return Json(new
                {
                    success = 1,
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "shopping-cart-info",
                        OrderTotalHtml = RenderPartialViewToString("OrderTotals", OrderTotalModel)
                    },
                    hasEMIPaymentMethod,
                    emiOption3Amount,
                    emiOption6Amount,
                    emiOption9Amount,
                    emiOption12Amount,
                    emiOption18Amount,
                    emiOption24Amount,
                    showWalletPay
                });
            }
            else
            {
                return Json(new
                {
                    success = 0
                });
            }
        }

        #endregion
    }
}