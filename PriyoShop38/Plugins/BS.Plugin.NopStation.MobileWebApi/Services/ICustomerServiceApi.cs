using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public interface ICustomerServiceApi
    {
        CustomerRole GetCustomerRoleBySystemName(string systemName);

        Customer InsertGuestCustomerByMobile(string deviceId);

        Customer GetCustomerByVendorId(int vendorId);
    }
}
