using System.Collections.Generic;
using Nop.Plugin.Widgets.BsAffiliate.Models;

namespace Nop.Plugin.Widgets.BsAffiliate.Services
{
    public interface IAffiliatePublicService
    {
        SaveResponseModel SaveInfo(AffiliateModel model);

        AffiliatePublicDetailsModel GetCuurentCustomerAffiliatedOrders(AffiliatedOrderListModel model);
    }
}