using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Widgets.BsAffiliate.Domain;
using Nop.Plugin.Widgets.BsAffiliate.Models;
using Nop.Services.Affiliates;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Widgets.BsAffiliate.Extensions;
using System.Data;
using Nop.Core.Domain.Affiliates;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.BsAffiliate.Services
{
    public class AffiliateCustomerMapService : IAffiliateCustomerMapService
    {
        private readonly ILogger _logger;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IAffiliateService _affiliateService;
        private readonly ICustomerService _customerService;
        private readonly IRepository<AffiliateCustomerMapping> _acMappingRepository;
        private readonly IRepository<GenericAttribute> _gaRepository;
        private readonly IGenericAttributeService _gaService;
        private readonly IWorkContext _workContext;
        private readonly BsAffiliateObjectContext _context;
        private readonly IDataProvider _dataProvider;
        private readonly IRepository<AffiliateType> _atRepository;

        public AffiliateCustomerMapService(IRepository<Customer> customerRepository,
            IGenericAttributeService gaService, IRepository<GenericAttribute> gaRepository,
            IRepository<AffiliateCustomerMapping> acMappingRepository, 
            ICustomerService customerService, ILogger logger,
            IAffiliateService affiliateService, IWorkContext workContext,
            BsAffiliateObjectContext context, IDataProvider dataProvider,
            IRepository<AffiliateType> atRepository)
        {
            _gaRepository = gaRepository;
            _customerRepository = customerRepository;
            _gaService = gaService;
            _acMappingRepository = acMappingRepository;
            _customerService = customerService;
            _logger = logger;
            _affiliateService = affiliateService;
            _workContext = workContext;
            _context = context;
            _dataProvider = dataProvider;
            _atRepository = atRepository;
        }

        public AffiliateCustomerMapping GetAffiliateCustomerMapByCustomerId(int customerId)
        {
            try
            {
                var result = _acMappingRepository.Table.FirstOrDefault(x => x.CustomerId == customerId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return null;
            }
        }

        public Affiliate GetAffiliateByCustomerId(int customerId)
        {
            try
            {
                var map = GetAffiliateCustomerMapByCustomerId(customerId);
                if (map == null)
                    return null;

                var affiliate = _affiliateService.GetAffiliateById(map.AffiliateId);
                return affiliate;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return null;
            }
        }

        public AffiliateCustomerMapModel GetCustomerByAffiliateId(int affiliateId)
        {
            try
            {
                var affiliate = _affiliateService.GetAffiliateById(affiliateId);
                if (affiliate == null)
                    return null;

                var model = new AffiliateCustomerMapModel()
                {
                    AffiliateId = affiliateId,
                    AffiliateName = affiliate.GetFullName() + " (" + affiliate.Address.Email + ")",
                };
                var map = _acMappingRepository.Table.Where(x => x.AffiliateId == affiliateId).FirstOrDefault();
                model.AvailableAffiliateTypes = GetAvailableAffiliateTypes();
                if (map != null)
                {
                    var customer = _customerService.GetCustomerById(map.CustomerId);
                    if (customer != null)
                    {
                        model.CustomerName = customer.GetFullName() + " (" + customer.Email + ")";
                        model.CustomerId = customer.Id;
                        model.AffiliateTypeId = map.AffiliateTypeId;
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return null;
            }
        }

        public List<SelectListItem> GetAvailableAffiliateTypes()
        {
            var list = new List<SelectListItem>();
            var types = _atRepository.Table.ToList();

            list.Add(
                new SelectListItem
                {
                    Text = "Select affiliate type",
                    Value = "0"
                });

            foreach (var item in types)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            return list;
        }

        public SaveResponseModel SaveAffiliateCustomer(AffiliateCustomerMapModel model)
        {
            var response = new SaveResponseModel();
            try
            {
                var affiliate = _affiliateService.GetAffiliateById(model.AffiliateId);
                if (affiliate == null)
                {
                    response.Status = false;
                    response.Message = "Affiliate not found.";
                }

                var customer = _customerService.GetCustomerById(model.CustomerId);
                if (customer != null)
                {
                    var cMap = _acMappingRepository.Table.Where(x => x.CustomerId == model.CustomerId).FirstOrDefault();
                    var role = GetAffiliateRole();

                    if (cMap == null)
                    {
                        var map = _acMappingRepository.Table.Where(x => x.AffiliateId == model.AffiliateId).FirstOrDefault();

                        if (map != null)
                        {
                            var oldCustomer = _customerService.GetCustomerById(map.CustomerId);
                            oldCustomer.CustomerRoles.Remove(role);
                            oldCustomer.AffiliateId = 0;
                            _customerService.UpdateCustomer(oldCustomer);

                            map.CustomerId = model.CustomerId;
                            map.AffiliateTypeId = model.AffiliateTypeId;

                            _acMappingRepository.Update(map);
                        }
                        else
                        {
                            map = new AffiliateCustomerMapping()
                            {
                                AffiliateId = model.AffiliateId,
                                CustomerId = model.CustomerId,
                                AffiliateTypeId = model.AffiliateTypeId
                            };
                            _acMappingRepository.Insert(map);
                        }

                        customer.CustomerRoles.Add(role);
                        customer.AffiliateId = model.AffiliateId;
                        _customerService.UpdateCustomer(customer);

                        response.Status = true;
                    }
                    else if (cMap != null && cMap.AffiliateId == model.AffiliateId)
                    {
                        customer.AffiliateId = model.AffiliateId;
                        
                        if (!customer.CustomerRoles.Any(x => x.SystemName == "BS Affiliate"))
                            customer.CustomerRoles.Add(role);

                        cMap.AffiliateTypeId = model.AffiliateTypeId;
                        _acMappingRepository.Update(cMap);

                        _customerService.UpdateCustomer(customer);
                        response.Status = true;
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Customer is mapped with another affiliate account.";
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = "Customer not found.";
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                response.Message = ex.Message;
                response.Status = false;
            }
            return response;
        }

        public CustomerRole GetAffiliateRole()
        {
            var role = _customerService.GetCustomerRoleBySystemName(BsCustomerRoleNames.BsAffiliate);
            if (role == null)
            {
                role = new CustomerRole()
                {
                    Active = true,
                    Name = "BS Affiliate",
                    SystemName = BsCustomerRoleNames.BsAffiliate,
                };
                _customerService.InsertCustomerRole(role);
            }
            return role;
        }

        public IList<AffiliateCustomerMapModel> GetCustomer(string queryString)
        {
            try
            {
                queryString = "%" + queryString + "%";
                var clause = _dataProvider.GetParameter();
                clause.ParameterName = "QueryWhere";
                clause.Value = queryString;
                clause.DbType = DbType.AnsiStringFixedLength;

                var result = _context.SqlQuery<AffiliateCustomerMapModel>(@"Select cu.Id CustomerId, fg.[Value] FirstName, 
                    lg.[Value] LastName, cu.Email from Customer cu left join GenericAttribute fg on (fg.EntityId = cu.Id 
                    and fg.KeyGroup = 'Customer' and fg.[Key] = 'FirstName') left join GenericAttribute lg on (lg.EntityId 
                    = cu.Id and lg.KeyGroup = 'Customer' and lg.[Key] = 'LastName') where fg.[Value] like @QueryWhere or 
                    lg.[Value] like @QueryWhere or cu.Email like @QueryWhere", clause).Take(10).ToList();


                //var query = _customerRepository.Table;

                //query = query
                //    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                //    .Where(z =>
                //        (z.Attribute.KeyGroup == "Customer" &&
                //            z.Attribute.Key == SystemCustomerAttributeNames.LastName &&
                //            z.Attribute.Value.Contains(queryString)) ||
                //        (z.Attribute.KeyGroup == "Customer" &&
                //            z.Attribute.Key == SystemCustomerAttributeNames.FirstName &&
                //            z.Attribute.Value.Contains(queryString)) ||
                //        z.Customer.Username.Contains(queryString) ||
                //        z.Customer.Email.Contains(queryString)
                //        )
                //    .Select(z => z.Customer);

                //var result = query.Distinct().OrderByDescending(c => c.CreatedOnUtc).Take(10).ToList().AsQueryable();

                 var customers = result.Select(x => new AffiliateCustomerMapModel()
                {
                    CustomerId = x.CustomerId,
                    CustomerName = x.FirstName + " " + x.LaststName + " (" + x.Email + ")"
                }).ToList();

                return customers;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return null;
            }
        }

        public void DeleteAffiliateCustomerMap(AffiliateCustomerMapping map)
        {
            try
            {
                _acMappingRepository.Delete(map);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        public bool IsActive()
        {
            AffiliateCustomerMapping customerMap = null;
            if (IsApplied(out customerMap))
            {
                var map = GetAffiliateCustomerMapByCustomerId(_workContext.CurrentCustomer.Id);
                var affiliate = _affiliateService.GetAffiliateById(map.AffiliateId);
                return affiliate.Active;
            }
            return false;
        }

        public bool IsApplied(out AffiliateCustomerMapping customerMap)
        {
            var map = GetAffiliateCustomerMapByCustomerId(_workContext.CurrentCustomer.Id);
            customerMap = map;

            if (map == null)
            {
                var role = GetAffiliateRole();
                if (_workContext.CurrentCustomer.CustomerRoles.Any(x => x.Id == role.Id))
                {
                    _workContext.CurrentCustomer.CustomerRoles.Remove(role);
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                }
                return false;
            }
            else
            {
                var affiliate = _affiliateService.GetAffiliateById(map.AffiliateId);
                if (affiliate == null)
                {
                    DeleteAffiliateCustomerMap(map);
                    return false;
                }
                _workContext.CurrentCustomer.AffiliateId = affiliate.Id;
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                return true;
            }
        }
    }
}
