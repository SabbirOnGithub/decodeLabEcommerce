using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;

namespace Nop.Plugin.Payments.EblSkyPay.Services
{
    public class SkyPayService: ISkyPayService
    {
        #region Constant

        private const string OTHOBA_SKYPAY_ITEM_BY_ID_KEY    = "Nop.skypay.id-{0}";
        private const string OTHOBA_SKYPAY_ITEM_PATTERN_KEY  = "Nop.skypay.";

        #endregion

        #region Field

        private readonly IRepository<Domain.EblSkyPay> _skyPayRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctr

        public SkyPayService(ICacheManager cacheManager, 
            IRepository<Domain.EblSkyPay> skyPayRepository)
        {
            _cacheManager = cacheManager;
            _skyPayRepository = skyPayRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create new Auction
        /// </summary>
        /// <param name="item"></param>
        public void Insert(Domain.EblSkyPay item)
        {
            _skyPayRepository.Insert(item);
            _cacheManager.RemoveByPattern(OTHOBA_SKYPAY_ITEM_BY_ID_KEY);
        }

        /// <summary>
        /// Remove auction by id
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            var auction = GetById(id);

            if (auction == null)
                throw new ArgumentNullException("skypay");

            _skyPayRepository.Delete(auction);

            _cacheManager.RemoveByPattern(OTHOBA_SKYPAY_ITEM_PATTERN_KEY);
        }

        /// <summary>
        /// Update auction
        /// </summary>
        /// <param name="item"></param>
        public void Update(Domain.EblSkyPay item)
        {
            if (item == null)
                throw new ArgumentNullException("skypay");

            var data = GetById(item.Id);

            data.Merchant = item.Merchant;
            data.OrderId = item.OrderId;
            data.Result = item.Result;
            data.SessionId = item.SessionId;
            data.SessionUpdateStatus = item.SessionUpdateStatus;
            data.SessionVersion = item.SessionVersion;
            data.SuccessIndicator = item.SuccessIndicator;
            data.OrderRetriveResponse = item.OrderRetriveResponse;
            data.Response = item.Response;
            data.Active = item.Active;
            data.PaymentDate = item.PaymentDate;
            data.PaymentStatusId = item.PaymentStatusId;

            _skyPayRepository.Update(data);
            _cacheManager.RemoveByPattern(OTHOBA_SKYPAY_ITEM_PATTERN_KEY);
        }

        /// <summary>
        /// Get auction by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Domain.EblSkyPay GetById(int id)
        {
            if (id == 0)
                return null;
            string key = string.Format(OTHOBA_SKYPAY_ITEM_BY_ID_KEY, id);

            return _cacheManager.Get(key, () =>
            {
                var n = _skyPayRepository.Table.FirstOrDefault(x=>x.Id==id && x.Active);
                return n;
            });
        }

        /// <summary>
        /// Get all auction 
        /// </summary>
        /// <returns></returns>
        public IList<Domain.EblSkyPay> GetAllItems()
        {
            var query = from s in _skyPayRepository.Table
                        where s.Active
                        select s;

            query = query.OrderBy(w => w.Id);

            return query.ToList();
        }

        /// <summary>
        /// Get all auction [paged list]
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IPagedList<Domain.EblSkyPay> GetAllPagedItems(int pageIndex, int pageSize)
        {
            var query = from t in _skyPayRepository.Table
                        where t.Active
                        select t;

            query = query.OrderByDescending(w => w.OrderId).ThenByDescending(x=>x.Id);

            return new PagedList<Domain.EblSkyPay>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Get by orderId
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public IList<Domain.EblSkyPay> GetByOrderId(int orderId)
        {
            var query = from r in _skyPayRepository.Table
                where r.OrderId == orderId && r.Active
                select r;

            return query.ToList();
        }

        /// <summary>
        /// Set status inactive
        /// </summary>
        /// <param name="orderId"></param>
        public void SetAsInActiveByOrderId(int orderId)
        {
            var query = from r in _skyPayRepository.Table
                        where r.OrderId == orderId && r.Active
                        select r;

            foreach (var item in query.ToList())
            {
                item.Active = false;
                _skyPayRepository.Update(item);
            }
        }

        #endregion
    }
}