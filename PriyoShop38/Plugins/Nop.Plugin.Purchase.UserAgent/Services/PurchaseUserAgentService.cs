using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Purchase.UserAgent.Models;
using Nop.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Purchase.UserAgent.Services
{
    public class PurchaseUserAgentService : IPurchaseUserAgentService
    {
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<GenericAttribute> _genericAttributeRepo;
        private readonly IGenericAttributeService _genericAttributeService;

        public PurchaseUserAgentService(IRepository<Order> orderRepo,
            IGenericAttributeService genericAttributeService,
            IRepository<GenericAttribute> genericAttributeRepo)
        {
            _orderRepo = orderRepo;
            _genericAttributeService = genericAttributeService;
            _genericAttributeRepo = genericAttributeRepo;
        }

        public IPagedList<OrderViewModel> GetOrderList(OrderViewModel model, int pageIndex, int pageSize)
        {
            model.UserAgent = model.UserAgentId == 0 ? "" : ((UserAgentType)model.UserAgentId).ToString();

            var query =
               from order in _orderRepo.Table
               join attr in _genericAttributeRepo.Table on order.Id equals attr.EntityId
               where attr.KeyGroup == "Order"
               select new OrderViewModel()
               {
                   UserAgent = attr.Value,
                   OrderId = order.Id,
                   CreatedOnUtc = order.CreatedOnUtc,
                   CustomerId = order.CustomerId,
                   CustomerName = order.Customer.Username
               };

            if (model.OrderId != 0)
                query = query.Where(x => x.OrderId == model.OrderId);
            if (model.CustomerId != 0)
                query = query.Where(x => x.CustomerId == model.CustomerId);
            if (!string.IsNullOrWhiteSpace(model.UserAgent))
                query = query.Where(x => x.UserAgent.Contains(model.UserAgent));
            if (!string.IsNullOrWhiteSpace(model.CustomerName))
                query = query.Where(x => x.CustomerName.Contains(model.CustomerName));

            return new PagedList<OrderViewModel>(query.OrderByDescending(x => x.OrderId), pageIndex, pageSize);
        }
    }
}
