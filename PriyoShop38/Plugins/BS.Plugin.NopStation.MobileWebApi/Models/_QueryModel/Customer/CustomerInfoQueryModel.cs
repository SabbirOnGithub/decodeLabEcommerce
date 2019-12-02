using System;
using System.Collections.Generic;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Customer;

namespace BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Customer
{
    public partial class CustomerInfoQueryModel : BaseResponse
    {
        public CustomerInfoQueryModel()
        {
           FormValues= new List<KeyValueApi>();
        }

        
        public string Email { get; set; }

      
        public string Username { get; set; }

        public string Gender { get; set; }

       
        public string FirstName { get; set; }
       
        public string LastName { get; set; }
       
        public int? DateOfBirthDay { get; set; }
     
        public int? DateOfBirthMonth { get; set; }
     
        public int? DateOfBirthYear { get; set; }
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

       
        public string Company { get; set; }

       
        public string StreetAddress { get; set; }

        public string StreetAddress2 { get; set; }

      
        public string ZipPostalCode { get; set; }
       
        public string City { get; set; }

       
        public int CountryId { get; set; }
      
       
        public int StateProvinceId { get; set; }
       
       
        public string Phone { get; set; }

      
        
        public string Fax { get; set; }

        
        public bool Newsletter { get; set; }

        //preferences
     
        public string Signature { get; set; }

        //EU VAT
        
        public string VatNumber { get; set; }
        public string VatNumberStatusNote { get; set; }
        public bool DisplayVatNumber { get; set; }
        public List<KeyValueApi> FormValues { get; set; }

        #region Nested classes

        
        #endregion
    }
}