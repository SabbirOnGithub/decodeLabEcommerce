using Nop.Core;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Plugin.Misc.OnePageCheckOutAdmin.Models.ShoppingCart;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;

using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Misc.OnePageCheckOutAdmin.Controllers
{
    
    public class DelivaryDateTimeController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerService _customerService;
        
        private readonly IWorkContext _workContext;
        public  readonly int DeliveryAtATime = 5;
        public DelivaryDateTimeController( 
            IOrderService orderService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            ICustomerService customerService,
            ISettingService settingService,
            IWorkContext workContext
            )
        {
            this._orderService = orderService;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._storeContext = storeContext;
            this._customerService = customerService;            
            this._workContext = workContext;
        }

        
       
        [HttpPost]
        public ActionResult SetAvailableDatetime(AdminDeliveryTimeQueryModel model)
        {
            var date = new DateTime(model.Year, model.Month, model.Day);
            var customer = _customerService.GetCustomerByEmail(model.CustomerEmail)??_workContext.CurrentCustomer;
            var dateTimes = NextDays();
            var present = from dateTime in dateTimes
                          where dateTime.Date == date.Date
                          select dateTime;
            var responseModel = new GeneralResponseModel<bool>();
            var count = 0;
            //var setting = _settingService.GetSettingByKey("Order.DeliveryAtATime", 0);            

            if (!responseModel.Data)
            {
                if (count == -1)
                {
                    responseModel.ErrorList.Add(_localizationService.GetResource("deliverytime.error"));
                }
                else
                {
                    responseModel.ErrorList.Add(_localizationService.GetResource("deliverytime.not.available"));
                }
                responseModel.Data = false;
                responseModel.StatusCode = 400;
            }
            return Json(responseModel);
        }

        public List<DateTime> NextDays()
        {
            var nextFiveWeekDays = new List<DateTime>();
            var testDate = DateTime.UtcNow.AddHours(2);

            while (nextFiveWeekDays.Count() < 5)
            {
                if (testDate.DayOfWeek != DayOfWeek.Friday)
                    nextFiveWeekDays.Add(testDate);

                testDate = testDate.AddDays(1);
            }

            return nextFiveWeekDays;
        }
    }
}
