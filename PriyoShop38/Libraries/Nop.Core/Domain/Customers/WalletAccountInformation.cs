using System;
using System.ComponentModel.DataAnnotations;

namespace Nop.Core.Domain.Customers
{
    public class WalletAccountInformation : BaseEntity
    {
        public long SystemID { get; set; }
        public long ContactNo { get; set; }
        public int CustomerID { get; set; }
        public int CustomerType { get; set; }
        public bool IsActive { get; set; }
        public DateTime EntryDate { get; set; }
    }
}
