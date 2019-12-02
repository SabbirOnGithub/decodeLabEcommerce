using Nop.Core;
using System;

namespace Nop.Plugin.Widgets.EkShopA2I.Domain
{
    public class EsOrder : BaseEntity
    {
        public string OrderCode { get; set; }
        public string LpCode { get; set; }
        public string LpName { get; set; }
        public string LpContactPerson { get; set; }
        public string LpContactNumber { get; set; }
        public string LpLocation { get; set; }
        public int DeliveryCharge { get; set; }
        public int PaymentMethod { get; set; }
        public decimal Total { get; set; }
        public string DeliveryDuration { get; set; }
        public string OtherRequiredData { get; set; }
        public DateTime CreatedOn { get; set; }
        public decimal? UdcCommission { get; set; }
        public int OrderId { get; set; }
    }
}
