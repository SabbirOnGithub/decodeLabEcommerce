using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Plugin.Widgets.EkShopA2I.Domain;

namespace Nop.Plugin.Widgets.EkShopA2I.Services
{
    public class EkshopOrderService : IEkshopOrderService
    {
        #region Fields

        private readonly IRepository<EsOrder> _esOrderRepository;

        #endregion

        #region Ctor

        public EkshopOrderService(IRepository<EsOrder> esOrderRepository)
        {
            _esOrderRepository = esOrderRepository;
        }

        #endregion

        public IPagedList<EsOrder> GetEkshopOrders(string orderCode = "", string lpCode = "", string lpContactNumber = "", 
            DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _esOrderRepository.Table;

            if (!string.IsNullOrWhiteSpace(orderCode))
                query = query.Where(x => x.OrderCode.Contains(orderCode));
            if (!string.IsNullOrWhiteSpace(lpCode))
                query = query.Where(x => x.LpCode.Contains(lpCode));
            if (!string.IsNullOrWhiteSpace(lpContactNumber))
                query = query.Where(x => x.LpContactNumber.Contains(lpContactNumber));
            if (startDate.HasValue)
                query = query.Where(x => x.CreatedOn >= startDate);
            if (endDate.HasValue)
                query = query.Where(x => x.CreatedOn <= endDate);

            query = query.OrderByDescending(x => x.CreatedOn);

            return new PagedList<EsOrder>(query, pageIndex, pageSize);
        }

        public EsOrder GetEkshopOrderById(int id)
        {
            return _esOrderRepository.GetById(id);
        }
    }
}
