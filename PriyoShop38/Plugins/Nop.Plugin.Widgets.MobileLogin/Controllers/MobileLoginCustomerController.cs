//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using Nop.Core;
//using Nop.Core.Plugins;
//using Nop.Plugin.Widgets.MobileLogin.Models;
//using Nop.Plugin.Widgets.MobileLogin.Services;
//using Nop.Services.Configuration;
//using Nop.Services.Localization;
//using Nop.Services.Logging;
//using Nop.Services.Security;
//using Nop.Services.Stores;
//using Nop.Web.Framework.Controllers;
//using Nop.Web.Framework.Kendoui;
//using Nop.Core.Domain.Customers;
//using Nop.Services.Authentication;
//using Nop.Services.Customers;
//using Nop.Services.Common;
//using Nop.Services.Tax;
//using Nop.Services.Helpers;
//using Nop.Core.Domain.Tax;
//using Nop.Services.Messages;
//using Nop.Core.Domain.Messages;
//using Nop.Core.Domain.Localization;
//using Nop.Services.Events;
//using Nop.Services.Authentication.External;
//using Nop.Services.Orders;
//using System.Text;
//using System.Security.Cryptography;

//namespace Nop.Plugin.Widgets.MobileLogin.Controllers
//{
//    public class MobileLoginCustomerController : BasePluginController
//    {
//        #region Fields
//        private readonly IAuthenticationService _authenticationService;
//        private readonly IDateTimeHelper _dateTimeHelper;
//        private readonly DateTimeSettings _dateTimeSettings;
//        private readonly TaxSettings _taxSettings;
//        private readonly ILocalizationService _localizationService;
//        private readonly IWorkContext _workContext;
//        private readonly IStoreContext _storeContext;
//        private readonly IStoreMappingService _storeMappingService;
//        private readonly ICustomerService _customerService;
//        private readonly ICustomerAttributeParser _customerAttributeParser;
//        private readonly ICustomerAttributeService _customerAttributeService;
//        private readonly IGenericAttributeService _genericAttributeService;
//        private readonly ICustomerRegistrationService _customerRegistrationService;
//        private readonly ITaxService _taxService;
//        private readonly IPluginFinder _pluginFinder;
//        private readonly ILogger _logger;
//        private readonly IWebHelper _webHelper;
//        private readonly IStoreService _storeService;
//        private readonly MobileLoginSettings _mobileLoginSettings;
//        private readonly ISettingService _settingService;
//        private readonly IPermissionService _permissionService;
//        private readonly IMobileLoginService _mobileLoginService;
//        private readonly CustomerSettings _customerSettings;

//        private readonly IOpenAuthenticationService _openAuthenticationService;
//        private readonly IEventPublisher _eventPublisher;
//        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
//        private readonly IWorkflowMessageService _workflowMessageService;
//        private readonly LocalizationSettings _localizationSettings;
//        private readonly IShoppingCartService _shoppingCartService;
//        private readonly ICustomerActivityService _customerActivityService;
//        private readonly HttpContextBase _httpContext;
//        #endregion

//        #region Ctor
//        public MobileLoginCustomerController(
//            IAuthenticationService authenticationService,
//            IDateTimeHelper dateTimeHelper,
//            DateTimeSettings dateTimeSettings,
//            TaxSettings taxSettings,
//            ILocalizationService localizationService,
//            IWorkContext workContext,
//            IStoreContext storeContext,
//            IStoreMappingService storeMappingService,
//            ICustomerService customerService,
//            ICustomerAttributeParser customerAttributeParser,
//            ICustomerAttributeService customerAttributeService,
//            IGenericAttributeService genericAttributeService,
//            ICustomerRegistrationService customerRegistrationService,
//            ITaxService taxService,
//            IPluginFinder pluginFinder,
//            ILogger logger,
//            IWebHelper webHelper,
//            IStoreService storeService,
//            MobileLoginSettings mobileLoginSettings,
//            ISettingService settingService,
//            IPermissionService permissionService,
//            IMobileLoginService mobileLoginService,
//            CustomerSettings customerSettings,
//            IOpenAuthenticationService openAuthenticationService,
//            IEventPublisher eventPublisher,
//            INewsLetterSubscriptionService newsLetterSubscriptionService,
//            IWorkflowMessageService workflowMessageService,
//            LocalizationSettings localizationSettings,
//            IShoppingCartService shoppingCartService,
//            ICustomerActivityService customerActivityService,
//            HttpContextBase httpContext)
//        {
//            this._authenticationService = authenticationService;
//            this._dateTimeHelper = dateTimeHelper;
//            this._dateTimeSettings = dateTimeSettings;
//            this._taxSettings = taxSettings;
//            this._localizationService = localizationService;
//            this._workContext = workContext;
//            this._storeContext = storeContext;
//            this._storeMappingService = storeMappingService;
//            this._customerService = customerService;
//            this._customerAttributeParser = customerAttributeParser;
//            this._customerAttributeService = customerAttributeService;
//            this._genericAttributeService = genericAttributeService;
//            this._customerRegistrationService = customerRegistrationService;
//            this._taxService = taxService;
//            this._pluginFinder = pluginFinder;
//            this._logger = logger;
//            this._webHelper = webHelper;
//            this._storeService = storeService;
//            this._mobileLoginSettings = mobileLoginSettings;
//            this._settingService = settingService;
//            this._permissionService = permissionService;
//            this._mobileLoginService = mobileLoginService;
//            this._customerSettings = customerSettings;

