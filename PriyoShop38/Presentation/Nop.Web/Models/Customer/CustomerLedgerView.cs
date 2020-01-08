using Nop.Core.Domain.Customers;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.Customer
{
    public class CustomerLedgerView : BaseNopModel
    {
        public WalletAccountView WalletAccountView { get; set; }
        public CustomerLedgerMaster CustomerLedgerMaster { get; set; }
        public CustomerWalletPayment CustomerWalletPayment { get; set; }
        public IList<CustomerLedgerDetail> CustomerLedgerDetail { get; set; }
    }
}