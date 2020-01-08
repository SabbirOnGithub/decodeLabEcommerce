using BS.Plugin.NopStation.MobileWebApi.Controllers;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Widgets.BsAffiliate.Domain;
using Nop.Plugin.Widgets.BsAffiliate.Services;
using Nop.Plugin.Widgets.BsAffiliate.Models;
using Nop.Services.Affiliates;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using System.Web.Http;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using Nop.Services.Common;
using Nop.Plugin.Widgets.BsAffiliate.Extensions;
using Nop.Core.Domain.Affiliates;

namespace Nop.Plugin.Widgets.BsAffiliate.Controllers
{
    public class BsAffiliateApiController : WebApiController
    {
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly IWebHelper _webHelper;
        private readonly IAffiliateCustomerMapService _acmService;
        private readonly IAffiliatePublicService _affiliatePublicService;
        private readonly IAffiliateService _affiliateService;
        private readonly ICountryService _countryService;
        private readonly ICustomerService _customerService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly CustomerSettings _customerSettings;
        private readonly ISettingService _settingService;
        private readonly IRepository<AffiliateType> _atRepository;

        public BsAffiliateApiController(IWorkContext workContext,
                IAffiliateCustomerMapService acmService,
                IAffiliateService affiliateService,
                ILogger logger,
                CustomerSettings customerSettings,
                ICountryService countryService,
                IStateProvinceService stateProvinceService,
                IAffiliatePublicService affiliatePublicService,
                ICustomerService customerService,
                ISettingService settingService,
                IWebHelper webHelper,
                IRepository<AffiliateType> atRepository)
        {
            _workContext = workContext;
            _acmService = acmService;
            _affiliateService = affiliateService;
            _logger = logger;
            _customerSettings = customerSettings;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _affiliatePublicService = affiliatePublicService;
            _customerService = customerService;
            _settingService = settingService;
            _webHelper = webHelper;
            _atRepository = atRepository;
        }

        [System.Web.Http.Route("api/bsaffiliate/orders")]
        public IHttpActionResult Orders(AffiliatedOrderListModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Unauthorized();

            var result = new GeneralResponseModel<AffiliatePublicDetailsModel>();

            AffiliateCustomerMapping map;
            if (_acmService.IsApplied(out map) && _acmService.IsActive())
            {
                result.Data = _affiliatePublicService.GetCuurentCustomerAffiliatedOrders(model);
                result.Data.Active = true;
                result.StatusCode = (int)ErrorType.Ok;
                return Ok(result);
            }

            result.StatusCode = (int)ErrorType.NotOk;
            return Ok(result);
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/bsaffiliate/info")]
        public IHttpActionResult Info()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Unauthorized();

            var model = PrepareAffiliateModel();
            model.Data.IsActive = _acmService.IsActive();
            return Ok(model);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/bsaffiliate/info")]
        public IHttpActionResult SaveInfo(AffiliateModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Unauthorized();
            var result = new GeneralResponseModel<AffiliateModel>();

            if (ModelState.IsValid)
            {
                var resposne = _affiliatePublicService.SaveInfo(model);
                if (resposne.Status)
                {
                    result = PrepareAffiliateModel();
                    return Ok(result);
                }
                else
                {
                    result.StatusCode = (int)ErrorType.NotOk;
                    result.ErrorList.Add(resposne.Message);
                    return Ok(result);
                }
            }
            
            result.ErrorList.AddRange(ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage));
            result.StatusCode = (int)ErrorType.NotOk;
            return Ok(result);
        }

        private GeneralResponseModel<AffiliateModel> PrepareAffiliateModel()
        {
            try
            {
                GeneralResponseModel<AffiliateModel> model = new GeneralResponseModel<AffiliateModel>();
                var customer = _workContext.CurrentCustomer;
                AffiliateCustomerMapping mapping = null;
                if (!_acmService.IsApplied(out mapping))
                {
                    var afModel = new AffiliateModel()
                    {
                        Address = new AddressModel()
                        {
                            FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName),
                            LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName),
                            Company = customer.GetAttribute<string>(SystemCustomerAttributeNames.Company),
                            Address1 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress),
                            Address2 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress2),
                            ZipPostalCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.ZipPostalCode),
                            City = customer.GetAttribute<string>(SystemCustomerAttributeNames.City),
                            CountryId = customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId),
                            StateProvinceId = customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId),
                            PhoneNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone),
                            FaxNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Fax),
                            Email = customer.Email,
                            AvailableCountries = GetAvailableCountries(),
                            AvailableStates = GetAvailableStates(customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId)),
                        },
                        FriendlyUrlName = "",
                    };
                    if (string.IsNullOrWhiteSpace(afModel.Address.LastName) && !string.IsNullOrWhiteSpace(afModel.Address.FirstName))
                    {
                        var names = afModel.Address.FirstName.Split(' ');
                        if (names.Length > 1)
                        {
                            afModel.Address.FirstName = string.Join(" ", names.Where((o, i) => i != names.Length - 1).ToArray());
                            afModel.Address.LastName = names[names.Length - 1];
                        }
                    }

                    model.Data = afModel;
                }
                else
                {
                    var affiliate = _acmService.GetAffiliateByCustomerId(_workContext.CurrentCustomer.Id);

                    var affiliateType = _atRepository.GetById(mapping.AffiliateTypeId);

                    var afModel = new AffiliateModel()
                    {
                        IsApplied = true,
                        Active = _acmService.IsActive(),
                        Address = new AddressModel
                        {
                            Address1 = affiliate.Address.Address1,
                            Address2 = affiliate.Address.Address2,
                            City = affiliate.Address.City,
                            Company = affiliate.Address.Company,
                            CountryId = affiliate.Address.CountryId,
                            Email = customer.Email,
                            AvailableCountries = GetAvailableCountries(),
                            AvailableStates = GetAvailableStates(customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId)),
                            LastName = affiliate.Address.LastName,
                            FaxNumber = affiliate.Address.FaxNumber,
                            FirstName = affiliate.Address.FirstName,
                            PhoneNumber = affiliate.Address.PhoneNumber,
                            StateProvinceId = affiliate.Address.StateProvinceId,
                            ZipPostalCode = affiliate.Address.ZipPostalCode,
                        },
                        FriendlyUrlName = affiliate.FriendlyUrlName,
                        BKash = _settingService.GetSettingByKey<string>(string.Format("Affiliate.Customer.BKashNumber-{0}", affiliate.Id)),
                        Url = affiliate.GenerateUrl(_webHelper, affiliateType)
                    };

                    model.Data = afModel;
                }
                return model;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return new GeneralResponseModel<AffiliateModel>();
            }
        }

        private IList<SelectListItem> GetAvailableCountries()
        {
            var countryList = new List<SelectListItem>();

            countryList.Add(new SelectListItem { Text = "Select Country", Value = "0" });
            foreach (var c in _countryService.GetAllCountries(showHidden: true))
            {
                countryList.Add(new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
            }
            return countryList;
        }

        private IList<SelectListItem> GetAvailableStates(int countryId)
        {
            var stateList = new List<SelectListItem>();
            var states = _stateProvinceService.GetStateProvincesByCountryId(countryId).ToList();
            stateList.Add(new SelectListItem { Text = "Select State", Value = "0" });

            if (states.Any())
            {
                foreach (var s in states)
                {
                    stateList.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
                }
            }
            else
            {
                stateList.Add(new SelectListItem
                {
                    Text = "Other (Non US)",
                    Value = "0"
                });
            }
            return stateList;
        }
    }
}
