using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Affiliates;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Framework.Security.Honeypot;
using Nop.Web.Models.Common;
using Nop.Web.Models.Customer;
using WebGrease.Css.Extensions;
using System.Text.RegularExpressions;


namespace Nop.Web.Controllers
{
    public partial class CustomerController : BasePublicController
    {
        #region Fields
        private readonly IAuthenticationService _authenticationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly TaxSettings _taxSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ITaxService _taxService;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly AddressSettings _addressSettings;
        private readonly ForumSettings _forumSettings;
        private readonly OrderSettings _orderSettings;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly IDownloadService _downloadService;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IEventPublisher _eventPublisher;

        private readonly MediaSettings _mediaSettings;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly SecuritySettings _securitySettings;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly IAffiliateService _affiliateService;
        private readonly IMobileLoginCustomerService _mobileLoginCustomerService;
        private readonly HttpContextBase _httpContext;
        private readonly ICustomerLedgerMasterService _customerLedgerMasterService;
        private readonly ICustomerLedgerDetailService _customerLedgerDetailService;

        #endregion

        #region Ctor

        public CustomerController(IAuthenticationService authenticationService,
            IDateTimeHelper dateTimeHelper,
            DateTimeSettings dateTimeSettings,
            TaxSettings taxSettings,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            ICustomerService customerService,
            ICustomerAttributeParser customerAttributeParser,
            ICustomerAttributeService customerAttributeService,
            IGenericAttributeService genericAttributeService,
            ICustomerRegistrationService customerRegistrationService,
            ITaxService taxService,
            RewardPointsSettings rewardPointsSettings,
            CustomerSettings customerSettings,
            AddressSettings addressSettings,
            ForumSettings forumSettings,
            OrderSettings orderSettings,
            IAddressService addressService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IOrderService orderService,
            IPictureService pictureService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IShoppingCartService shoppingCartService,
            IOpenAuthenticationService openAuthenticationService,
            IDownloadService downloadService,
            IWebHelper webHelper,
            ICustomerActivityService customerActivityService,
            IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            IAddressAttributeFormatter addressAttributeFormatter,
            IReturnRequestService returnRequestService,
            IEventPublisher eventPublisher,
            MediaSettings mediaSettings,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            CaptchaSettings captchaSettings,
            SecuritySettings securitySettings,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            StoreInformationSettings storeInformationSettings,
            CatalogSettings catalogSettings,
            VendorSettings vendorSettings,
            IAffiliateService affiliateService,
            IMobileLoginCustomerService mobileLoginCustomerService,
            HttpContextBase httpContext,
            ICustomerLedgerMasterService customerLedgerMasterService,
            ICustomerLedgerDetailService customerLedgerDetailService)
        {
            _authenticationService = authenticationService;
            _dateTimeHelper = dateTimeHelper;
            _dateTimeSettings = dateTimeSettings;
            _taxSettings = taxSettings;
            _localizationService = localizationService;
            _workContext = workContext;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _customerService = customerService;
            _customerAttributeParser = customerAttributeParser;
            _customerAttributeService = customerAttributeService;
            _genericAttributeService = genericAttributeService;
            _customerRegistrationService = customerRegistrationService;
            _taxService = taxService;
            _rewardPointsSettings = rewardPointsSettings;
            _customerSettings = customerSettings;
            _addressSettings = addressSettings;
            _forumSettings = forumSettings;
            _orderSettings = orderSettings;
            _addressService = addressService;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _orderService = orderService;
            _pictureService = pictureService;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _shoppingCartService = shoppingCartService;
            _openAuthenticationService = openAuthenticationService;
            _downloadService = downloadService;
            _webHelper = webHelper;
            _customerActivityService = customerActivityService;
            _addressAttributeParser = addressAttributeParser;
            _addressAttributeService = addressAttributeService;
            _addressAttributeFormatter = addressAttributeFormatter;
            _returnRequestService = returnRequestService;
            _eventPublisher = eventPublisher;
            _mediaSettings = mediaSettings;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
            _captchaSettings = captchaSettings;
            _securitySettings = securitySettings;
            _externalAuthenticationSettings = externalAuthenticationSettings;
            _storeInformationSettings = storeInformationSettings;
            _catalogSettings = catalogSettings;
            _vendorSettings = vendorSettings;
            _affiliateService = affiliateService;
            _mobileLoginCustomerService = mobileLoginCustomerService;
            _httpContext = httpContext;

            _customerLedgerMasterService = customerLedgerMasterService;
            _customerLedgerDetailService = customerLedgerDetailService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected bool IsValidToken(MobileLoginCustomer mobileLogin)
        {
            if (string.IsNullOrWhiteSpace(mobileLogin.Token))
                return false;

            if (mobileLogin.IsTokenValid)
                return false;

            if (_customerSettings.TokenValidationDuration > 0 &&
                mobileLogin.TokenCreatedOnUtc < DateTime.UtcNow.AddMinutes(_customerSettings.TokenValidationDuration))
                return false;

            return true;
        }

        [NonAction]
        protected string GenerateOTP()
        {
            var rnd = new Random();
            var otp = rnd.Next(0, 9999).ToString("0000");
            return otp;
        }

        [NonAction]
        protected bool SendOTP(string mobileNumber, string otp)
        {
            string smsText = "Your OTP for mobile login in " + _storeContext.CurrentStore.CompanyName + " is " + otp + ".";

            if (_customerSettings.TokenValidationDuration > 0)
                smsText += " This OTP will valid upto next " + _customerSettings.TokenValidationDuration + " minutes.";

            string smsReceiverPhoneNumber = "88" + mobileNumber;

            string URI = "http://sms.sslwireless.com/pushapi/dynamic/server.php";
            string myParameters = "user=" + _customerSettings.SMSGatewayUsername +
                "&pass=" + _customerSettings.SMSGatewayPassword +
                "&sms[0][0]=" + smsReceiverPhoneNumber + "&sms[0][1]=" + HttpUtility.UrlEncode(smsText) +
                "&sms[0][2]=" + mobileNumber + "&sid=" + _customerSettings.SMSGatewaySID;

            var request = (HttpWebRequest)WebRequest.Create(URI);

            byte[] data = Encoding.ASCII.GetBytes(myParameters);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            String responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            Response.Write(responseString);

            if (response.StatusCode != HttpStatusCode.OK)
                return false;

            return true;
        }

        [NonAction]
        protected virtual void TryAssociateAccountWithExternalAccount(Customer customer)
        {
            var parameters = ExternalAuthorizerHelper.RetrieveParametersFromRoundTrip(true);
            if (parameters == null)
                return;

            if (_openAuthenticationService.AccountExists(parameters))
                return;

            _openAuthenticationService.AssociateExternalAccountWithUser(customer, parameters);
        }

        /// <summary>
        /// Prepare custom customer attribute models
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="overrideAttributesXml">When specified we do not use attributes of a customer</param>
        /// <returns>A list of customer attribute models</returns>
        [NonAction]
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
                                if (enteredText.Any())
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
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

        [NonAction]
        protected virtual void PrepareCustomerInfoModel(CustomerInfoModel model, Customer customer,
            bool excludeProperties, string overrideCustomCustomerAttributesXml = "")
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (customer == null)
                throw new ArgumentNullException("customer");

            model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == _dateTimeHelper.CurrentTimeZone.Id) });

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
            }
            else
            {
                if (_customerSettings.UsernamesEnabled && !_customerSettings.AllowUsersToChangeUsernames)
                    model.Username = customer.Username;
            }

            //countries and states
            if (_customerSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries(_workContext.WorkingLanguage.Id))
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = c.GetLocalized(x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (_customerSettings.StateProvinceEnabled)
                {
                    //states
                    var states = _stateProvinceService.GetStateProvincesByCountryId(model.CountryId, _workContext.WorkingLanguage.Id).ToList();
                    if (states.Any())
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem { Text = s.GetLocalized(x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                        }
                    }
                    else
                    {
                        bool anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);

                        model.AvailableStates.Add(new SelectListItem
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

            //external authentication
            model.NumberOfExternalAuthenticationProviders = _openAuthenticationService
                .LoadActiveExternalAuthenticationMethods(_storeContext.CurrentStore.Id)
                .Count;
            foreach (var ear in _openAuthenticationService.GetExternalIdentifiersFor(customer))
            {
                var authMethod = _openAuthenticationService.LoadExternalAuthenticationMethodBySystemName(ear.ProviderSystemName);
                if (authMethod == null || !authMethod.IsMethodActive(_externalAuthenticationSettings))
                    continue;

                model.AssociatedExternalAuthRecords.Add(new CustomerInfoModel.AssociatedExternalAuthModel
                {
                    Id = ear.Id,
                    Email = ear.Email,
                    ExternalIdentifier = ear.ExternalIdentifier,
                    AuthMethodName = authMethod.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id)
                });
            }

            //custom customer attributes
            var customAttributes = PrepareCustomCustomerAttributes(customer, overrideCustomCustomerAttributesXml);
            customAttributes.ForEach(model.CustomerAttributes.Add);
        }

        [NonAction]
        protected virtual void PrepareCustomerRegisterModel(RegisterModel model, bool excludeProperties,
            string overrideCustomCustomerAttributesXml = "")
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == _dateTimeHelper.CurrentTimeZone.Id) });

            model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            //form fields
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
            model.AcceptPrivacyPolicyEnabled = _customerSettings.AcceptPrivacyPolicyEnabled;
            model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            model.CheckUsernameAvailabilityEnabled = _customerSettings.CheckUsernameAvailabilityEnabled;
            model.HoneypotEnabled = _securitySettings.HoneypotEnabled;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage;
            model.EnteringEmailTwice = _customerSettings.EnteringEmailTwice;

            //countries and states
            if (_customerSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });

                foreach (var c in _countryService.GetAllCountries(_workContext.WorkingLanguage.Id))
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = c.GetLocalized(x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (_customerSettings.StateProvinceEnabled)
                {
                    //states
                    var states = _stateProvinceService.GetStateProvincesByCountryId(model.CountryId, _workContext.WorkingLanguage.Id).ToList();
                    if (states.Any())
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem { Text = s.GetLocalized(x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                        }
                    }
                    else
                    {
                        bool anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);

                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = _localizationService.GetResource(anyCountrySelected ? "Address.OtherNonUS" : "Address.SelectState"),
                            Value = "0"
                        });
                    }

                }
            }

            //custom customer attributes
            var customAttributes = PrepareCustomCustomerAttributes(_workContext.CurrentCustomer, overrideCustomCustomerAttributesXml);
            customAttributes.ForEach(model.CustomerAttributes.Add);
        }

        [NonAction]
        protected virtual string ParseCustomCustomerAttributes(FormCollection form)
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
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.FileUpload:
                    //not supported customer attributes
                    default:
                        break;
                }
            }

            return attributesXml;
        }

        #endregion

        #region Login / logout

        [NopHttpsRequirement(SslRequirement.Yes)]
        //available even when a store is closed
        [StoreClosed(true)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public ActionResult Login(bool? checkoutAsGuest)
        {
            var model = new LoginModel();
            model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            model.CheckoutAsGuest = checkoutAsGuest.GetValueOrDefault();
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage;
            return View(model);
        }

        [HttpPost]
        [CaptchaValidator]
        //available even when a store is closed
        [StoreClosed(true)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public ActionResult Login(LoginModel model, string returnUrl, bool captchaValid)
        {
            if (model.LoginType == "Email")
            {
                #region Emaill login

                ModelState.Remove("MobileNumber");

                //validate CAPTCHA
                if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !captchaValid)
                {
                    ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
                }

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

                                //migrate shopping cart
                                _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);

                                //sign in new customer
                                _authenticationService.SignIn(customer, model.RememberMe);

                                //raise event       
                                _eventPublisher.Publish(new CustomerLoggedinEvent(customer));

                                //activity log
                                _customerActivityService.InsertActivity("PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"), customer);

                                if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                                    return RedirectToRoute("HomePage");

                                return Redirect(returnUrl);
                            }
                        case CustomerLoginResults.CustomerNotExist:
                            ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist"));
                            break;
                        case CustomerLoginResults.Deleted:
                            ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.Deleted"));
                            break;
                        case CustomerLoginResults.NotActive:
                            ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotActive"));
                            break;
                        case CustomerLoginResults.NotRegistered:
                            ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered"));
                            break;
                        case CustomerLoginResults.WrongPassword:
                        default:
                            ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials"));
                            break;
                    }
                }

                //If we got this far, something failed, redisplay form
                model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
                model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage;

                #endregion
            }
            else
            {
                #region Mobile login

                model.MobileNumber = model.MobileNumber.Trim();

                if (model.MobileNumber.StartsWith("+880") || model.MobileNumber.StartsWith("+88"))
                {
                    model.MobileNumber = model.MobileNumber.Replace("+8800", "0").Replace("+880", "0").Replace("+88", "0");
                }
                else if (!model.MobileNumber.StartsWith("0"))
                {
                    model.MobileNumber = "0" + model.MobileNumber;
                }

                var loginResult = _mobileLoginCustomerService.ValidateCustomerByMobileNumber(model.MobileNumber);
                switch (loginResult)
                {
                    case CustomerLoginResults.Successful:
                        {
                            var customer = _mobileLoginCustomerService.GetCustomerByMobileNumber(model.MobileNumber);



                            if (!_customerSettings.OTPValidationRequired)
                            {



                                //migrate shopping cart
                                _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);

                                //sign in new customer
                                _authenticationService.SignIn(customer, model.RememberMe);

                                //raise event       
                                _eventPublisher.Publish(new CustomerLoggedinEvent(customer));

                                //activity log
                                _customerActivityService.InsertActivity("PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"), customer);

                                if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                                    return RedirectToRoute("HomePage");

                                return Redirect(returnUrl);
                            }
                            else
                            {
                                var otp = GenerateOTP();

                                if (SendOTP(model.MobileNumber, otp))
                                {
                                    var mobileLogin = _mobileLoginCustomerService.GetMobileLoginCustomerByMobileNumber(model.MobileNumber);
                                    mobileLogin.Token = otp;
                                    mobileLogin.TokenCreatedOnUtc = DateTime.UtcNow;

                                    _mobileLoginCustomerService.UpdateMobileLoginCustomer(mobileLogin);

                                    _httpContext.Session["MobileNumber"] = model.MobileNumber;

                                    if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                                        return RedirectToRoute("LoginStep2");

                                    return RedirectToRoute("LoginStep2", new { returnUrl = returnUrl });
                                }
                            }
                            ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.SomeThingWrong"));
                            break;
                        }
                    case CustomerLoginResults.CustomerNotExist:
                        {
                            var otp = _customerSettings.OTPValidationRequired ? GenerateOTP() : "";

                            if (!_customerSettings.OTPValidationRequired || SendOTP(model.MobileNumber, otp))
                            {
                                var mobileLoginCustomerModel = new MobileLoginCustomer();
                                mobileLoginCustomerModel.CustomerId = _workContext.CurrentCustomer.Id;
                                mobileLoginCustomerModel.Token = otp;
                                mobileLoginCustomerModel.IsTokenValid = true;
                                mobileLoginCustomerModel.MobileNumber = model.MobileNumber;
                                mobileLoginCustomerModel.TokenCreatedOnUtc = DateTime.UtcNow;

                                _mobileLoginCustomerService.InsertMobileLoginCustomer(mobileLoginCustomerModel);

                                _httpContext.Session["MobileNumber"] = model.MobileNumber;

                                return RedirectToRoute("LoginStep2");
                            }
                            ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.SomeThingWrong"));
                            break;
                        }
                    case CustomerLoginResults.Deleted:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.Deleted"));
                        break;
                    case CustomerLoginResults.NotActive:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotActive"));
                        break;
                    case CustomerLoginResults.NotRegistered:
                        {
                            _httpContext.Session["MobileNumber"] = model.MobileNumber;
                            return RedirectToRoute("LoginStep2", new { returnUrl });
                        }
                    case CustomerLoginResults.WrongPassword:
                    default:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials"));
                        break;
                }
                #endregion
            }

            return View(model);
        }

        public ActionResult LoginStep2(string returnUrl)
        {
            if (_httpContext.Session["MobileNumber"] == null)
                return RedirectToRoute("login");

            if (_workContext.CurrentCustomer.IsRegistered() && !_customerSettings.OTPValidationRequired)
            {
                if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                    return RedirectToRoute("HomePage");
                return Redirect(returnUrl);
            }

            var mobileLogin = _mobileLoginCustomerService.GetMobileLoginCustomerByCustomerId(_workContext.CurrentCustomer.Id);
            if (mobileLogin == null)
                return RedirectToRoute("Login");

            var model = new MobileLoginStep2Model();
            model.NewCustomer = !_workContext.CurrentCustomer.IsRegistered();
            model.OTPValidationRequired = _customerSettings.OTPValidationRequired;

            return View(model);
        }

        [HttpPost]
        public ActionResult LoginStep2(MobileLoginStep2Model model, string returnUrl)
        {
            if (_httpContext.Session["MobileNumber"] == null)
                return RedirectToRoute("Login");

            if (_workContext.CurrentCustomer.IsRegistered())
            {
                if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                    return RedirectToRoute("HomePage");
                return Redirect(returnUrl);
            }

            var mobileNumber = _httpContext.Session["MobileNumber"].ToString();

            var mobileLogin = _mobileLoginCustomerService.GetMobileLoginCustomerByCustomerId(_workContext.CurrentCustomer.Id);
            if (mobileLogin == null || mobileLogin.MobileNumber != mobileNumber)
                return RedirectToRoute("Login");

            if (_workContext.CurrentCustomer.IsRegistered())
            {
                ModelState.Remove("Name");
                ModelState.Remove("Email");
            }

            if (ModelState.IsValid)
            {
                if (_customerSettings.OTPValidationRequired)
                {
                    if (mobileLogin.Token == model.OTP && IsValidToken(mobileLogin))
                    {
                        mobileLogin.Token = "";
                        mobileLogin.IsTokenValid = false;

                        _mobileLoginCustomerService.UpdateMobileLoginCustomer(mobileLogin);
                    }
                    else
                    {
                        if (mobileLogin.Token != model.OTP)
                            ModelState.AddModelError("", "OTP is not valid.");
                        else if (!IsValidToken(mobileLogin))
                            ModelState.AddModelError("", "OTP is not correct.");
                        return View(model);
                    }
                }

                if (!_workContext.CurrentCustomer.IsRegistered())
                {
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.FirstName, model.Name);
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.Phone, mobileNumber);

                    var registrationRequest = new CustomerRegistrationRequest(_workContext.CurrentCustomer,
                        model.Email,
                        model.Email,
                        "",
                        _customerSettings.DefaultPasswordFormat,
                        _storeContext.CurrentStore.Id,
                        true);
                    var result = _customerRegistrationService.RegisterCustomerWithMobile(registrationRequest);
                    if (!result.Success)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                        return View(model);
                    }
                }
                _authenticationService.SignIn(mobileLogin.Customer, true);

                if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                    return RedirectToRoute("HomePage");

                return Redirect(returnUrl);
            }

            model.NewCustomer = !_workContext.CurrentCustomer.IsRegistered();
            model.OTPValidationRequired = _customerSettings.OTPValidationRequired;

            return View(model);
        }

        [HttpPost]
        public ActionResult RequestOTP()
        {
            if (_httpContext.Session["MobileNumber"] == null)
                return Json(new { data = false });

            if (!_customerSettings.OTPValidationRequired)
                return Json(new { data = false });

            var mobileLogin = _mobileLoginCustomerService.GetMobileLoginCustomerByCustomerId(_workContext.CurrentCustomer.Id);
            if (mobileLogin == null)
                return Json(new { data = false });

            var otp = GenerateOTP();
            if (SendOTP(mobileLogin.MobileNumber, otp))
                return Json(new { data = true });

            return Json(new { data = false });
        }

        //available even when a store is closed
        [StoreClosed(true)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public ActionResult Logout()
        {
            //external authentication
            ExternalAuthorizerHelper.RemoveParameters();

            if (_workContext.OriginalCustomerIfImpersonated != null)
            {
                //activity log
                _customerActivityService.InsertActivity(_workContext.OriginalCustomerIfImpersonated, "Impersonation.Finished",
                    _localizationService.GetResource("ActivityLog.Impersonation.Finished.StoreOwner"), _workContext.CurrentCustomer.Email, _workContext.CurrentCustomer.Id);
                _customerActivityService.InsertActivity("Impersonation.Finished",
                    _localizationService.GetResource("ActivityLog.Impersonation.Finished.Customer"), _workContext.OriginalCustomerIfImpersonated.Email, _workContext.OriginalCustomerIfImpersonated.Id);

                //logout impersonated customer
                _genericAttributeService.SaveAttribute<int?>(_workContext.OriginalCustomerIfImpersonated,
                    SystemCustomerAttributeNames.ImpersonatedCustomerId, null);

                //redirect back to customer details page (admin area)
                return RedirectToAction("Edit", "Customer", new { id = _workContext.CurrentCustomer.Id, area = "Admin" });
            }

            //activity log
            _customerActivityService.InsertActivity("PublicStore.Logout", _localizationService.GetResource("ActivityLog.PublicStore.Logout"));
            //standard logout 
            _authenticationService.SignOut();

            //EU Cookie
            if (_storeInformationSettings.DisplayEuCookieLawWarning)
            {
                //the cookie law message should not pop up immediately after logout.
                //otherwise, the user will have to click it again...
                //and thus next visitor will not click it... so violation for that cookie law..
                //the only good solution in this case is to store a temporary variable
                //indicating that the EU cookie popup window should not be displayed on the next page open (after logout redirection to homepage)
                //but it'll be displayed for further page loads
                TempData["nop.IgnoreEuCookieLawWarning"] = true;
            }

            return RedirectToRoute("HomePage");
        }

        #endregion

        #region Password recovery

        [NopHttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public ActionResult PasswordRecovery()
        {
            var model = new PasswordRecoveryModel();
            return View(model);
        }

        [HttpPost, ActionName("PasswordRecovery")]
        [PublicAntiForgery]
        [FormValueRequired("send-email")]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public ActionResult PasswordRecoverySend(PasswordRecoveryModel model)
        {
            if (ModelState.IsValid)
            {
                var customer = _customerService.GetCustomerByEmail(model.Email);
                if (customer != null && customer.Active && !customer.Deleted)
                {
                    //save token and current date
                    var passwordRecoveryToken = Guid.NewGuid();
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.PasswordRecoveryToken, passwordRecoveryToken.ToString());
                    DateTime? generatedDateTime = DateTime.UtcNow;
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.PasswordRecoveryTokenDateGenerated, generatedDateTime);

                    //send email
                    _workflowMessageService.SendCustomerPasswordRecoveryMessage(customer, _workContext.WorkingLanguage.Id);

                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.EmailHasBeenSent");
                }
                else
                {
                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.EmailNotFound");
                }

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }


        [NopHttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public ActionResult PasswordRecoveryConfirm(string token, string email)
        {
            var customer = _customerService.GetCustomerByEmail(email);
            if (customer == null)
                return RedirectToRoute("HomePage");

            var model = new PasswordRecoveryConfirmModel();

            //validate token
            if (!customer.IsPasswordRecoveryTokenValid(token))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.WrongToken");
            }

            //validate token expiration date
            if (customer.IsPasswordRecoveryLinkExpired(_customerSettings))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.LinkExpired");
            }

            return View(model);
        }

        [HttpPost, ActionName("PasswordRecoveryConfirm")]
        [PublicAntiForgery]
        [FormValueRequired("set-password")]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public ActionResult PasswordRecoveryConfirmPOST(string token, string email, PasswordRecoveryConfirmModel model)
        {
            var customer = _customerService.GetCustomerByEmail(email);
            if (customer == null)
                return RedirectToRoute("HomePage");

            //validate token
            if (!customer.IsPasswordRecoveryTokenValid(token))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.WrongToken");
            }

            //validate token expiration date
            if (customer.IsPasswordRecoveryLinkExpired(_customerSettings))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.LinkExpired");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var response = _customerRegistrationService.ChangePassword(new ChangePasswordRequest(email,
                    false, _customerSettings.DefaultPasswordFormat, model.NewPassword));
                if (response.Success)
                {
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.PasswordRecoveryToken, "");

                    model.DisablePasswordChanging = true;
                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.PasswordHasBeenChanged");
                }
                else
                {
                    model.Result = response.Errors.FirstOrDefault();
                }

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Register

        [NopHttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public ActionResult Register()
        {
            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

            var model = new RegisterModel();
            PrepareCustomerRegisterModel(model, false);
            //enable newsletter by default
            model.Newsletter = _customerSettings.NewsletterTickedByDefault;

            return View(model);
        }

        [HttpPost]
        [CaptchaValidator]
        [HoneypotValidator]
        [PublicAntiForgery]
        [ValidateInput(false)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public ActionResult Register(RegisterModel model, string returnUrl, bool captchaValid, FormCollection form)
        {
            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

            if (_workContext.CurrentCustomer.IsRegistered())
            {
                //Already registered customer. 
                _authenticationService.SignOut();

                //Save a new record
                _workContext.CurrentCustomer = _customerService.InsertGuestCustomer();
            }
            var customer = _workContext.CurrentCustomer;

            //custom customer attributes
            var customerAttributesXml = ParseCustomCustomerAttributes(form);
            var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
            foreach (var error in customerAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

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
                    //properties
                    if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    {
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.TimeZoneId, model.TimeZoneId);
                    }
                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.VatNumber, model.VatNumber);
                        var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out string vatName, out string vatAddress);
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
                    TryAssociateAccountWithExternalAccount(customer);

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
                    if (_addressService.IsAddressValid(defaultAddress))
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

                    //raise event       
                    _eventPublisher.Publish(new CustomerRegisteredEvent(customer));

                    switch (_customerSettings.UserRegistrationType)
                    {
                        case UserRegistrationType.EmailValidation:
                            {
                                //email validation message
                                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AccountActivationToken, Guid.NewGuid().ToString());
                                _workflowMessageService.SendCustomerEmailValidationMessage(customer, _workContext.WorkingLanguage.Id);

                                //result
                                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.EmailValidation });
                            }
                        case UserRegistrationType.AdminApproval:
                            {
                                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.AdminApproval });
                            }
                        case UserRegistrationType.Standard:
                            {
                                //send customer welcome message
                                _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);

                                var redirectUrl = Url.RouteUrl("RegisterResult", new { resultId = (int)UserRegistrationType.Standard });
                                if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                                    redirectUrl = _webHelper.ModifyQueryString(redirectUrl, "returnurl=" + HttpUtility.UrlEncode(returnUrl), null);
                                return Redirect(redirectUrl);
                            }
                        default:
                            {
                                return RedirectToRoute("HomePage");
                            }
                    }
                }

                //errors
                foreach (var error in registrationResult.Errors)
                    ModelState.AddModelError("", error);
            }

            //If we got this far, something failed, redisplay form
            PrepareCustomerRegisterModel(model, true, customerAttributesXml);
            return View(model);
        }

        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public ActionResult RegisterResult(int resultId)
        {
            var resultText = "";
            switch ((UserRegistrationType)resultId)
            {
                case UserRegistrationType.Disabled:
                    resultText = _localizationService.GetResource("Account.Register.Result.Disabled");
                    break;
                case UserRegistrationType.Standard:
                    resultText = _localizationService.GetResource("Account.Register.Result.Standard");
                    break;
                case UserRegistrationType.AdminApproval:
                    resultText = _localizationService.GetResource("Account.Register.Result.AdminApproval");
                    break;
                case UserRegistrationType.EmailValidation:
                    resultText = _localizationService.GetResource("Account.Register.Result.EmailValidation");
                    break;
                default:
                    break;
            }
            var model = new RegisterResultModel
            {
                Result = resultText
            };
            return View(model);
        }

        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        [HttpPost]
        public ActionResult RegisterResult(string returnUrl)
        {
            if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                return RedirectToRoute("HomePage");

            return Redirect(returnUrl);
        }

        [HttpPost]
        [PublicAntiForgery]
        [ValidateInput(false)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public ActionResult CheckUsernameAvailability(string username)
        {
            var usernameAvailable = false;
            var statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.NotAvailable");

            if (_customerSettings.UsernamesEnabled && !String.IsNullOrWhiteSpace(username))
            {
                if (_workContext.CurrentCustomer != null &&
                    _workContext.CurrentCustomer.Username != null &&
                    _workContext.CurrentCustomer.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase))
                {
                    statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.CurrentUsername");
                }
                else
                {
                    var customer = _customerService.GetCustomerByUsername(username);
                    if (customer == null)
                    {
                        statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.Available");
                        usernameAvailable = true;
                    }
                }
            }

            return Json(new { Available = usernameAvailable, Text = statusText });
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public ActionResult AccountActivation(string token, string email)
        {
            var customer = _customerService.GetCustomerByEmail(email);
            if (customer == null)
                return RedirectToRoute("HomePage");

            var cToken = customer.GetAttribute<string>(SystemCustomerAttributeNames.AccountActivationToken);
            if (String.IsNullOrEmpty(cToken))
                return RedirectToRoute("HomePage");

            if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("HomePage");

            //activate user account
            customer.Active = true;
            _customerService.UpdateCustomer(customer);
            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AccountActivationToken, "");
            //send welcome message
            _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);

            var model = new AccountActivationModel();
            model.Result = _localizationService.GetResource("Account.AccountActivation.Activated");
            return View(model);
        }

        #endregion

        #region My account / Wallet
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Wallet()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var model = new CustomerLedgerView();

            var CustomerAccountInformation = _customerLedgerMasterService.GetCustomerWalletAccountByCustomerId(_workContext.CurrentCustomer.Id);

            long Phone = 0;

            if (CustomerAccountInformation != null)
            {
                Phone = CustomerAccountInformation.ContactNo;
            }

            //var longExtractor = new Regex("[^0-9]");
            //var PhoneP = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);

            //if (string.IsNullOrEmpty(PhoneP))
            //{
            //    PhoneP = "-1";
            //}

            //var Phone = longExtractor.Replace(PhoneP, "");
            var customerLedgerMaster = _customerLedgerMasterService.GetCustomerLedgerMasterByContactNo(Phone);

            model.CustomerLedgerMaster = customerLedgerMaster;
            model.CustomerLedgerDetail = _customerLedgerDetailService.GetCustomerLedgerDetailByContactNo(Phone);

            return View(model);
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult CreateWallet(CustomerLedgerView customerLedgerView)
        {
            try
            {
                if (!_workContext.CurrentCustomer.IsRegistered())
                    return new HttpUnauthorizedResult();

                var modelW = new WalletAccountInformationTemp();
                var model = new CustomerLedgerView();

                //if (ModelState.IsValid)
                //{
                bool canConvert = long.TryParse(customerLedgerView.WalletAccountView.ContactNo.ToString(), out long number1);

                if (canConvert == false)
                {
                    TempData["Message"] = "Please enter valid contact number.";
                }
                else if (customerLedgerView.WalletAccountView.ContactNo.ToString().Length < 10 || customerLedgerView.WalletAccountView.ContactNo.ToString().Length > 11)
                {
                    TempData["Message"] = "Please enter valid contact number.";
                }
                else
                {
                    modelW.ContactNo = Convert.ToInt64(customerLedgerView.WalletAccountView.ContactNo.ToString());
                    modelW.CustomerID = _workContext.CurrentCustomer.Id;

                    WalletAccountInformation CW = _customerLedgerMasterService.GetCustomerWalletAccountByContactNo(modelW.ContactNo);

                    if (CW == null)
                    {
                        _customerLedgerMasterService.InsertWalletAccountInformation(modelW);
                        return View(model);
                    }
                    else
                    {
                        TempData["Message"] = "Contact number already exists. Please enter another valid contact number.";
                        return RedirectToRoute("CustomerWallet");
                    }
                }
                //}

                return RedirectToRoute("CustomerWallet");
            }
            catch (Exception ex)
            {
                return RedirectToRoute("CustomerWallet");
            }
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult CheckWalletOTP(CustomerLedgerView customerLedgerView)
        {
            try
            {
                if (!_workContext.CurrentCustomer.IsRegistered())
                    return new HttpUnauthorizedResult();

                var modelW = new WalletAccountInformationTemp();
                var model = new CustomerLedgerView();

                if (ModelState.IsValid)
                {
                    bool canConvert = int.TryParse(customerLedgerView.WalletAccountView.OTP, out int number1);

                    if (!canConvert)
                    {
                        TempData["Message"] = "Please enter valid OTP.";
                    }
                    else if (customerLedgerView.WalletAccountView.OTP.ToString().Length < 5 || customerLedgerView.WalletAccountView.OTP.ToString().Length > 10)
                    {
                        TempData["Message"] = "Please enter valid OTP.";
                    }
                    else
                    {
                        modelW.OTP = Convert.ToInt32(customerLedgerView.WalletAccountView.OTP);
                        modelW.CustomerID = _workContext.CurrentCustomer.Id;

                        WalletAccountInformationTemp CW = _customerLedgerMasterService.GetCustomerWalletAccount(modelW.CustomerID, modelW.OTP);

                        if (CW != null)
                        {
                            DateTime date1 = CW.OTPEntryDate;
                            DateTime date2 = DateTime.Now;
                            TimeSpan interval = date2 - date1;

                            if (interval.TotalSeconds < CW.OTPExpireTime * 60)
                            {
                                var customerFullName = _workContext.CurrentCustomer.GetFullName();
                                _customerLedgerMasterService.CheckWalletOTP(modelW, customerFullName);

                                return RedirectToRoute("CustomerWallet");
                            }
                            else
                            {
                                TempData["Message"] = "OTP expired";
                                return View(model);
                                //return RedirectToRoute("CustomerWallet");
                            }
                        }
                        else
                        {
                            TempData["Message"] = "Valid OTP not found...";
                            return View(model);
                            //return RedirectToRoute("CustomerWallet");
                        }
                    }
                }

                //return RedirectToRoute("CustomerWallet");
                return View(model);

            }
            catch (Exception)
            {
                return RedirectToRoute("CustomerWallet");
                throw;
            }
        }

        #endregion

        #region My account / Info

        [ChildActionOnly]
        public ActionResult CustomerNavigation(int selectedTabId = 0)
        {
            var model = new CustomerNavigationModel();

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerInfo",
                Title = _localizationService.GetResource("Account.CustomerInfo"),
                Tab = CustomerNavigationEnum.Info,
                ItemClass = "customer-info"
            });



            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerAddresses",
                Title = _localizationService.GetResource("Account.CustomerAddresses"),
                Tab = CustomerNavigationEnum.Addresses,
                ItemClass = "customer-addresses"
            });

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerOrders",
                Title = _localizationService.GetResource("Account.CustomerOrders"),
                Tab = CustomerNavigationEnum.Orders,
                ItemClass = "customer-orders"
            });

            if (_orderSettings.ReturnRequestsEnabled &&
                _returnRequestService.SearchReturnRequests(_storeContext.CurrentStore.Id,
                    _workContext.CurrentCustomer.Id, 0, null, 0, 1).Any())
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerReturnRequests",
                    Title = _localizationService.GetResource("Account.CustomerReturnRequests"),
                    Tab = CustomerNavigationEnum.ReturnRequests,
                    ItemClass = "return-requests"
                });
            }

            if (!_customerSettings.HideDownloadableProductsTab)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerDownloadableProducts",
                    Title = _localizationService.GetResource("Account.DownloadableProducts"),
                    Tab = CustomerNavigationEnum.DownloadableProducts,
                    ItemClass = "downloadable-products"
                });
            }

            if (!_customerSettings.HideBackInStockSubscriptionsTab)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerBackInStockSubscriptions",
                    Title = _localizationService.GetResource("Account.BackInStockSubscriptions"),
                    Tab = CustomerNavigationEnum.BackInStockSubscriptions,
                    ItemClass = "back-in-stock-subscriptions"
                });
            }

            if (_rewardPointsSettings.Enabled)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerRewardPoints",
                    Title = _localizationService.GetResource("Account.RewardPoints"),
                    Tab = CustomerNavigationEnum.RewardPoints,
                    ItemClass = "reward-points"
                });
            }

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerChangePassword",
                Title = _localizationService.GetResource("Account.ChangePassword"),
                Tab = CustomerNavigationEnum.ChangePassword,
                ItemClass = "change-password"
            });

            if (_customerSettings.AllowCustomersToUploadAvatars)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerAvatar",
                    Title = _localizationService.GetResource("Account.Avatar"),
                    Tab = CustomerNavigationEnum.Avatar,
                    ItemClass = "customer-avatar"
                });
            }

            if (_forumSettings.ForumsEnabled && _forumSettings.AllowCustomersToManageSubscriptions)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerForumSubscriptions",
                    Title = _localizationService.GetResource("Account.ForumSubscriptions"),
                    Tab = CustomerNavigationEnum.ForumSubscriptions,
                    ItemClass = "forum-subscriptions"
                });
            }
            if (_catalogSettings.ShowProductReviewsTabOnAccountPage)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerProductReviews",
                    Title = _localizationService.GetResource("Account.CustomerProductReviews"),
                    Tab = CustomerNavigationEnum.ProductReviews,
                    ItemClass = "customer-reviews"
                });
            }
            if (_vendorSettings.AllowVendorsToEditInfo && _workContext.CurrentVendor != null)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerVendorInfo",
                    Title = _localizationService.GetResource("Account.VendorInfo"),
                    Tab = CustomerNavigationEnum.VendorInfo,
                    ItemClass = "customer-vendor-info"
                });
            }
            var affiliate = _affiliateService.GetAffiliateByCustomerId(_workContext.CurrentCustomer.Id);
            if (_customerSettings.EnableAffiliate)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerAffiliateInfo",
                    Title = affiliate != null ? _localizationService.GetResource("Account.AffiliateInfo") :
                        _localizationService.GetResource("Account.ApplyAffiliate"),
                    Tab = CustomerNavigationEnum.AffiliateInfo,
                    ItemClass = "customer-affiliate-info"
                });
            }
            if (_customerSettings.EnableAffiliate && affiliate != null && !affiliate.Deleted && affiliate.Active)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerAffiliatedOrders",
                    Title = _localizationService.GetResource("Account.AffiliatedOrders"),
                    Tab = CustomerNavigationEnum.AffiliatedOrders,
                    ItemClass = "customer-affiliated-orders"
                });
            }
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerWallet",
                Title = _localizationService.GetResource("Wallet"),
                Tab = CustomerNavigationEnum.Wallet,
                ItemClass = "customer-orders"
            });

            model.SelectedTab = (CustomerNavigationEnum)selectedTabId;

            return PartialView(model);
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Info()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            var model = new CustomerInfoModel();
            PrepareCustomerInfoModel(model, customer, false);

            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        [ValidateInput(false)]
        public ActionResult Info(CustomerInfoModel model, FormCollection form)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            //custom customer attributes
            var customerAttributesXml = ParseCustomCustomerAttributes(form);
            var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
            foreach (var error in customerAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    //username 
                    if (_customerSettings.UsernamesEnabled && _customerSettings.AllowUsersToChangeUsernames)
                    {
                        if (!customer.Username.Equals(model.Username.Trim(), StringComparison.InvariantCultureIgnoreCase))
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
                    if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    {
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.TimeZoneId, model.TimeZoneId);
                    }
                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        var prevVatNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.VatNumber);

                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.VatNumber, model.VatNumber);
                        if (prevVatNumber != model.VatNumber)
                        {
                            var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out string vatName, out string vatAddress);
                            _genericAttributeService.SaveAttribute(customer,
                                    SystemCustomerAttributeNames.VatNumberStatusId,
                                    (int)vatNumberStatus);
                            //send VAT number admin notification
                            if (!String.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                                _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer, model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
                        }
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
                        var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, _storeContext.CurrentStore.Id);
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
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Signature, model.Signature);

                    //save customer attributes
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.CustomCustomerAttributes, customerAttributesXml);

                    return RedirectToRoute("CustomerInfo");
                }
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
            }

            //If we got this far, something failed, redisplay form
            PrepareCustomerInfoModel(model, customer, true, customerAttributesXml);
            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        public ActionResult RemoveExternalAssociation(int id)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            //ensure it's our record
            var ear = _openAuthenticationService.GetExternalIdentifiersFor(_workContext.CurrentCustomer)
                .FirstOrDefault(x => x.Id == id);

            if (ear == null)
            {
                return Json(new
                {
                    redirect = Url.Action("Info"),
                });
            }

            _openAuthenticationService.DeletExternalAuthenticationRecord(ear);

            return Json(new
            {
                redirect = Url.Action("Info"),
            });
        }

        #endregion

        #region My account / Addresses

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Addresses()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            var model = new CustomerAddressListModel();
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
                    localizationService: _localizationService,
                    stateProvinceService: _stateProvinceService,
                    addressAttributeFormatter: _addressAttributeFormatter,
                    loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id));
                model.Addresses.Add(addressModel);
            }
            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult AddressDelete(int addressId)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            //find address (ensure that it belongs to the current customer)
            var address = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address != null)
            {
                customer.RemoveAddress(address);
                _customerService.UpdateCustomer(customer);
                //now delete the address record
                _addressService.DeleteAddress(address);
            }

            //redirect to the address list page
            return Json(new
            {
                redirect = Url.RouteUrl("CustomerAddresses"),
            });
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult AddressAdd()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var model = new CustomerAddressEditModel();
            model.Address.PrepareModel(
                address: null,
                excludeProperties: false,
                addressSettings: _addressSettings,
                localizationService: _localizationService,
                stateProvinceService: _stateProvinceService,
                addressAttributeService: _addressAttributeService,
                addressAttributeParser: _addressAttributeParser,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id));

            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        [ValidateInput(false)]
        public ActionResult AddressAdd(CustomerAddressEditModel model, FormCollection form)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            //custom address attributes
            var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                var address = model.Address.ToEntity();
                address.CustomAttributes = customAttributes;
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                customer.Addresses.Add(address);
                _customerService.UpdateCustomer(customer);

                return RedirectToRoute("CustomerAddresses");
            }

            //If we got this far, something failed, redisplay form
            model.Address.PrepareModel(
                address: null,
                excludeProperties: true,
                addressSettings: _addressSettings,
                localizationService: _localizationService,
                stateProvinceService: _stateProvinceService,
                addressAttributeService: _addressAttributeService,
                addressAttributeParser: _addressAttributeParser,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id),
                overrideAttributesXml: customAttributes);

            return View(model);
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult AddressEdit(int addressId)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;
            //find address (ensure that it belongs to the current customer)
            var address = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
                //address is not found
                return RedirectToRoute("CustomerAddresses");

            var model = new CustomerAddressEditModel();
            model.Address.PrepareModel(address: address,
                excludeProperties: false,
                addressSettings: _addressSettings,
                localizationService: _localizationService,
                stateProvinceService: _stateProvinceService,
                addressAttributeService: _addressAttributeService,
                addressAttributeParser: _addressAttributeParser,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id));

            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        [ValidateInput(false)]
        public ActionResult AddressEdit(CustomerAddressEditModel model, int addressId, FormCollection form)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;
            //find address (ensure that it belongs to the current customer)
            var address = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
                //address is not found
                return RedirectToRoute("CustomerAddresses");

            //custom address attributes
            var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                address.CustomAttributes = customAttributes;
                _addressService.UpdateAddress(address);

                return RedirectToRoute("CustomerAddresses");
            }

            //If we got this far, something failed, redisplay form
            model.Address.PrepareModel(
                address: address,
                excludeProperties: true,
                addressSettings: _addressSettings,
                localizationService: _localizationService,
                stateProvinceService: _stateProvinceService,
                addressAttributeService: _addressAttributeService,
                addressAttributeParser: _addressAttributeParser,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id),
                overrideAttributesXml: customAttributes);
            return View(model);
        }

        #endregion

        #region My account / Downloadable products

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult DownloadableProducts()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            var model = new CustomerDownloadableProductsModel();
            var items = _orderService.GetDownloadableOrderItems(customer.Id);
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
                    itemModel.DownloadId = item.Product.DownloadId;

                if (_downloadService.IsLicenseDownloadAllowed(item))
                    itemModel.LicenseId = item.LicenseDownloadId.HasValue ? item.LicenseDownloadId.Value : 0;
            }

            return View(model);
        }

        public ActionResult UserAgreement(Guid orderItemId)
        {
            var orderItem = _orderService.GetOrderItemByGuid(orderItemId);
            if (orderItem == null)
                return RedirectToRoute("HomePage");

            var product = orderItem.Product;
            if (product == null || !product.HasUserAgreement)
                return RedirectToRoute("HomePage");

            var model = new UserAgreementModel();
            model.UserAgreementText = product.UserAgreementText;
            model.OrderItemGuid = orderItemId;

            return View(model);
        }

        #endregion

        #region My account / Change password

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult ChangePassword()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var model = new ChangePasswordModel();
            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            if (ModelState.IsValid)
            {
                var changePasswordRequest = new ChangePasswordRequest(customer.Email,
                    true, _customerSettings.DefaultPasswordFormat, model.NewPassword, model.OldPassword);
                var changePasswordResult = _customerRegistrationService.ChangePassword(changePasswordRequest);
                if (changePasswordResult.Success)
                {
                    model.Result = _localizationService.GetResource("Account.ChangePassword.Success");
                    return View(model);
                }

                //errors
                foreach (var error in changePasswordResult.Errors)
                    ModelState.AddModelError("", error);
            }


            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region My account / Avatar

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Avatar()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (!_customerSettings.AllowCustomersToUploadAvatars)
                return RedirectToRoute("CustomerInfo");

            var customer = _workContext.CurrentCustomer;

            var model = new CustomerAvatarModel();
            model.AvatarUrl = _pictureService.GetPictureUrl(
                customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                _mediaSettings.AvatarPictureSize,
                false);
            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [PublicAntiForgery]
        [FormValueRequired("upload-avatar")]
        public ActionResult UploadAvatar(CustomerAvatarModel model, HttpPostedFileBase uploadedFile)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (!_customerSettings.AllowCustomersToUploadAvatars)
                return RedirectToRoute("CustomerInfo");

            var customer = _workContext.CurrentCustomer;

            if (ModelState.IsValid)
            {
                try
                {
                    var customerAvatar = _pictureService.GetPictureById(customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId));
                    if ((uploadedFile != null) && (!String.IsNullOrEmpty(uploadedFile.FileName)))
                    {
                        int avatarMaxSize = _customerSettings.AvatarMaximumSizeBytes;
                        if (uploadedFile.ContentLength > avatarMaxSize)
                            throw new NopException(string.Format(_localizationService.GetResource("Account.Avatar.MaximumUploadedFileSize"), avatarMaxSize));

                        byte[] customerPictureBinary = uploadedFile.GetPictureBits();
                        if (customerAvatar != null)
                            customerAvatar = _pictureService.UpdatePicture(customerAvatar.Id, customerPictureBinary, uploadedFile.ContentType, null);
                        else
                            customerAvatar = _pictureService.InsertPicture(customerPictureBinary, uploadedFile.ContentType, null);
                    }

                    int customerAvatarId = 0;
                    if (customerAvatar != null)
                        customerAvatarId = customerAvatar.Id;

                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AvatarPictureId, customerAvatarId);

                    model.AvatarUrl = _pictureService.GetPictureUrl(
                        customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                        _mediaSettings.AvatarPictureSize,
                        false);
                    return View(model);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.Message);
                }
            }


            //If we got this far, something failed, redisplay form
            model.AvatarUrl = _pictureService.GetPictureUrl(
                customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                _mediaSettings.AvatarPictureSize,
                false);
            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [PublicAntiForgery]
        [FormValueRequired("remove-avatar")]
        public ActionResult RemoveAvatar(CustomerAvatarModel model, HttpPostedFileBase uploadedFile)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (!_customerSettings.AllowCustomersToUploadAvatars)
                return RedirectToRoute("CustomerInfo");

            var customer = _workContext.CurrentCustomer;

            var customerAvatar = _pictureService.GetPictureById(customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId));
            if (customerAvatar != null)
                _pictureService.DeletePicture(customerAvatar);
            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AvatarPictureId, 0);

            return RedirectToRoute("CustomerAvatar");
        }

        #endregion
    }
}
