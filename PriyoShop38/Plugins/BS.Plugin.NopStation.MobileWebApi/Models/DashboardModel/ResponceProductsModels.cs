using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel
{
    public class ResponceProductsModels
    {
        public ResponceProductsModels()
        {
            Data = new Dictionary<string, ProuctDetailModels>();
        }

        public IDictionary<string, ProuctDetailModels> Data { get; set; }

        public string Status { get; set; }

        public string Error { get; set; }

        public string Plugin_Version { get; set; }
        
    }
}