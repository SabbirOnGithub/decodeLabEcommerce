using Nop.Core;
using Nop.Core.Data;
using Nop.Plugin.Payments.Dmoney.Domains;
using System;
using System.Linq;

namespace Nop.Plugin.Payments.Dmoney.Services
{
    public class DmoneyTransactionService : IDmoneyTransactionService
    {
        #region Fields

        private readonly IRepository<DmoneyTransaction> _dmoneyTransactionRepository;

        #endregion

        #region Ctor

        public DmoneyTransactionService(IRepository<DmoneyTransaction> dmoneyTransactionRepository)
        {
            this._dmoneyTransactionRepository = dmoneyTransactionRepository;
        }
        #endregion


        public virtual IPagedList<DmoneyTransaction> GetAllTransactions(DateTime? fromDate = null, DateTime? toDate = null, 
            int orderId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _dmoneyTransactionRepository.Table;

            if (fromDate.HasValue)
                query = query.Where(x => x.CreatedOnUtc >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.CreatedOnUtc <= toDate.Value);

            if (orderId > 0)
                query = query.Where(x => x.OrderId == orderId);

            query = query.OrderByDescending(x => x.CreatedOnUtc);

            var transactions = new PagedList<DmoneyTransaction>(query, pageIndex, pageSize);
            return transactions;
        }

        public DmoneyTransaction GetTransactionByTrackingNo(string transactionTrackingNo)
        {
            return _dmoneyTransactionRepository.Table.Where(x => x.TransactionTrackingNo == transactionTrackingNo).FirstOrDefault();
        }

        public virtual void InsertTransaction(DmoneyTransaction dmoneyTransaction)
        {
            if (dmoneyTransaction == null)
            {
                throw new ArgumentNullException("dmoneyTransaction");
            }

            _dmoneyTransactionRepository.Insert(dmoneyTransaction);
        }

        public virtual void UpdateTransaction(DmoneyTransaction dmoneyTransaction)
        {
            if (dmoneyTransaction == null)
            {
                throw new ArgumentNullException("dmoneyTransaction");
            }

            _dmoneyTransactionRepository.Update(dmoneyTransaction);
        }

        public virtual void DeleteTransaction(DmoneyTransaction dmoneyTransaction)
        {
            if (dmoneyTransaction == null)
            {
                throw new ArgumentNullException("dmoneyTransaction");
            }

            dmoneyTransaction.Delete = true;

            _dmoneyTransactionRepository.Update(dmoneyTransaction);
        }

    }
}
