using System;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.EblSkyPay.Models
{
    /// <summary>
    /// Represents a EblSkypay Transaction History
    /// </summary>
    public class EblSkyPayModel : BaseNopModel
    {
        public int Id { get; set; }
        public string Merchant { get; set; }
        public string Result { get; set; }
        public string SessionId { get; set; }
        public string SessionUpdateStatus { get; set; }
        public string SessionVersion { get; set; }
        public string SuccessIndicator { get; set; }
        public int OrderId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public string OrderRetriveResponse { get; set; }
        public bool Active { get; set; }
        public string Response { get; set; }
        public int PaymentStatusId { get; set; }
        public string PaymentDate { get; set; }

        public decimal Amount { get; internal set; }
        public string Status { get; internal set; }
        public string TrxDate { get; internal set; }
        public string TrxCurrency { get; internal set; }
        public string PaymentStatus { get; set;}
    }
}