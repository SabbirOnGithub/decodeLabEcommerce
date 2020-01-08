using Nop.Plugin.Payments.EblSkyPay.Models;

namespace Nop.Plugin.Payments.EblSkyPay.Extensions
{
    public static class MappingExtensions
    {
        public static EblSkyPayModel ToModel(this Domain.EblSkyPay entity)
        {
            if (entity == null)
                return null;

            var model = new EblSkyPayModel()
            {
                Id = entity.Id,
                OrderId = entity.OrderId,
                SuccessIndicator = entity.SuccessIndicator,
                SessionUpdateStatus = entity.SessionUpdateStatus,
                Merchant = entity.Merchant,
                SessionVersion = entity.SessionVersion,
                SessionId = entity.SessionId,
                Result = entity.Result,
                CreatedOnUtc= entity.CreatedOnUtc,
                OrderRetriveResponse = entity.OrderRetriveResponse,
                Response=entity.Response,
                PaymentStatusId=entity.PaymentStatusId,
                PaymentDate=entity.PaymentDate,
                Active=entity.Active,
            };
            return model;
        }

        public static Domain.EblSkyPay ToEntity(this EblSkyPayModel model)
        {
            var entity = new Domain.EblSkyPay()
            {
                Id = model.Id,
                Merchant = model.Merchant,
                Result = model.Result,
                SessionId = model.SessionId,
                SessionUpdateStatus = model.SessionUpdateStatus,
                SessionVersion = model.SessionVersion,
                OrderId = model.OrderId,
                SuccessIndicator = model.SuccessIndicator,
                CreatedOnUtc = model.CreatedOnUtc,
                OrderRetriveResponse = model.OrderRetriveResponse,
                Active=model.Active,
                PaymentDate=model.PaymentDate,
                PaymentStatusId=model.PaymentStatusId,
                Response=model.Response
            };
            return entity;
        }
    }
}