using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Purchase.UserAgent.Models;
using System.Collections.Generic;

namespace Nop.Plugin.Purchase.UserAgent.Services
{
    public interface IPurchaseUserAgentService
    {
        IPagedList<OrderViewModel> GetOrderList(OrderViewModel model, int pageIndex, int pageSize);
    }
}