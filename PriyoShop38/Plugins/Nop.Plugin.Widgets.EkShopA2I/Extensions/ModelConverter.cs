using Nop.Plugin.Widgets.EkShopA2I.Domain;
using Nop.Plugin.Widgets.EkShopA2I.Models;

namespace Nop.Plugin.Widgets.EkShopA2I.Extensions
{
    public static class ModelConverter
    {
        public static EsOrderModel ToModel(this EsOrder order)
        {
            var model = new EsOrderModel()
            {
                Id = order.Id,
                CreatedOn = order.CreatedOn.AddHours(6),
                DeliveryCharge = order.DeliveryCharge,
                DeliveryDuration = order.DeliveryDuration,
                LpCode = order.LpCode,
                LpContactNumber = order.LpContactNumber,
                LpContactPerson = order.LpContactPerson,
                LpLocation = order.LpLocation,
                LpName = order.LpName,
                OrderCode = order.OrderCode,
                OrderId = order.OrderId,
                OtherRequiredData = order.OtherRequiredData,
                PaymentMethod = order.PaymentMethod,
                Total = order.Total,
                UdcCommission = order.UdcCommission
            };
            return model;
        }
    }
}
