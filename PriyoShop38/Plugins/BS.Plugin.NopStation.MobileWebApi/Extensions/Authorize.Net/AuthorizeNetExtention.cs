using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Payments;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileWebApi.PluginSettings;
using Nop.Services.Directory;
using Nop.Services.Orders;
using Nop.Services.Payments;

namespace BS.Plugin.NopStation.MobileWebApi.Extensions.Authorize.Net
{
    public class AuthorizeNetExtention
    {

        private static string GetApiVersion()
        {
            return "3.1";
        }
        private static string GetAuthorizeNetUrl(AuthorizeNetPaymentSettings authorizeNetPaymentSettings)
        {
            return authorizeNetPaymentSettings.UseSandbox ? "https://test.authorize.net/gateway/transact.dll" :
                "https://secure.authorize.net/gateway/transact.dll";
        }
        public static ProcessPaymentResult ExcuteTransaction(AuthorizeQueryModel authorizeNet, AuthorizeNetPaymentSettings authorizeNetPaymentSettings, ICurrencyService currencyService, CurrencySettings currencySettings, Customer customer, IOrderService orderService)
        {
            var result = new ProcessPaymentResult();
            var order = orderService.GetOrderById(authorizeNet.OrderId);
            var webClient = new WebClient();
            var form = new NameValueCollection();
            form.Add("x_login", authorizeNetPaymentSettings.LoginId);
            form.Add("x_tran_key", authorizeNetPaymentSettings.TransactionKey);

            //we should not send "x_test_request" parameter. otherwise, the transaction won't be logged in the sandbox
            //if (_authorizeNetPaymentSettings.UseSandbox)
            //    form.Add("x_test_request", "TRUE");
            //else
            //    form.Add("x_test_request", "FALSE");

            form.Add("x_delim_data", "TRUE");
            form.Add("x_delim_char", "|");
            form.Add("x_encap_char", "");
            form.Add("x_version", GetApiVersion());
            form.Add("x_relay_response", "FALSE");
            form.Add("x_method", "CC");
            form.Add("x_currency_code", currencyService.GetCurrencyById(currencySettings.PrimaryStoreCurrencyId).CurrencyCode);
            if (authorizeNetPaymentSettings.TransactMode == TransactMode.Authorize)
                form.Add("x_type", "AUTH_ONLY");
            else if (authorizeNetPaymentSettings.TransactMode == TransactMode.AuthorizeAndCapture)
                form.Add("x_type", "AUTH_CAPTURE");
            else
                throw new NopException("Not supported transaction mode");

            var orderTotal = Math.Round(order.OrderTotal, 2);
            form.Add("x_amount", orderTotal.ToString("0.00", CultureInfo.InvariantCulture));
            form.Add("x_card_num", authorizeNet.CreditCardNumber);
            form.Add("x_exp_date", authorizeNet.CreditCardExpireMonth.ToString("D2") + authorizeNet.CreditCardExpireYear.ToString());
            form.Add("x_card_code", authorizeNet.CreditCardCvv2);
            form.Add("x_first_name", customer.BillingAddress.FirstName);
            form.Add("x_last_name", customer.BillingAddress.LastName);
            form.Add("x_email", customer.BillingAddress.Email);
            if (!string.IsNullOrEmpty(customer.BillingAddress.Company))
                form.Add("x_company", customer.BillingAddress.Company);
            form.Add("x_address", customer.BillingAddress.Address1);
            form.Add("x_city", customer.BillingAddress.City);
            if (customer.BillingAddress.StateProvince != null)
                form.Add("x_state", customer.BillingAddress.StateProvince.Abbreviation);
            form.Add("x_zip", customer.BillingAddress.ZipPostalCode);
            if (customer.BillingAddress.Country != null)
                form.Add("x_country", customer.BillingAddress.Country.TwoLetterIsoCode);
            //x_invoice_num is 20 chars maximum. hece we also pass x_description
            form.Add("x_invoice_num", order.OrderGuid.ToString().Substring(0, 20));
            form.Add("x_description", string.Format("Full order #{0}", order.OrderGuid));
            //form.Add("x_customer_ip", _webHelper.GetCurrentIpAddress());

            var responseData = webClient.UploadValues(GetAuthorizeNetUrl(authorizeNetPaymentSettings), form);
            var reply = Encoding.ASCII.GetString(responseData);

            if (!String.IsNullOrEmpty(reply))
            {
                string[] responseFields = reply.Split('|');
                switch (responseFields[0])
                {
                    case "1":
                        result.AuthorizationTransactionCode = string.Format("{0},{1}", responseFields[6], responseFields[4]);
                        result.AuthorizationTransactionResult = string.Format("Approved ({0}: {1})", responseFields[2], responseFields[3]);
                        result.AvsResult = responseFields[5];
                        //responseFields[38];
                        if (authorizeNetPaymentSettings.TransactMode == TransactMode.Authorize)
                        {
                            result.NewPaymentStatus = PaymentStatus.Authorized;
                        }
                        else
                        {
                            result.NewPaymentStatus = PaymentStatus.Paid;
                        }
                        break;
                    case "2":
                        result.AddError(string.Format("Declined ({0}: {1})", responseFields[2], responseFields[3]));
                        break;
                    case "3":
                        result.AddError(string.Format("Error: {0}", reply));
                        break;

                }
            }
            else
            {
                result.AddError("Authorize.NET unknown error");
            }

            return result;
            
            
        }
    }
}
