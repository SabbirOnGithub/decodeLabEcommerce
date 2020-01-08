using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Dmoney.Models;
using Nop.Services.Logging;
using Nop.Services.Orders;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Nop.Plugin.Payments.Dmoney.Services
{
    public class DmoneyPaymentService : IDmoneyPaymentService
    {
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;
        private readonly DmoneyPaymentSettings _dmoneyPaymentSettings;
        private readonly IDmoneyTransactionService _dmoneyTransactionService;

        public DmoneyPaymentService(ILogger logger,
            DmoneyPaymentSettings dmoneyPaymentSettings, 
            IOrderService orderService,
            IDmoneyTransactionService dmoneyTransactionService)
        {
            this._logger = logger;
            this._dmoneyPaymentSettings = dmoneyPaymentSettings;
            this._orderService = orderService;
            this._dmoneyTransactionService = dmoneyTransactionService;
        }

        public void CheckPaymentTransactionStatus(string transactionTrackingNo)
        {
            try
            {
                var transaction = _dmoneyTransactionService.GetTransactionByTrackingNo(transactionTrackingNo);
                if (transaction == null)
                    throw new NopException("transaction");

                var order = _orderService.GetOrderById(transaction.OrderId);

                var postModel = new { transactionTrackingNo };

                var jsonModel = JsonConvert.SerializeObject(postModel);
                var data = Encoding.ASCII.GetBytes(jsonModel);

                var request = (HttpWebRequest)WebRequest.Create(_dmoneyPaymentSettings.TransactionVerificationUrl);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.ContentLength = data.Length;

                request.Headers.Add("orgCode", _dmoneyPaymentSettings.OrgCode);
                request.Headers.Add("password", _dmoneyPaymentSettings.Password);
                request.Headers.Add("secretKey", _dmoneyPaymentSettings.SecretKey);

                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                if (_dmoneyPaymentSettings.EnableLog)
                    _logger.Information("Create Payment: " + responseString);
                
                var model = JsonConvert.DeserializeObject<TransactionStatusModel>(responseString);

                transaction.LastUpdatedOnUtc = DateTime.UtcNow;
                transaction.MerchantWalletNo = model.data.merchantWalletNo;
                transaction.PaymentStatus = model.data.paymentStatus;
                transaction.Status = model.status;
                transaction.StatusMessage = model.data.statusMessage;
                transaction.TransactionReferenceId = model.data.transactionReferenceId;
                transaction.TransactionTime = model.data.transactionTime;
                transaction.TransactionType = model.data.transactionType;
                transaction.Amount = model.data.amount;
                transaction.CustomerWalletNo = model.data.customerWalletNo;
                transaction.ErrorMessage = model.error.message;
                transaction.ErrorCode = model.error.code; 

                _dmoneyTransactionService.UpdateTransaction(transaction);


                if (model.status == 200 && model.data.statusCode.Equals("SUCCESS", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (model.data.paymentStatus.Equals("COMPLETED", StringComparison.InvariantCultureIgnoreCase) &&
                        model.data.amount >= order.OrderTotal)
                    {
                        order.PaymentStatus = PaymentStatus.Paid;
                        order.OrderStatus = OrderStatus.Processing;
                        _orderService.UpdateOrder(order);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

    }
}
