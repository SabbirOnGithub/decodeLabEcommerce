using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Customer
{
    public partial class CustomerInfoResponseModel : BaseResponse
    {
        public CustomerInfoResponseModel()
        {
            this.AvailableCountries = new List<SelectListItem>();
            this.AvailableStates = new List<SelectListItem>();
            this.CustomerAttributes = new List<CustomerAttributeModel>();
        }

        
        public string Email { get; set; }

        public bool CheckUsernameAvailabilityEnabled { get; set; }
        public bool AllowUsersToChangeUsernames { get; set; }
        public bool UsernamesEnabled { get; set; }
       
        public string Username { get; set; }

        //form fields & properties
        public bool GenderEnabled { get; set; }
       
        public string Gender { get; set; }

       
        public string FirstName { get; set; }
       
        public string LastName { get; set; }


        public bool DateOfBirthEnabled { get; set; }
       
        public int? DateOfBirthDay { get; set; }
     
        public int? DateOfBirthMonth { get; set; }
     
        public int? DateOfBirthYear { get; set; }
        public bool DateOfBirthRequired { get; set; }
        public DateTime? ParseDateOfBirth()
        {
            if (!DateOfBirthYear.HasValue || !DateOfBirthMonth.HasValue || !DateOfBirthDay.HasValue)
                return null;

            DateTime? dateOfBirth = null;
            try
            {
                dateOfBirth = new DateTime(DateOfBirthYear.Value, DateOfBirthMonth.Value, DateOfBirthDay.Value);
            }
            catch { }
            return dateOfBirth;
        }

        public bool CompanyEnabled { get; set; }
        public bool CompanyRequired { get; set; }
        public string Company { get; set; }

        public bool StreetAddressEnabled { get; set; }
        public bool StreetAddressRequired { get; set; }
        public string StreetAddress { get; set; }

        public bool StreetAddress2Enabled { get; set; }
        public bool StreetAddress2Required { get; set; }
        public string StreetAddress2 { get; set; }

        public bool ZipPostalCodeEnabled { get; set; }
        public bool ZipPostalCodeRequired { get; set; }
        public string ZipPostalCode { get; set; }
        public bool CityEnabled { get; set; }
        public bool CityRequired { get; set; }
        public string City { get; set; }

        public bool CountryEnabled { get; set; }
        public bool CountryRequired { get; set; }
        public int CountryId { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }

        public bool StateProvinceEnabled { get; set; }
        public bool StateProvinceRequired { get; set; }
       
        public int StateProvinceId { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }

        public bool PhoneEnabled { get; set; }
        public bool PhoneRequired { get; set; }
       
        public string Phone { get; set; }

        public bool FaxEnabled { get; set; }
        public bool FaxRequired { get; set; }
        
        public string Fax { get; set; }

        public bool NewsletterEnabled { get; set; }
      
        public bool Newsletter { get; set; }

        //preferences
        public bool SignatureEnabled { get; set; }
      
        public string Signature { get; set; }

        //EU VAT
        
        public string VatNumber { get; set; }
        public string VatNumberStatusNote { get; set; }
        public bool DisplayVatNumber { get; set; }

        public int RewardPoint { get; set; }

        public string PictureUrl { get; set; }

        public string NewReferCode { get; set; }
        public string UsedReferCode { get; set; }
        public bool IsReferCodeUsed { get; set; }

        public IList<CustomerAttributeModel> CustomerAttributes { get; set; }

        #region Nested classes

        public partial class AssociatedExternalAuthModel : BaseNopEntityModel
        {
            public string Email { get; set; }

            public string ExternalIdentifier { get; set; }

            public string AuthMethodName { get; set; }
        }
        
        #endregion
    }
}