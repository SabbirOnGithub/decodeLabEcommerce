using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Customer
{
    public class CustomerPriyoCoinModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerEmail { get; set; }
        public int? PointNumber { get; set; }
        public string PointSourceDescription { get; set; }
        public string PointType { get; set; }
        public DateTime? Date { get; set; }
    }
}
