using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;

namespace Nop.Plugin.Widgets.EkShopA2I.Models
{
    public class EsOrderModel : BaseNopModel
    {
        public int Id { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.Domain.OrderCode")]
        public string OrderCode { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.Domain.LpCode")]
        public string LpCode { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.Domain.LpName")]
        public string LpName { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.Domain.LpContactPerson")]
        public string LpContactPerson { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.Domain.LpContactNumber")]
        public string LpContactNumber { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.Domain.LpLocation")]
        public string LpLocation { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.Domain.DeliveryCharge")]
        public int DeliveryCharge { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.Domain.PaymentMethod")]
        public int PaymentMethod { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.Domain.Total")]
        public decimal Total { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.Domain.DeliveryDuration")]
        public string DeliveryDuration { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.Domain.OtherRequiredData")]
        public string OtherRequiredData { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.Domain.CreatedOn")]
        public DateTime CreatedOn { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.Domain.UdcCommission")]
        public decimal? UdcCommission { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.Domain.OrderId")]
        public int OrderId { get; set; }
    }
}
