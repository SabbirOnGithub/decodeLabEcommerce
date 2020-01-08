using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.Payza.Models
{
    public class PaymentInfoModel : BaseNopModel
    {
       
        public IDictionary<string, string> Parameters { get; set; }
    
        public string ConfirmUrl { get; set; }
    }
}