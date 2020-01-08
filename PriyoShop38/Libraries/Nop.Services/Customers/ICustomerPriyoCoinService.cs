using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Customer service interface
    /// </summary>
    public partial interface ICustomerPriyoCoinService
    {
        #region Customers

        /// <summary>
        /// Gets all customerPriyoCoin
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Customers</returns>
        IPagedList<CustomerPriyoCoin> GetAllCustomerPriyoCoins(DateTime? createdFromUtc = null,
            DateTime? createdToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue - 1);



        /// <summary>
        /// Delete a customer
        /// </summary>
        /// <param name="customerPriyoCoin">Customer Priyo Coin</param>
        void DeleteCustomerPriyoCoin(CustomerPriyoCoin customerPriyoCoin);

        /// <summary>
        /// Gets a customer
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>A customer</returns>
        IQueryable<CustomerPriyoCoin> GetCustomerPriyoCoinByCustomerId(int customerId);

        /// <summary>
        /// Get customers by identifiers
        /// </summary>
        /// <param name="customerIds">Customer identifiers</param>
        /// <returns>Customers</returns>
        IList<CustomerPriyoCoin> GetCustomerPriyoCoinByCustomerIds(int[] customerIds);


        /// <summary>
        /// Insert a customer Priyo coin
        /// </summary>
        /// <param name="customerPriyoCoin">Customer</param>
        void InsertCustomerPriyoCoin(CustomerPriyoCoin customerPriyoCoin);

        /// <summary>
        /// Updates the customer
        /// </summary>
        /// <param name="customerPriyoCoin">Customer</param>
        void UpdateCustomerPriyoCoin(CustomerPriyoCoin customerPriyoCoin);

        #endregion

    }
}