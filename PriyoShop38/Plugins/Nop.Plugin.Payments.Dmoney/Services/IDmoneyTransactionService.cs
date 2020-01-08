using Nop.Core;
using Nop.Plugin.Payments.Dmoney.Domains;
using System;

namespace Nop.Plugin.Payments.Dmoney.Services
{
    public interface IDmoneyTransactionService
    {
        IPagedList<DmoneyTransaction> GetAllTransactions(DateTime? fromDate = null, DateTime? toDate = null,
            int orderId = 0, int pageIndex = 0, int pageSize = int.MaxValue);

        DmoneyTransaction GetTransactionByTrackingNo(string transactionTrackingNo);

        void InsertTransaction(DmoneyTransaction dmoneyTransaction);

        void UpdateTransaction(DmoneyTransaction dmoneyTransaction);

        void DeleteTransaction(DmoneyTransaction dmoneyTransaction);
    }
}