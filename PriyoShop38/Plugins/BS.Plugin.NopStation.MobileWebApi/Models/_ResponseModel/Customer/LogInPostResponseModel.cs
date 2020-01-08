using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Customer
{
    public class LogInPostResponseModel :BaseResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string StreetAddress { get; set; }
        public string StreetAddress2 { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string CountryId { get; set; }
        public int StateProvinceId { get; set; }
        public int CustomerId { get; set; }
        public string Token { get; set; }
        public string PictureUrl { get; set; }
        public int AffiliateId { get; set; }
        public string AffiliateUrl { get; set; }
        public string AffiliateFriendlyName { get; set; }
        public decimal WalletAmount { get; set; }
    }
}
