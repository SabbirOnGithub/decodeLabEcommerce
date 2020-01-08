using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.EblSkyPay.Models
{
    public class DataLayerTrxModel
    {
        public DataLayerTrxModel()
        {
            Products = new List<MinProductModel>();
        }
        public int OrderId { get; set; }
        public List<MinProductModel> Products;
        public string SourcePage { get; set; }
        public string PayemntMethods { get; set; }
        public string List { get; set; }
    }
    public class MinProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string VendorName { get; set; }
        public string ManufacturerName { get; set; }
        public decimal Price { get; set; }
        public decimal ProductCost { get; set; }
        public int Quantity { get; set; }
    }
}