//            this._openAuthenticationService = openAuthenticationService;
//            this._eventPublisher = eventPublisher;
//            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
//            this._workflowMessageService = workflowMessageService;
//            this._localizationSettings = localizationSettings;
//            this._shoppingCartService = shoppingCartService;
//            this._customerActivityService = customerActivityService;
//            this._httpContext = httpContext;
//        }
//        #endregion

//        #region Utilities
//        public static string Base64Encode(string plainText)
//        {
//            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
//            return System.Convert.ToBase64String(plainTextBytes);
//        }

//        [NonAction]
//        protected virtual string GetCustomerRolesNames(IList<CustomerRole> customerRoles, string separator = ",")
//        {
//            var sb = new StringBuilder();
//            for (int i = 0; i < customerRoles.Count; i++)
//            {
//                sb.Append(customerRoles[i].Name);
//                if (i != customerRoles.Count - 1)
//                {
//                    sb.Append(separator);
//                    sb.Append(" ");
//                }
//            }
//            return sb.ToString();
//        }
//        public MobileLoginRecordModel ToMobileLoginRecordModel(Customer entity)
//        {
//            var model = new MobileLoginRecordModel();
//            model.CustomerId = entity.Id;
//            model.CustomerRoleNames = GetCustomerRolesNames(entity.CustomerRoles.ToList());
//            model.Email = entity.Email;
//            model.Name = entity.GetFullName();

//            var mobileLoginCustomers = _mobileLoginService.GetAll();
//            var m = mobileLoginCustomers.First(c => c.CustomerId == model.CustomerId);
//            model.MobileNumber = m.MobileNumber;
//            model.Used = m.Used;
//            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(m.CreatedOnUtc, DateTimeKind.Utc);

//            return model;
//        }

//        [NonAction]
//        protected virtual void TryAssociateAccountWithExternalAccount(Customer customer)
//        {
//            var parameters = ExternalAuthorizerHelper.RetrieveParametersFromRoundTrip(true);
//            if (parameters == null)
//                return;

//            if (_openAuthenticationService.AccountExists(parameters))
//                return;

//            _openAuthenticationService.AssociateExternalAccountWithUser(customer, parameters);
//        }

//        [NonAction]
//        protected virtual bool RegisterCustomer(RegisterModel model)
//        {
//            //check whether registration is allowed
//            //if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
//            //    return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

//            if (_workContext.CurrentCustomer.IsRegistered())
//            {
//                //Already registered customer. 
//                _authenticationService.SignOut();

//                //Save a new record
//                _workContext.CurrentCustomer = _customerService.InsertGuestCustomer();
//            }
//            var customer = _workContext.CurrentCustomer;                     
            
//            if (_customerSettings.UsernamesEnabled && model.Username != null)
//            {
//                model.Username = model.Username.Trim();
//            }

//            bool isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
//            var registrationRequest = new CustomerRegistrationRequest(customer,
//                model.Email,
//                _customerSettings.UsernamesEnabled ? model.Username : model.Email,
//                model.Password,
//                _customerSettings.DefaultPasswordFormat,
//                _storeContext.CurrentStore.Id,
//                isApproved);
//            var registrationResult = _mobileLoginService.RegisterCustomer(registrationRequest);

//            if (registrationResult.Success)
//            {
//                //save mobile login customer data
//                //var mobileLoginCustomerModel = new MobileLoginCustomer();
//                //mobileLoginCustomerModel.CustomerId = customer.Id;
//                //mobileLoginCustomerModel.MobileNumber = model.MobileNumber;
//                //mobileLoginCustomerModel.CreatedOnUtc = DateTime.UtcNow;
//                //mobileLoginCustomerModel.UpdatedOnUtc = DateTime.UtcNow;
//                //_mobileLoginService.Insert(mobileLoginCustomerModel);
//                var mobileLoginCustomer = _mobileLoginService.GetByCustomerId(_workContext.CurrentCustomer.Id);
//                if (mobileLoginCustomer != null)
//                {
//                    mobileLoginCustomer.CustomerId = customer.Id;
//                    mobileLoginCustomer.IsRegistered = customer.IsRegistered();
//                    mobileLoginCustomer.MobileNumber = model.MobileNumber;                    
//                    _mobileLoginService.Update(mobileLoginCustomer);
//                }

