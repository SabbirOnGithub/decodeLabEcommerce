using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Widgets.BsAffiliate.Domain;
using Nop.Plugin.Widgets.BsAffiliate.Extensions;
using Nop.Plugin.Widgets.BsAffiliate.Models;
using Nop.Plugin.Widgets.BsAffiliate.Services;
using Nop.Services.Affiliates;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.BsAffiliate.Controllers
{
    public class BsAffiliateController : Controller
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

        public BsAffiliateController(IWorkContext workContext,
            IAffiliateCustomerMapService acmService,
            IAffiliateService affiliateService, ILogger logger,
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

        public ActionResult Orders(AffiliatedOrderListModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var result = new AffiliatePublicDetailsModel();
            AffiliateCustomerMapping map;
            if (_acmService.IsApplied(out map) && _acmService.IsActive())
            {
                result = _affiliatePublicService.GetCuurentCustomerAffiliatedOrders(model);
                result.Active = true;
                return View("~/Plugins/Widgets.BsAffiliate/Views/BsAffiliate/AffiliatedOrders.cshtml", result);
            }
            return RedirectToRoute("CustomerAffiliateInfo");
        }

        public ActionResult Info()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var model = PrepareAffiliateModel();
            ViewBag.IsActive = _acmService.IsActive();
            return View("~/Plugins/Widgets.BsAffiliate/Views/BsAffiliate/AffiliateInfo.cshtml", model);
        }

        [HttpPost]
        public ActionResult Info(AffiliateModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                var resposne = _affiliatePublicService.SaveInfo(model);
                if (resposne.Status)
                    return RedirectToRoute("CustomerAffiliateInfo");
                else
                    ModelState.AddModelError("", resposne.Message);
            }
            model.Address.AvailableCountries = GetAvailableCountries();
            model.Address.AvailableStates = GetAvailableStates(model.Address.CountryId.HasValue ? model.Address.CountryId.Value : 0);
            return View("~/Plugins/Widgets.BsAffiliate/Views/BsAffiliate/AffiliateInfo.cshtml", model);

        }

        private AffiliateModel PrepareAffiliateModel()
        {
            try
            {
                AffiliateModel model = null;
                var customer = _workContext.CurrentCustomer;
                AffiliateCustomerMapping mapping = null;
                if (!_acmService.IsApplied(out mapping))
                {
                    model = new AffiliateModel()
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
                    if (string.IsNullOrWhiteSpace(model.Address.LastName) && !string.IsNullOrWhiteSpace(model.Address.FirstName))
                    {
                        var names = model.Address.FirstName.Split(' ');
                        if (names.Length > 1)
                        {
                            model.Address.FirstName = string.Join(" ", names.Where((o, i) => i != names.Length - 1).ToArray());
                            model.Address.LastName = names[names.Length - 1];
                        }
                    }
                }
                else
                {
                    var affiliate = _acmService.GetAffiliateByCustomerId(_workContext.CurrentCustomer.Id);

                    var affiliateType = _atRepository.GetById(mapping.AffiliateTypeId);

                    model = new AffiliateModel()
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
                }
                return model;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return new AffiliateModel();
            }
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

        public ActionResult Public(string widgetZone)
        {
            AffiliateCustomerMapping map;
            ViewBag.IsApplied = _acmService.IsApplied(out map);
            ViewBag.IsActive = _acmService.IsActive();

            if (widgetZone == "account_navigation_after")
                return View("~/Plugins/Widgets.BsAffiliate/Views/BsAffiliate/Public.cshtml");
            return Content("");
        }
    }
}
