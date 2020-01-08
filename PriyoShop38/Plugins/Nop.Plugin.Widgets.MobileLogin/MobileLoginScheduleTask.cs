using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.MobileLogin.Services;
using Nop.Services.Tasks;

namespace Nop.Plugin.Widgets.MobileLogin
{
    public class MobileLoginScheduleTask : ITask
    {
        private readonly IMobileLoginService _mobileLoginService;
        public MobileLoginScheduleTask(IMobileLoginService mobileLoginService)
        {
            this._mobileLoginService = mobileLoginService;
        }

        public void Execute()
        {            
            var mobileLoginCustomers = _mobileLoginService.GetAll();
            var currentTime = DateTime.UtcNow;

            foreach (var mobileLoginCustomer in mobileLoginCustomers)
            {
                var updatedTime = mobileLoginCustomer.UpdatedOnUtc;
                var needToInvalidTime = updatedTime.AddMinutes(20);
                if (currentTime >= needToInvalidTime)
                {
                    mobileLoginCustomer.IsTokenValid = false;
                    _mobileLoginService.Update(mobileLoginCustomer);
                }
            }
        }

        public int Order
        {
            //ensure that this task is run first 
            get { return 0; }
        }
    }
}
