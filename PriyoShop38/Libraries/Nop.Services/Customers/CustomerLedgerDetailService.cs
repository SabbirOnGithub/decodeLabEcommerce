using System;
using System.Linq;
using System.Collections.Generic;

using Nop.Core.Data;
using Nop.Core.Domain.Customers;

using Nop.Services.Events;

namespace Nop.Services.Customers
{
    /// <summary>
    /// CustomerLedgerDetail service
    /// </summary>
    public class CustomerLedgerDetailService : ICustomerLedgerDetailService
    {
        #region Fields

        private readonly IRepository<CustomerLedgerDetail> _CustomerLedgerDetailRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public CustomerLedgerDetailService(IRepository<CustomerLedgerDetail> CustomerLedgerDetailRepository, IEventPublisher eventPublisher)
        {
            _CustomerLedgerDetailRepository = CustomerLedgerDetailRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        #region CustomerLedgerDetails

        /// <summary>
        /// Delete a CustomerLedgerDetail
        /// </summary>
        /// <param name="CustomerLedgerDetail">CustomerLedgerDetail</param>
        public virtual void DeleteCustomerLedgerDetail(CustomerLedgerDetail CustomerLedgerDetail)
        {
            if (CustomerLedgerDetail == null)
                throw new ArgumentNullException(nameof(CustomerLedgerDetail));

            _CustomerLedgerDetailRepository.Delete(CustomerLedgerDetail);
        }

        /// <summary>
        /// Gets a customer
        /// </summary>
        /// <param name="SystemID">CustomerLedgerDetail identifier</param>
        /// <returns>A CustomerLedgerDetail</returns>
        public virtual CustomerLedgerDetail GetCustomerLedgerDetailById(long SystemID)
        {
            if (SystemID == 0)
                return null;

            return _CustomerLedgerDetailRepository.GetById(SystemID);
        }

        public IList<CustomerLedgerDetail> GetCustomerLedgerDetailByContactNo(long ContactNo)
        {

            var query = from c in _CustomerLedgerDetailRepository.Table
                        where ContactNo.Equals(c.ContactNo)
                        select c;

            return query.ToList();
        }

        /// <summary>
        /// Get CustomerLedgerDetails by identifiers
        /// </summary>
        /// <param name="SystemIDs">CustomerLedgerDetail identifiers</param>
        /// <returns>CustomerLedgerDetails</returns>
        public virtual IList<CustomerLedgerDetail> GetCustomerLedgerDetailsByIds(long[] SystemIDs)
        {
            if (SystemIDs == null || SystemIDs.Length == 0)
                return new List<CustomerLedgerDetail>();

            var query = from c in _CustomerLedgerDetailRepository.Table
                        where SystemIDs.Contains(c.SystemID)
                        select c;

            var CustomerLedgerDetails = query.ToList();

            //sort by passed identifiers
            var sortedCustomerLedgerDetails = new List<CustomerLedgerDetail>();

            foreach (long SystemID in SystemIDs)
            {
                var CustomerLedgerDetail = CustomerLedgerDetails.Find(x => x.SystemID == SystemID);
                if (CustomerLedgerDetail != null)
                {
                    sortedCustomerLedgerDetails.Add(CustomerLedgerDetail);
                }
            }

            return sortedCustomerLedgerDetails;
        }

        /// <summary>
        /// Insert a CustomerLedgerDetail
        /// </summary>
        /// <param name="CustomerLedgerDetail">CustomerLedgerDetail</param>
        public virtual void InsertCustomerLedgerDetail(CustomerLedgerDetail CustomerLedgerDetail)
        {
            if (CustomerLedgerDetail == null)
                throw new ArgumentNullException(nameof(CustomerLedgerDetail));

            _CustomerLedgerDetailRepository.Insert(CustomerLedgerDetail);

            //event notification
            _eventPublisher.EntityInserted(CustomerLedgerDetail);
        }

        /// <summary>
        /// Updates the CustomerLedgerDetail
        /// </summary>
        /// <param name="CustomerLedgerDetail">CustomerLedgerDetail</param>
        public virtual void UpdateCustomerLedgerDetail(CustomerLedgerDetail CustomerLedgerDetail)
        {
            if (CustomerLedgerDetail == null)
                throw new ArgumentNullException(nameof(CustomerLedgerDetail));

            _CustomerLedgerDetailRepository.Update(CustomerLedgerDetail);

            //event notification
            _eventPublisher.EntityUpdated(CustomerLedgerDetail);
        }

        #endregion

        #endregion
    }
}