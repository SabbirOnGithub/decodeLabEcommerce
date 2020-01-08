using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Web.Models.Customer;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Customer
{
    public class CustomerLedgerViewModel
    {
        public CustomerLedgerViewModel()
        {
            CustomerLedgerDetail = new List<CustomerLedgerDetailViewModel>();
        }
        public WalletAccountView WalletAccountView { get; set; }
        public CustomerLedgerMaster CustomerLedgerMaster { get; set; }
        public CustomerWalletPayment CustomerWalletPayment { get; set; }
        public IList<CustomerLedgerDetailViewModel> CustomerLedgerDetail { get; set; }
    }
}
