using Newtonsoft.Json;
using Nop.Plugin.Payments.Dmoney.Domains;
using Nop.Web.Framework.Mvc;
using System;

namespace Nop.Plugin.Payments.Dmoney.Models
{
    public class ErrorModel
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class DataModel
    {
        public string merchantWalletNo { get; set; }
        public string customerWalletNo { get; set; }
        public decimal amount { get; set; }
        public string transactionType { get; set; }
        public string billOrInvoiceNo { get; set; }
        public string transactionTime { get; set; }
        public string statusCode { get; set; }
        public string statusMessage { get; set; }
        public string paymentStatus { get; set; }
        public string transactionReferenceId { get; set; }
        public string transactionTrackingNo { get; set; }
    }

    public class TransactionStatusModel
    {
        public TransactionStatusModel()
        {
            error = new ErrorModel();
            data = new DataModel();
        }

        public ErrorModel error { get; set; }
        public int status { get; set; }
        public DataModel data { get; set; }
        public string requestId { get; set; }
    }
}
