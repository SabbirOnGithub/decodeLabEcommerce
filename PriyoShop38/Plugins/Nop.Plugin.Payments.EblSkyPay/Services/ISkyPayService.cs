using System.Collections.Generic;
using Nop.Core;

namespace Nop.Plugin.Payments.EblSkyPay.Services
{
    public interface ISkyPayService
    {
        void Insert(Domain.EblSkyPay item);
        void Delete(int id);
        void Update(Domain.EblSkyPay item);
        Domain.EblSkyPay GetById(int id);
        IList<Domain.EblSkyPay> GetAllItems();
        IPagedList<Domain.EblSkyPay> GetAllPagedItems(int pageIndex, int pageSize);
        IList<Domain.EblSkyPay> GetByOrderId(int orderId);
        void SetAsInActiveByOrderId(int orderId);
    }
}