//                //properties
//                if (_dateTimeSettings.AllowCustomersToSetTimeZone)
//                {
//                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.TimeZoneId, model.TimeZoneId);
//                }
//                //VAT number
//                if (_taxSettings.EuVatEnabled)
//                {
//                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.VatNumber, model.VatNumber);

//                    string vatName;
//                    string vatAddress;
//                    var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out vatName, out vatAddress);
//                    _genericAttributeService.SaveAttribute(customer,
//                        SystemCustomerAttributeNames.VatNumberStatusId,
//                        (int)vatNumberStatus);
//                    //send VAT number admin notification
//                    if (!String.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
//                        _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer, model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);

//                }

//                //form fields                    
//                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
//                //_genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);

//                //newsletter
//                if (_customerSettings.NewsletterEnabled)
//                {
//                    //save newsletter value
//                    var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(model.Email, _storeContext.CurrentStore.Id);
//                    if (newsletter != null)
//                    {
//                        if (model.Newsletter)
//                        {
//                            newsletter.Active = true;
//                            _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);
//                        }
//                        //else
//                        //{
//                        //When registering, not checking the newsletter check box should not take an existing email address off of the subscription list.
//                        //_newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
//                        //}
//                    }
//                    else
//                    {
//                        if (model.Newsletter)
//                        {
//                            _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
//                            {
//                                NewsLetterSubscriptionGuid = Guid.NewGuid(),
//                                Email = model.Email,
//                                Active = true,
//                                StoreId = _storeContext.CurrentStore.Id,
//                                CreatedOnUtc = DateTime.UtcNow
//                            });
//                        }
//                    }
//                }

//                //save customer attributes
//                //_genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CustomCustomerAttributes, customerAttributesXml);

//                //migrate shopping cart
//                _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);

//                //login customer now
//                if (isApproved)
//                    _authenticationService.SignIn(customer, true);

//                //associated with external account (if possible)
//                TryAssociateAccountWithExternalAccount(customer);
                    
//                //notifications
//                if (_customerSettings.NotifyNewCustomerRegistration)
//                    _workflowMessageService.SendCustomerRegisteredNotificationMessage(customer, _localizationSettings.DefaultAdminLanguageId);

//                //raise event       
//                _eventPublisher.Publish(new CustomerRegisteredEvent(customer));

//                //Registration type as UserRegistrationType.Standard
//                //send customer welcome message
//                _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);

//                return true;
//            }

//            //errors
//            foreach (var error in registrationResult.Errors)
//                ModelState.AddModelError("", error);

//            return false;
//        }

//        [NonAction]
//        public string GenerateOTPOld(int length)
//        {
//            char[] chars = new char[62];
//            string a;

//            a = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";            
//            chars = a.ToCharArray();

//            int size = length;
//            byte[] data = new byte[1];

//            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
//            crypto.GetNonZeroBytes(data);
//            size = length;
//            data = new byte[size];
//            crypto.GetNonZeroBytes(data);

//            StringBuilder result = new StringBuilder(size);

//            foreach (byte b in data)
//            {
//                result.Append(chars[b % (chars.Length - 1)]);
//            }
//            return result.ToString();
//        }

//        [NonAction]
//        public string GenerateOTP(int length)
//        {
//            //const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
//            const string valid = "1234567890";
//            StringBuilder res = new StringBuilder();
//            Random rnd = new Random();
//            while (0 < length--)
//            {
//                res.Append(valid[rnd.Next(valid.Length)]);
//            }
//            return res.ToString();
//        }

//        [NonAction]
//        public bool LoginWithMobileNumber(LoginModel model)
//        {
//            model.MobileNumber = model.MobileNumber.Trim();            

//            if (model.MobileNumber.StartsWith("+880") || model.MobileNumber.StartsWith("+88"))
//            {
//                model.MobileNumber = model.MobileNumber.Replace("+8800", "0").Replace("+880", "0").Replace("+88", "0");
//            }
//            else if(!model.MobileNumber.StartsWith("0"))
//            {
//                model.MobileNumber = "0" + model.MobileNumber;
//            }

