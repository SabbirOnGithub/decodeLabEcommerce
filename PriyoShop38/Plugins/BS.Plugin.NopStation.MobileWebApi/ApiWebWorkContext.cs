using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Tax;
using Nop.Core.Fakes;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Framework;
using Nop.Services.Configuration;

namespace BS.Plugin.NopStation.MobileWebApi
{
    public class ApiWebWorkContext : WebWorkContext
    {
        private Customer _cachedCustomer;
        private Customer _originalCustomerIfImpersonated;
        private readonly HttpContextBase _httpContext;
        private readonly ICustomerService _customerService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICustomerServiceApi _customerServiceApi;

        public ApiWebWorkContext(HttpContextBase httpContext,
            ICustomerService customerService,
            IVendorService vendorService,
            IStoreContext storeContext,
            IAuthenticationService authenticationService,
            ILanguageService languageService,
            ICurrencyService currencyService,
            IGenericAttributeService genericAttributeService,
            TaxSettings taxSettings,
            CurrencySettings currencySettings,
            LocalizationSettings localizationSettings,
            IUserAgentHelper userAgentHelper,
            IStoreMappingService storeMappingService,
            ICustomerServiceApi customerServiceApi)
            : base(httpContext, customerService,
                vendorService, storeContext,
                authenticationService, languageService,
                currencyService, genericAttributeService,
                taxSettings, currencySettings,
                localizationSettings, userAgentHelper,
                storeMappingService)
        {
            this._httpContext = httpContext;
            this._customerService = customerService;
            this._userAgentHelper = userAgentHelper;
            this._authenticationService = authenticationService;
            this._customerServiceApi = customerServiceApi;
        }


        public Customer GetCustomerFromToken()
        {
            //IEnumerable<string> headerValues;
            try
            {
                int Id = 0;
                var secretKey = Constant.SecretKey;
                var keyFound = _httpContext.Request.Headers.GetValues(Constant.TokenName);
                var token = keyFound.FirstOrDefault();
                var load = JWT.JsonWebToken.DecodeToObject(token, secretKey) as IDictionary<string, object>;
                if (load != null)
                {
                    Id = Convert.ToInt32(load[Constant.CustomerIdName]);
                    return _customerService.GetCustomerById(Id);
                }

            }
            catch
            {
                return null;
            }
            return null;
        }

        public Customer GetCustomerFromDeviceId()
        {
            var keyFound = _httpContext.Request.Headers.GetValues(Constant.DeviceIdName);
            var deviceId = keyFound.FirstOrDefault();
            var getGuidForDevice = HelperExtension.GetGuid(deviceId);
            Customer customer = _customerService.GetCustomerByGuid(getGuidForDevice);
            if (customer != null)
            {
                if (customer.IsRegistered())
                {
                    customer.CustomerGuid = Guid.NewGuid();
                    _customerService.UpdateCustomer(customer);

                    customer = _customerServiceApi.InsertGuestCustomerByMobile(deviceId);
                }

                return customer;
            }
            else
            {
                customer = _customerServiceApi.InsertGuestCustomerByMobile(deviceId);
                return customer;
            }
        }
        public override Customer CurrentCustomer
        {
            get
            {
                if (_cachedCustomer != null)
                    return _cachedCustomer;

                Customer customer = null;
                if (_httpContext == null || _httpContext is FakeHttpContext)
                {
                    //check whether request is made by a background task
                    //in this case return built-in customer record for background task
                    customer = _customerService.GetCustomerBySystemName(SystemCustomerNames.BackgroundTask);
                }

                //check whether request is made by a search engine
                //in this case return built-in customer record for search engines 
                //or comment the following two lines of code in order to disable this functionality
                if (customer == null || customer.Deleted || !customer.Active)
                {
                    if (_userAgentHelper.IsSearchEngine())
                        customer = _customerService.GetCustomerBySystemName(SystemCustomerNames.SearchEngine);
                }

                //registered user
                if (customer == null || customer.Deleted || !customer.Active)
                {
                    customer = _authenticationService.GetAuthenticatedCustomer();
                }

                //impersonate user if required (currently used for 'phone order' support)
                if (customer != null && !customer.Deleted && customer.Active)
                {
                    var impersonatedCustomerId = customer.GetAttribute<int?>(SystemCustomerAttributeNames.ImpersonatedCustomerId);
                    if (impersonatedCustomerId.HasValue && impersonatedCustomerId.Value > 0)
                    {
                        var impersonatedCustomer = _customerService.GetCustomerById(impersonatedCustomerId.Value);
                        if (impersonatedCustomer != null && !impersonatedCustomer.Deleted && impersonatedCustomer.Active)
                        {
                            //set impersonated customer
                            _originalCustomerIfImpersonated = customer;
                            customer = impersonatedCustomer;
                        }
                    }
                }


                //load mobile customer
                if (_httpContext != null && _httpContext.Request.RawUrl != null && _httpContext.Request.RawUrl.StartsWith("/api/"))
                {
                    //check whether request is made by a background task
                    //in this case return built-in customer record for background task
                    if (_httpContext.Request.Headers[Constant.TokenName] != null)
                    {
                        customer = GetCustomerFromToken();
                        if (customer != null)
                        {
                            _cachedCustomer = customer;
                            return customer;
                        }
                    }
                    else if (_httpContext.Request.Headers[Constant.DeviceIdName] != null)
                    {
                        customer = GetCustomerFromDeviceId();
                        if (customer != null)
                        {
                            _cachedCustomer = customer;
                            return customer;
                        }
                    }
                }

                //load guest customer
                if (customer == null || customer.Deleted || !customer.Active)
                {
                    var customerCookie = GetCustomerCookie();
                    if (customerCookie != null && !String.IsNullOrEmpty(customerCookie.Value))
                    {
                        Guid customerGuid;
                        if (Guid.TryParse(customerCookie.Value, out customerGuid))
                        {
                            var customerByCookie = _customerService.GetCustomerByGuid(customerGuid);
                            if (customerByCookie != null &&
                                //this customer (from cookie) should not be registered
                                !customerByCookie.IsRegistered())
                                customer = customerByCookie;
                        }
                    }
                }

                //create guest if not exists
                if (customer == null || customer.Deleted || !customer.Active)
                {
                    customer = _customerService.InsertGuestCustomer();
                }


                //validation
                if (!customer.Deleted && customer.Active)
                {
                    SetCustomerCookie(customer.CustomerGuid);
                    _cachedCustomer = customer;
                }

                return _cachedCustomer;
            }
            set
            {
                SetCustomerCookie(value.CustomerGuid);
                _cachedCustomer = value;
            }
        }

        public override Customer OriginalCustomerIfImpersonated
        {
            get
            {
                return _originalCustomerIfImpersonated;
            }
        }
    }
}
