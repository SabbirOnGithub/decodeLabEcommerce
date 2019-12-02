using Nop.Plugin.Purchase.UserAgent.Models;
using Nop.Plugin.Purchase.UserAgent.Services;
using Nop.Services;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Purchase.UserAgent.Controllers
{
    public class PurchaseUserAgentController : BasePluginController
    {
        private readonly IPurchaseUserAgentService _purchaseUserAgentService;
        public PurchaseUserAgentController(IPurchaseUserAgentService purchaseUserAgentService)
        {
            _purchaseUserAgentService = purchaseUserAgentService;
        }

        public ActionResult List()
        {
            var model = new OrderViewModel();
            model.AvailableUserAgentTypes = UserAgentType.Mobile.ToSelectList().ToList();
            model.AvailableUserAgentTypes.Insert(0, new SelectListItem() { Value = "0", Text = "All" });
            return View("~/Plugins/Purchase.UserAgent/Views/PurchaseUserAgent/List.cshtml", model);
        }

        [HttpPost]
        public JsonResult GetList(DataSourceRequest command, OrderViewModel model)
        {
            var orders = _purchaseUserAgentService.GetOrderList(model, command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = orders,
                Total = orders.TotalCount
            };
            return Json(gridModel);
        }
    }
}
