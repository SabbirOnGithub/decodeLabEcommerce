using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Web.Framework.Mvc;
using System.Collections;
using System.Web.Mvc;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel
{
    public class ProductModels 
    {

        public ProductModels()
        {
            productDetail = new ProuctDetailModels();
        }

        public int product_id { get; set; }

        public ProuctDetailModels productDetail { get; set; }

    }

    public partial class ProuctDetailModels
    {
        public ProuctDetailModels()
        {

        }

        public int product_id { get; set; }
        public string product_sku { get; set; }
        public string product_name { get; set; }
        public DateTime modified_on { get; set; }
        public string productThumbPath { get; set; }
        public string prod_url { get; set; }
        public string[] categories { get; set; }
    }
}