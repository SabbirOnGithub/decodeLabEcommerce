using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Purchase.UserAgent.Models;
using Nop.Services.Common;
using Nop.Services.Events;
using System.Web;

namespace Nop.Plugin.Purchase.UserAgent.Services
{
    public class OrderConsumer : IConsumer<OrderPlacedEvent>
    {
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;

        public OrderConsumer(IGenericAttributeService genericAttributeService,
            IWorkContext workContext, IStoreContext storeContext)
        {
            _genericAttributeService = genericAttributeService;
            _workContext = workContext;
            _storeContext = storeContext;
        }

        public void HandleEvent(OrderPlacedEvent eventMessage)
        {
            var agentName = HttpContext.Current.Request
                .UserAgent
                .ToLower()
                .StartsWith("okhttp") ? "Mobile" : "Web";

            _genericAttributeService.SaveAttribute(eventMessage.Order, OrderAttributeNames.UserAgent, agentName); 
        }
    }
}
