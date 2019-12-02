using System;
using System.Linq;
using System.Web.Http.Routing;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Payment;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Controllers;
using Nop.Web.Framework.Controllers;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{

      [AdminAuthorize]
    public class MobileWebApiAdminController : BasePublicController
    {
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IWebHelper _webHelper;
        private readonly ILogger _logger;
        private readonly IStoreContext _storeContext;
        public MobileWebApiAdminController(IWorkContext workContext, OrderSettings orderSettings,
            IOrderService orderService, IPaymentService paymentService,
            IWebHelper webHelper, ILogger logger,
            IStoreContext storeContext)
        {
            this._workContext = workContext;
            this._orderSettings = orderSettings;
            this._orderService = orderService;
            this._paymentService = paymentService;
            this._webHelper = webHelper;
            this._logger = logger;
            this._storeContext = storeContext;

        }

        [ChildActionOnly]
        public ActionResult Configure()
        {
            return View("~/Plugins/NopStation.MobileWebApi/Views/BsInstagramAdMarket/Configure.cshtml");
        }
    }
}
