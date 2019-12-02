using System;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents a CustomerLedgerMaster
    /// </summary>
    public class CustomerLedgerMaster : BaseEntity
    {
        #region Properties
        public long SystemID { get; set; }
        public long ContactNo { get; set; }
        public string CustomerName { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal TotalBalance { get; set; }

        public DateTime LastUpdated { get; set; }
        #endregion
    }
}
