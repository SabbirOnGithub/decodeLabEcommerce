using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Payments.EblSkyPay.Models
{
    public class FailOrderModel : BaseNopModel
    {
        public int OrderId { get; set; }
        public string FailReason { get; set; }
        public DataLayerTrxModel TrxModel { get; set; }
    }
}