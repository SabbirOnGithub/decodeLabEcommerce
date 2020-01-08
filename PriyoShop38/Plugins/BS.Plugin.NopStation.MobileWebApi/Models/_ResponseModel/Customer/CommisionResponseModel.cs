using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Customer
{
    public class CommisionResponseModel
    {
        public decimal TotalCommission { get; set; }
        public decimal TotalWithDrawn { get; set; }
        public decimal TotalBalance { get; set; }

    }
}