//            var loginResult = _mobileLoginService.ValidateCustomerByMobileNumber(model.MobileNumber);
//            switch (loginResult)
//            {
//                case CustomerLoginResults.Successful:
//                case CustomerLoginResults.CustomerNotExist:
//                    {
//                        //string otp = GenerateOTP(4);

//                        #region SMS
//                        //string smsText = otp;

//                        //if (smsText == null)
//                        //{
//                        //    smsText = "Test otp";
//                        //}
//                        //smsText = "Your OTP for mobile login in " + _storeContext.CurrentStore.CompanyName + " is " +
//                        //          smsText + ". This OTP will valid upto next 20 minutes.";

//                        //string smsReceiverPhoneNumber = "88" + model.MobileNumber;

//                        //String sid = "SplendorIT";
//                        //String user = "splendorit";
//                        //String pass = "smsmgt";
//                        //string URI = "http://sms.sslwireless.com/pushapi/dynamic/server.php";
//                        //string myParameters = "user=" + user +
//                        //    "&pass=" + pass +
//                        //    "&sms[0][0]=" + smsReceiverPhoneNumber + "&sms[0][1]=" + System.Web.HttpUtility.UrlEncode(smsText) +
//                        //    "&sms[0][2]=" + model.MobileNumber + "&sid=" + sid;

//                        //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URI);
//                        //byte[] data = Encoding.ASCII.GetBytes(myParameters);
//                        //request.Method = "POST";
//                        //request.ContentType = "application/x-www-form-urlencoded";
//                        //request.ContentLength = data.Length;
//                        //using (Stream stream = request.GetRequestStream())
//                        //{
//                        //    stream.Write(data, 0, data.Length);
//                        //}
//                        //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
//                        //String responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
//                        //Response.Write(responseString);

//                        //if (response.StatusCode != HttpStatusCode.OK)
//                        //    return false;

//                        #endregion

//                        var mobileLoginCustomer = _mobileLoginService.GetByMobileNumber(model.MobileNumber);
//                        if (mobileLoginCustomer != null)
//                        {
//                            mobileLoginCustomer.IsRegistered =
//                                _customerService.GetCustomerById(mobileLoginCustomer.CustomerId).IsRegistered();
//                            mobileLoginCustomer.Token = ""; // otp;
//                            mobileLoginCustomer.IsTokenValid = true;
//                            mobileLoginCustomer.UpdatedOnUtc = DateTime.UtcNow;
//                            _mobileLoginService.Update(mobileLoginCustomer);
//                        }
//                        else
//                        {
//                            var mobileLoginCustomerModel = new Domain.MobileLoginCustomer();
//                            mobileLoginCustomerModel.CustomerId = _workContext.CurrentCustomer.Id;
//                            mobileLoginCustomerModel.IsRegistered = false;
//                            mobileLoginCustomerModel.Token = ""; // otp;
//                            mobileLoginCustomerModel.IsTokenValid = true;
//                            mobileLoginCustomerModel.MobileNumber = model.MobileNumber;                            
//                            mobileLoginCustomerModel.CreatedOnUtc = DateTime.UtcNow;
//                            mobileLoginCustomerModel.UpdatedOnUtc = DateTime.UtcNow;
//                            _mobileLoginService.Insert(mobileLoginCustomerModel);
//                        }                        
//                        _httpContext.Session["MobileNumber"] = model.MobileNumber;

//                        return true;
//                    }
//                case CustomerLoginResults.Deleted:
//                    ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.Deleted"));
//                    break;
//                case CustomerLoginResults.NotActive:
//                    ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotActive"));
//                    break;
//                case CustomerLoginResults.NotRegistered:
//                    ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered"));
//                    break;
//                case CustomerLoginResults.WrongPassword:
//                default:
//                    ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials"));
//                    break;
//            }
//            return false;
//        }
        
//        [NonAction]
//        public string LoginWithEmail(LoginModel model, string returnUrl)
//        {
//            //validate CAPTCHA
//            //if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !captchaValid)
//            //{
//            //    ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
//            //}

//            if (ModelState.IsValid)
//            {
//                if (_customerSettings.UsernamesEnabled && model.Username != null)
//                {
//                    model.Username = model.Username.Trim();
//                }
//                var loginResult = _customerRegistrationService.ValidateCustomer(_customerSettings.UsernamesEnabled ? model.Username : model.Email, model.Password);
//                switch (loginResult)
//                {
//                    case CustomerLoginResults.Successful:
//                        {
//                            var customer = _customerSettings.UsernamesEnabled ? _customerService.GetCustomerByUsername(model.Username) : _customerService.GetCustomerByEmail(model.Email);

//                            //migrate shopping cart
//                            _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);

