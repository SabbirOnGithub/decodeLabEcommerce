using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Customer
{
    public class CustomerLedgerDetailViewModel
    {
        public int Id { get; set; }
        public long SystemID { get; set; }
        public long ContactNo { get; set; }
        public long LedgerMasterID { get; set; }
        public string AmountDescription { get; set; }
        public decimal Amount { get; set; }
        public string AmountType { get; set; }
        public byte AmountSource { get; set; }
        public long LastAddedBy { get; set; }
        public DateTime LastAddedDate { get; set; }
    }
}
