using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Nop.Core.Domain.Customers;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Customer;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Customer;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Orders;
using Nop.Services.Orders;
using Nop.Services.Media;
using Nop.Core;
using System.Net;
using Nop.Core.Domain.Tax;
using Nop.Services.Seo;
using Nop.Services.Helpers;
using Nop.Services.Messages;
using Nop.Services.Directory;
using Nop.Services.Authentication.External;
using Nop.Core.Domain.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using Nop.Core.Domain.Messages;
using Nop.Services.Authentication;
using Nop.Services.Tax;
using Nop.Core.Domain.Localization;
using System.Collections.Specialized;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using Nop.Services.Stores;
using Nop.Core.Domain.Media;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using Nop.Web.Models.Common;
using Nop.Web.Extensions;
using System.Net.Http;
using System.Web;
using System.IO;
using BS.Plugin.NopStation.MobileWebApi.Model;
using Nop.Services.Affiliates;
using CustomerLedgerView = Nop.Web.Models.Customer.CustomerLedgerView;
using SelectListItem = System.Web.Mvc.SelectListItem;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class CustomerController : WebApiController
    {
        #region Field

        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerPriyoCoinService _customerPriyoCoinService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly AddressSettings _addressSettings;
        private readonly ForumSettings _forumSettings;
        private readonly OrderSettings _orderSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly TaxSettings _taxSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ITaxService _taxService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IAddressService _addressService;
        private readonly IAffiliateService _affiliateService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IDownloadService _downloadService;
        private readonly IDeviceService _deviceService;
        private readonly ILanguageService _languageService;
        private readonly ICurrencyService _currencyService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IPictureService _pictureService;
        private readonly ICustomerLedgerMasterService _customerLedgerMasterService;
        private readonly ICustomerLedgerDetailService _customerLedgerDetailService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public CustomerController(CustomerSettings customerSettings,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            RewardPointsSettings rewardPointsSettings,
            AddressSettings addressSettings,
            ForumSettings forumSettings,
            OrderSettings orderSettings,
            DateTimeSettings dateTimeSettings,
            TaxSettings taxSettings,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            LocalizationSettings localizationSettings,
            MediaSettings mediaSettings,
            IOrderService orderService,
            IShoppingCartService shoppingCartService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IDateTimeHelper dateTimeHelper,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ICustomerAttributeService customerAttributeService,
            IOpenAuthenticationService openAuthenticationService,
            ICustomerAttributeParser customerAttributeParser,
            IGenericAttributeService genericAttributeService,
            IAuthenticationService authenticationService,
            ITaxService taxService,
            IWorkflowMessageService workflowMessageService,
            IStoreMappingService storeMappingService,
            IAddressService addressService,
            IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            IAddressAttributeFormatter addressAttributeFormatter,
            IDownloadService downloadService,
            IReturnRequestService returnRequestService,
            IDeviceService deviceService,
            ILanguageService languageService,
            ICurrencyService currencyService,
            IRewardPointService rewardPointService,
            IPictureService pictureService,
            ICustomerLedgerMasterService customerLedgerMasterService,
            IAffiliateService affiliateService,
            ICustomerLedgerDetailService customerLedgerDetailService,
            IWebHelper webHelper, ICustomerPriyoCoinService customerPriyoCoinService)
        {
            this._customerSettings = customerSettings;
            this._customerRegistrationService = customerRegistrationService;
            this._customerService = customerService;
            this._customerActivityService = customerActivityService;
            this._localizationService = localizationService;
            this._rewardPointsSettings = rewardPointsSettings;
            this._addressSettings = addressSettings;
            this._webHelper = webHelper;
            this._customerPriyoCoinService = customerPriyoCoinService;
            this._forumSettings = forumSettings;
            this._orderSettings = orderSettings;
            this._dateTimeSettings = dateTimeSettings;
            this._taxSettings = taxSettings;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._localizationSettings = localizationSettings;
            this._mediaSettings = mediaSettings;
            this._orderService = orderService;
            this._shoppingCartService = shoppingCartService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._dateTimeHelper = dateTimeHelper;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._customerAttributeService = customerAttributeService;
            this._openAuthenticationService = openAuthenticationService;
            this._customerAttributeParser = customerAttributeParser;
            this._genericAttributeService = genericAttributeService;
            this._authenticationService = authenticationService;
            this._taxService = taxService;
            this._workflowMessageService = workflowMessageService;
            this._storeMappingService = storeMappingService;
            this._addressService = addressService;
            this._addressAttributeParser = addressAttributeParser;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._addressAttributeService = addressAttributeService;
            this._downloadService = downloadService;
            this._returnRequestService = returnRequestService;
            this._deviceService = deviceService;
            this._languageService = languageService;
            this._currencyService = currencyService;
            this._rewardPointService = rewardPointService;
            this._pictureService = pictureService;
            this._affiliateService = affiliateService;
            this._customerLedgerMasterService = customerLedgerMasterService;
            this._customerLedgerDetailService = customerLedgerDetailService;
        }

        #endregion

        #region Utility

        public static string GetRootUrl()
        {
            var tmpURi = HttpContext.Current.Request.Url;
            var tmpPort = tmpURi.Port > 0 && tmpURi.Port != 80 ? ":" + tmpURi.Port : "";
            var rootUrl = tmpURi.Scheme + "://" + tmpURi.Host + tmpPort + "/";
            return rootUrl;
        }

        private string GenerateRandomReferCode(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        protected string GetToken(int customerId)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var now = Math.Round((DateTime.UtcNow.AddDays(180) - unixEpoch).TotalSeconds);


            var payload = new Dictionary<string, object>()
                                {
                                    { Constant.CustomerIdName, customerId },
                                    { "exp", now }
                                };
            string secretKey = Constant.SecretKey;
            var token = JWT.JsonWebToken.Encode(payload, secretKey, JWT.JwtHashAlgorithm.HS256);

            return token;
        }

        void PrepareCustomerInfoModel(LogInPostResponseModel model, Customer customer)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (customer == null)
                throw new ArgumentNullException("customer");
            model.FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName);
            model.LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName);
            model.City = customer.GetAttribute<string>(SystemCustomerAttributeNames.City);
            model.StreetAddress = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress);
            model.StreetAddress2 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress2);
            model.CountryId = customer.GetAttribute<string>(SystemCustomerAttributeNames.CountryId);
            model.StateProvinceId = customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId);
            model.Phone = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
            model.CustomerId = customer.Id;
            model.Email = customer.Email;
            model.Username = customer.Username;
            model.AffiliateId = customer.AffiliateId;
            if (customer.AffiliateId > 0)
            {
                var affiliateObj = _affiliateService.GetAffiliateById(customer.AffiliateId);
                if (affiliateObj != null)
                {
                    model.AffiliateFriendlyName = affiliateObj.FriendlyUrlName;
                    model.AffiliateUrl = affiliateObj.GenerateUrl(_webHelper, affiliateObj.AffiliateType);
                }
            }
            model.Token = GetToken(customer.Id);
            if (customer.ImageUrl != null)
            {
                model.PictureUrl = GetRootUrl() + "Content/files/UserImage/" + customer.ImageUrl;
            }
            else
            {
                model.PictureUrl = _pictureService.GetPictureUrl(customer.PictureId, 200);
            }

            //get Wallet Information
            string phone = _customerService.GetCustomerPhoneById(customer.Id);
            if (!string.IsNullOrEmpty(phone))
            {
                var customerLedgerMaster = _customerLedgerMasterService.GetCustomerLedgerMasterByContactNo(Convert.ToInt64(phone));
                if (customerLedgerMaster != null)
                {
                    model.WalletAmount = customerLedgerMaster.TotalBalance;
                }
            }
        }

        protected virtual void PrepareCustomerInfoModel(CustomerInfoResponseModel model, Customer customer,
            bool excludeProperties, string overrideCustomCustomerAttributesXml = "")
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (customer == null)
                throw new ArgumentNullException("customer");

            //model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            //foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
            //    model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == _dateTimeHelper.CurrentTimeZone.Id) });

            if (!excludeProperties)
            {
                model.VatNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.VatNumber);
                model.FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName);
                model.LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName);
                model.Gender = customer.GetAttribute<string>(SystemCustomerAttributeNames.Gender);
                var dateOfBirth = customer.GetAttribute<DateTime?>(SystemCustomerAttributeNames.DateOfBirth);
                if (dateOfBirth.HasValue)
                {
                    model.DateOfBirthDay = dateOfBirth.Value.Day;
                    model.DateOfBirthMonth = dateOfBirth.Value.Month;
                    model.DateOfBirthYear = dateOfBirth.Value.Year;
                }
                model.Company = customer.GetAttribute<string>(SystemCustomerAttributeNames.Company);
                model.StreetAddress = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress);
                model.StreetAddress2 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress2);
                model.ZipPostalCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.ZipPostalCode);
                model.City = customer.GetAttribute<string>(SystemCustomerAttributeNames.City);
                model.CountryId = customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId);
                model.StateProvinceId = customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId);
                model.Phone = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                model.Fax = customer.GetAttribute<string>(SystemCustomerAttributeNames.Fax);

                //newsletter
                var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, _storeContext.CurrentStore.Id);
                model.Newsletter = newsletter != null && newsletter.Active;

                model.Signature = customer.GetAttribute<string>(SystemCustomerAttributeNames.Signature);

                model.Email = customer.Email;
                model.Username = customer.Username;
                model.RewardPoint = _rewardPointService.GetRewardPointsBalance(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
            }
            else
            {
                if (_customerSettings.UsernamesEnabled && !_customerSettings.AllowUsersToChangeUsernames)
                    model.Username = customer.Username;
            }

            //countries and states
            if (_customerSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new System.Web.Mvc.SelectListItem { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries())
                {
                    model.AvailableCountries.Add(new System.Web.Mvc.SelectListItem
                    {
                        Text = c.GetLocalized(x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (_customerSettings.StateProvinceEnabled)
                {
                    //states
                    var states = _stateProvinceService.GetStateProvincesByCountryId(model.CountryId).ToList();
                    if (states.Count > 0)
                    {
                        model.AvailableStates.Add(new System.Web.Mvc.SelectListItem { Text = _localizationService.GetResource("Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem { Text = s.GetLocalized(x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                        }
                    }
                    else
                    {
                        bool anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);

                        model.AvailableStates.Add(new System.Web.Mvc.SelectListItem
                        {
                            Text = _localizationService.GetResource(anyCountrySelected ? "Address.OtherNonUS" : "Address.SelectState"),
                            Value = "0"
                        });
                    }

                }
            }
            model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            model.VatNumberStatusNote = ((VatNumberStatus)customer.GetAttribute<int>(SystemCustomerAttributeNames.VatNumberStatusId))
                .GetLocalizedEnum(_localizationService, _workContext);
            model.GenderEnabled = _customerSettings.GenderEnabled;
            model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
            model.DateOfBirthRequired = _customerSettings.DateOfBirthRequired;
            model.CompanyEnabled = _customerSettings.CompanyEnabled;
            model.CompanyRequired = _customerSettings.CompanyRequired;
            model.StreetAddressEnabled = _customerSettings.StreetAddressEnabled;
            model.StreetAddressRequired = _customerSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = _customerSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = _customerSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = _customerSettings.ZipPostalCodeRequired;
            model.CityEnabled = _customerSettings.CityEnabled;
            model.CityRequired = _customerSettings.CityRequired;
            model.CountryEnabled = _customerSettings.CountryEnabled;
            model.CountryRequired = _customerSettings.CountryRequired;
            model.StateProvinceEnabled = _customerSettings.StateProvinceEnabled;
            model.StateProvinceRequired = _customerSettings.StateProvinceRequired;
            model.PhoneEnabled = _customerSettings.PhoneEnabled;
            model.PhoneRequired = _customerSettings.PhoneRequired;
            model.FaxEnabled = _customerSettings.FaxEnabled;
            model.FaxRequired = _customerSettings.FaxRequired;
            model.NewsletterEnabled = _customerSettings.NewsletterEnabled;
            model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            model.AllowUsersToChangeUsernames = _customerSettings.AllowUsersToChangeUsernames;
            model.CheckUsernameAvailabilityEnabled = _customerSettings.CheckUsernameAvailabilityEnabled;
            model.SignatureEnabled = _forumSettings.ForumsEnabled && _forumSettings.SignaturesEnabled;
            model.PictureUrl = _pictureService.GetPictureUrl(customer.PictureId, 200);



            //external authentication
            //model.NumberOfExternalAuthenticationProviders = _openAuthenticationService
            //    .LoadActiveExternalAuthenticationMethods(_storeContext.CurrentStore.Id)
            //    .Count;
            //foreach (var ear in _openAuthenticationService.GetExternalIdentifiersFor(customer))
            //{
            //    var authMethod = _openAuthenticationService.LoadExternalAuthenticationMethodBySystemName(ear.ProviderSystemName);
            //    if (authMethod == null || !authMethod.IsMethodActive(_externalAuthenticationSettings))
            //        continue;

            //    model.AssociatedExternalAuthRecords.Add(new CustomerInfoResponseModel.AssociatedExternalAuthModel
            //    {
            //        Id = ear.Id,
            //        Email = ear.Email,
            //        ExternalIdentifier = ear.ExternalIdentifier,
            //        AuthMethodName = authMethod.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id)
            //    });
            //}

            //custom customer attributes
            var customAttributes = PrepareCustomCustomerAttributes(customer, overrideCustomCustomerAttributesXml);
            foreach (var item in customAttributes)
            {
                model.CustomerAttributes.Add(item);
            }
        }


        protected virtual IList<CustomerAttributeModel> PrepareCustomCustomerAttributes(Customer customer,
            string overrideAttributesXml = "")
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var result = new List<CustomerAttributeModel>();

            var customerAttributes = _customerAttributeService.GetAllCustomerAttributes();
            foreach (var attribute in customerAttributes)
            {
                var attributeModel = new CustomerAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.GetLocalized(x => x.Name),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _customerAttributeService.GetCustomerAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var valueModel = new CustomerAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.GetLocalized(x => x.Name),
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(valueModel);
                    }
                }

                //set already selected attributes
                var selectedAttributesXml = !String.IsNullOrEmpty(overrideAttributesXml) ?
                    overrideAttributesXml :
                    customer.GetAttribute<string>(SystemCustomerAttributeNames.CustomCustomerAttributes, _genericAttributeService);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                        {
                            if (!String.IsNullOrEmpty(selectedAttributesXml))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = _customerAttributeParser.ParseCustomerAttributeValues(selectedAttributesXml);
                                foreach (var attributeValue in selectedValues)
                                    foreach (var item in attributeModel.Values)
                                        if (attributeValue.Id == item.Id)
                                            item.IsPreSelected = true;
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //do nothing
                            //values are already pre-set
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            if (!String.IsNullOrEmpty(selectedAttributesXml))
                            {
                                var enteredText = _customerAttributeParser.ParseValues(selectedAttributesXml, attribute.Id);
                                if (enteredText.Count > 0)
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    default:
                    //not supported attribute control types
                    break;
                }

                result.Add(attributeModel);
            }


            return result;
        }

        protected virtual string ParseCustomCustomerAttributes(NameValueCollection form)
        {
            if (form == null)
                throw new ArgumentNullException("form");

            string attributesXml = "";
            var attributes = _customerAttributeService.GetAllCustomerAttributes();
            foreach (var attribute in attributes)
            {
                string controlId = string.Format("customer_attribute_{0}", attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _customerAttributeService.GetCustomerAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                string enteredText = ctrlAttributes.Trim();
                                attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.FileUpload:
                    //not supported customer attributes
                    default:
                    break;
                }
            }

            return attributesXml;
        }

        public void AddressBind(Address model, Address destination, bool trimFields = true)
        {


            if (trimFields)
            {
                if (model.FirstName != null)
                    model.FirstName = model.FirstName.Trim();
                if (model.LastName != null)
                    model.LastName = model.LastName.Trim();
                if (model.Email != null)
                    model.Email = model.Email.Trim();
                if (model.Company != null)
                    model.Company = model.Company.Trim();
                if (model.City != null)
                    model.City = model.City.Trim();
                if (model.Address1 != null)
                    model.Address1 = model.Address1.Trim();
                if (model.Address2 != null)
                    model.Address2 = model.Address2.Trim();
                if (model.ZipPostalCode != null)
                    model.ZipPostalCode = model.ZipPostalCode.Trim();
                if (model.PhoneNumber != null)
                    model.PhoneNumber = model.PhoneNumber.Trim();
                if (model.FaxNumber != null)
                    model.FaxNumber = model.FaxNumber.Trim();
            }

            destination.FirstName = model.FirstName;
            destination.LastName = model.LastName;
            destination.Email = model.Email;
            destination.Company = model.Company;
            destination.CountryId = model.CountryId;
            destination.StateProvinceId = model.StateProvinceId;
            destination.City = model.City;
            destination.Address1 = model.Address1;
            destination.Address2 = model.Address2;
            destination.ZipPostalCode = model.ZipPostalCode;
            destination.PhoneNumber = model.PhoneNumber;
            destination.FaxNumber = model.FaxNumber;
        }

        #endregion

        #region Action Method

        #region login
        [System.Web.Http.Route("api/login")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult Login(LoginQueryModel model)
        {
            var customerModel = new LogInPostResponseModel();
            customerModel.StatusCode = (int)ErrorType.NotOk;
            ValidationExtension.LoginValidator(ModelState, model, _localizationService, _customerSettings);
            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }
                var loginResult = _customerRegistrationService.ValidateCustomer(_customerSettings.UsernamesEnabled ? model.Username : model.Email, model.Password);

                switch (loginResult)
                {
                    case CustomerLoginResults.Successful:
                        {
                            var customer = _customerSettings.UsernamesEnabled ? _customerService.GetCustomerByUsername(model.Username) : _customerService.GetCustomerByEmail(model.Email);
                            PrepareCustomerInfoModel(customerModel, customer);
                            customerModel.StatusCode = (int)ErrorType.Ok;
                            //migrate shopping cart
                            _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);
                            //activity log
                            _customerActivityService.InsertActivity("PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"), customer);
                            string deviceId = GetDeviceIdFromHeader();
                            var device = _deviceService.GetDeviceByDeviceToken(deviceId);
                            if (device != null)
                            {
                                device.CustomerId = customer.Id;
                                device.IsRegistered = customer.IsRegistered();
                                _deviceService.UpdateDevice(device);
                            }
                            break;

                        }
                    case CustomerLoginResults.CustomerNotExist:
                        {
                            var customer = _workContext.CurrentCustomer;

                            bool isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
                            var registrationRequest = new CustomerRegistrationRequest(customer,
                               model.Email,
                               _customerSettings.UsernamesEnabled ? model.Username : model.Email,
                               model.Password,
                               _customerSettings.DefaultPasswordFormat,
                               _storeContext.CurrentStore.Id,
                               isApproved);
                            var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
                            if (registrationResult.Success)
                            {
                                customer.CustomerGuid = Guid.NewGuid();
                                _customerService.UpdateCustomer(customer);
                            }
                            customerModel.StatusCode = (int)ErrorType.Ok;
                            customerModel.SuccessMessage = _localizationService.GetResource("Account.Register.Result.Standard");
                            PrepareCustomerInfoModel(customerModel, customer);
                        }
                        break;
                    case CustomerLoginResults.Deleted:

                    customerModel.ErrorList = new List<string>
                        {
                            _localizationService.GetResource("Account.Login.WrongCredentials.Deleted")
                        };
                    break;
                    case CustomerLoginResults.NotActive:

                    customerModel.ErrorList = new List<string>
                        {
                            _localizationService.GetResource("Account.Login.WrongCredentials.NotActive")
                        };
                    break;
                    case CustomerLoginResults.NotRegistered:

                    customerModel.ErrorList = new List<string>
                        {
                            _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered")
                        };
                    break;
                    case CustomerLoginResults.WrongPassword:
                    default:

                    customerModel.ErrorList = new List<string>
                        {
                            _localizationService.GetResource("Account.Login.WrongCredentials")
                        };
                    break;
                }
            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        customerModel.ErrorList.Add(error.ErrorMessage);
                    }
                }
            }
            //If we got this far, something failed, redisplay form

            return Ok(customerModel);
        }
        #endregion


        #region Register

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/customer/register")]
        public IHttpActionResult Register(RegisterQueryModel model)
        {
            var customer = _workContext.CurrentCustomer;
            var form = model.FormValue.ToNameValueCollection();
            var response = new RegisterResponseModel();
            //custom customer attributes
            var customerAttributesXml = ParseCustomCustomerAttributes(form);
            var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
            foreach (var error in customerAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            ValidationExtension.RegisterValidator(ModelState, model, _localizationService, _stateProvinceService, _customerSettings);
            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }

                bool isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
                var registrationRequest = new CustomerRegistrationRequest(customer,
                   model.Email,
                   _customerSettings.UsernamesEnabled ? model.Username : model.Email,
                   model.Password,
                   _customerSettings.DefaultPasswordFormat,
                   _storeContext.CurrentStore.Id,
                   isApproved);
                var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
                if (registrationResult.Success)
                {
                    customer.CustomerGuid = new Guid();
                    customer.NewReferCode = GenerateRandomReferCode(6);
                    _customerService.UpdateCustomer(customer);
                    //properties
                    if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    {
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.TimeZoneId, model.TimeZoneId);
                    }
                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.VatNumber, model.VatNumber);

                        string vatName;
                        string vatAddress;
                        var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out vatName, out vatAddress);
                        _genericAttributeService.SaveAttribute(customer,
                            SystemCustomerAttributeNames.VatNumberStatusId,
                            (int)vatNumberStatus);
                        //send VAT number admin notification
                        if (!String.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                            _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer, model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
                    }

                    //form fields
                    if (_customerSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Gender, model.Gender);
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
                    if (_customerSettings.DateOfBirthEnabled)
                    {
                        DateTime? dateOfBirth = model.ParseDateOfBirth();
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.DateOfBirth, dateOfBirth);
                    }
                    if (_customerSettings.CompanyEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Company, model.Company);
                    if (_customerSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress, model.StreetAddress);
                    if (_customerSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress2, model.StreetAddress2);
                    if (_customerSettings.ZipPostalCodeEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.ZipPostalCode, model.ZipPostalCode);
                    if (_customerSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.City, model.City);
                    if (_customerSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CountryId, model.CountryId);
                    if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StateProvinceId, model.StateProvinceId);
                    if (_customerSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Phone, model.Phone);
                    if (_customerSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Fax, model.Fax);

                    //newsletter
                    if (_customerSettings.NewsletterEnabled)
                    {
                        //save newsletter value
                        var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(model.Email, _storeContext.CurrentStore.Id);
                        if (newsletter != null)
                        {
                            if (model.Newsletter)
                            {
                                newsletter.Active = true;
                                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);
                            }
                            //else
                            //{
                            //When registering, not checking the newsletter check box should not take an existing email address off of the subscription list.
                            //_newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
                            //}
                        }
                        else
                        {
                            if (model.Newsletter)
                            {
                                _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = model.Email,
                                    Active = true,
                                    StoreId = _storeContext.CurrentStore.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                            }
                        }
                    }

                    //save customer attributes
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CustomCustomerAttributes, customerAttributesXml);

                    //login customer now
                    if (isApproved)
                        _authenticationService.SignIn(customer, true);

                    //associated with external account (if possible)
                    //TryAssociateAccountWithExternalAccount(customer);

                    //insert default address (if possible)
                    var defaultAddress = new Address
                    {
                        FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName),
                        LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName),
                        Email = customer.Email,
                        Company = customer.GetAttribute<string>(SystemCustomerAttributeNames.Company),
                        CountryId = customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId) > 0 ?
                            (int?)customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId) : null,
                        StateProvinceId = customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId) > 0 ?
                            (int?)customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId) : null,
                        City = customer.GetAttribute<string>(SystemCustomerAttributeNames.City),
                        Address1 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress),
                        Address2 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress2),
                        ZipPostalCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.ZipPostalCode),
                        PhoneNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone),
                        FaxNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Fax),
                        CreatedOnUtc = customer.CreatedOnUtc
                    };
                    if (this._addressService.IsAddressValid(defaultAddress))
                    {
                        //some validation
                        if (defaultAddress.CountryId == 0)
                            defaultAddress.CountryId = null;
                        if (defaultAddress.StateProvinceId == 0)
                            defaultAddress.StateProvinceId = null;
                        //set default address
                        customer.Addresses.Add(defaultAddress);
                        customer.BillingAddress = defaultAddress;
                        customer.ShippingAddress = defaultAddress;
                        _customerService.UpdateCustomer(customer);
                    }

                    //notifications
                    if (_customerSettings.NotifyNewCustomerRegistration)
                        _workflowMessageService.SendCustomerRegisteredNotificationMessage(customer, _localizationSettings.DefaultAdminLanguageId);

                    switch (_customerSettings.UserRegistrationType)
                    {
                        case UserRegistrationType.EmailValidation:
                            {
                                //email validation message
                                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AccountActivationToken, Guid.NewGuid().ToString());
                                _workflowMessageService.SendCustomerEmailValidationMessage(customer, _workContext.WorkingLanguage.Id);
                                response.SuccessMessage = _localizationService.GetResource("Account.Register.Result.EmailValidation");
                                break;
                                //result
                                //return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.EmailValidation });
                            }
                        case UserRegistrationType.AdminApproval:
                            {
                                response.SuccessMessage = _localizationService.GetResource("Account.Register.Result.AdminApproval");
                                break;
                                //return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.AdminApproval });
                            }
                        case UserRegistrationType.Standard:
                        default:
                            {

                                //send customer welcome message
                                _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);
                                response.SuccessMessage = _localizationService.GetResource("Account.Register.Result.Standard");
                                break;
                                //var redirectUrl = Url.RouteUrl("RegisterResult", new { resultId = (int)UserRegistrationType.Standard });
                                //if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                                //    redirectUrl = _webHelper.ModifyQueryString(redirectUrl, "returnurl=" + HttpUtility.UrlEncode(returnUrl), null);
                                //return Redirect(redirectUrl);
                            }
                    }
                }

                //errors
                foreach (var error in registrationResult.Errors)
                {
                    response.StatusCode = (int)ErrorType.NotOk;
                    response.ErrorList.Add(error);
                }

            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        response.ErrorList.Add(error.ErrorMessage);
                    }
                }
                response.StatusCode = (int)ErrorType.NotOk;
            }
            //If we got this far, something failed, redisplay form

            return Ok(response);
        }


        #endregion



        #endregion

        #region PriyoCoin


        [Route("api/customer/getPriyoCoinInfo")]
        [HttpPost]
        public IHttpActionResult GetPriyoCoinInfo()
        {
            var response = new GeneralResponseModel<List<CustomerPriyoCoinModel>>();
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["CustomerId"]))
            {
                int customerId = Convert.ToInt32(HttpContext.Current.Request["CustomerId"]);
                var priyoCoins = _customerPriyoCoinService.GetCustomerPriyoCoinByCustomerId(customerId);
                var priyoCoinList = new List<CustomerPriyoCoinModel>();
                foreach (var customerPriyoCoinModel in priyoCoins)
                {
                    var priyoCoinVm = new CustomerPriyoCoinModel();

                    priyoCoinVm.Id = customerPriyoCoinModel.Id;
                    priyoCoinVm.CustomerId = customerPriyoCoinModel.CustomerId;
                    priyoCoinVm.PointNumber = customerPriyoCoinModel.PointNumber;
                    priyoCoinVm.PointSourceDescription = customerPriyoCoinModel.PointSourceDescription;
                    priyoCoinVm.PointType = customerPriyoCoinModel.PointType;
                    priyoCoinVm.PointType = customerPriyoCoinModel.PointType;
                    priyoCoinVm.Date = customerPriyoCoinModel.Date;
                    if (customerPriyoCoinModel.Date != null)
                        priyoCoinVm.Date = _dateTimeHelper.ConvertToUserTime(customerPriyoCoinModel.Date.Value, DateTimeKind.Utc);
                    priyoCoinVm.CustomerEmail = customerPriyoCoinModel.Customer.Email;
                    priyoCoinList.Add(priyoCoinVm);
                }

                response.Data = priyoCoinList;
                return Ok(response);

            }
            else
            {
                response.StatusCode = 400;
                return Ok(response);
            }
        }


        [HttpPost]
        [Route("api/customer/convertPriyoCoinIntoWallet")]
        public IHttpActionResult ConvertPriyoCoinIntoWallet()
        {
            var result = new GeneralResponseModel<string>();
            var contactNo = "";
            var customerName = "";
            decimal amount = new decimal(0);
            var amountType = "CREDIT";
            string amountDescription = null;
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["phone"]))
            {
                contactNo = HttpContext.Current.Request["phone"];
            }
            else
            {
                result.Data = "Please provide Phone Number";
                result.StatusCode = 400;

                return Ok(result);
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["customerName"]))
            {
                customerName = HttpContext.Current.Request["customerName"];
            }
            else
            {
                result.Data = "Please provide Customer Name";
                result.StatusCode = 400;

                return Ok(result);
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["amount"]))
            {
                amount = Convert.ToDecimal(HttpContext.Current.Request["amount"]);
            }
            else
            {
                result.Data = "Please provide Amount";
                result.StatusCode = 400;

                return Ok(result);
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["amountDescription"]))
            {
                amountDescription = HttpContext.Current.Request["amountDescription"];
            }


            if (!string.IsNullOrEmpty(HttpContext.Current.Request["amountType"]))
            {
                amountType = HttpContext.Current.Request["amountType"];
            }


            _customerService.InsertCustomerLedgerDetails(string.Format(@"[Customer].[spSaveCustomerLedgerInformation]
                                                                                            @ContactNo = {0},
                                                                                            @CustomerName = '{1}',
                                                                                            @Amount = {2},
                                                                                            @AmountType = {3},
                                                                                            @AmountDescription = '{4}',
                                                                                            @AmountSource = 1,
                                                                                            @EntryBy = 1",
                contactNo, customerName, amount, amountType, amountDescription));

            result.Data = "Added Successfully";
            result.StatusCode = 200;

            return Ok(result);
        }

        #endregion

        #region My account / Info

        #region ReferCode


        [Route("api/customer/getReferCode")]
        [HttpPost]
        public IHttpActionResult GetReferCode()
        {

            var response = new GeneralResponseModel<ReferCodeModel>();

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["CustomerId"]))
            {
                int customerId = Convert.ToInt32(HttpContext.Current.Request["CustomerId"]);
                if (customerId == 0)
                {
                    response.StatusCode = 400;
                    return Ok(response);
                }

                var customerObj = _customerService.GetCustomerById(customerId);
                if (customerObj != null)
                {
                    response.Data = new ReferCodeModel();
                    response.Data.ReferCode = customerObj.NewReferCode;
                    response.Data.Point = customerObj.ReferCodeValue;
                }
                else
                {
                    response.StatusCode = 400;
                    response.SuccessMessage = "No Customer found by this Id = " + customerId;
                }

            }
            return Ok(response);
        }


        [Route("api/customer/useReferCode")]
        [HttpPost]
        public IHttpActionResult UseReferCode()
        {
            var response = new GeneralResponseModel<string>();
            int customerId = 0;
            string referCode = null;
            var pointType = "CREDIT";
            var pointDescription = "By Refer Code";
            var pointNumber = 5;

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["CustomerId"]))
            {
                customerId = Convert.ToInt32(HttpContext.Current.Request["CustomerId"]);
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["ReferCode"]))
            {
                referCode = HttpContext.Current.Request["ReferCode"];
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["PointType"]))
            {
                pointType = HttpContext.Current.Request["PointType"];
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["PointSourceDescription"]))
            {
                pointDescription = HttpContext.Current.Request["PointSourceDescription"];
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["PointNumber"]))
            {
                pointNumber = Convert.ToInt32(HttpContext.Current.Request["PointNumber"]);
            }

            if (customerId == 0)
            {
                response.StatusCode = 400;
                response.SuccessMessage = "No Customer Selected";
                return Ok(response);
            }

            if (string.IsNullOrEmpty(referCode))
            {
                response.StatusCode = 400;
                response.SuccessMessage = "Refer code is empty";
                return Ok(response);
            }


            var referringCustomer = _customerService.GetCustomerByReferCode(referCode);
            if (referringCustomer.Id == 0)
            {
                response.StatusCode = 400;
                response.SuccessMessage = "Invalid Refer Code";
                return Ok(response);
            }

            //if (referringCustomer.IsReferCodeUsed.HasValue && referringCustomer.IsReferCodeUsed.Value)
            //{
            //    response.StatusCode = 400;
            //    response.SuccessMessage = "Code is already referred";
            //    return Ok(response);
            //}

            var referredCustomer = _customerService.GetCustomerById(customerId);
            if (referredCustomer.Id == 0)
            {
                response.StatusCode = 400;
                response.SuccessMessage = "Invalid Customer";
                return Ok(response);
            }

            referringCustomer.IsReferCodeUsed = true;
            _customerService.UpdateCustomer(referringCustomer);

            referredCustomer.UsedReferCode = referCode;
            _customerService.UpdateCustomer(referredCustomer);
            pointNumber = referringCustomer.ReferCodeValue ?? 10;


            var referringCustomerPriyoCoin = new CustomerPriyoCoin
            {
                CustomerId = referringCustomer.Id,
                PointNumber = pointNumber,
                PointType = pointType,
                PointSourceDescription = pointDescription,
                Date = DateTime.UtcNow
            };
            _customerPriyoCoinService.InsertCustomerPriyoCoin(referringCustomerPriyoCoin);


            var referredCustomerPriyoCoin = new CustomerPriyoCoin
            {
                CustomerId = referredCustomer.Id,
                PointNumber = pointNumber,
                PointType = pointType,
                PointSourceDescription = pointDescription,
                Date = DateTime.UtcNow
            };
            _customerPriyoCoinService.InsertCustomerPriyoCoin(referredCustomerPriyoCoin);

            response.StatusCode = 200;
            response.SuccessMessage = "Referring done";
            return Ok(response);
        }


        #endregion


        [System.Web.Http.Route("api/myaccount/menu")]
        public IHttpActionResult CustomerNavigation(int selectedTabId = 0)
        {
            var model = new CustomerNavigationModel();
            model.HideAvatar = !_customerSettings.AllowCustomersToUploadAvatars;
            model.HideRewardPoints = !_rewardPointsSettings.Enabled;
            model.HideForumSubscriptions = !_forumSettings.ForumsEnabled || !_forumSettings.AllowCustomersToManageSubscriptions;
            model.HideReturnRequests = !_orderSettings.ReturnRequestsEnabled ||
                _returnRequestService.SearchReturnRequests(_storeContext.CurrentStore.Id, _workContext.CurrentCustomer.Id, 0, null, 0, 1).Count == 0;
            model.HideDownloadableProducts = _customerSettings.HideDownloadableProductsTab;
            model.HideBackInStockSubscriptions = _customerSettings.HideBackInStockSubscriptionsTab;

            model.SelectedTab = (CustomerNavigationEnum)selectedTabId;

            return Ok(model);
        }

        [System.Web.Http.Route("api/customer/info")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult Info()
        {
            var model = new CustomerInfoResponseModel();
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                model.StatusCode = (int)ErrorType.NotOk;
            }
            else
            {
                var customer = _workContext.CurrentCustomer;

                //var model = new CustomerInfoResponseModel();
                PrepareCustomerInfoModel(model, customer, false);
            }


            return Ok(model);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/customer/info")]
        public IHttpActionResult Info(CustomerInfoQueryModel queryModel)
        {
            var model = new CustomerInfoResponseModel();
            model.ErrorList = new List<string>();
            if (!_workContext.CurrentCustomer.IsRegistered())
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            var customer = _workContext.CurrentCustomer;

            var form = queryModel.FormValues.ToNameValueCollection();

            //custom customer attributes
            var customerAttributesXml = ParseCustomCustomerAttributes(form);
            var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
            foreach (var error in customerAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }
            ValidationExtension.CustomerInfoValidator(ModelState, queryModel, _localizationService, _stateProvinceService, _customerSettings);
            try
            {
                if (ModelState.IsValid)
                {

                    model = queryModel.MapTo<CustomerInfoQueryModel, CustomerInfoResponseModel>();
                    //username 
                    if (_customerSettings.UsernamesEnabled && this._customerSettings.AllowUsersToChangeUsernames)
                    {
                        if (
                            !customer.Username.Equals(model.Username.Trim(), StringComparison.InvariantCultureIgnoreCase))
                        {
                            //change username
                            _customerRegistrationService.SetUsername(customer, model.Username.Trim());
                            //re-authenticate
                            _authenticationService.SignIn(customer, true);
                        }
                    }
                    //email
                    if (!customer.Email.Equals(model.Email.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        //change email
                        _customerRegistrationService.SetEmail(customer, model.Email.Trim());
                        //re-authenticate (if usernames are disabled)
                        if (!_customerSettings.UsernamesEnabled)
                        {
                            _authenticationService.SignIn(customer, true);
                        }
                    }

                    //properties
                    //if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    //{
                    //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.TimeZoneId, model.TimeZoneId);
                    //}
                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        var prevVatNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.VatNumber);

                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.VatNumber,
                            model.VatNumber);
                        if (prevVatNumber != model.VatNumber)
                        {
                            string vatName;
                            string vatAddress;
                            var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out vatName,
                                out vatAddress);
                            _genericAttributeService.SaveAttribute(customer,
                                SystemCustomerAttributeNames.VatNumberStatusId,
                                (int)vatNumberStatus);
                            //send VAT number admin notification
                            if (!String.IsNullOrEmpty(model.VatNumber) &&
                                _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                                _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer,
                                    model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
                        }
                    }

                    //form fields
                    if (_customerSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Gender,
                            model.Gender);
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName,
                        model.FirstName);
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastName,
                        model.LastName);
                    if (_customerSettings.DateOfBirthEnabled)
                    {
                        DateTime? dateOfBirth = model.ParseDateOfBirth();
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.DateOfBirth,
                            dateOfBirth);
                    }
                    if (_customerSettings.CompanyEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Company,
                            model.Company);
                    if (_customerSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress,
                            model.StreetAddress);
                    if (_customerSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress2,
                            model.StreetAddress2);
                    if (_customerSettings.ZipPostalCodeEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.ZipPostalCode,
                            model.ZipPostalCode);
                    if (_customerSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.City, model.City);
                    if (_customerSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CountryId,
                            model.CountryId);
                    if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StateProvinceId,
                            model.StateProvinceId);
                    if (_customerSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Phone, model.Phone);
                    if (_customerSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Fax, model.Fax);

                    //newsletter
                    if (_customerSettings.NewsletterEnabled)
                    {
                        //save newsletter value
                        var newsletter =
                            _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email,
                                _storeContext.CurrentStore.Id);
                        if (newsletter != null)
                        {
                            if (model.Newsletter)
                            {
                                newsletter.Active = true;
                                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);
                            }
                            else
                                _newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
                        }
                        else
                        {
                            if (model.Newsletter)
                            {
                                _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = customer.Email,
                                    Active = true,
                                    StoreId = _storeContext.CurrentStore.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                            }
                        }
                    }

                    if (_forumSettings.ForumsEnabled && _forumSettings.SignaturesEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Signature,
                            model.Signature);

                    //save customer attributes
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.CustomCustomerAttributes, customerAttributesXml);

                    model.RewardPoint = _rewardPointService.GetRewardPointsBalance(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);

                    return Ok(model);
                }
                else
                {
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            model.ErrorList.Add(error.ErrorMessage);
                        }
                    }
                    model.StatusCode = (int)ErrorType.NotOk;
                    return Ok(model);
                }
            }
            catch (Exception exc)
            {
                // ModelState.AddModelError("", exc.Message);
                model.StatusCode = (int)ErrorType.NotOk;
                model.ErrorList.Add(exc.Message);
            }


            //If we got this far, something failed, redisplay form
            PrepareCustomerInfoModel(model, customer, true, customerAttributesXml);
            return Ok(model);
        }

        public IHttpActionResult RemoveExternalAssociation(int id)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            //ensure it's our record
            var ear = _openAuthenticationService.GetExternalIdentifiersFor(_workContext.CurrentCustomer)
                .FirstOrDefault(x => x.Id == id);


            if (!_workContext.CurrentCustomer.IsRegistered())
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            var customer = _workContext.CurrentCustomer;

            var model = new CustomerInfoResponseModel();
            PrepareCustomerInfoModel(model, customer, false);

            if (ear == null)
                return Ok(customer);

            _openAuthenticationService.DeletExternalAuthenticationRecord(ear);

            return Ok(customer);
        }

        #endregion

        //#region check

        #region My account / Addresses

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/customer/addresses")]
        public IHttpActionResult Addresses()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            var customer = _workContext.CurrentCustomer;

            var model = new ExistingAddressCommonResponseModel();
            var addresses = customer.Addresses
                //enabled for the current store
                .Where(a => a.Country == null || _storeMappingService.Authorize(a.Country))
                .ToList();
            foreach (var address in addresses)
            {
                var addressModel = new AddressModel();
                addressModel.PrepareModel(
                    address: address,
                    excludeProperties: false,
                    addressSettings: _addressSettings,
                    addressAttributeFormatter: _addressAttributeFormatter
                   );

                model.ExistingAddresses.Add(addressModel);
            }
            return Ok(model);
        }

        [System.Web.Http.Route("api/customer/address/remove/{addressId}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult AddressDelete(int addressId)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            var model = new GeneralResponseModel<bool>();

            var customer = _workContext.CurrentCustomer;

            //find address (ensure that it belongs to the current customer)
            var address = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address != null)
            {
                customer.RemoveAddress(address);
                _customerService.UpdateCustomer(customer);
                //now delete the address record
                _addressService.DeleteAddress(address);
                model.Data = true;
            }
            else
            {
                model.StatusCode = (int)ErrorType.NotOk;
                model.Data = false;
            }
            return Ok(model);
        }

        [System.Web.Http.Route("api/customer/address/add")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult AddressAdd()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            var model = new AddAdressCommonResponseModel();
            model.Address.PrepareModel(
                address: null,
                excludeProperties: false,
                addressSettings: _addressSettings,
                localizationService: _localizationService,
                stateProvinceService: _stateProvinceService,
                addressAttributeService: _addressAttributeService,
                addressAttributeParser: _addressAttributeParser,
                loadCountries: () => _countryService.GetAllCountries());

            return Ok(model);
        }

        [System.Web.Http.Route("api/customer/address/add")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult AddressAdd(List<KeyValueApi> formValues)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            var customer = _workContext.CurrentCustomer;

            var form = formValues.ToNameValueCollection();

            var responseModel = new GeneralResponseModel<bool>();
            responseModel.ErrorList = new List<string>();

            //custom address attributes
            var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            string prefix = HelperExtension.GetEnumDescription((AddressType)3);
            Address address = form.AddressFromToModel(prefix);

            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }
            ValidationExtension.AddressValidator(ModelState, address, _localizationService, _addressSettings, _stateProvinceService);
            if (ModelState.IsValid)
            {

                address.CustomAttributes = customAttributes;
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                customer.Addresses.Add(address);
                _customerService.UpdateCustomer(customer);

                responseModel.Data = true;
                return Ok(responseModel);
            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        responseModel.ErrorList.Add(error.ErrorMessage);
                    }
                }
                responseModel.StatusCode = (int)ErrorType.NotOk;
            }
            //If we got this far, something failed, redisplay form
            return Ok(responseModel);
        }

        [System.Web.Http.Route("api/customer/address/edit/{addressId}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult AddressEdit(int addressId)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            var customer = _workContext.CurrentCustomer;
            var model = new AddAdressCommonResponseModel();
            //find address (ensure that it belongs to the current customer)
            var address = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
            {
                //address is not found
                model.StatusCode = (int)ErrorType.NotOk;
                model.ErrorList.Add("Not Found");

            }


            model.Address.PrepareModel(address: address,
                excludeProperties: false,
                addressSettings: _addressSettings,
                localizationService: _localizationService,
                stateProvinceService: _stateProvinceService,
                addressAttributeService: _addressAttributeService,
                addressAttributeParser: _addressAttributeParser,
                loadCountries: () => _countryService.GetAllCountries());

            return Ok(model);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/customer/address/edit/{addressId}")]
        public IHttpActionResult AddressEdit(int addressId, List<KeyValueApi> formValues)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            var customer = _workContext.CurrentCustomer;

            var form = formValues.ToNameValueCollection();

            //find address (ensure that it belongs to the current customer)
            var addressFirst = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
            var responseModel = new GeneralResponseModel<bool>();

            if (addressFirst == null)
                //address is not found
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            string prefix = HelperExtension.GetEnumDescription((AddressType)3);
            Address address = form.AddressFromToModel(prefix);

            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }
            ValidationExtension.AddressValidator(ModelState, address, _localizationService, _addressSettings, _stateProvinceService);


            if (ModelState.IsValid)
            {
                AddressBind(address, addressFirst);
                addressFirst.CustomAttributes = customAttributes;
                _addressService.UpdateAddress(addressFirst);
                responseModel.Data = true;
                return Ok(responseModel);
            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        responseModel.ErrorList.Add(error.ErrorMessage);
                    }
                }
                responseModel.StatusCode = (int)ErrorType.NotOk;
            }
            //If we got this far, something failed, redisplay form


            return Ok(responseModel);
            //If we got this far, something failed, redisplay form

        }

        #endregion

        #region My account / Wallet

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/customer/wallet")]
        public IHttpActionResult Wallet()
        {
            var model = new CustomerLedgerViewModel();
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["phone"]))
            {
                var phone = Convert.ToInt64(HttpContext.Current.Request["phone"]);
                var ledgerDetail = _customerLedgerDetailService.GetCustomerLedgerDetailByContactNo(phone);
                var customerLedgerMaster = _customerLedgerMasterService.GetCustomerLedgerMasterByContactNo(phone);

                model.CustomerLedgerMaster = customerLedgerMaster;

                if (ledgerDetail.Any())
                {
                    foreach (var customerLedgerDetail in ledgerDetail)
                    {
                        var cLd = new CustomerLedgerDetailViewModel();

                        cLd.Id = customerLedgerDetail.Id;
                        cLd.ContactNo = customerLedgerDetail.ContactNo;
                        cLd.LedgerMasterID = customerLedgerDetail.LedgerMasterID;
                        cLd.AmountDescription = customerLedgerDetail.AmountDescription;
                        cLd.Amount = customerLedgerDetail.Amount;
                        cLd.AmountType = customerLedgerDetail.AmountType;
                        cLd.AmountSource = customerLedgerDetail.AmountSource;
                        cLd.LastAddedDate = customerLedgerDetail.LastAddedDate;
                        model.CustomerLedgerDetail.Add(cLd);
                    }
                }
            }


            var result = new GeneralResponseModel<CustomerLedgerViewModel>()
            {
                Data = model
            };

            if (model.CustomerLedgerDetail == null || model.CustomerLedgerDetail.Count == 0)
            {
                result.StatusCode = 400;
                result.SuccessMessage = "No Balance";
            }

            return Ok(result);

        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/customer/createWallet")]
        public IHttpActionResult CreateWallet()
        {
            var result = new GeneralResponseModel<string>();
            var contactNumber = "";
            var customerId = 0;
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["phone"]))
            {
                contactNumber = HttpContext.Current.Request["phone"];
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["customerId"]))
            {
                customerId = Convert.ToInt32(HttpContext.Current.Request["customerId"]);
            }

            var modelW = new WalletAccountInformationTemp();
            bool canConvert = long.TryParse(contactNumber, out long number1);
            if (canConvert == false)
            {
                result.Data = "Please enter valid contact number.";
                result.StatusCode = 400;
                return Ok(result);
            }
            else if (contactNumber.Length < 10 || contactNumber.Length > 11)
            {
                result.Data = "Please enter valid contact number.";
                result.StatusCode = 400;
                return Ok(result);
            }
            else
            {
                modelW.ContactNo = Convert.ToInt64(contactNumber);
                modelW.CustomerID = customerId;

                WalletAccountInformation CW = _customerLedgerMasterService.GetCustomerWalletAccountByContactNo(modelW.ContactNo);

                if (CW == null)
                {
                    _customerLedgerMasterService.InsertWalletAccountInformation(modelW);
                    result.Data = "OTP Created";
                    result.StatusCode = 200;
                    return Ok(result);
                }
                else
                {
                    result.Data = "Contact number already exists. Please enter another valid contact number.";
                    result.StatusCode = 400;
                    return Ok(result);
                }


                
            }
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/customer/checkWalletOTP")]
        public IHttpActionResult CheckWalletOTP()
        {
            var result = new GeneralResponseModel<string>();
            var otp = "";
            var customerId = 0;
            var customerFullName = "";
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["otp"]))
            {
                otp = HttpContext.Current.Request["otp"];
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["customerId"]))
            {
                customerId = Convert.ToInt32(HttpContext.Current.Request["customerId"]);
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["customerFullName"]))
            {
                customerFullName = HttpContext.Current.Request["customerFullName"];
            }

            var modelW = new WalletAccountInformationTemp();

            bool canConvert = int.TryParse(otp, out int number1);
            if (!canConvert)
            {
                result.Data = "Please enter valid OTP.";
                result.StatusCode = 400;
                return Ok(result);
            }
            else if (otp.Length < 5 || otp.Length > 10)
            {
                result.Data = "Please enter valid OTP.";
                result.StatusCode = 400;
                return Ok(result);
            }
            else
            {
                modelW.OTP = Convert.ToInt32(otp);
                modelW.CustomerID = customerId;

                WalletAccountInformationTemp CW = _customerLedgerMasterService.GetCustomerWalletAccount(modelW.CustomerID, modelW.OTP);

                if (CW != null)
                {
                    DateTime date1 = CW.OTPEntryDate;
                    DateTime date2 = DateTime.Now;
                    TimeSpan interval = date2 - date1;

                    if (interval.TotalSeconds < CW.OTPExpireTime * 60)
                    {
                        _customerLedgerMasterService.CheckWalletOTP(modelW, customerFullName);
                        result.Data = "OTP Matched";
                        result.StatusCode = 200;
                        return Ok(result);
                    }
                    else
                    {
                        result.Data = "OTP expired";
                        result.StatusCode = 400;
                        return Ok(result);
                    }
                }
                else
                {
                    result.Data = "Valid OTP not found...";
                    result.StatusCode = 400;
                    return Ok(result);
                }
            }

        }

        


        [HttpPost]
        [Route("api/customer/commissionInfo")]
        public IHttpActionResult CommissionInfo()
        {
            var result = new GeneralResponseModel<CustomerCommission>();
            int customerId = 0;
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["customerId"]))
            {
                customerId = Convert.ToInt32(HttpContext.Current.Request["customerId"]);

            }

            try
            {
                var commission = _customerService.InsertCustomerCommissionInfo(string.Format(@"[Customer].[spGetCustomerCommissionMasterInformation]   @CustomerID = {0}", customerId));
                commission.CustomerId = customerId;
                result.Data = commission;
                result.StatusCode = 200;
            }
            catch (Exception e)
            {
                result.StatusCode = 400;
                result.SuccessMessage = e.Message;
                result.ErrorList.Add(e.Message);
            }

            return Ok(result);
        }



        #endregion

        //#region My account / Downloadable products

        //public IHttpActionResult DownloadableProducts()
        //{
        //    if (!_workContext.CurrentCustomer.IsRegistered())
        //        throw new HttpResponseException(HttpStatusCode.Unauthorized);

        //    var customer = _workContext.CurrentCustomer;

        //    var model = new CustomerDownloadableProductsModel();
        //    var items = _orderService.GetAllOrderItems(null, customer.Id, null, null,
        //        null, null, null, true);
        //    foreach (var item in items)
        //    {
        //        var itemModel = new CustomerDownloadableProductsModel.DownloadableProductsModel
        //        {
        //            OrderItemGuid = item.OrderItemGuid,
        //            OrderId = item.OrderId,
        //            CreatedOn = _dateTimeHelper.ConvertToUserTime(item.Order.CreatedOnUtc, DateTimeKind.Utc),
        //            ProductName = item.Product.GetLocalized(x => x.Name),
        //            ProductSeName = item.Product.GetSeName(),
        //            ProductAttributes = item.AttributeDescription,
        //            ProductId = item.ProductId
        //        };
        //        model.Items.Add(itemModel);

        //        if (_downloadService.IsDownloadAllowed(item))
        //            itemModel.DownloadId = item.Product.DownloadId;

        //        if (_downloadService.IsLicenseDownloadAllowed(item))
        //            itemModel.LicenseId = item.LicenseDownloadId.HasValue ? item.LicenseDownloadId.Value : 0;
        //    }

        //    return Ok(model);
        //}

        //public IHttpActionResult UserAgreement(Guid orderItemId)
        //{
        //    var orderItem = _orderService.GetOrderItemByGuid(orderItemId);
        //    if (orderItem == null)
        //        throw new HttpResponseException(HttpStatusCode.NotFound);

        //    var product = orderItem.Product;
        //    if (product == null || !product.HasUserAgreement)
        //        throw new HttpResponseException(HttpStatusCode.NotAcceptable);

        //    var model = new UserAgreementModel();
        //    model.UserAgreementText = product.UserAgreementText;
        //    model.OrderItemGuid = orderItemId;

        //    return Ok(model);
        //}

        //#endregion

        #region My account / Change password

        //public IHttpActionResult ChangePassword()
        //{
        //    if (!_workContext.CurrentCustomer.IsRegistered())
        //        throw new HttpResponseException(HttpStatusCode.Unauthorized);

        //    var model = new ChangePasswordModel();
        //    return Ok(model);
        //}

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api /customer/changepass")]
        public IHttpActionResult ChangePassword(ChangePasswordQueryModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            var customer = _workContext.CurrentCustomer;
            var response = new GeneralResponseModel<string>();
            if (ModelState.IsValid)
            {
                var changePasswordRequest = new ChangePasswordRequest(customer.Email,
                    true, _customerSettings.DefaultPasswordFormat, model.NewPassword, model.OldPassword);
                var changePasswordResult = _customerRegistrationService.ChangePassword(changePasswordRequest);
                if (changePasswordResult.Success)
                {
                    response.Data = _localizationService.GetResource("Account.ChangePassword.Success");
                    return Ok(response);
                }

                //errors
                foreach (var error in changePasswordResult.Errors)
                {
                    response.StatusCode = (int)ErrorType.NotOk;
                    response.ErrorList.Add(error);
                }

            }


            //If we got this far, something failed, redisplay form
            return Ok(model);
        }

        #endregion

        //#region My account / Avatar

        //public IHttpActionResult Avatar()
        //{
        //    if (!_workContext.CurrentCustomer.IsRegistered())
        //        throw new HttpResponseException(HttpStatusCode.Unauthorized);

        //    if (!_customerSettings.AllowCustomersToUploadAvatars)
        //        throw new HttpResponseException(HttpStatusCode.Unauthorized);

        //    var customer = _workContext.CurrentCustomer;

        //    var model = new CustomerAvatarModel();
        //    model.AvatarUrl = _pictureService.GetPictureUrl(
        //        customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
        //        _mediaSettings.AvatarPictureSize,
        //        false);
        //    return Ok(model);
        //}

        //[System.Web.Http.HttpPost, System.Web.Http.ActionName("Avatar")]
        //public IHttpActionResult UploadAvatar(CustomerAvatarModel model, HttpPostedFileBase uploadedFile)
        //{
        //    if (!_workContext.CurrentCustomer.IsRegistered())
        //        throw new HttpResponseException(HttpStatusCode.Unauthorized);

        //    if (!_customerSettings.AllowCustomersToUploadAvatars)
        //        throw new HttpResponseException(HttpStatusCode.Unauthorized);

        //    var customer = _workContext.CurrentCustomer;

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var customerAvatar = _pictureService.GetPictureById(customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId));
        //            if ((uploadedFile != null) && (!String.IsNullOrEmpty(uploadedFile.FileName)))
        //            {
        //                int avatarMaxSize = _customerSettings.AvatarMaximumSizeBytes;
        //                if (uploadedFile.ContentLength > avatarMaxSize)
        //                    throw new NopException(string.Format(_localizationService.GetResource("Account.Avatar.MaximumUploadedFileSize"), avatarMaxSize));

        //                byte[] customerPictureBinary = uploadedFile.GetPictureBits();
        //                if (customerAvatar != null)
        //                    customerAvatar = _pictureService.UpdatePicture(customerAvatar.Id, customerPictureBinary, uploadedFile.ContentType, null);
        //                else
        //                    customerAvatar = _pictureService.InsertPicture(customerPictureBinary, uploadedFile.ContentType, null);
        //            }

        //            int customerAvatarId = 0;
        //            if (customerAvatar != null)
        //                customerAvatarId = customerAvatar.Id;

        //            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AvatarPictureId, customerAvatarId);

        //            model.AvatarUrl = _pictureService.GetPictureUrl(
        //                customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
        //                _mediaSettings.AvatarPictureSize,
        //                false);
        //            return Ok(model);
        //        }
        //        catch (Exception exc)
        //        {
        //            ModelState.AddModelError("", exc.Message);
        //        }
        //    }


        //    //If we got this far, something failed, redisplay form
        //    model.AvatarUrl = _pictureService.GetPictureUrl(
        //        customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
        //        _mediaSettings.AvatarPictureSize,
        //        false);
        //    return Ok(model);
        //}

        //[System.Web.Http.HttpPost, System.Web.Http.ActionName("Avatar")]
        //public IHttpActionResult RemoveAvatar(CustomerAvatarModel model, HttpPostedFileBase uploadedFile)
        //{
        //    if (!_workContext.CurrentCustomer.IsRegistered())
        //        throw new HttpResponseException(HttpStatusCode.Unauthorized);

        //    if (!_customerSettings.AllowCustomersToUploadAvatars)
        //        throw new HttpResponseException(HttpStatusCode.Unauthorized);

        //    var customer = _workContext.CurrentCustomer;

        //    var customerAvatar = _pictureService.GetPictureById(customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId));
        //    if (customerAvatar != null)
        //        _pictureService.DeletePicture(customerAvatar);
        //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AvatarPictureId, 0);

        //    return Ok<string>("Uploaded Successfuly");
        //}

        //#endregion
        //#endregion 

        #region Downloadable Product
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/customer/downloadableproducts")]
        public IHttpActionResult DownloadableProducts()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            var customer = _workContext.CurrentCustomer;

            var model = new CustomerDownloadableProductsModel();
            //var items = _orderService.GetAllOrderItems(null, customer.Id, null, null,
            //    null, null, null, true);
            var items = _orderService.GetDownloadableOrderItems(customer.Id); // change 3.8
            foreach (var item in items)
            {
                var itemModel = new CustomerDownloadableProductsModel.DownloadableProductsModel
                {
                    OrderItemGuid = item.OrderItemGuid,
                    OrderId = item.OrderId,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(item.Order.CreatedOnUtc, DateTimeKind.Utc),
                    ProductName = item.Product.GetLocalized(x => x.Name),
                    ProductSeName = item.Product.GetSeName(),
                    ProductAttributes = item.AttributeDescription,
                    ProductId = item.ProductId
                };
                model.Items.Add(itemModel);

                if (_downloadService.IsDownloadAllowed(item))
                {
                    itemModel.IsDownloadable = true;

                    itemModel.DownloadUrl = _storeContext.CurrentStore.Url + "/download/getdownload/" + item.OrderItemGuid;

                }

                if (_downloadService.IsLicenseDownloadAllowed(item))
                {
                    itemModel.IsLicenseDownloadable = true;

                    itemModel.LicenseDownloadUrl = _storeContext.CurrentStore.Url + "/download/getlicense/" + item.OrderItemGuid;

                }
                // itemModel.LicenseId = item.LicenseDownloadId.HasValue ? item.LicenseDownloadId.Value : 0;

            }
            if (model.Items.Count() > 0)
            {
                model.StatusCode = (int)ErrorType.Ok;
            }
            else
            {
                model.StatusCode = (int)ErrorType.NotOk;
            }

            return Ok(model);
        }
        #endregion

        #region Language And Currency Save

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/SetLanguageAndCurrency")]
        public IHttpActionResult SetLanguageAndCurrency(CustomerLanguageAndCurrencyModel languageAndCurrencyModel)
        {
            var result = new CustomerLaguageAndCurrencyResponceModel();
            var languageId = languageAndCurrencyModel.LanguageId != 0 ? languageAndCurrencyModel.LanguageId : 0;
            var language = _languageService.GetLanguageById(languageId);
            var currencyId = languageAndCurrencyModel.CurrencyId != 0
                ? languageAndCurrencyModel.CurrencyId
                : language != null ? language.DefaultCurrencyId : 0;
            var currency = _currencyService.GetCurrencyById(currencyId);
            try
            {
                //_genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                //SystemCustomerAttributeNames.LanguageId,
                //languageId, _storeContext.CurrentStore.Id);
                ////reset cache
                //_cachedLanguage = null;

                if (language != null && language.Published && currency != null && currency.Published)
                {
                    _workContext.WorkingLanguage = language;
                    _workContext.WorkingCurrency = currency;
                    result.Success = true;
                    result.StatusCode = (int)ErrorType.Ok;
                    result.SuccessMessage = "SaveSuccesfully";
                }
                else if (language != null && language.Published)
                {
                    _workContext.WorkingLanguage = language;
                    result.Success = true;
                    result.StatusCode = (int)ErrorType.Ok;
                    result.SuccessMessage = "SaveSuccesfully";
                }
                else if (currency != null && currency.Published)
                {
                    _workContext.WorkingCurrency = currency;
                    result.Success = true;
                    result.StatusCode = (int)ErrorType.Ok;
                    result.SuccessMessage = "SaveSuccesfully";
                }
                else
                {
                    result.Success = false;
                    result.StatusCode = (int)ErrorType.NotOk;
                    result.SuccessMessage = "NotSuccesfully";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.SuccessMessage = "NotSuccesfully";
                result.StatusCode = (int)ErrorType.NotOk; ;
            }
            return Ok(result);
        }

        #endregion

        #region Picture

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/uploadimage")]
        public IHttpActionResult UploadImage()
        {
            Stream stream = null;
            var fileName = "";
            var contentType = "";
            if (string.IsNullOrEmpty(HttpContext.Current.Request["image"]))
            {
                // IE
                HttpPostedFile httpPostedFile = HttpContext.Current.Request.Files[0];
                if (httpPostedFile == null)
                    throw new ArgumentException("No file uploaded");
                stream = httpPostedFile.InputStream;
                fileName = Path.GetFileName(httpPostedFile.FileName);
                contentType = httpPostedFile.ContentType;
            }
            else
            {
                //Webkit, Mozilla
                stream = HttpContext.Current.Request.InputStream;
                fileName = HttpContext.Current.Request["image"];
            }

            var fileBinary = new byte[stream.Length];
            stream.Read(fileBinary, 0, fileBinary.Length);

            var fileExtension = Path.GetExtension(fileName);
            if (!String.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();
            //contentType is not always available 
            //that's why we manually update it here
            //http://www.sfsu.edu/training/mimetype.htm
            if (String.IsNullOrEmpty(contentType))
            {
                switch (fileExtension)
                {
                    case ".bmp":
                    contentType = MimeTypes.ImageBmp;
                    break;
                    case ".gif":
                    contentType = MimeTypes.ImageGif;
                    break;
                    case ".jpeg":
                    case ".jpg":
                    case ".jpe":
                    case ".jfif":
                    case ".pjpeg":
                    case ".pjp":
                    contentType = MimeTypes.ImageJpeg;
                    break;
                    case ".png":
                    contentType = MimeTypes.ImagePng;
                    break;
                    case ".tiff":
                    case ".tif":
                    contentType = MimeTypes.ImageTiff;
                    break;
                    default:
                    break;
                }
            }

            var picture = _pictureService.InsertPicture(fileBinary, contentType, null);
            var customer = _workContext.CurrentCustomer;
            customer.PictureId = picture.Id;
            _customerService.UpdateCustomer(customer);

            var response = new GeneralResponseModel<bool>();
            response.Data = true;

            return Ok(response);
        }



        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/CustomerImageUpload")]
        public IHttpActionResult UploadImageInFolder()
        {
            var response = new GeneralResponseModel<LogInPostResponseModel>();
            var customerModel = new LogInPostResponseModel();
            var imageName = "";
            try
            {
                var httpRequest = HttpContext.Current.Request;
                if (string.IsNullOrEmpty(HttpContext.Current.Request["image"]))
                {
                    var postedFile = httpRequest.Files[0];
                    imageName = postedFile.FileName;
                    var filePath = HttpContext.Current.Server.MapPath("~/Content/files/UserImage/" + postedFile.FileName);
                    postedFile.SaveAs(filePath);
                    response.StatusCode = 200;
                }
            }
            catch (Exception e)
            {
                response.StatusCode = 400;
                response.SuccessMessage = "Image Upload Failed";
            }

            if (response.StatusCode == 200)
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Request["CustomerId"]))
                {
                    int customerId = Convert.ToInt32(HttpContext.Current.Request["CustomerId"]);
                    var customer = _customerService.GetCustomerById(customerId);
                    if (customer != null)
                    {
                        customer.ImageUrl = imageName;
                        _customerService.UpdateCustomer(customer);
                    }
                    PrepareCustomerInfoModel(customerModel, customer);
                }
            }

            return Ok(customerModel);
        }

        #endregion
    }
}