//                            //sign in new customer
//                            _authenticationService.SignIn(customer, model.RememberMe);

//                            //raise event       
//                            _eventPublisher.Publish(new CustomerLoggedinEvent(customer));

//                            //activity log
//                            _customerActivityService.InsertActivity("PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"), customer);

//                            if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
//                            {
//                                //return RedirectToRoute("HomePage");
//                                return "HomePage";
//                            }

//                            return returnUrl;
                            
//                        }
//                    case CustomerLoginResults.CustomerNotExist:
//                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist"));
//                        break;
//                    case CustomerLoginResults.Deleted:
//                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.Deleted"));
//                        break;
//                    case CustomerLoginResults.NotActive:
//                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotActive"));
//                        break;
//                    case CustomerLoginResults.NotRegistered:
//                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered"));
//                        break;
//                    case CustomerLoginResults.WrongPassword:
//                    default:
//                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials"));
//                        break;
//                }
//            }

//            //If we got this far, something failed, redisplay form
//            model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
//            //model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage;
//            return "";
//        }

//        [NonAction]
//        protected virtual void RegisterCustomerFromAdminPanel(RegisterModel model)
//        {                        
//            //if (_customerSettings.UsernamesEnabled && model.Username != null)
//            //{
//            //    model.Username = model.Username.Trim();
//            //}

//            var customer = new Customer
//            {
//                CustomerGuid = Guid.NewGuid(),
//                Email = model.Email,
//                Username = model.Username,                
//                Active = true,
//                CreatedOnUtc = DateTime.UtcNow,
//                LastActivityDateUtc = DateTime.UtcNow,
//            };
//            _customerService.InsertCustomer(customer);            

//            var mobileLoginCustomer = _mobileLoginService.GetByCustomerId(_workContext.CurrentCustomer.Id);
//            if (mobileLoginCustomer == null)
//            {
//                var mobileLoginCustomerModel = new Domain.MobileLoginCustomer();
//                mobileLoginCustomerModel.CustomerId = customer.Id;
//                mobileLoginCustomerModel.IsRegistered = customer.IsRegistered();
//                mobileLoginCustomerModel.Token = "";
//                mobileLoginCustomerModel.IsTokenValid = false;
//                mobileLoginCustomerModel.MobileNumber = model.MobileNumber;
//                mobileLoginCustomerModel.CreatedOnUtc = DateTime.UtcNow;
//                mobileLoginCustomerModel.UpdatedOnUtc = DateTime.UtcNow;
//                _mobileLoginService.Insert(mobileLoginCustomerModel);
//            }            

//            //form fields
//            if (_dateTimeSettings.AllowCustomersToSetTimeZone)
//                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.TimeZoneId, model.TimeZoneId);
//            if (_customerSettings.GenderEnabled)
//                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Gender, model.Gender);
//            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
//            //_genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
//            //if (_customerSettings.DateOfBirthEnabled)
//            //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.DateOfBirth, model.DateOfBirth);
//            if (_customerSettings.CompanyEnabled)
//                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Company, model.Company);
//            if (_customerSettings.StreetAddressEnabled)
//                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress, model.StreetAddress);
//            if (_customerSettings.StreetAddress2Enabled)
//                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress2, model.StreetAddress2);
//            if (_customerSettings.ZipPostalCodeEnabled)
//                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.ZipPostalCode, model.ZipPostalCode);
//            if (_customerSettings.CityEnabled)
//                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.City, model.City);
//            if (_customerSettings.CountryEnabled)
//                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CountryId, model.CountryId);
//            if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
//                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StateProvinceId, model.StateProvinceId);
//            if (_customerSettings.PhoneEnabled)
//                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Phone, model.Phone);
//            if (_customerSettings.FaxEnabled)
//                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Fax, model.Fax);

//            //custom customer attributes
//            //var customerAttributes = ParseCustomCustomerAttributes(customer, form);
//            //_genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CustomCustomerAttributes, customerAttributes);


//            //newsletter
//            if (_customerSettings.NewsletterEnabled)
//            {
//                //save newsletter value
//                var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(model.Email, _storeContext.CurrentStore.Id);
//                if (newsletter != null)
//                {
//                    if (model.Newsletter)
//                    {
//                        newsletter.Active = true;
//                        _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);
//                    }
//                    //else
//                    //{
//                    //When registering, not checking the newsletter check box should not take an existing email address off of the subscription list.
//                    //_newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
//                    //}
//                }
//                else
//                {
//                    if (model.Newsletter)
//                    {
//                        _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
//                        {
//                            NewsLetterSubscriptionGuid = Guid.NewGuid(),
//                            Email = model.Email,
//                            Active = true,
//                            StoreId = _storeContext.CurrentStore.Id,
//                            CreatedOnUtc = DateTime.UtcNow
//                        });
//                    }
//                }
//            }

