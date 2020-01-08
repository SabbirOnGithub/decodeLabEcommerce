using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.Affiliates
{
    public class AffiliatedOrderListModel
    {
        public AffiliatedOrderListModel()
        {
            Orders = new List<AffiliatedOrderModel>();
        }

        public bool Active { get; set; }

        public string TotalCommission { get; set; }

        public string PayableCommission { get; set; }

        public string PaidCommission { get; set; }

        public string UnpaidCommission { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public bool HasPreviousPage { get; set; }

        public bool HasNextPage { get; set; }

        public IList<AffiliatedOrderModel> Orders { get; set; }
    }
}