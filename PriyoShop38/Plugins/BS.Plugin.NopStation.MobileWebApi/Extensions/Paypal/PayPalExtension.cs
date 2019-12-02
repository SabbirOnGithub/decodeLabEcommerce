using System;
using System.Collections.Generic;
using System.Linq;
using PayPal.Api;

namespace BS.Plugin.NopStation.MobileWebApi.Extensions.Paypal
{
    public static class PayPalExtension
    {
        public  static string ClientId="AQGvRxqTvxUMakgJc8M5q4W7Jw-WryPypJNsz-5Ns_z9Z19SL1TumRnil6LUf5Z3MMFwo4Uv2ikqxPlt";
        public  static string ClientSecret;



        private static Dictionary<string, string> Configuration() 
        {
            var defaultConfig = new Dictionary<string, string>();
            // Default connection timeout in milliseconds
            defaultConfig[BaseConstants.HttpConnectionTimeoutConfig] = "30000";
            defaultConfig[BaseConstants.HttpConnectionRetryConfig] = "3";
            defaultConfig[BaseConstants.ApplicationModeConfig] = BaseConstants.SandboxMode;

            return defaultConfig;
        }

        // Create accessToken
        private static string GetAccessToken()
        {

            var config = Configuration();
            ClientSecret = "EKld9w88GVm_A5aSaaPFtq9i1svLpM-N1rUqwkMVZtUVTc_9N6kYbdVSngsh7llfVOdtgt3FSujEUjoB";
            string accessToken = new OAuthTokenCredential(ClientId, ClientSecret, config).GetAccessToken();
            return accessToken;
        }

        // Returns APIContext object
        public static APIContext GetAPIContext(string accessToken = "")
        {
             
            var apiContext = new APIContext(string.IsNullOrEmpty(accessToken) ? GetAccessToken() : accessToken);
            apiContext.Config = Configuration();


            return apiContext;
        }

        public static PaypalDetailModel GetAmount(string paymentId)
        {
            var apiContext = GetAPIContext();
            var payDetail = new PaypalDetailModel();
            try
            {
               
                var payment = Payment.Get(apiContext, paymentId);
                if (payment.state.Equals("approved"))
                {
                    payDetail.PaymentStatus = payment.state;
                    payDetail.Currency = payment.transactions.FirstOrDefault().amount.currency;
                    payDetail.Total = payment.transactions.FirstOrDefault().amount.total;
                    payDetail.PayeeId = payment.payer.payer_info.payer_id;

                }
            }
            catch (Exception)
            {
                payDetail = null;

            }
            return payDetail;
        }

    }
}
 
