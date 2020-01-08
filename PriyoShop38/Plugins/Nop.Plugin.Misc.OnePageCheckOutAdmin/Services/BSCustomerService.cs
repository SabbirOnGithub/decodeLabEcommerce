using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc.Html;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.OnePageCheckOutAdmin.Models.Common;
using Nop.Services.Customers;

namespace Nop.Plugin.Misc.OnePageCheckOutAdmin.Services
{
    public partial class BSCustomerService:IBSCustomerService
    {
       #region Fields
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<GenericAttribute> _genericAttributeRepository; 
        private readonly IRepository<CustomerRole> _customerRoleRepository;
        private readonly CommonSettings _commonSettings;
        private readonly ICacheManager _cacheManager;
        #endregion

        #region Ctor

        public BSCustomerService(ICacheManager cacheManager,
            IRepository<Customer> customerRepository,
            IRepository<GenericAttribute> genericAttributeRepository,
            IRepository<CustomerRole> customerRoleRepository,
            CommonSettings commonSettings)
        {
            this._cacheManager = cacheManager;
            this._customerRepository = customerRepository;
            this._customerRoleRepository = customerRoleRepository;
            this._commonSettings = commonSettings;
            this._genericAttributeRepository = genericAttributeRepository;
        }

        #endregion
        #region methode
        public bool IsPhoneNumber(string number)
        {
            bool result = Regex.Match(number, @"^(-?[0-9]+\d*([.]\d+)?)$|^(-?0[.]\d*[0-9]+)$|^0$|^0.0$").Success;
            return result;
        }
        public virtual IPagedList<Customer> GetAllCustomers(string keywords = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _customerRepository.Table;
            List<Customer> emailResult= new List<Customer>();

            string name;
            var firstCount = 0;
            List<int> ids= new List<int>();
            if (!String.IsNullOrWhiteSpace(keywords))
            {
                query = from p in query
                       where (p.Email.Contains(keywords))
                       select p;
                query = query.OrderByDescending(c => c.CreatedOnUtc);
               
                emailResult = new PagedList<Customer>(query, pageIndex, pageSize);
                
                firstCount= emailResult.Count;
            }
            if (keywords != null && !keywords.Contains("@") && firstCount < pageSize)
            {
                ids = emailResult.Select(x => x.Id).ToList();
                var queryForFirstAndLastName = _genericAttributeRepository.Table.Where(x => x.KeyGroup.Equals("Customer") && !ids.Contains(x.EntityId));
                //&& (x.Key.Equals("FirstName") || x.Key.Equals("LastName")
                var splitedName = keywords.Split(' ');
                if (splitedName.Count() > 1)
                {
                    name = splitedName[0].Trim();
                    var name1 = name;
                    name = splitedName[1].Trim();
                    var queryForFirst =
                        queryForFirstAndLastName.Where(x => (x.Key.Equals("FirstName") && x.Value.Contains(name1)));
                    IQueryable<GenericAttribute> queryForLast = null;
                    if (name != string.Empty)
                    {
                       queryForLast =
                       queryForFirstAndLastName.Where(x => x.Key.Equals("LastName") && x.Value.Contains(name));
                    }
                   
                    var firstLastNameQuery = from cus in _customerRepository.Table
                                             join gen1 in queryForFirst on cus.Id equals gen1.EntityId
                                               
                                             select cus;
                    if (queryForLast != null)
                    {
                        firstLastNameQuery = from cus in firstLastNameQuery
                            join gen2 in queryForLast on cus.Id equals gen2.EntityId
                            select cus;

                    }
                    firstLastNameQuery = firstLastNameQuery.OrderByDescending(c => c.CreatedOnUtc);
                    List<Customer> firstLastNameResult = new PagedList<Customer>(firstLastNameQuery, pageIndex, pageSize - firstCount);
                    emailResult.AddRange(firstLastNameResult);
                }
                else
                {
                    name = splitedName[0];
                    var queryForFirst =
                        queryForFirstAndLastName.Where(x => x.Key.Equals("FirstName") && x.Value.Contains(name));
                    var firstNameQuery = from cus in _customerRepository.Table
                                         join gen in queryForFirst on cus.Id equals gen.EntityId
                                            select cus;
                    firstNameQuery = firstNameQuery.OrderByDescending(c => c.CreatedOnUtc); 
                    var firstNameTemp = new PagedList<Customer>(firstNameQuery, pageIndex, pageSize - firstCount);
                    emailResult.AddRange(firstNameTemp);
                    if (firstCount + firstNameTemp.Count < pageSize)
                    {
                        var queryForLast =
                        queryForFirstAndLastName.Where(x => x.Key.Equals("LastName") && x.Value.Contains(name));
                        var lastNameQuery = from cus in _customerRepository.Table
                                            join gen in queryForLast on cus.Id equals gen.EntityId
                                             select cus;
                        lastNameQuery = lastNameQuery.OrderByDescending(c => c.CreatedOnUtc); 
                        var lastNameTemp = new PagedList<Customer>(lastNameQuery, pageIndex, pageSize - firstCount - firstNameTemp.Count);
                        emailResult.AddRange(lastNameTemp);
                    }
                }

            }
            if (keywords != null && IsPhoneNumber(keywords) && emailResult.Count < pageSize)
            {
                ids = emailResult.Select(x => x.Id).ToList();
                var queryForPhone = _genericAttributeRepository.Table.Where(x => x.KeyGroup.Equals("Customer") && !ids.Contains(x.EntityId) && x.Key.Equals("Phone") && x.Value.Contains(keywords));
                var phoneCustomer = from cus in _customerRepository.Table
                                    join gen in queryForPhone on cus.Id equals gen.EntityId
                                     select cus;
                phoneCustomer = phoneCustomer.OrderByDescending(c => c.CreatedOnUtc);
                var phoneTemp = new PagedList<Customer>(phoneCustomer, pageIndex, pageSize - emailResult.Count);
                emailResult.AddRange(phoneTemp);
            }
            //query = query.OrderByDescending(c => c.CreatedOnUtc);
            
            return new PagedList<Customer>(emailResult, pageIndex, pageSize);
            
            
        }


        public virtual IPagedList<Customer> GetAllCustomersByFirstNameAndLastName(string keywords = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var queryForFirstAndLastName = _genericAttributeRepository.Table.Where(x=>x.KeyGroup.Equals("Customer") && (x.Key.Equals("FirstName") || x.Key.Equals("LastName")));

            var customerWithFullName = from entityId in queryForFirstAndLastName
                group entityId by entityId.EntityId
                into g
                select new CustomerGroup
                {
                    EntityId = g.Key,
                    Name = String.Join(" ", g.Select(x => x.Value))
                };

            var selectedCustomer = customerWithFullName.Where(x => x.Name.Contains(keywords));

            var customers = from cus in _customerRepository.Table
                join sC in selectedCustomer on cus.Id equals sC.EntityId
                select cus;



            customers = customers.OrderByDescending(c => c.CreatedOnUtc);
            return new PagedList<Customer>(customers, pageIndex, pageSize);
            

        }

        #endregion
    }
}
