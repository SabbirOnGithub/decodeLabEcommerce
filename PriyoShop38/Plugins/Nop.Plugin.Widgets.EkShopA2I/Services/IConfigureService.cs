using Nop.Core.Domain.Customers;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.EkShopA2I.Services
{
    public interface IConfigureService
    {
        Customer GetA2iCustomer(string apiKey);

        Customer UpdateA2iCustomer(string apiKey, string oldApiKey);

        List<int> GetRestrictedVendors();

        void UpdateRestrictedVendors(List<int> vendorIds);
    }
}