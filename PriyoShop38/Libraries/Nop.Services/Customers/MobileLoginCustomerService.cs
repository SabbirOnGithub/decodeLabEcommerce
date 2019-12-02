using System;
using System.IO;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Data;
using System.Data;

namespace Nop.Services.Customers
{
    public partial class MobileLoginCustomerService : IMobileLoginCustomerService
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IRepository<MobileLoginCustomer> _mlcRepository;
        private readonly IRepository<GenericAttribute> _gaRepository;
        private readonly ICustomerService _customerService;
        private readonly CustomerSettings _customerSettings;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;

        #endregion

        #region Ctor

        public MobileLoginCustomerService(ILocalizationService localizationService,
            IRepository<MobileLoginCustomer> mlcRepository,
            IRepository<GenericAttribute> gaRepository,
            ICustomerService customerService,
            CustomerSettings customerSettings,
            IRepository<Customer> customerRepository,
            IDataProvider dataProvider,
            IDbContext dbContext)
        {
            this._localizationService = localizationService;
            this._mlcRepository = mlcRepository;
            this._gaRepository = gaRepository;
            this._customerService = customerService;
            this._customerSettings = customerSettings;
            this._customerRepository = customerRepository;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
        }

        #endregion
        
        #region Methods

        public virtual CustomerLoginResults ValidateCustomerByMobileNumber(string mobileNumber)
        {
            var customer = GetCustomerByMobileNumber(mobileNumber);                            

            if (customer == null)
                return CustomerLoginResults.CustomerNotExist;
            if (customer.Deleted)
                return CustomerLoginResults.Deleted;
            if (!customer.Active)
                return CustomerLoginResults.NotActive;
            //only registered can login
            if (!customer.IsRegistered())
                return CustomerLoginResults.NotRegistered;                       

            return CustomerLoginResults.Successful;
        }

        public virtual void DeleteMobileLoginCustomer(MobileLoginCustomer mobileLoginCustomerRecord)
        {
            if (mobileLoginCustomerRecord == null)
                throw new ArgumentNullException("mobileLoginCustomerRecord");

            _mlcRepository.Delete(mobileLoginCustomerRecord);
        }

        public virtual MobileLoginCustomer GetMobileLoginCustomerById(int mobileLoginCustomerId)
        {
            if (mobileLoginCustomerId == 0)
                return null;

            return _mlcRepository.GetById(mobileLoginCustomerId);
        }

        public virtual MobileLoginCustomer GetMobileLoginCustomerByCustomerId(int customerId)
        {
            if (customerId == 0)
                return null;

            var query = from gp in _mlcRepository.Table
                        where gp.CustomerId == customerId
                        orderby gp.Id
                        select gp;
            var record = query.FirstOrDefault();
            return record;
        }

        public virtual MobileLoginCustomer GetMobileLoginCustomerByMobileNumber(string mobileNumber)
        {
            if (mobileNumber == null)
                return null;

            var query = from mlc in _mlcRepository.Table
                        where mlc.MobileNumber == mobileNumber                        
                        select mlc;
            var record = query.FirstOrDefault();
            return record;
        }

        public virtual Customer GetCustomerByMobileNumber(string mobileNumber)
        {
            var mlcQuery = from mlc in _mlcRepository.Table
                        where mlc.MobileNumber == mobileNumber
                        select mlc;
            var mlcRecord = mlcQuery.FirstOrDefault();

            if (mlcRecord == null)
                return null;

            return mlcRecord.Customer;
        }

        public virtual void InsertMobileLoginCustomer(MobileLoginCustomer mobileLoginCustomer)
        {
            if (mobileLoginCustomer == null)
                throw new ArgumentNullException("mobileLoginCustomer");

            _mlcRepository.Insert(mobileLoginCustomer);
        }

        public virtual void UpdateMobileLoginCustomer(MobileLoginCustomer mobileLoginCustomer)
        {
            if (mobileLoginCustomer == null)
                throw new ArgumentNullException("mobileLoginCustomer");

            _mlcRepository.Update(mobileLoginCustomer);
        } 

        public virtual IPagedList<MobileLoginCustomer> GetAllMobileLoginCustomers(string searchEmail, string searchName,
            string searchMobileNumber, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            if (pageSize == int.MaxValue)
                pageSize = int.MaxValue - 1;
            
            var pEmail = _dataProvider.GetParameter();
            pEmail.ParameterName = "Email";
            pEmail.Value = string.IsNullOrWhiteSpace(searchEmail) ? DBNull.Value : (object)searchEmail;
            pEmail.DbType = DbType.String;

            var pName = _dataProvider.GetParameter();
            pName.ParameterName = "Name";
            pName.Value = string.IsNullOrWhiteSpace(searchName) ? DBNull.Value : (object)searchName;
            pName.DbType = DbType.String;

            var pMobileNumber = _dataProvider.GetParameter();
            pMobileNumber.ParameterName = "MobileNumber";
            pMobileNumber.Value = string.IsNullOrWhiteSpace(searchMobileNumber) ? DBNull.Value : (object)searchMobileNumber;
            pMobileNumber.DbType = DbType.String;
            
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
            pTotalRecords.DbType = DbType.Int32;
            pTotalRecords.Direction = ParameterDirection.Output;

            //invoke stored procedure
            var customers = _dbContext.ExecuteStoredProcedureList<MobileLoginCustomer>(
                 "MobileLoginCustomerFilterPaged",
                 pEmail,
                 pName,
                 pMobileNumber,
                 pPageIndex,
                 pPageSize,
                 pTotalRecords);

            var totalRecords = pTotalRecords.Value != DBNull.Value ? Convert.ToInt32(pTotalRecords.Value) : 0;
            return new PagedList<MobileLoginCustomer>(customers, pageIndex, pageSize, totalRecords);
        }
        #endregion
    }
}