//            //password
//            if (!String.IsNullOrWhiteSpace(model.Password))
//            {
//                var changePassRequest = new ChangePasswordRequest(model.Email, false, _customerSettings.DefaultPasswordFormat, model.Password);
//                var changePassResult = _customerRegistrationService.ChangePassword(changePassRequest);
//                if (!changePassResult.Success)
//                {
//                    foreach (var changePassError in changePassResult.Errors)
//                        ErrorNotification(changePassError);
//                }
//            }            

//            //add to 'Registered' role
//            var registeredRole = _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Registered);
//            if (registeredRole == null)
//                throw new NopException("'Registered' role could not be loaded");
//            customer.CustomerRoles.Add(registeredRole);

//            _customerService.UpdateCustomer(customer);         

//            //activity log
//            _customerActivityService.InsertActivity("AddNewCustomer", _localizationService.GetResource("ActivityLog.AddNewCustomer"), customer.Id);

//            //SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Added"));            
            
//        }

//        #endregion

//        #region Actions
//        [AdminAuthorize]
//        public ActionResult Configure()
//        {
//            return View("~/Plugins/Widgets.MobileLogin/Views/MobileLogin/Configure.cshtml");
//        }

//        [AdminAuthorize]
//        public ActionResult Settings()
//        {
//            var model = new MobileLoginSettingsModel();
//            var mobileLoginSettings = _settingService.LoadSetting<MobileLoginSettings>();
//            model.CountryCode = mobileLoginSettings.CountryCode;
//            model.SmsGatewayUserName = mobileLoginSettings.SmsGatewayUserName;
//            model.SmsGatewayPassword = mobileLoginSettings.SmsGatewayPassword;

//            return View("~/Plugins/Widgets.MobileLogin/Views/MobileLogin/Settings.cshtml", model);
//        }

//        [HttpPost]
//        [AdminAuthorize]
//        [FormValueRequired("save")]
//        public ActionResult Settings(MobileLoginSettingsModel model)
//        {
//            var settings = new MobileLoginSettings()
//            {
//                CountryCode = model.CountryCode,
//                SmsGatewayUserName = model.SmsGatewayUserName,
//                SmsGatewayPassword = model.SmsGatewayPassword
//            };

//            _settingService.SaveSetting(settings);
//            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

//            //redisplay the form
//            return Settings();
//        }

//        //[NopHttpsRequirement(SslRequirement.Yes)]
//        //available even when a store is closed
//        //[StoreClosed(true)]
//        //available even when navigation is not allowed
//        //[PublicStoreAllowNavigation(true)]
//        public ActionResult Login(bool? checkoutAsGuest)
//        {
//            var model = new LoginModel();
//            model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
//            model.CheckoutAsGuest = checkoutAsGuest.GetValueOrDefault();

//            return View("~/Plugins/Widgets.MobileLogin/Views/MobileLogin/Login.cshtml", model);
//        }

//        [HttpPost]
//        public ActionResult Login(LoginModel model, string returnUrl)
//        {
//            if (model.LoginType == "Email")
//            {
//                ModelState.Remove("MobileNumber");

//                if (ModelState.IsValid)
//                {
//                    var redirectPath = LoginWithEmail(model, returnUrl);
//                    if (redirectPath == "HomePage")
//                    {
//                        return RedirectToRoute("HomePage");
//                    }
//                    else if (redirectPath != "" && redirectPath != "HomePage")
//                    {
//                        return Redirect(returnUrl);
//                    }
//                }
//            }
//            else
//            {
//                ModelState.Remove("Email");
//                if (ModelState.IsValid)
//                {
//                    var redirectStep2 = LoginWithMobileNumber(model);
//                    if (redirectStep2)
//                    {
//                        if (String.IsNullOrEmpty(returnUrl))
//                        {
//                            return RedirectToRoute("Plugin.Widgets.MobileLogin.LoginStep2");
//                        }

//                        return RedirectToRoute("Plugin.Widgets.MobileLogin.LoginStep2", new { returnUrl = returnUrl });
//                    }
//                    else
//                    {
//                        ModelState.AddModelError("", "Something went wrong. Please Try again.");
//                    }
//                }
//            }

//            return View("~/Plugins/Widgets.MobileLogin/Views/MobileLogin/Login.cshtml", model);
//        }

//        public ActionResult LoginStep2(string returnUrl)
//        {
//            if (_httpContext.Session["MobileNumber"] == null)
//            {
//                return RedirectToRoute("login");
//            }
//            var mobileNumber = _httpContext.Session["MobileNumber"].ToString();

