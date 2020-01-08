using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;
using System.Web.WebPages;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Customer;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Product;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Models.Common;

namespace BS.Plugin.NopStation.MobileWebApi.Extensions
{
    public static class ValidationExtension
    {

        #region Utility
        private static bool IsNotValidEmail(this string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return !Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        private static bool IsNull(this object value)
        {
            return value == null;
        }

        private static bool IsNotRightLength(this string value, int min,int max)
        {
            return !(value.Length >= min && value.Length <= max);
        }

        private static bool IsNotEqual(this string value, string checkValue)
        {
            return !value.Equals(checkValue);
        }
        private static void WithMessage(this bool flag, ModelStateDictionary modelState, string message)
        {
            if (flag == true)
            {
                modelState.AddModelError("", message);
            }
        }
        #endregion
        #region validation helpers
        public static void AddressValidator(ModelStateDictionary modelState, Address model,ILocalizationService localizationService, AddressSettings addressSettings,IStateProvinceService stateProvinceService)
        {
            //model.FirstName.IsEmpty()
            //    .WithMessage(modelState, localizationService.GetResource("Address.Fields.FirstName.Required"));
            model.FirstName.IsEmpty()
                .WithMessage(modelState, "Name is Required");
            //model.LastName.IsEmpty()
            //    .WithMessage(modelState, localizationService.GetResource("Address.Fields.LastName.Required"));
            model.Email.IsEmpty().
                WithMessage(modelState, localizationService.GetResource("Address.Fields.Email.Required"));
            model.Email.IsNotValidEmail().
                WithMessage(modelState, localizationService.GetResource("Common.WrongEmail"));

            if (addressSettings.CountryEnabled)
            {

                model.CountryId.IsNull().
                    WithMessage(modelState,localizationService.GetResource("Address.Fields.Country.Required"));
                model.CountryId.Equals(0)
                    .WithMessage(modelState, localizationService.GetResource("Address.Fields.Country.Required"));
            }
            if (addressSettings.CountryEnabled && addressSettings.StateProvinceEnabled)
            {

                //does selected country has states?
                var countryId = model.CountryId.HasValue ? model.CountryId.Value : 0;
                var hasStates = stateProvinceService.GetStateProvincesByCountryId(countryId).Count > 0;

                if (hasStates)
                {
                    //if yes, then ensure that state is selected
                    if (!model.StateProvinceId.HasValue || model.StateProvinceId.Value == 0)
                    {
                        modelState.AddModelError("StateProvinceId", localizationService.GetResource("Address.Fields.StateProvince.Required"));
                    }
                }

            }
            if (addressSettings.CompanyRequired && addressSettings.CompanyEnabled)
            {
                model.Company.IsEmpty()
                    .WithMessage(modelState, localizationService.GetResource("Address.Fields.Company.Required"));
            }
            if (addressSettings.StreetAddressRequired && addressSettings.StreetAddressEnabled)
            {
                model.Address1.IsEmpty().
                    WithMessage(modelState,localizationService.GetResource("Address.Fields.StreetAddress.Required"));
             
            }
            if (addressSettings.StreetAddress2Required && addressSettings.StreetAddress2Enabled)
            {
                model.Address2.IsEmpty().WithMessage(modelState,localizationService.GetResource("Address.Fields.StreetAddress2.Required"));
               
            }
            //if (addressSettings.ZipPostalCodeRequired && addressSettings.ZipPostalCodeEnabled)
            //{
            //    model.ZipPostalCode.IsEmpty().WithMessage(modelState, localizationService.GetResource("Address.Fields.ZipPostalCode.Required"));
            //}
            //if (addressSettings.CityRequired && addressSettings.CityEnabled)
            //{
            //    model.City.IsEmpty().WithMessage(modelState, localizationService.GetResource("Address.Fields.City.Required"));
            //}
            if (addressSettings.PhoneRequired && addressSettings.PhoneEnabled)
            {
                model.PhoneNumber.IsEmpty().WithMessage(modelState,localizationService.GetResource("Address.Fields.Phone.Required"));
            }
            if (addressSettings.FaxRequired && addressSettings.FaxEnabled)
            {
                model.FaxNumber.IsEmpty().WithMessage(modelState, localizationService.GetResource("Address.Fields.Fax.Required"));
            }
        }



        public static void CustomerInfoValidator(ModelStateDictionary modelState,CustomerInfoQueryModel model, ILocalizationService localizationService,
            IStateProvinceService stateProvinceService, 
            CustomerSettings customerSettings)
        {
            model.FirstName.IsEmpty()
                .WithMessage(modelState, localizationService.GetResource("Address.Fields.FirstName.Required"));
            model.LastName.IsEmpty()
                .WithMessage(modelState, localizationService.GetResource("Address.Fields.LastName.Required"));
            model.Email.IsEmpty().
                WithMessage(modelState, localizationService.GetResource("Address.Fields.Email.Required"));
            model.Email.IsNotValidEmail().
                WithMessage(modelState, localizationService.GetResource("Common.WrongEmail"));

            if (customerSettings.UsernamesEnabled && customerSettings.AllowUsersToChangeUsernames)
            {
                model.Username.IsEmpty().WithMessage(modelState,localizationService.GetResource("Account.Fields.Username.Required"));
            }

            //form fields
            if (customerSettings.CountryEnabled && customerSettings.CountryRequired)
            {
                model.CountryId.Equals(0)
                    .WithMessage(modelState, localizationService.GetResource("Address.Fields.Country.Required"));
            }
            if (customerSettings.CountryEnabled && 
                customerSettings.StateProvinceEnabled &&
                customerSettings.StateProvinceRequired)
            {
               
                var hasStates = stateProvinceService.GetStateProvincesByCountryId(model.CountryId).Count > 0;

                if (hasStates)
                {
                    //if yes, then ensure that state is selected
                    if (model.StateProvinceId == 0)
                    {
                        modelState.AddModelError("StateProvinceId", localizationService.GetResource("Address.Fields.StateProvince.Required"));
                    }
                }
            }
            if (customerSettings.DateOfBirthRequired && customerSettings.DateOfBirthEnabled)
            {
                 var dateOfBirth = model.ParseDateOfBirth();
                    if (dateOfBirth == null)
                    {
                        modelState.AddModelError("", localizationService.GetResource("Account.Fields.DateOfBirth.Required"));
                    }
                    
            }
            
            if (customerSettings.CompanyRequired && customerSettings.CompanyEnabled)
            {
                model.Company.IsEmpty()
                    .WithMessage(modelState, localizationService.GetResource("Address.Fields.Company.Required"));
            }
            if (customerSettings.StreetAddressRequired && customerSettings.StreetAddressEnabled)
            {
                model.StreetAddress.IsEmpty().
                    WithMessage(modelState, localizationService.GetResource("Address.Fields.StreetAddress.Required"));

            }
            if (customerSettings.StreetAddress2Required && customerSettings.StreetAddress2Enabled)
            {
                model.StreetAddress2.IsEmpty().WithMessage(modelState, localizationService.GetResource("Address.Fields.StreetAddress2.Required"));

            }
            if (customerSettings.ZipPostalCodeRequired && customerSettings.ZipPostalCodeEnabled)
            {
                model.ZipPostalCode.IsEmpty().WithMessage(modelState, localizationService.GetResource("Address.Fields.ZipPostalCode.Required"));
            }
            if (customerSettings.CityRequired && customerSettings.CityEnabled)
            {
                model.City.IsEmpty().WithMessage(modelState, localizationService.GetResource("Address.Fields.City.Required"));
            }
            if (customerSettings.PhoneRequired && customerSettings.PhoneEnabled)
            {
                model.Phone.IsEmpty().WithMessage(modelState, localizationService.GetResource("Address.Fields.Phone.Required"));
            }
            if (customerSettings.FaxRequired && customerSettings.FaxEnabled)
            {
                model.Fax.IsEmpty().WithMessage(modelState, localizationService.GetResource("Address.Fields.Fax.Required"));
            }
        }

        public static void RegisterValidator(ModelStateDictionary modelState, RegisterQueryModel model, ILocalizationService localizationService, 
            IStateProvinceService stateProvinceService,
            CustomerSettings customerSettings)
        {
            model.FirstName.IsEmpty()
                .WithMessage(modelState, localizationService.GetResource("Address.Fields.FirstName.Required"));
            model.LastName.IsEmpty()
                .WithMessage(modelState, localizationService.GetResource("Address.Fields.LastName.Required"));
            model.Email.IsEmpty().
                WithMessage(modelState, localizationService.GetResource("Address.Fields.Email.Required"));
            model.Email.IsNotValidEmail().
                WithMessage(modelState, localizationService.GetResource("Common.WrongEmail"));

            if (customerSettings.UsernamesEnabled && customerSettings.AllowUsersToChangeUsernames)
            {
                model.Username.IsEmpty().WithMessage(modelState, localizationService.GetResource("Account.Fields.Username.Required"));
            }
            
           
            model.Password.IsEmpty().WithMessage(modelState,localizationService.GetResource("Account.Fields.Password.Required"));
            model.Password.IsNotRightLength(customerSettings.PasswordMinLength, 999).WithMessage(modelState, string.Format(localizationService.GetResource("Account.Fields.Password.LengthValidation"), customerSettings.PasswordMinLength));
            model.ConfirmPassword.IsEmpty().WithMessage(modelState,localizationService.GetResource("Account.Fields.ConfirmPassword.Required"));
            model.ConfirmPassword.IsNotEqual(model.Password).WithMessage(modelState,localizationService.GetResource("Account.Fields.Password.EnteredPasswordsDoNotMatch"));

            if (customerSettings.CountryEnabled && customerSettings.CountryRequired)
            {
                model.CountryId.Equals(0)
                    .WithMessage(modelState, localizationService.GetResource("Address.Fields.Country.Required"));
            }
            if (customerSettings.CountryEnabled &&
                customerSettings.StateProvinceEnabled &&
                customerSettings.StateProvinceRequired)
            {

                var hasStates = stateProvinceService.GetStateProvincesByCountryId(model.CountryId).Count > 0;

                if (hasStates)
                {
                    //if yes, then ensure that state is selected
                    if (model.StateProvinceId == 0)
                    {
                        modelState.AddModelError("StateProvinceId", localizationService.GetResource("Address.Fields.StateProvince.Required"));
                    }
                }
            }
            if (customerSettings.DateOfBirthRequired && customerSettings.DateOfBirthEnabled)
            {
                var dateOfBirth = model.ParseDateOfBirth();
                if (dateOfBirth == null)
                {
                    modelState.AddModelError("", localizationService.GetResource("Account.Fields.DateOfBirth.Required"));
                }

            }

            if (customerSettings.CompanyRequired && customerSettings.CompanyEnabled)
            {
                model.Company.IsEmpty()
                    .WithMessage(modelState, localizationService.GetResource("Address.Fields.Company.Required"));
            }
            if (customerSettings.StreetAddressRequired && customerSettings.StreetAddressEnabled)
            {
                model.StreetAddress.IsEmpty().
                    WithMessage(modelState, localizationService.GetResource("Address.Fields.StreetAddress.Required"));

            }
            if (customerSettings.StreetAddress2Required && customerSettings.StreetAddress2Enabled)
            {
                model.StreetAddress2.IsEmpty().WithMessage(modelState, localizationService.GetResource("Address.Fields.StreetAddress2.Required"));

            }
            if (customerSettings.ZipPostalCodeRequired && customerSettings.ZipPostalCodeEnabled)
            {
                model.ZipPostalCode.IsEmpty().WithMessage(modelState, localizationService.GetResource("Address.Fields.ZipPostalCode.Required"));
            }
            if (customerSettings.CityRequired && customerSettings.CityEnabled)
            {
                model.City.IsEmpty().WithMessage(modelState, localizationService.GetResource("Address.Fields.City.Required"));
            }
            if (customerSettings.PhoneRequired && customerSettings.PhoneEnabled)
            {
                model.Phone.IsEmpty().WithMessage(modelState, localizationService.GetResource("Address.Fields.Phone.Required"));
            }
            if (customerSettings.FaxRequired && customerSettings.FaxEnabled)
            {
                model.Fax.IsEmpty().WithMessage(modelState, localizationService.GetResource("Address.Fields.Fax.Required"));
            }
        }

        public static void  LoginValidator(ModelStateDictionary modelState, LoginQueryModel model,ILocalizationService localizationService, CustomerSettings customerSettings)
        {
            if (!customerSettings.UsernamesEnabled)
            {
                //login by email
                model.Email.IsEmpty().WithMessage(modelState,localizationService.GetResource("Account.Login.Fields.Email.Required"));
                model.Email.IsNotValidEmail().WithMessage(modelState,localizationService.GetResource("Common.WrongEmail"));
            }
        }

        public static void WriteReviewValidator(ModelStateDictionary modelState, ProductReviewQueryModel model, ILocalizationService localizationService)
        {

            model.Title.IsEmpty().WithMessage(modelState,localizationService.GetResource("Reviews.Fields.Title.Required"));
            model.Title.IsNotRightLength(1, 200)
                .WithMessage(modelState,
                    string.Format(localizationService.GetResource("Reviews.Fields.Title.MaxLengthValidation"), 200));
            model.ReviewText.IsEmpty()
                .WithMessage(modelState, localizationService.GetResource("Reviews.Fields.ReviewText.Required"));
        }
        #endregion
    }
}
