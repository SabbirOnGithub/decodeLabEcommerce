using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models
{
    public class AffiliateModel
    {

        public int Id { get; set; }
        public int CustomerId { get; set; }

        public string Url { get; set; }

        public string FriendlyUrlName { get; set; }

        public string BKashNumber { get; set; }

        public bool Active { get; set; }

        public bool Applied { get; set; }

        public bool Deleted { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Company { get; set; }

        public int? CountryId { get; set; }

        public string CountryName { get; set; }

        public int? StateProvinceId { get; set; }

        public string StateProvinceName { get; set; }

        public string City { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string ZipPostalCode { get; set; }

        public string PhoneNumber { get; set; }

        public string FaxNumber { get; set; }

        public string AddressHtml { get; set; }
    }
}
