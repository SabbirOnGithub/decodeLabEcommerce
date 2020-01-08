using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using BS.Plugin.NopStation.MobileWebApi.Extensions;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public class CustomerServiceApi : ICustomerServiceApi
    {
        private const string CUSTOMERROLES_BY_SYSTEMNAME_KEY = "Nop.customerrole.systemname-{0}";
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerRole> _customerRoleRepository;
        private readonly ICacheManager _cacheManager;
        public CustomerServiceApi(IRepository<Customer> customerRepository,
            IRepository<CustomerRole> customerRoleRepository,
            ICacheManager cacheManager)
        {
            this._customerRepository = customerRepository;
            this._customerRoleRepository = customerRoleRepository;
            this._cacheManager = cacheManager;
        }

        public virtual CustomerRole GetCustomerRoleBySystemName(string systemName)
        {
            if (String.IsNullOrWhiteSpace(systemName))
                return null;

            string key = string.Format(CUSTOMERROLES_BY_SYSTEMNAME_KEY, systemName);
            return _cacheManager.Get(key, () =>
            {
                var query = from cr in _customerRoleRepository.Table
                            orderby cr.Id
                            where cr.SystemName == systemName
                            select cr;
                var customerRole = query.FirstOrDefault();
                return customerRole;
            });
        }


        public Customer InsertGuestCustomerByMobile(string deviceId)
        {
            var customer = new Customer
            {
                CustomerGuid = HelperExtension.GetGuid(deviceId),
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow
            };

            //add to 'Guests' role
            var guestRole = GetCustomerRoleBySystemName(SystemCustomerRoleNames.Guests);
            if (guestRole == null)
                throw new NopException("'Guests' role could not be loaded");
            customer.CustomerRoles.Add(guestRole);

            _customerRepository.Insert(customer);

            return customer;
        }

        public Customer GetCustomerByVendorId(int vendorId)
        {
            var query = from cr in _customerRepository.Table
                        orderby cr.Id
                        where cr.VendorId == vendorId
                        select cr;
            var customer = new Customer();
            if (query.Any())
            {
                customer = query.FirstOrDefault();
            }
            return customer;
        }
    }
}
