using System.Collections.Generic;

namespace Nop.Plugin.Widgets.BsAffiliate.Models
{
    public class AffiliatePublicDetailsModel
    {
        public AffiliatePublicDetailsModel()
        {
            Orders = new List<AffiliatedOrderModel>();
        }

        public string Total { get; set; }
        public string Paid { get; set; }
        public string Unpaid { get; set; }
        public string Payable { get; set; }
        public bool Active { get; set; }

        public IList<AffiliatedOrderModel> Orders { get; set; }
    }
}
