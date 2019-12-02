using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Widgets.BsAffiliate.Models;
using Nop.Web.Framework.Kendoui;

namespace Nop.Plugin.Widgets.BsAffiliate.Services
{
    public interface IAffiliateConfigureService
    {
        void UpdateSettings(AffiliateConfigureModel model);

        AffiliateConfigureModel LoadSettings();

        IList<AffiliateUserCommissionModel> GetAllCommissions();

        void UpdateCommission(AffiliateUserCommissionModel model);

        void ResetCommission(int id);

        DataSourceResult GetAllOrders(DataSourceRequest command, AffiliatedOrderModel model);

        void MarkAsPaid(int id);
    }
}