//            var model = new MobileLoginStep2Model();

//            var customer = _mobileLoginService.GetCustomerByMobileNumber(mobileNumber);
//            if (customer != null && customer.IsRegistered())
//            {
//                _authenticationService.SignIn(customer, true);
//                UpdateDataOnLogin(customer);

//                if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
//                    return RedirectToRoute("HomePage");

//                return Redirect(returnUrl);
//            }
//            else
//            {
//                model.CustomerType = "New";
//            }
//            return View("~/Plugins/Widgets.MobileLogin/Views/MobileLogin/LoginStep2.cshtml", model);
//        }

//        [HttpPost]
//        public ActionResult LoginStep2(MobileLoginStep2Model model, string returnUrl)
//        {
//            if (_httpContext.Session["MobileNumber"] == null)
//            {
//                return RedirectToRoute("login");
//            }
//            ModelState.Remove("OTP");

//            var mobileNumber = _httpContext.Session["MobileNumber"].ToString();
//            if (ModelState.IsValid)
//            {
//                //string otp = string.Empty;
//                //var isValidToken = false;
//                //var mobileLoginCustomer = _mobileLoginService.GetByMobileNumber(mobileNumber);
//                //if (mobileLoginCustomer != null)
//                //{
//                //    otp = mobileLoginCustomer.Token;
//                //    isValidToken = mobileLoginCustomer.IsTokenValid;
//                //}
//                //else
//                //{
//                //    var guestMLC = _mobileLoginService.GetByCustomerId(_workContext.CurrentCustomer.Id);
//                //    otp = guestMLC.Token;
//                //    isValidToken = guestMLC.IsTokenValid;
//                //}

//                //if (model.OTP == otp && isValidToken)
//                //{
//                    #region customer login or registration
//                    var customer = _mobileLoginService.GetCustomerByMobileNumber(mobileNumber);

//                    if (customer == null)
//                    {
//                        ModelState.AddModelError("Email", "Customer not found");
//                        return View("~/Plugins/Widgets.MobileLogin/Views/MobileLogin/LoginStep2.cshtml", model);
//                    }

//                if (customer.IsGuest())
//                    {
//                        var existingCustomer = _customerService.GetCustomerByEmail(model.Email);
//                        if (existingCustomer != null)
//                        {
//                            ModelState.AddModelError("", "The specified email already exists.");
//                        }
//                        else
//                        {
//                            var customerModel = new RegisterModel();
//                            customerModel.FirstName = model.Name;
//                            //customerModel.LastName = model.LastName;
//                            if (!string.IsNullOrEmpty(model.Email))
//                            {
//                                customerModel.Email = model.Email;
//                            } else
//                            {
//                                customerModel.Email = mobileNumber + "@gmail.com";
//                            }
                            
//                            customerModel.MobileNumber = mobileNumber;
//                            var registerResult = RegisterCustomer(customerModel);

//                            if (!String.IsNullOrEmpty(returnUrl))
//                            {
//                                return Redirect(returnUrl);
//                            }
//                            if (registerResult)
//                            {
//                                var redirectUrl = Url.RouteUrl("RegisterResult", new { resultId = (int)UserRegistrationType.Standard });
//                                return Redirect(redirectUrl);
//                            }
//                        }

//                    }
//                    else
//                    {
//                        UpdateDataOnLogin(customer);

//                        if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
//                            return RedirectToRoute("HomePage");

//                        return Redirect(returnUrl);
//                    }
//                    #endregion

//                //}
//                //else
//                //{
//                //    if (!isValidToken)
//                //        ModelState.AddModelError("", "OTP is not valid.");
//                //    else
//                //        ModelState.AddModelError("", "OTP is not correct.");
//                //}

//            }
//            return View("~/Plugins/Widgets.MobileLogin/Views/MobileLogin/LoginStep2.cshtml", model);
//        }

//        private void UpdateDataOnLogin(Customer customer)
//        {
//            //migrate shopping cart
//            _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);

//            //sign in new customer
//            _authenticationService.SignIn(customer, false);

//            //raise event       
//            _eventPublisher.Publish(new CustomerLoggedinEvent(customer));

//            //activity log
//            _customerActivityService.InsertActivity("PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"), customer);
//        }

//        public ActionResult MobileLoginData()
//        {
//            //var model = _mobileLoginService.getAllMobileLoginCustomers();            
//            var model = new CustomerListModel();

//            return View("~/Plugins/Widgets.MobileLogin/Views/MobileLogin/List.cshtml", model);
//        }

