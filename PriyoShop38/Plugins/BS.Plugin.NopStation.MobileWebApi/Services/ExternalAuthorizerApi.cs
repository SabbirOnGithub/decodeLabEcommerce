using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public class ExternalAuthorizerApi :  IExternalAuthorizerApi
    {
        #region Fields
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly IWorkContext _workContext;
        private readonly CustomerSettings _customerSettings;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly LocalizationSettings _localizationSettings;
        #endregion

        #region Constructor
        public ExternalAuthorizerApi(IAuthenticationService authenticationService,
            IOpenAuthenticationService openAuthenticationService,
            IGenericAttributeService genericAttributeService,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IStoreContext storeContext,
            CustomerSettings customerSettings,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            IShoppingCartService shoppingCartService,
            IWorkflowMessageService workflowMessageService,
            IEventPublisher eventPublisher,
            LocalizationSettings localizationSettings)
        {
            this._openAuthenticationService = openAuthenticationService;
            this._workContext = workContext;
            this._customerSettings = customerSettings;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._storeContext = storeContext;
            this._customerRegistrationService = customerRegistrationService;
            this._genericAttributeService = genericAttributeService;
            this._authenticationService = authenticationService;
            this._workflowMessageService = workflowMessageService;
            this._eventPublisher = eventPublisher;
            this._shoppingCartService = shoppingCartService;
            this._customerActivityService = customerActivityService;
            this._localizationService = localizationService;
            this._localizationSettings = localizationSettings;
        }
        #endregion

        #region Utilities
        private bool AccountAlreadyExists(Customer userFound, Customer userLoggedIn)
        {
            return userFound != null && userLoggedIn != null;
        }

        private bool AccountIsAssignedToLoggedOnAccount(Customer userFound, Customer userLoggedIn)
        {
            return userFound.Id.Equals(userLoggedIn.Id);
        }

        private bool AccountDoesNotExistAndUserIsNotLoggedOn(Customer userFound, Customer userLoggedIn)
        {
            return userFound == null && userLoggedIn == null;
        }

        private bool AutoRegistrationIsEnabled()
        {
            return _customerSettings.UserRegistrationType != UserRegistrationType.Disabled && _externalAuthenticationSettings.AutoRegisterEnabled;
        }

        private bool RegistrationIsEnabled()
        {
            return _customerSettings.UserRegistrationType != UserRegistrationType.Disabled && !_externalAuthenticationSettings.AutoRegisterEnabled;
        }
        #endregion

        #region Method
        public AuthorizationResultApi Authorize(OpenAuthenticationParameters parameters)
        {
            var userFound = _openAuthenticationService.GetUser(parameters);

            var userLoggedIn = _workContext.CurrentCustomer.IsRegistered() ? _workContext.CurrentCustomer : null;

            if (AccountAlreadyExists(userFound, userLoggedIn))
            {
                if (AccountIsAssignedToLoggedOnAccount(userFound, userLoggedIn))
                {
                    // The person is trying to log in as himself.. bit weird
                    return new AuthorizationResultApi(OpenAuthenticationStatus.Authenticated, userFound.Id);
                }

                var result = new AuthorizationResultApi(OpenAuthenticationStatus.Error,0);
                result.AddError("Account is already assigned");
                return result;
            }
            if (AccountDoesNotExistAndUserIsNotLoggedOn(userFound, userLoggedIn))
            {
                //ExternalAuthorizerHelper.StoreParametersForRoundTrip(parameters);

                if (AutoRegistrationIsEnabled())
                {
                    #region Register user

                    var currentCustomer = _workContext.CurrentCustomer;
                    var details = new RegistrationDetails(parameters);
                    var randomPassword = CommonHelper.GenerateRandomDigitCode(20);


                    bool isApproved =
                        //standard registration
                        (_customerSettings.UserRegistrationType == UserRegistrationType.Standard) ||
                        //skip email validation?
                        (_customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation &&
                         !_externalAuthenticationSettings.RequireEmailValidation);

                    var registrationRequest = new CustomerRegistrationRequest(currentCustomer,
                        details.EmailAddress,
                        _customerSettings.UsernamesEnabled ? details.UserName : details.EmailAddress,
                        randomPassword,
                        PasswordFormat.Clear,
                        _storeContext.CurrentStore.Id,
                        isApproved);
                    var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
                    if (registrationResult.Success)
                    {
                        //store other parameters (form fields)
                        if (!String.IsNullOrEmpty(details.FirstName))
                            _genericAttributeService.SaveAttribute(currentCustomer, SystemCustomerAttributeNames.FirstName, details.FirstName);
                        if (!String.IsNullOrEmpty(details.LastName))
                            _genericAttributeService.SaveAttribute(currentCustomer, SystemCustomerAttributeNames.LastName, details.LastName);


                        userFound = currentCustomer;
                        _openAuthenticationService.AssociateExternalAccountWithUser(currentCustomer, parameters);
                        //ExternalAuthorizerHelper.RemoveParameters();

                        //code below is copied from CustomerController.Register method

                        //authenticate
                        //if (isApproved)
                        //    _authenticationService.SignIn(userFound ?? userLoggedIn, false);

                        //notifications
                        if (_customerSettings.NotifyNewCustomerRegistration)
                            _workflowMessageService.SendCustomerRegisteredNotificationMessage(currentCustomer, _localizationSettings.DefaultAdminLanguageId);

                        //raise event       
                        _eventPublisher.Publish(new CustomerRegisteredEvent(currentCustomer));

                        if (isApproved)
                        {
                            //standard registration
                            //or
                            //skip email validation

                            //send customer welcome message
                            _workflowMessageService.SendCustomerWelcomeMessage(currentCustomer, _workContext.WorkingLanguage.Id);

                            //result
                            return new AuthorizationResultApi(OpenAuthenticationStatus.AutoRegisteredStandard, userFound.Id);
                        }
                        else if (_customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation)
                        {
                            //email validation message
                            _genericAttributeService.SaveAttribute(currentCustomer, SystemCustomerAttributeNames.AccountActivationToken, Guid.NewGuid().ToString());
                            _workflowMessageService.SendCustomerEmailValidationMessage(currentCustomer, _workContext.WorkingLanguage.Id);

                            //result
                            return new AuthorizationResultApi(OpenAuthenticationStatus.AutoRegisteredEmailValidation, 0);
                        }
                        else if (_customerSettings.UserRegistrationType == UserRegistrationType.AdminApproval)
                        {
                            //result
                            return new AuthorizationResultApi(OpenAuthenticationStatus.AutoRegisteredAdminApproval, 0);
                        }
                    }
                    else
                    {
                        //ExternalAuthorizerHelper.RemoveParameters();

                        var result = new AuthorizationResultApi(OpenAuthenticationStatus.Error,0);
                        foreach (var error in registrationResult.Errors)
                            result.AddError(string.Format(error));
                        return result;
                    }

                    #endregion
                }
                else if (RegistrationIsEnabled())
                {
                    return new AuthorizationResultApi(OpenAuthenticationStatus.AssociateOnLogon, 0);
                }
                else
                {
                    //ExternalAuthorizerHelper.RemoveParameters();

                    var result = new AuthorizationResultApi(OpenAuthenticationStatus.Error, 0);
                    result.AddError("Registration is disabled");
                    return result;
                }
            }
            if (userFound == null)
            {
                _openAuthenticationService.AssociateExternalAccountWithUser(userLoggedIn, parameters);
            }

            //migrate shopping cart
            //_shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, userFound ?? userLoggedIn, true);
            //authenticate
            //_authenticationService.SignIn(userFound ?? userLoggedIn, false);
            //raise event       
            _eventPublisher.Publish(new CustomerLoggedinEvent(userFound ?? userLoggedIn));
            //activity log
            _customerActivityService.InsertActivity("PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"),
                userFound ?? userLoggedIn);

            return new AuthorizationResultApi(OpenAuthenticationStatus.Authenticated, userFound.Id);
        }
        #endregion
    }
}
