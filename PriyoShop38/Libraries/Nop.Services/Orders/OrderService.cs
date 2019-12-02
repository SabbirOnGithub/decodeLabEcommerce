using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Events;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Order service
    /// </summary>
    public partial class OrderService : IOrderService
    {
        #region Fields

        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<RecurringPayment> _recurringPaymentRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="orderRepository">Order repository</param>
        /// <param name="orderItemRepository">Order item repository</param>
        /// <param name="orderNoteRepository">Order note repository</param>
        /// <param name="productRepository">Product repository</param>
        /// <param name="recurringPaymentRepository">Recurring payment repository</param>
        /// <param name="customerRepository">Customer repository</param>
        /// <param name="eventPublisher">Event published</param>
        public OrderService(IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<OrderNote> orderNoteRepository,
            IRepository<Product> productRepository,
            IRepository<RecurringPayment> recurringPaymentRepository,
            IRepository<Customer> customerRepository, 
            IEventPublisher eventPublisher,
            IDataProvider dataProvider,
            IDbContext dbContext)
        {
            this._orderRepository = orderRepository;
            this._orderItemRepository = orderItemRepository;
            this._orderNoteRepository = orderNoteRepository;
            this._productRepository = productRepository;
            this._recurringPaymentRepository = recurringPaymentRepository;
            this._customerRepository = customerRepository;
            this._eventPublisher = eventPublisher;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
        }

        #endregion

        #region Methods

        #region Orders


        public string SaveMakePayment(Order order)
        {
            order.Id = 0;
          //  sql objParam = new object[] { order.Id, order.StoreId, order.CustomerId, order.PaymentStatus, order.PaymentStatusId, order.PaymentMethodSystemName, order.OrderTotal, order.MakeAmount };

            // var obj0 = _dbContext.ExecuteStoredProcedureList<MakePayment>("EXEC AddMakePayment",objParam);


            // var obj1 = _dbContext.ExecuteStoredProcedureList<MakePayment>(string.Format("EXEC AddMakePayment", objParam));
           // var obj2 = _dbContext.SqlQuery<MakePayment>(string.Format("EXEC AddMakePayment  ", objParam));
            var obj2 = _dbContext.ExecuteSqlCommand(string.Format("EXEC AddMakePayment  @StoreId = {0},@CustomerId = {1},@PaymentStatus ='{2}' ,@PaymentStatusId = {3},@PaymentMethodSystemName = '{4}',@MakeAmount = {5},@id = {5} ", order.StoreId, order.CustomerId, order.PaymentStatus, order.PaymentStatusId, order.PaymentMethodSystemName, order.OrderTotal, order.MakeAmount,order.Id));

            return obj2.ToString();
        }

        
        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        /// <returns>Order</returns>
        public virtual Order GetOrderById(int orderId)
        {
            if (orderId == 0)
                return null;

            return _orderRepository.GetById(orderId);
        }

        /// <summary>
        /// Get orders by identifiers
        /// </summary>
        /// <param name="orderIds">Order identifiers</param>
        /// <returns>Order</returns>
        public virtual IList<Order> GetOrdersByIds(int[] orderIds)
        {
            if (orderIds == null || orderIds.Length == 0)
                return new List<Order>();

            var query = from o in _orderRepository.Table
                        where orderIds.Contains(o.Id)
                        select o;
            var orders = query.ToList();
            //sort by passed identifiers
            var sortedOrders = new List<Order>();
            foreach (int id in orderIds)
            {
                var order = orders.Find(x => x.Id == id);
                if (order != null)
                    sortedOrders.Add(order);
            }
            return sortedOrders;
        }

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderGuid">The order identifier</param>
        /// <returns>Order</returns>
        public virtual Order GetOrderByGuid(Guid orderGuid)
        {
            if (orderGuid == Guid.Empty)
                return null;

            var query = from o in _orderRepository.Table
                        where o.OrderGuid == orderGuid
                        select o;
            var order = query.FirstOrDefault();
            return order;
        }

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="order">The order</param>
        public virtual void DeleteOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            order.Deleted = true;
            UpdateOrder(order);
        }

        /// <summary>
        /// Search orders
        /// </summary>
        /// <param name="storeId">Store identifier; 0 to load all orders</param>
        /// <param name="vendorId">Vendor identifier; null to load all orders</param>
        /// <param name="customerId">Customer identifier; 0 to load all orders</param>
        /// <param name="productId">Product identifier which was purchased in an order; 0 to load all orders</param>
        /// <param name="affiliateId">Affiliate identifier; 0 to load all orders</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all orders</param>
        /// <param name="warehouseId">Warehouse identifier, only orders with products from a specified warehouse will be loaded; 0 to load all orders</param>
        /// <param name="paymentMethodSystemName">Payment method system name; null to load all records</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="osIds">Order status identifiers; null to load all orders</param>
        /// <param name="psIds">Payment status identifiers; null to load all orders</param>
        /// <param name="ssIds">Shipping status identifiers; null to load all orders</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <param name="billingLastName">Billing last name. Leave empty to load all records.</param>
        /// <param name="orderNotes">Search in order notes. Leave empty to load all records.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Orders</returns>
        public virtual IPagedList<Order> SearchOrders(int storeId = 0,
            int vendorId = 0, int customerId = 0,
            int productId = 0, int affiliateId = 0, int warehouseId = 0,
            int billingCountryId = 0, string paymentMethodSystemName = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
            List<int> uatIds = null, string billingEmail = null, string billingLastName = "",
            string orderNotes = null, int pageIndex = 0, int pageSize = int.MaxValue - 1,
            bool? acps = null)
        {

            //prepare parameters
            var pStoreId = _dataProvider.GetParameter();
            pStoreId.ParameterName = "StoreId";
            pStoreId.Value = storeId;
            pStoreId.DbType = DbType.Int32;

            var pVendorId = _dataProvider.GetParameter();
            pVendorId.ParameterName = "VendorId";
            pVendorId.Value = vendorId;
            pVendorId.DbType = DbType.Int32;

            var pCustomerId = _dataProvider.GetParameter();
            pCustomerId.ParameterName = "CustomerId";
            pCustomerId.Value = customerId;
            pCustomerId.DbType = DbType.Int32;

            var pProductId = _dataProvider.GetParameter();
            pProductId.ParameterName = "ProductId";
            pProductId.Value = productId;
            pProductId.DbType = DbType.Int32;

            var pAffiliateId = _dataProvider.GetParameter();
            pAffiliateId.ParameterName = "AffiliateId";
            pAffiliateId.Value = affiliateId;
            pAffiliateId.DbType = DbType.Int32;

            var pWarehouseId = _dataProvider.GetParameter();
            pWarehouseId.ParameterName = "WarehouseId";
            pWarehouseId.Value = warehouseId;
            pWarehouseId.DbType = DbType.Int32;

            var pBillingCountryId = _dataProvider.GetParameter();
            pBillingCountryId.ParameterName = "BillingCountryId";
            pBillingCountryId.Value = billingCountryId;
            pBillingCountryId.DbType = DbType.Int32;

            var pPaymentMethodSystemName = _dataProvider.GetParameter();
            pPaymentMethodSystemName.ParameterName = "PaymentMethodSystemName";
            pPaymentMethodSystemName.Value = (object)paymentMethodSystemName ?? DBNull.Value;
            pPaymentMethodSystemName.DbType = DbType.String;

            var pCreatedFromUtc = _dataProvider.GetParameter();
            pCreatedFromUtc.ParameterName = "CreatedFromUtc";
            pCreatedFromUtc.Value = (object)createdFromUtc ?? DBNull.Value;
            pCreatedFromUtc.DbType = DbType.DateTime2;

            var pCreatedToUtc = _dataProvider.GetParameter();
            pCreatedToUtc.ParameterName = "CreatedToUtc";
            pCreatedToUtc.Value = (object)createdToUtc ?? DBNull.Value;
            pCreatedToUtc.DbType = DbType.DateTime2;

            var pOsIds = _dataProvider.GetParameter();
            pOsIds.ParameterName = "OsIds";
            pOsIds.Value = osIds != null && osIds.Any() ? (object)string.Join(",", osIds) : DBNull.Value;
            pOsIds.DbType = DbType.String;

            var pPsIds = _dataProvider.GetParameter();
            pPsIds.ParameterName = "PsIds";
            pPsIds.Value = psIds != null && psIds.Any() ? (object)string.Join(",", psIds) : DBNull.Value;
            pPsIds.DbType = DbType.String;

            var pSsIds = _dataProvider.GetParameter();
            pSsIds.ParameterName = "SsIds";
            pSsIds.Value = ssIds != null && ssIds.Any() ? (object)string.Join(",", ssIds) : DBNull.Value;
            pSsIds.DbType = DbType.String;

            var pUatIds = _dataProvider.GetParameter();
            pUatIds.ParameterName = "UatIds";
            pUatIds.Value = uatIds != null && uatIds.Any() ? (object)string.Join(",", uatIds) : DBNull.Value;
            pUatIds.DbType = DbType.String;

            var pBillingEmail = _dataProvider.GetParameter();
            pBillingEmail.ParameterName = "BillingEmail";
            pBillingEmail.Value = (object)billingEmail ?? DBNull.Value;
            pBillingEmail.DbType = DbType.String;

            var pBillingLastName = _dataProvider.GetParameter();
            pBillingLastName.ParameterName = "BillingLastName";
            pBillingLastName.Value = (object)billingLastName ?? DBNull.Value;
            pBillingLastName.DbType = DbType.String;

            var pOrderNotes = _dataProvider.GetParameter();
            pOrderNotes.ParameterName = "OrderNotes";
            pOrderNotes.Value = (object)orderNotes ?? DBNull.Value;
            pOrderNotes.DbType = DbType.String;

            var pAcps = _dataProvider.GetParameter();
            pAcps.ParameterName = "Acps";
            pAcps.Value = (object)acps ?? DBNull.Value;
            pAcps.DbType = DbType.Boolean;

            var pPageIndex = _dataProvider.GetParameter();
            pPageIndex.ParameterName = "PageIndex";
            pPageIndex.Value = pageIndex;
            pPageIndex.DbType = DbType.Int32;

            var pPageSize = _dataProvider.GetParameter();
            pPageSize.ParameterName = "PageSize";
            pPageSize.Value = pageSize;
            pPageSize.DbType = DbType.Int32;

            var pTotalRecords = _dataProvider.GetParameter();
            pTotalRecords.ParameterName = "TotalRecords";
            pTotalRecords.Direction = ParameterDirection.Output;
            pTotalRecords.DbType = DbType.Int32;

            var orders = _dbContext.ExecuteStoredProcedureList<Order>(
                    "OrderLoadAllPaged",
                    pStoreId,
                    pVendorId,
                    pCustomerId,
                    pProductId,
                    pAffiliateId,
                    pWarehouseId,
                    pBillingCountryId,
                    pPaymentMethodSystemName,
                    pCreatedFromUtc,
                    pCreatedToUtc,
                    pOsIds,
                    pPsIds,
                    pSsIds,
                    pUatIds,
                    pBillingEmail,
                    pBillingLastName,
                    pOrderNotes,
                    pAcps,
                    pPageIndex,
                    pPageSize,
                    pTotalRecords);

            //return products
            int totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;

            #region Old LINQ

            //var query = _orderRepository.Table;
            //if (storeId > 0)
            //    query = query.Where(o => o.StoreId == storeId);
            //if (vendorId > 0)
            //{
            //    query = query
            //        .Where(o => o.OrderItems
            //        .Any(orderItem => orderItem.Product.VendorId == vendorId));
            //}
            //if (customerId > 0)
            //    query = query.Where(o => o.CustomerId == customerId);
            //if (productId > 0)
            //{
            //    query = query
            //        .Where(o => o.OrderItems
            //        .Any(orderItem => orderItem.Product.Id == productId));
            //}
            //if (warehouseId > 0)
            //{
            //    var manageStockInventoryMethodId = (int)ManageInventoryMethod.ManageStock;
            //    query = query
            //        .Where(o => o.OrderItems
            //        .Any(orderItem =>
            //            //"Use multiple warehouses" enabled
            //            //we search in each warehouse
            //            (orderItem.Product.ManageInventoryMethodId == manageStockInventoryMethodId && 
            //            orderItem.Product.UseMultipleWarehouses &&
            //            orderItem.Product.ProductWarehouseInventory.Any(pwi => pwi.WarehouseId == warehouseId))
            //            ||
            //            //"Use multiple warehouses" disabled
            //            //we use standard "warehouse" property
            //            ((orderItem.Product.ManageInventoryMethodId != manageStockInventoryMethodId ||
            //            !orderItem.Product.UseMultipleWarehouses) &&
            //            orderItem.Product.WarehouseId == warehouseId))
            //            );
            //}
            //if (billingCountryId > 0)
            //    query = query.Where(o => o.BillingAddress != null && o.BillingAddress.CountryId == billingCountryId);
            //if (!String.IsNullOrEmpty(paymentMethodSystemName))
            //    query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);
            //if (affiliateId > 0)
            //    query = query.Where(o => o.AffiliateId == affiliateId);
            //if (createdFromUtc.HasValue)
            //    query = query.Where(o => createdFromUtc.Value <= o.CreatedOnUtc);
            //if (createdToUtc.HasValue)
            //    query = query.Where(o => createdToUtc.Value >= o.CreatedOnUtc);
            //if (osIds != null && osIds.Any())
            //    query = query.Where(o => osIds.Contains(o.OrderStatusId));
            //if (psIds != null && psIds.Any())
            //    query = query.Where(o => psIds.Contains(o.PaymentStatusId));
            //if (ssIds != null && ssIds.Any())
            //    query = query.Where(o => ssIds.Contains(o.ShippingStatusId));
            //if (uatIds != null && uatIds.Any())
            //    query = query.Where(o => uatIds.Contains(o.UserAgentTypeId));
            //if (!String.IsNullOrEmpty(billingEmail))
            //    query = query.Where(o => o.BillingAddress != null && !String.IsNullOrEmpty(o.BillingAddress.Email) && o.BillingAddress.Email.Contains(billingEmail));
            //if (!String.IsNullOrEmpty(billingLastName))
            //    query = query.Where(o => o.BillingAddress != null && !String.IsNullOrEmpty(o.BillingAddress.LastName) && o.BillingAddress.LastName.Contains(billingLastName));
            //if (!String.IsNullOrEmpty(orderNotes))
            //    query = query.Where(o => o.OrderNotes.Any(on => on.Note.Contains(orderNotes)));
            //query = query.Where(o => !o.Deleted);

            //if (acps.HasValue)
            //    query = query.Where(x => x.IsCommissionPaid == acps.Value);

            //query = query.OrderByDescending(o => o.CreatedOnUtc);

            #endregion

            //database layer paging
            return new PagedList<Order>(orders, pageIndex, pageSize, totalRecords);
        }

        /// <summary>
        /// Inserts an order
        /// </summary>
        /// <param name="order">Order</param>
        public virtual void InsertOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            _orderRepository.Insert(order);

            //event notification
            _eventPublisher.EntityInserted(order);
        }

        /// <summary>
        /// Updates the order
        /// </summary>
        /// <param name="order">The order</param>
        public virtual void UpdateOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            _orderRepository.Update(order);

            //event notification
            _eventPublisher.EntityUpdated(order);
        }

        /// <summary>
        /// Get an order by authorization transaction ID and payment method system name
        /// </summary>
        /// <param name="authorizationTransactionId">Authorization transaction ID</param>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>Order</returns>
        public virtual Order GetOrderByAuthorizationTransactionIdAndPaymentMethod(string authorizationTransactionId, 
            string paymentMethodSystemName)
        { 
            var query = _orderRepository.Table;
            if (!String.IsNullOrWhiteSpace(authorizationTransactionId))
                query = query.Where(o => o.AuthorizationTransactionId == authorizationTransactionId);
            
            if (!String.IsNullOrWhiteSpace(paymentMethodSystemName))
                query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);
            
            query = query.OrderByDescending(o => o.CreatedOnUtc);
            var order = query.FirstOrDefault();
            return order;
        }
        
        #endregion
        
        #region Orders items

        /// <summary>
        /// Gets an order item
        /// </summary>
        /// <param name="orderItemId">Order item identifier</param>
        /// <returns>Order item</returns>
        public virtual OrderItem GetOrderItemById(int orderItemId)
        {
            if (orderItemId == 0)
                return null;

            return _orderItemRepository.GetById(orderItemId);
        }

        /// <summary>
        /// Gets an item
        /// </summary>
        /// <param name="orderItemGuid">Order identifier</param>
        /// <returns>Order item</returns>
        public virtual OrderItem GetOrderItemByGuid(Guid orderItemGuid)
        {
            if (orderItemGuid == Guid.Empty)
                return null;

            var query = from orderItem in _orderItemRepository.Table
                        where orderItem.OrderItemGuid == orderItemGuid
                        select orderItem;
            var item = query.FirstOrDefault();
            return item;
        }
        
        /// <summary>
        /// Gets all downloadable order items
        /// </summary>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <returns>Order items</returns>
        public virtual IList<OrderItem> GetDownloadableOrderItems(int customerId)
        {
            if (customerId == 0)
                throw new ArgumentOutOfRangeException("customerId");

            var query = from orderItem in _orderItemRepository.Table
                        join o in _orderRepository.Table on orderItem.OrderId equals o.Id
                        join p in _productRepository.Table on orderItem.ProductId equals p.Id
                        where customerId == o.CustomerId &&
                        p.IsDownload &&
                        !o.Deleted
                        orderby o.CreatedOnUtc descending, orderItem.Id
                        select orderItem;

            var orderItems = query.ToList();
            return orderItems;
        }

        /// <summary>
        /// Delete an order item
        /// </summary>
        /// <param name="orderItem">The order item</param>
        public virtual void DeleteOrderItem(OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException("orderItem");

            _orderItemRepository.Delete(orderItem);

            //event notification
            _eventPublisher.EntityDeleted(orderItem);
        }

        #endregion

        #region Orders notes

        /// <summary>
        /// Gets an order note
        /// </summary>
        /// <param name="orderNoteId">The order note identifier</param>
        /// <returns>Order note</returns>
        public virtual OrderNote GetOrderNoteById(int orderNoteId)
        {
            if (orderNoteId == 0)
                return null;

            return _orderNoteRepository.GetById(orderNoteId);
        }

        /// <summary>
        /// Deletes an order note
        /// </summary>
        /// <param name="orderNote">The order note</param>
        public virtual void DeleteOrderNote(OrderNote orderNote)
        {
            if (orderNote == null)
                throw new ArgumentNullException("orderNote");

            _orderNoteRepository.Delete(orderNote);

            //event notification
            _eventPublisher.EntityDeleted(orderNote);
        }

        #endregion

        #region Recurring payments

        /// <summary>
        /// Deletes a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public virtual void DeleteRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");

            recurringPayment.Deleted = true;
            UpdateRecurringPayment(recurringPayment);
        }

        /// <summary>
        /// Gets a recurring payment
        /// </summary>
        /// <param name="recurringPaymentId">The recurring payment identifier</param>
        /// <returns>Recurring payment</returns>
        public virtual RecurringPayment GetRecurringPaymentById(int recurringPaymentId)
        {
            if (recurringPaymentId == 0)
                return null;

           return _recurringPaymentRepository.GetById(recurringPaymentId);
        }

        /// <summary>
        /// Inserts a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public virtual void InsertRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");

            _recurringPaymentRepository.Insert(recurringPayment);

            //event notification
            _eventPublisher.EntityInserted(recurringPayment);
        }

        /// <summary>
        /// Updates the recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public virtual void UpdateRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");

            _recurringPaymentRepository.Update(recurringPayment);

            //event notification
            _eventPublisher.EntityUpdated(recurringPayment);
        }

        /// <summary>
        /// Search recurring payments
        /// </summary>
        /// <param name="storeId">The store identifier; 0 to load all records</param>
        /// <param name="customerId">The customer identifier; 0 to load all records</param>
        /// <param name="initialOrderId">The initial order identifier; 0 to load all records</param>
        /// <param name="initialOrderStatus">Initial order status identifier; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Recurring payments</returns>
        public virtual IPagedList<RecurringPayment> SearchRecurringPayments(int storeId = 0,
            int customerId = 0, int initialOrderId = 0, OrderStatus? initialOrderStatus = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            int? initialOrderStatusId = null;
            if (initialOrderStatus.HasValue)
                initialOrderStatusId = (int)initialOrderStatus.Value;

            var query1 = from rp in _recurringPaymentRepository.Table
                         join c in _customerRepository.Table on rp.InitialOrder.CustomerId equals c.Id
                         where
                         (!rp.Deleted) &&
                         (showHidden || !rp.InitialOrder.Deleted) &&
                         (showHidden || !c.Deleted) &&
                         (showHidden || rp.IsActive) &&
                         (customerId == 0 || rp.InitialOrder.CustomerId == customerId) &&
                         (storeId == 0 || rp.InitialOrder.StoreId == storeId) &&
                         (initialOrderId == 0 || rp.InitialOrder.Id == initialOrderId) &&
                         (!initialOrderStatusId.HasValue || initialOrderStatusId.Value == 0 || rp.InitialOrder.OrderStatusId == initialOrderStatusId.Value)
                         select rp.Id;

            var query2 = from rp in _recurringPaymentRepository.Table
                         where query1.Contains(rp.Id)
                         orderby rp.StartDateUtc, rp.Id
                         select rp;

            var recurringPayments = new PagedList<RecurringPayment>(query2, pageIndex, pageSize);
            return recurringPayments;
        }

        #endregion

        #region BS-23

        public AffiliatedOrderSummary GetAffiliatedOrdersSummary(int affiliateId = 0,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null, List<int> osIds = null, 
            List<int> psIds = null, int pageIndex = 0, int pageSize = int.MaxValue -1)
        {
            var pAffiliateId = _dataProvider.GetParameter();
            pAffiliateId.ParameterName = "AffiliateId";
            pAffiliateId.Value = affiliateId;
            pAffiliateId.DbType = DbType.Int32;

            var pCreatedFromUtc = _dataProvider.GetParameter();
            pCreatedFromUtc.ParameterName = "CreatedFromUtc";
            pCreatedFromUtc.Value = createdFromUtc.HasValue ? (object)createdFromUtc : DBNull.Value;
            pCreatedFromUtc.DbType = DbType.DateTime;

            var pCreatedToUtc = _dataProvider.GetParameter();
            pCreatedToUtc.ParameterName = "CreatedToUtc";
            pCreatedToUtc.Value = createdToUtc.HasValue ? (object)createdToUtc : DBNull.Value;
            pCreatedToUtc.DbType = DbType.DateTime;

            var pOrderStatusIds = _dataProvider.GetParameter();
            pOrderStatusIds.ParameterName = "OrderStatusIds";
            pOrderStatusIds.Value = osIds != null && osIds.Any() ? (object)string.Join(",", osIds) : DBNull.Value;
            pOrderStatusIds.DbType = DbType.String;

            var pPaymentStatusIds = _dataProvider.GetParameter();
            pPaymentStatusIds.ParameterName = "PaymentStatusIds";
            pPaymentStatusIds.Value = psIds != null && psIds.Any() ? (object)string.Join(",", psIds) : DBNull.Value;
            pPaymentStatusIds.DbType = DbType.String;

            var pPageIndex = _dataProvider.GetParameter();
            pPageIndex.ParameterName = "PageIndex";
            pPageIndex.Value = pageIndex;
            pPageIndex.DbType = DbType.Int32;

            var pPageSize = _dataProvider.GetParameter();
            pPageSize.ParameterName = "PageSize";
            pPageSize.Value = pageSize;
            pPageSize.DbType = DbType.Int32;

            var pTotalRecords = _dataProvider.GetParameter();
            pTotalRecords.ParameterName = "TotalRecords";
            pTotalRecords.Direction = ParameterDirection.Output;
            pTotalRecords.DbType = DbType.Int32;

            var pTotalCommission = _dataProvider.GetParameter();
            pTotalCommission.ParameterName = "TotalCommission";
            pTotalCommission.Direction = ParameterDirection.Output;
            pTotalCommission.DbType = DbType.Decimal;

            var pPayableCommission = _dataProvider.GetParameter();
            pPayableCommission.ParameterName = "PayableCommission";
            pPayableCommission.Direction = ParameterDirection.Output;
            pPayableCommission.DbType = DbType.Decimal;

            var pPaidCommission = _dataProvider.GetParameter();
            pPaidCommission.ParameterName = "PaidCommission";
            pPaidCommission.Direction = ParameterDirection.Output;
            pPaidCommission.DbType = DbType.Decimal;

            var pUnpaidCommission = _dataProvider.GetParameter();
            pUnpaidCommission.ParameterName = "UnpaidCommission";
            pUnpaidCommission.Direction = ParameterDirection.Output;
            pUnpaidCommission.DbType = DbType.Decimal;

            var result = _dbContext.ExecuteStoredProcedureList<Order>("AffiliateOrderSummary",
                pAffiliateId,
                pCreatedFromUtc,
                pCreatedToUtc,
                pOrderStatusIds,
                pPaymentStatusIds,
                pPageIndex,
                pPageSize,
                pTotalRecords,
                pTotalCommission,
                pPayableCommission,
                pPaidCommission,
                pUnpaidCommission);

            var summary = new AffiliatedOrderSummary();
            var totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            summary.TotalCommission = (pTotalCommission.Value != DBNull.Value) ? Convert.ToDecimal(pTotalCommission.Value) : 0;
            summary.PayableCommission = (pPayableCommission.Value != DBNull.Value) ? Convert.ToDecimal(pPayableCommission.Value) : 0;
            summary.PaidCommission = (pPaidCommission.Value != DBNull.Value) ? Convert.ToDecimal(pPaidCommission.Value) : 0;
            summary.UnpaidCommission = (pUnpaidCommission.Value != DBNull.Value) ? Convert.ToDecimal(pUnpaidCommission.Value) : 0;

            summary.Orders = new PagedList<Order>(result, pageIndex, pageSize, totalRecords);
            return summary;
        }

        #endregion

        #endregion
    }
}
