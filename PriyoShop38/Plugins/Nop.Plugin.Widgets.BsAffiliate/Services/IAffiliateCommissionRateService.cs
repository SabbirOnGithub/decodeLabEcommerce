using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.BsAffiliate.Models;
using Nop.Web.Framework.Kendoui;

namespace Nop.Plugin.Widgets.BsAffiliate.Services
{
    public interface IAffiliateCommissionRateService
    {
        AffiliateCommissionRateModel GetCommissionRate(int commissionRateId);

        SaveResponseModel AddCommission(AffiliateCommissionRateModel model);

        SelectList GetCategoryList();

        SelectList GetVendorList();

        SaveResponseModel EditCommission(AffiliateCommissionRateModel model);

        PagedList<AffiliateCommissionRateModel> GetAffiliateCommissions(DataSourceRequest command);

        void DeleteRate(int id);
    }
}