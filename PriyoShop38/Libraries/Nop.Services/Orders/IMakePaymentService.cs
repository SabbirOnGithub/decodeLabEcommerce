using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Orders;

namespace Nop.Services.Orders
{
    /// <summary>
    /// MakePayment service interface
    /// </summary>
    public partial interface IMakePaymentService
    {
        #region Orders

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        /// <returns>MakePayment</returns>
        MakePayment GetOrderById(int orderId);

        /// <summary>
        /// Get orders by identifiers
        /// </summary>
        /// <param name="orderIds">MakePayment identifiers</param>
        /// <returns>MakePayment</returns>
        IList<MakePayment> GetOrdersByIds(int[] orderIds);

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderGuid">The order identifier</param>
        /// <returns>MakePayment</returns>
        MakePayment GetOrderByGuid(Guid orderGuid);

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="order">The order</param>
        void DeleteOrder(MakePayment order);

        

        /// <summary>
        /// Inserts an order
        /// </summary>
        /// <param name="order">MakePayment</param>
        void InsertOrder(MakePayment order);

        /// <summary>
        /// Updates the order
        /// </summary>
        /// <param name="order">The order</param>
        void UpdateOrder(MakePayment order);

        /// <summary>
        /// Get an order by authorization transaction ID and payment method system name
        /// </summary>
        /// <param name="authorizationTransactionId">Authorization transaction ID</param>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>MakePayment</returns>
        MakePayment GetOrderByAuthorizationTransactionIdAndPaymentMethod(string authorizationTransactionId, string paymentMethodSystemName);

        #endregion

        #region Orders items

        /// <summary>
        /// Gets an order item
        /// </summary>
        /// <param name="orderItemId">MakePayment item identifier</param>
        /// <returns>MakePayment item</returns>
        OrderItem GetOrderItemById(int orderItemId);

        /// <summary>
        /// Gets an order item
        /// </summary>
        /// <param name="orderItemGuid">MakePayment item identifier</param>
        /// <returns>MakePayment item</returns>
        OrderItem GetOrderItemByGuid(Guid orderItemGuid);

      
       

        /// <summary>
        /// Delete an order item
        /// </summary>
        /// <param name="orderItem">The order item</param>
        void DeleteOrderItem(OrderItem orderItem);

        #endregion

        #region MakePayment notes

        /// <summary>
        /// Gets an order note
        /// </summary>
        /// <param name="orderNoteId">The order note identifier</param>
        /// <returns>MakePayment note</returns>
        OrderNote GetOrderNoteById(int orderNoteId);

        /// <summary>
        /// Deletes an order note
        /// </summary>
        /// <param name="orderNote">The order note</param>
        void DeleteOrderNote(OrderNote orderNote);

        #endregion

     


    }
}
