using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nop.Core.Data;
using Nop.Plugin.Widgets.MobileLogin.Domain;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Services.Orders;
using Nop.Plugin.Widgets.MobileLogin.Models;
using Nop.Services.Helpers;

namespace Nop.Plugin.Widgets.MobileLogin.Services
{
    public partial class MobileLoginService : IMobileLoginService
    {
        #region Fields
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<Domain.MobileLoginCustomer> _mlcRepository;
        private readonly IRepository<GenericAttribute> _gaRepository;
        private readonly ICustomerService _customerService;
        private readonly CustomerSettings _customerSettings;
        private readonly IRepository<Customer> _customerRepository;

        #endregion

        #region Ctor
        public MobileLoginService(
            ILocalizationService localizationService,
            IRepository<Domain.MobileLoginCustomer> mlcRepository,
            IRepository<GenericAttribute> gaRepository,
            ICustomerService customerService,
            CustomerSettings customerSettings,
            IRepository<Customer> customerRepository
            )
        {
            this._localizationService = localizationService;
            this._mlcRepository = mlcRepository;
            this._gaRepository = gaRepository;
            this._customerService = customerService;
            this._customerSettings = customerSettings;
            _customerRepository = customerRepository;
        }

        #endregion

        #region Utilties

        private string GetEmbeddedFileContent(string resourceName)
        {
            string fullResourceName = string.Format("Nop.Plugin.Widgets.MobileLogin.Files.{0}", resourceName);
            var assem = this.GetType().Assembly;
            using (var stream = assem.GetManifestResourceStream(fullResourceName))
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
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
        public virtual void Delete(Domain.MobileLoginCustomer mobileLoginCustomerRecord)
        {
            if (mobileLoginCustomerRecord == null)
                throw new ArgumentNullException("mobileLoginCustomerRecord");

            _mlcRepository.Delete(mobileLoginCustomerRecord);
        }
        public virtual IList<Domain.MobileLoginCustomer> GetAll()
        {
            var query = from mlc in _mlcRepository.Table
                        where mlc.MobileNumber != null
                        orderby mlc.Id                        
                        select mlc;
            var records = query.ToList();
            return records;
        }
        public virtual Domain.MobileLoginCustomer GetById(int mobileLoginCustomerId)
        {
            if (mobileLoginCustomerId == 0)
                return null;

            return _mlcRepository.GetById(mobileLoginCustomerId);
        }
        public virtual Domain.MobileLoginCustomer GetByCustomerId(int customerId)
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
        public virtual Domain.MobileLoginCustomer GetByMobileNumber(string mobileNumber)
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

            if (mlcRecord == null) return null;
            //var query = from c in _customerService.GetAllCustomers()
            //    where c.Id == mlcRecord.CustomerId
            //    select c;
            return _customerRepository.GetById(mlcRecord.CustomerId);
            //if (query.Count() == 0)
            //{
            //   _mlcRepository.Delete(mlcRecord);
            //    return null;
            //}
            //var customer = query.FirstOrDefault();
            //return customer;
        }
        public virtual void Insert(Domain.MobileLoginCustomer mobileLoginCustomer)
        {
            if (mobileLoginCustomer == null)
                throw new ArgumentNullException("mobileLoginCustomer");

            _mlcRepository.Insert(mobileLoginCustomer);
        }
        public virtual void Update(Domain.MobileLoginCustomer mobileLoginCustomer)
        {
            if (mobileLoginCustomer == null)
                throw new ArgumentNullException("mobileLoginCustomer");

            _mlcRepository.Update(mobileLoginCustomer);
        }        
        public virtual CustomerRegistrationResult RegisterCustomer(CustomerRegistrationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.Customer == null)
                throw new ArgumentException("Can't load current customer");

            var result = new CustomerRegistrationResult();
            if (request.Customer.IsSearchEngineAccount())
            {
                result.AddError("Search engine can't be registered");
                return result;
            }
            if (request.Customer.IsBackgroundTaskAccount())
            {
                result.AddError("Background task account can't be registered");
                return result;
            }
            if (request.Customer.IsRegistered())
            {
                result.AddError("Current customer is already registered");
                return result;
            }
            if (String.IsNullOrEmpty(request.Email))
            {
                result.AddError(_localizationService.GetResource("Account.Register.Errors.EmailIsNotProvided"));
                return result;
            }
            if (!CommonHelper.IsValidEmail(request.Email))
            {
                result.AddError(_localizationService.GetResource("Common.WrongEmail"));
                return result;
            }

            if (_customerSettings.UsernamesEnabled)
            {
                if (String.IsNullOrEmpty(request.Username))
                {
                    result.AddError(_localizationService.GetResource("Account.Register.Errors.UsernameIsNotProvided"));
                    return result;
                }
            }

            //validate unique user
            if (_customerService.GetCustomerByEmail(request.Email) != null)
            {
                result.AddError(_localizationService.GetResource("Account.Register.Errors.EmailAlreadyExists"));
                return result;
            }
            if (_customerSettings.UsernamesEnabled)
            {
                if (_customerService.GetCustomerByUsername(request.Username) != null)
                {
                    result.AddError(_localizationService.GetResource("Account.Register.Errors.UsernameAlreadyExists"));
                    return result;
                }
            }

            //at this point request is valid
            request.Customer.Username = request.Username;
            request.Customer.Email = request.Email;
            request.Customer.PasswordFormat = request.PasswordFormat;
            request.Customer.Password = null;            

            request.Customer.Active = request.IsApproved;

            //add to 'Registered' role
            var registeredRole = _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Registered);
            if (registeredRole == null)
                throw new NopException("'Registered' role could not be loaded");
            request.Customer.CustomerRoles.Add(registeredRole);
            //remove from 'Guests' role
            var guestRole = request.Customer.CustomerRoles.FirstOrDefault(cr => cr.SystemName == SystemCustomerRoleNames.Guests);
            if (guestRole != null)
                request.Customer.CustomerRoles.Remove(guestRole);

            _customerService.UpdateCustomer(request.Customer);
            return result;
        }
        public virtual IPagedList<Customer> getAllMobileLoginCustomers(string searchEmail, string searchName,
            string searchMobileNumber, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var mobileLoginCustomers = GetAll();

            int total = mobileLoginCustomers.Count();
            int[] ids = new int[total];

            int i = 0;
            foreach (var mlc in mobileLoginCustomers)
            {
                ids[i] = mlc.CustomerId;                
                i++;
            }
            var customerList = _customerService.GetCustomersByIds(ids);
            if (!String.IsNullOrWhiteSpace(searchEmail))
            {
                customerList = customerList.Where(x => x.Email == searchEmail).ToList();
            }
            if (!String.IsNullOrWhiteSpace(searchMobileNumber))
            {
                int searchMobileNumberCustomerId = 0;
                var searchMobileNumberCustomer = GetCustomerByMobileNumber(searchMobileNumber);
                if (searchMobileNumberCustomer != null)
                    searchMobileNumberCustomerId = searchMobileNumberCustomer.Id;
                customerList = customerList.Where(x => x.Id == searchMobileNumberCustomerId).ToList();
            }
            if (!String.IsNullOrWhiteSpace(searchName))
            {
                customerList = customerList
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Customer" &&
                        z.Attribute.Key == SystemCustomerAttributeNames.FirstName &&
                        z.Attribute.Value.Contains(searchName)))
                    .Select(z => z.Customer).ToList();
            }                
            var customers = new PagedList<Customer>(customerList, pageIndex, pageSize);

            return customers;
        }
        #endregion
    }
}
