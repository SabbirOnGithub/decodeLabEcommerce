using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Widgets.BsAffiliate.Domain;
using Nop.Plugin.Widgets.BsAffiliate.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.BsAffiliate.Services
{
    public interface IAffiliateCustomerMapService
    {
        IList<AffiliateCustomerMapModel> GetCustomer(string queryString);

        AffiliateCustomerMapModel GetCustomerByAffiliateId(int affiliateId);

        SaveResponseModel SaveAffiliateCustomer(AffiliateCustomerMapModel model);

        AffiliateCustomerMapping GetAffiliateCustomerMapByCustomerId(int id);

        CustomerRole GetAffiliateRole();

        void DeleteAffiliateCustomerMap(AffiliateCustomerMapping map);

        Affiliate GetAffiliateByCustomerId(int customerId);

        bool IsActive();

        bool IsApplied(out AffiliateCustomerMapping customerMapping);

        List<SelectListItem> GetAvailableAffiliateTypes();
    }
}