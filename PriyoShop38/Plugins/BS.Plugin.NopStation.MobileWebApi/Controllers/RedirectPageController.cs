using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Controllers;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{


    public class RedirectPageController : BasePublicController
    {
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IWebHelper _webHelper;
        private readonly ILogger _logger;
        private readonly IStoreContext _storeContext;

        public RedirectPageController(IWorkContext workContext, OrderSettings orderSettings,
            IOrderService orderService, IPaymentService paymentService,
            IWebHelper webHelper, ILogger logger,
            IStoreContext storeContext, HttpContextBase httpContext)
        {
            this._workContext = workContext;
            this._orderSettings = orderSettings;
            this._orderService = orderService;
            this._paymentService = paymentService;
            this._webHelper = webHelper;
            this._logger = logger;
            this._storeContext = storeContext;
        }
        
        public ActionResult OpcCompleteRedirectionPayment()
        {
            try
            {
                if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                    return new HttpUnauthorizedResult();

                //get the order
                var order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1)
                    .FirstOrDefault();
                if (order == null)
                {
                    return RedirectToRoute("HomePage");
                }

                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(order.PaymentMethodSystemName);
                if (paymentMethod == null)
                {
                    return RedirectToRoute("HomePage");
                }
                if (paymentMethod.PaymentMethodType != PaymentMethodType.Redirection)
                {
                    return RedirectToRoute("HomePage");
                }

                //ensure that order has been just placed
                if ((DateTime.UtcNow - order.CreatedOnUtc).TotalMinutes > 3)
                {
                    return RedirectToRoute("HomePage");
                }

                //Redirection will not work on one page checkout page because it's AJAX request.
                //That's why we process it here
                var postProcessPaymentRequest = new PostProcessPaymentRequest
                {
                    Order = order
                };

                _paymentService.PostProcessPayment(postProcessPaymentRequest);

                if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
                {
                    //redirection or POST has been done in PostProcessPayment
                    return Content("Redirected");
                }

                //if no redirection has been done (to a third-party payment page)
                //theoretically it's not possible
                return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return Content(exc.Message);
            }
        }
    }
}
