using System;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel
{   
    public partial class ShoppingCartItemModel : BaseNopEntityModel
    {
       
        public string Store { get; set; }
       
        public int ProductId { get; set; }
      
        public string ProductName { get; set; }
        public string AttributeInfo { get; set; }

      
        public string UnitPrice { get; set; }
       
        public int Quantity { get; set; }
     
        public string Total { get; set; }
       
        public DateTime UpdatedOn { get; set; }
    }
}