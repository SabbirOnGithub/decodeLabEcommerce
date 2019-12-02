using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.bKashAdvance.Models;
using Nop.Services.Payments;

namespace Nop.Plugin.Payments.bKashAdvance.Services
{
    public interface IBkashAdvanceService
    {
        PaymentResponseModel CreatePayment(Order order);

        PaymentResponseModel ExecutePayment(Order order);

        CapturePaymentResult CapturePayment(Order order);

        RefundPaymentResult RefundPayment(Order order);

        VoidPaymentResult VoidPayment(Order order);

        QueryResponseModel GetTransaction(string transactionId);

        QueryResponseModel GetBalance();

        QueryResponseModel GetPayment(string paymentId);
    }
}