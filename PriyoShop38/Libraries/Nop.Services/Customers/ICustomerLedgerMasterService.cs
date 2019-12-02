using System.Collections.Generic;

using Nop.Core.Domain.Customers;

namespace Nop.Services.Customers
{
    /// <summary>
    /// CustomerLedgerMaster service interface
    /// </summary>
    public interface ICustomerLedgerMasterService
    {
        #region CustomerLedgerMasters
        /// <summary>
        /// Delete a CustomerLedgerMaster
        /// </summary>
        /// <param name="CustomerLedgerMaster">CustomerLedgerMaster</param>
        void DeleteCustomerLedgerMaster(CustomerLedgerMaster CustomerLedgerMaster);

        /// <summary>
        /// Gets a CustomerLedgerMaster
        /// </summary>
        /// <param name="SystemID">CustomerLedgerMaster identifier</param>
        /// <returns>A CustomerLedgerMaster</returns>
        CustomerLedgerMaster GetCustomerLedgerMasterById(long SystemID);

        /// <summary>
        /// Gets a CustomerLedgerMaster
        /// </summary>
        /// <param name="ContactNo">CustomerLedgerMaster identifier</param>
        /// <returns>A CustomerLedgerMaster</returns>
        CustomerLedgerMaster GetCustomerLedgerMasterByContactNo(long ContactNo);

        /// <summary>
        /// Get CustomerLedgerMasters by identifiers
        /// </summary>
        /// <param name="SystemIDs">CustomerLedgerMaster identifiers</param>
        /// <returns>CustomerLedgerMasters</returns>
        IList<CustomerLedgerMaster> GetCustomerLedgerMastersByIds(long[] SystemIDs);

        /// <summary>
        /// Insert a CustomerLedgerMaster
        /// </summary>
        /// <param name="CustomerLedgerMaster">CustomerLedgerMaster</param>
        void InsertCustomerLedgerMaster(CustomerLedgerMaster CustomerLedgerMaster);

        /// <summary>
        /// Updates the CustomerLedgerMaster
        /// </summary>
        /// <param name="CustomerLedgerMaster">CustomerLedgerMaster</param>
        void UpdateCustomerLedgerMaster(CustomerLedgerMaster CustomerLedgerMaster);



        string InsertWalletAccountInformation(WalletAccountInformationTemp walletAccount);
        string CheckWalletOTP(WalletAccountInformationTemp walletAccount, string CustomerFullName);
        WalletAccountInformationTemp GetCustomerWalletAccount(int CustomerID, int OTP);
        WalletAccountInformation GetCustomerWalletAccountByContactNo(long ContactNo);
        WalletAccountInformation GetCustomerWalletAccountByCustomerId(int CustomerId);

        #endregion
    }
}