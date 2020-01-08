using BS.Plugin.NopStation.MobileWebApi.Extensions;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Widgets.EkShopA2I.Domain;
using Nop.Plugin.Widgets.EkShopA2I.Extensions;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Widgets.EkShopA2I.Services
{
    public class ConfigureService : IConfigureService
    {
        #region Fields

        private const string CUSTOMERROLES_BY_SYSTEMNAME_KEY = "Nop.customerrole.systemname-{0}";
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly ICustomerService _customerService;
        private readonly IVendorService _vendorService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger _logger;
        private readonly IStoreContext _storeContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStoreService _storeService;
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<EsUdcCommissionRate> _udcCommissionRateRepository;

        #endregion

        #region Ctor

        public ConfigureService(IWorkContext workContext,
            IWebHelper webHelper, 
            ILogger logger, 
            ISettingService settingService,
            IStoreContext storeContext, 
            IStoreService storeService,
            ICustomerService customerService,
            ICacheManager cacheManager, 
            IRepository<EsOrder> esOrderRepository, 
            IGenericAttributeService genericAttributeService,
            IRepository<EsUdcCommissionRate> udcCommissionRateRepository, 
            ICategoryService categoryService,
            IVendorService vendorService)
        {
            _vendorService = vendorService;
            _categoryService = categoryService;
            _workContext = workContext;
            _logger = logger;
            _storeContext = storeContext;
            _settingService = settingService;
            _storeService = storeService;
            _customerService = customerService;
            _cacheManager = cacheManager;
            _genericAttributeService = genericAttributeService;
            _udcCommissionRateRepository = udcCommissionRateRepository;
        }

        #endregion

        #region A2I Customer

        public Customer GetA2iCustomer(string apiKey)
        {
            var getGuidForDevice = HelperExtension.GetGuid(apiKey);
            Customer customer = _customerService.GetCustomerByGuid(getGuidForDevice);
            if (customer != null)
            {
                return customer;
            }
            else
            {
                customer = InsertA2iCustomer(apiKey);
                return customer;
            }
        }

        protected Customer InsertA2iCustomer(string apiKey)
        {
            var customer = new Customer
            {
                CustomerGuid = HelperExtension.GetGuid(apiKey),
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow
            };

            var customerRole = _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Registered);
            if (customerRole == null)
                throw new NopException("'Registered' role could not be loaded");
            customer.CustomerRoles.Add(customerRole);

            _customerService.InsertCustomer(customer);
            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName, "Ek-Shop a2i");
            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Company, "Access to Information - a2i");

            return customer;
        }

        public Customer UpdateA2iCustomer(string apiKey, string oldApiKey)
        {
            var customer = GetA2iCustomer(oldApiKey);
            customer.CustomerGuid = HelperExtension.GetGuid(apiKey);

            _customerService.UpdateCustomer(customer);
            return customer;
        }

        #endregion


        public List<int> GetRestrictedVendors()
        {
            var vendorIdsString = _settingService.GetSettingByKey<string>(SettingKey.RestrictedVendors);
            if (!string.IsNullOrWhiteSpace(vendorIdsString))
            {
                var arr = vendorIdsString.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                return arr.Select(int.Parse).ToList();
            }
            return new List<int>();
        }

        public void UpdateRestrictedVendors(List<int> vendorIds)
        {
            if (vendorIds == null || vendorIds.Count == 0)
            {
                var vendorIdsString = _settingService.GetSetting(SettingKey.RestrictedVendors);
                if (vendorIdsString != null)
                    _settingService.DeleteSetting(vendorIdsString);
            }
            _settingService.SetSetting(SettingKey.RestrictedVendors, string.Join(",", vendorIds));
        }
    }
}
