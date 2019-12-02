using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.EblSkyPay.Models
{
    public class ErrorCallbackModel : BaseNopModel
    {
        public int OrderId { get; set; }
        public string FailReason { get; set; }
    }
}