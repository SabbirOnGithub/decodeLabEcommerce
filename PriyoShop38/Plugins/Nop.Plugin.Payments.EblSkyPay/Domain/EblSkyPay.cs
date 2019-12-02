using System;
using Nop.Core;
using Nop.Core.Domain.Payments;

namespace Nop.Plugin.Payments.EblSkyPay.Domain
{
    /// <summary>
    /// Represents a EblSkypay Transaction History
    /// </summary>
    public class EblSkyPay : BaseEntity
    {
        public string Merchant { get; set; }
        public string Result { get; set; }
        public string SessionId { get; set; }
        public string SessionUpdateStatus { get; set; }
        public string SessionVersion { get; set; }
        public string SuccessIndicator { get; set; }
        public int OrderId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public string Response { get; set; }
        public string OrderRetriveResponse { get; set; }
        public bool Active { get; set; }
        public int PaymentStatusId { get; set; }
        public string PaymentDate { get; set; }

        /// <summary>
        /// Gets or sets the payment status
        /// </summary>
        public PaymentStatus PaymentStatus
        {
            get
            {
                return (PaymentStatus)this.PaymentStatusId;
            }
            set
            {
                this.PaymentStatusId = (int)value;
            }
        }
    }
}