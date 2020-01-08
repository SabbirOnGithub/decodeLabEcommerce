using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BS.Plugin.NopStation.MobileApp.Extensions
{
    public static class FcmExtension
    {
        public static TDestination RetriveData<TDestination>(string destinationUrl, string authCode, string requestXml = "", int type = 0)
        {
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
            request.Headers.Add("Authorization", "key=" + authCode);
            request.ContentType = "application/json";
            if (type == 1)
            {
                var bytes = Encoding.ASCII.GetBytes(requestXml);
                request.Method = "POST";
                request.ContentLength = bytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }
            else
            {
                request.Method = "GET";
            }
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    if (responseStream != null)
                    {
                        string responseStr = new StreamReader(responseStream).ReadToEnd();
                        if (!String.IsNullOrEmpty(responseStr))
                        {
                            return JsonConvert.DeserializeObject<TDestination>(responseStr);
                        }
                        return default(TDestination);
                    }
                }
                return default(TDestination);
            }
            catch (Exception)
            {
                return default(TDestination);
            }
        }
    }
}
