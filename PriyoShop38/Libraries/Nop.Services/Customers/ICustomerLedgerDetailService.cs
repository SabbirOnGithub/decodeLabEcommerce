using System.Collections.Generic;

using Nop.Core.Domain.Customers;

namespace Nop.Services.Customers
{
    /// <summary>
    /// CustomerLedgerDetail service interface
    /// </summary>
    public interface ICustomerLedgerDetailService
    {
        #region CustomerLedgerDetails
        /// <summary>
        /// Delete a CustomerLedgerDetail
        /// </summary>
        /// <param name="CustomerLedgerDetail">CustomerLedgerDetail</param>
        void DeleteCustomerLedgerDetail(CustomerLedgerDetail CustomerLedgerDetail);

        /// <summary>
        /// Gets a CustomerLedgerDetail
        /// </summary>
        /// <param name="SystemID">CustomerLedgerDetail identifier</param>
        /// <returns>A CustomerLedgerDetail</returns>
        CustomerLedgerDetail GetCustomerLedgerDetailById(long SystemID);

        /// <summary>
        /// Gets a CustomerLedgerDetail
        /// </summary>
        /// <param name="ContactNo">CustomerLedgerDetail identifier</param>
        /// <returns>A CustomerLedgerDetail</returns>
        IList<CustomerLedgerDetail> GetCustomerLedgerDetailByContactNo(long ContactNo);

        /// <summary>
        /// Get CustomerLedgerDetails by identifiers
        /// </summary>
        /// <param name="SystemIDs">CustomerLedgerDetail identifiers</param>
        /// <returns>CustomerLedgerDetails</returns>
        IList<CustomerLedgerDetail> GetCustomerLedgerDetailsByIds(long[] SystemIDs);

        /// <summary>
        /// Insert a CustomerLedgerDetail
        /// </summary>
        /// <param name="CustomerLedgerDetail">CustomerLedgerDetail</param>
        void InsertCustomerLedgerDetail(CustomerLedgerDetail CustomerLedgerDetail);

        /// <summary>
        /// Updates the CustomerLedgerDetail
        /// </summary>
        /// <param name="CustomerLedgerDetail">CustomerLedgerDetail</param>
        void UpdateCustomerLedgerDetail(CustomerLedgerDetail CustomerLedgerDetail);

        #endregion
    }
}