//        [HttpPost]
//        public ActionResult MobileLoginCustomerList(DataSourceRequest command, CustomerListModel model)
//        {
//            var customers = _mobileLoginService.getAllMobileLoginCustomers(model.SearchEmail, model.SearchName, model.SearchMobileNumber, pageIndex: command.Page - 1,
//                pageSize: command.PageSize);

//            var gridModel = new DataSourceResult();
//            gridModel.Data = customers.Select(x =>
//            {
//                var mobileLoginRecordModel = ToMobileLoginRecordModel(x);
//                return mobileLoginRecordModel;
//            });
//            gridModel.Total = customers.TotalCount;

//            return Json(gridModel);
//        }
        
//        [HttpPost]
//        public bool RequestOTP(string email)
//        {            
//            var customer = _customerService.GetCustomerByEmail(email);
//            var mobileLoginCustomer = new Domain.MobileLoginCustomer();                      

//            if (customer == null)
//            {
//                customer = _workContext.CurrentCustomer;
//            }
             
//            mobileLoginCustomer = _mobileLoginService.GetByCustomerId(customer.Id);            

//            var otp = "";
//            if (mobileLoginCustomer != null)
//            {
//                otp = GenerateOTP(4);                
//                mobileLoginCustomer.Token = otp;
//                mobileLoginCustomer.IsTokenValid = true;
//                mobileLoginCustomer.UpdatedOnUtc = DateTime.UtcNow;
//                _mobileLoginService.Update(mobileLoginCustomer);                
//            }

//            bool statusResult;

//            #region SMS
//            string smsText = otp;

//            if (smsText == null)
//            {
//                smsText = "Test otp";
//            }
//            smsText = "Your OTP for mobile login in " + _storeContext.CurrentStore.CompanyName + " is " +
//                      smsText + ". This OTP will valid upto next 20 minutes.";
            
//            string smsReceiverPhoneNumber = "88" + mobileLoginCustomer.MobileNumber;
        
//            String sid = "SplendorIT";
//            String user = "splendorit";
//            String pass = "smsmgt";
//            string URI = "http://sms.sslwireless.com/pushapi/dynamic/server.php";
//            string myParameters = "user=" + user + 
//                "&pass=" + pass + 
//                "&sms[0][0]=" + smsReceiverPhoneNumber  + "&sms[0][1]=" + System.Web.HttpUtility.UrlEncode(smsText) + 
//                "&sms[0][2]=" + mobileLoginCustomer.MobileNumber + "&sid=" + sid;            

//            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URI);
//            byte[] data = Encoding.ASCII.GetBytes(myParameters);
//            request.Method = "POST";
//            request.ContentType = "application/x-www-form-urlencoded";
//            request.ContentLength = data.Length;
//            using (Stream stream = request.GetRequestStream()) {
//                stream.Write(data, 0, data.Length);
//            }
//            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
//            String responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
//            Response.Write(responseString);
                        
//            if (response.StatusCode != HttpStatusCode.OK)
//                statusResult = false;
//            else
//                statusResult = true;
            
//            #endregion

//            //return Json(new { status = statusResult });            
//            return statusResult;
//        }

//        public ActionResult Create()
//        {                        
//            var model = new MobileRegistrationModel();            
//            return View("~/Plugins/Widgets.MobileLogin/Views/MobileLogin/Create.cshtml", model);
//        }

//        [HttpPost]
//        public ActionResult Create(MobileRegistrationModel model)
//        {                        
//            if (ModelState.IsValid)
//            {
////                string otp = string.Empty;
                
//                var mobileLoginCustomer = _mobileLoginService.GetByMobileNumber(model.MobileNumber);                
                
//                #region customer registration
////                var customer = _mobileLoginService.GetCustomerByMobileNumber(model.MobileNumber);

//                if (mobileLoginCustomer == null)
//                {
//                    var existingCustomer = _customerService.GetCustomerByEmail(model.Email);
//                    if (existingCustomer != null)
//                    {
//                        ModelState.AddModelError("", "The specified email already exists.");
//                    }
//                    else
//                    {
//                        var customerModel = new RegisterModel();
//                        customerModel.FirstName = model.Name;
//                        //customerModel.LastName = model.LastName;
//                        customerModel.Email = model.Email;
//                        customerModel.MobileNumber = model.MobileNumber;
//                        RegisterCustomerFromAdminPanel(customerModel);                        
//                    }
//                }
//                else
//                {
//                    ModelState.AddModelError("", "Mobile number already exists.");
//                }
//                #endregion

                

//            }
//            return View("~/Plugins/Widgets.MobileLogin/Views/MobileLogin/LoginStep2.cshtml", model);
//        }


//        #endregion
//    }
//}
