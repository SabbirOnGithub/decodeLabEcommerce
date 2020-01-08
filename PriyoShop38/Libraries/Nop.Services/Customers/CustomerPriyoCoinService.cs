using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Polls;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Events;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Customer service
    /// </summary>
    public partial class CustomerPriyoCoinService : ICustomerPriyoCoinService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string CUSTOMERROLES_ALL_KEY = "Nop.customerrole.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : system name
        /// </remarks>
        private const string CUSTOMERROLES_BY_SYSTEMNAME_KEY = "Nop.customerrole.systemname-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CUSTOMERROLES_PATTERN_KEY = "Nop.customerrole.";

        #endregion

        #region Fields

        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerPriyoCoin> _customerPriyoCoinRepository;
        private readonly IRepository<CustomerRole> _customerRoleRepository;
        private readonly IRepository<GenericAttribute> _gaRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<ForumPost> _forumPostRepository;
        private readonly IRepository<ForumTopic> _forumTopicRepository;
        private readonly IRepository<BlogComment> _blogCommentRepository;
        private readonly IRepository<NewsComment> _newsCommentRepository;
        private readonly IRepository<PollVotingRecord> _pollVotingRecordRepository;
        private readonly IRepository<ProductReview> _productReviewRepository;
        private readonly IRepository<ProductReviewHelpfulness> _productReviewHelpfulnessRepository;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly CustomerSettings _customerSettings;
        private readonly CommonSettings _commonSettings;

        #endregion

        #region Ctor

        public CustomerPriyoCoinService(ICacheManager cacheManager,
            IRepository<CustomerPriyoCoin> customerPriyoCoinRepository,
            IRepository<Customer> customerRepository,
            IRepository<CustomerRole> customerRoleRepository,
            IRepository<GenericAttribute> gaRepository,
            IRepository<Order> orderRepository,
            IRepository<ForumPost> forumPostRepository,
            IRepository<ForumTopic> forumTopicRepository,
            IRepository<BlogComment> blogCommentRepository,
            IRepository<NewsComment> newsCommentRepository,
            IRepository<PollVotingRecord> pollVotingRecordRepository,
            IRepository<ProductReview> productReviewRepository,
            IRepository<ProductReviewHelpfulness> productReviewHelpfulnessRepository,
            IGenericAttributeService genericAttributeService,
            IDataProvider dataProvider,
            IDbContext dbContext,
            IEventPublisher eventPublisher, 
            CustomerSettings customerSettings,
            CommonSettings commonSettings)
        {
            this._cacheManager = cacheManager;
            this._customerPriyoCoinRepository = customerPriyoCoinRepository;
            this._customerRepository = customerRepository;
            this._customerRoleRepository = customerRoleRepository;
            this._gaRepository = gaRepository;
            this._orderRepository = orderRepository;
            this._forumPostRepository = forumPostRepository;
            this._forumTopicRepository = forumTopicRepository;
            this._blogCommentRepository = blogCommentRepository;
            this._newsCommentRepository = newsCommentRepository;
            this._pollVotingRecordRepository = pollVotingRecordRepository;
            this._productReviewRepository = productReviewRepository;
            this._productReviewHelpfulnessRepository = productReviewHelpfulnessRepository;
            this._genericAttributeService = genericAttributeService;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
            this._eventPublisher = eventPublisher;
            this._customerSettings = customerSettings;
            this._commonSettings = commonSettings;
        }

        #endregion

        #region Methods

        public IPagedList<CustomerPriyoCoin> GetAllCustomerPriyoCoins(DateTime? createdFromUtc = null, DateTime? createdToUtc = null, int pageIndex = 0,
            int pageSize = int.MaxValue - 1)
        {
            if (pageSize == int.MaxValue)
                pageSize = int.MaxValue - 1;

            var query = _customerPriyoCoinRepository.Table;
            return new PagedList<CustomerPriyoCoin>(query, pageIndex, pageSize, query.Count());
        }

        public void DeleteCustomerPriyoCoin(CustomerPriyoCoin customerPriyoCoin)
        {
            if (customerPriyoCoin == null)
                throw new ArgumentNullException(nameof(customerPriyoCoin));

            _customerPriyoCoinRepository.Delete(customerPriyoCoin);
            _eventPublisher.EntityDeleted(customerPriyoCoin);
        }

        public IQueryable<CustomerPriyoCoin> GetCustomerPriyoCoinByCustomerId(int customerId)
        {
            if (customerId == 0)
                return null;
            return _customerPriyoCoinRepository.Table.Include(y=>y.Customer).Where(x => x.CustomerId == customerId);
        }

        public IList<CustomerPriyoCoin> GetCustomerPriyoCoinByCustomerIds(int[] customerIds)
        {
            if (customerIds == null || customerIds.Length == 0)
                return new List<CustomerPriyoCoin>();

            var query = from c in _customerPriyoCoinRepository.Table
                where customerIds.Contains(c.CustomerId)
                select c;
            var customers = query.ToList();
            //sort by passed identifiers
            var sortedCustomers = new List<CustomerPriyoCoin>();
            foreach (int id in customerIds)
            {
                var customer = customers.Find(x => x.CustomerId == id);
                if (customer != null)
                    sortedCustomers.Add(customer);
            }
            return sortedCustomers;
        }

        public void InsertCustomerPriyoCoin(CustomerPriyoCoin customerPriyoCoin)
        {
            if (customerPriyoCoin == null)
                throw new ArgumentNullException(nameof(customerPriyoCoin));

            _customerPriyoCoinRepository.Insert(customerPriyoCoin);

            //event notification
            _eventPublisher.EntityInserted(customerPriyoCoin);
        }

        public void UpdateCustomerPriyoCoin(CustomerPriyoCoin customerPriyoCoin)
        {
            if (customerPriyoCoin == null)
                throw new ArgumentNullException(nameof(customerPriyoCoin));

            _customerPriyoCoinRepository.Update(customerPriyoCoin);

            //event notification
            _eventPublisher.EntityUpdated(customerPriyoCoin);
        }
        #endregion
    }
}