using System;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents a CustomerLedgerDetail
    /// </summary>
    public class CustomerLedgerDetail : BaseEntity
    {
        #region Properties
        public long SystemID { get; set; }
        public long ContactNo { get; set; }
        public long LedgerMasterID { get; set; }
        public string AmountDescription { get; set; }
        public decimal Amount { get; set; }
        public string AmountType { get; set; }
        public byte AmountSource { get; set; }
        public long LastAddedBy { get; set; }
        public DateTime LastAddedDate { get; set; }
        #endregion
    }
}
