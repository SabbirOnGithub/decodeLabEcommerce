using System;
using System.Linq;
using System.Collections.Generic;

using Nop.Core.Data;
using Nop.Core.Domain.Customers;

using Nop.Services.Events;
using Nop.Data;

namespace Nop.Services.Customers
{
    /// <summary>
    /// CustomerLedgerMaster service
    /// </summary>
    public class CustomerLedgerMasterService : ICustomerLedgerMasterService
    {
        #region Fields

        private readonly IRepository<CustomerLedgerMaster> _CustomerLedgerMasterRepository;
        private readonly IRepository<WalletAccountInformationTemp> _CustomerWalletAccountRepositoryTemp;
        private readonly IRepository<WalletAccountInformation> _CustomerWalletAccountRepository;
        private readonly IDbContext _DbContext;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public CustomerLedgerMasterService(
            IRepository<CustomerLedgerMaster> CustomerLedgerMasterRepository,
            IRepository<WalletAccountInformationTemp> CustomerWalletAccountRepositoryTemp,
            IRepository<WalletAccountInformation> CustomerWalletAccountRepository,
            IEventPublisher eventPublisher,
            IDbContext DbContext)
        {
            _CustomerLedgerMasterRepository = CustomerLedgerMasterRepository;
            _CustomerWalletAccountRepositoryTemp = CustomerWalletAccountRepositoryTemp;
            _CustomerWalletAccountRepository = CustomerWalletAccountRepository;
            _eventPublisher = eventPublisher;
            _DbContext = DbContext;
        }

        #endregion

        #region Methods

        #region CustomerLedgerMasters

        /// <summary>
        /// Delete a CustomerLedgerMaster
        /// </summary>
        /// <param name="CustomerLedgerMaster">CustomerLedgerMaster</param>
        public virtual void DeleteCustomerLedgerMaster(CustomerLedgerMaster CustomerLedgerMaster)
        {
            if (CustomerLedgerMaster == null)
                throw new ArgumentNullException(nameof(CustomerLedgerMaster));

            _CustomerLedgerMasterRepository.Delete(CustomerLedgerMaster);
        }

        /// <summary>
        /// Gets a customer
        /// </summary>
        /// <param name="SystemID">CustomerLedgerMaster identifier</param>
        /// <returns>A CustomerLedgerMaster</returns>
        public virtual CustomerLedgerMaster GetCustomerLedgerMasterById(long SystemID)
        {
            if (SystemID == 0)
                return null;

            return _CustomerLedgerMasterRepository.GetById(SystemID);
        }

        public CustomerLedgerMaster GetCustomerLedgerMasterByContactNo(long ContactNo)
        {

            var query = from c in _CustomerLedgerMasterRepository.Table
                        where ContactNo.Equals(c.ContactNo)
                        select c;

            return query.ToList().FirstOrDefault();
        }

        /// <summary>
        /// Get CustomerLedgerMasters by identifiers
        /// </summary>
        /// <param name="SystemIDs">CustomerLedgerMaster identifiers</param>
        /// <returns>CustomerLedgerMasters</returns>
        public virtual IList<CustomerLedgerMaster> GetCustomerLedgerMastersByIds(long[] SystemIDs)
        {
            if (SystemIDs == null || SystemIDs.Length == 0)
                return new List<CustomerLedgerMaster>();

            var query = from c in _CustomerLedgerMasterRepository.Table
                        where SystemIDs.Contains(c.SystemID)
                        select c;

            var CustomerLedgerMasters = query.ToList();

            //sort by passed identifiers
            var sortedCustomerLedgerMasters = new List<CustomerLedgerMaster>();

            foreach (long SystemID in SystemIDs)
            {
                var CustomerLedgerMaster = CustomerLedgerMasters.Find(x => x.SystemID == SystemID);
                if (CustomerLedgerMaster != null)
                {
                    sortedCustomerLedgerMasters.Add(CustomerLedgerMaster);
                }
            }

            return sortedCustomerLedgerMasters;
        }

        /// <summary>
        /// Insert a CustomerLedgerMaster
        /// </summary>
        /// <param name="CustomerLedgerMaster">CustomerLedgerMaster</param>
        public virtual void InsertCustomerLedgerMaster(CustomerLedgerMaster CustomerLedgerMaster)
        {
            if (CustomerLedgerMaster == null)
                throw new ArgumentNullException(nameof(CustomerLedgerMaster));

            _CustomerLedgerMasterRepository.Insert(CustomerLedgerMaster);

            //event notification
            _eventPublisher.EntityInserted(CustomerLedgerMaster);
        }

        /// <summary>
        /// Updates the CustomerLedgerMaster
        /// </summary>
        /// <param name="CustomerLedgerMaster">CustomerLedgerMaster</param>
        public virtual void UpdateCustomerLedgerMaster(CustomerLedgerMaster CustomerLedgerMaster)
        {
            if (CustomerLedgerMaster == null)
                throw new ArgumentNullException(nameof(CustomerLedgerMaster));

            _CustomerLedgerMasterRepository.Update(CustomerLedgerMaster);

            //event notification
            _eventPublisher.EntityUpdated(CustomerLedgerMaster);
        }

        public string InsertWalletAccountInformation(WalletAccountInformationTemp walletAccount)
        {
            if (walletAccount == null)
                throw new ArgumentNullException(nameof(walletAccount));

            var obj = _DbContext.ExecuteSqlCommand(string.Format("EXEC [Customer].[spSaveTempWalletAccountInformation] @ContactNo = {0}, @CustomerID = {1}, @MSG = ''", walletAccount.ContactNo, walletAccount.CustomerID));

            return obj.ToString();
        }

        public string CheckWalletOTP(WalletAccountInformationTemp walletAccount, string CustomerFullName)
        {
            if (walletAccount == null)
                throw new ArgumentNullException(nameof(walletAccount));

            var obj = _DbContext.ExecuteSqlCommand(string.Format("EXEC [Customer].[spCheckWalletOTP] @OTP = {0}, @CustomerID = {1}, @CustomerFullName = '{2}', @MSG = ''", walletAccount.OTP, walletAccount.CustomerID, CustomerFullName));

            return obj.ToString();
        }

        public WalletAccountInformationTemp GetCustomerWalletAccount(int CustomerID, int OTP)
        {
            var query = from c in _CustomerWalletAccountRepositoryTemp.Table
                        where c.CustomerID == CustomerID && c.OTP == OTP
                        orderby c.OTPEntryDate descending
                        select c;

            return query.ToList().FirstOrDefault();
        }

        public WalletAccountInformation GetCustomerWalletAccountByContactNo(long ContactNo)
        {
            var query = from c in _CustomerWalletAccountRepository.Table
                        where ContactNo.Equals(c.ContactNo)
                        select c;

            return query.ToList().FirstOrDefault();
        }

        public WalletAccountInformation GetCustomerWalletAccountByCustomerId(int CustomerId)
        {
            var query = from c in _CustomerWalletAccountRepository.Table
                        where c.CustomerID == CustomerId
                        select c;

            return query.ToList().FirstOrDefault();
        }

        #endregion

        #endregion
    }
}