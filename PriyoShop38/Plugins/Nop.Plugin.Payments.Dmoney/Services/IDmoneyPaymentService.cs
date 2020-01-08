using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Payments.Dmoney.Services
{
    public interface IDmoneyPaymentService
    {
        void CheckPaymentTransactionStatus(string transactionTrackingNo);
    }
}