using Nop.Core;
using Nop.Plugin.Widgets.EkShopA2I.Domain;
using System;

namespace Nop.Plugin.Widgets.EkShopA2I.Services
{
    public interface IEkshopOrderService
    {
        IPagedList<EsOrder> GetEkshopOrders(string orderCode = "", string lpCode = "", string lpContactNumber = "",
            DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue);

        EsOrder GetEkshopOrderById(int id);
    }
}