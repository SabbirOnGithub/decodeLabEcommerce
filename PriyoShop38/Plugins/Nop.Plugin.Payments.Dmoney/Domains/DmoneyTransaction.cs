using Nop.Core;
using Nop.Core.Domain.Orders;
using System;

namespace Nop.Plugin.Payments.Dmoney.Domains
{
    public class DmoneyTransaction : BaseEntity
    {
        public int Status { get; set; }

        public string ErrorMessage { get; set; }

        public string MerchantWalletNo { get; set; }

        public string CustomerWalletNo { get; set; }

        public decimal Amount { get; set; }

        public string TransactionType { get; set; }

        public int OrderId { get; set; }

        public string TransactionTime { get; set; }

        public string PaymentStatus { get; set; }

        public string TransactionReferenceId { get; set; }

        public string TransactionTrackingNo { get; set; }

        public string StatusMessage { get; set; }

        public bool Delete { get; set; }

        public DateTime CreatedOnUtc { get; set; } 

        public DateTime LastUpdatedOnUtc { get; set; }

        public TransactionStatus TransactionStatus { get; set; }

        public string ErrorCode { get;  set; }
    }
}
