using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Models.Common;
using Nop.Web.Validators.Affiliates;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Web.Models.Affiliates
{
    [Validator(typeof(AffiliateInfoValidator))]
    public class AffiliateInfoModel
    {
        public AffiliateInfoModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
        }


        [NopResourceDisplayName("Account.Affiliates.Fields.ID")]
        public int Id { get; set; }

        [NopResourceDisplayName("Account.Affiliates.Fields.URL")]
        public string Url { get; set; }

        [NopResourceDisplayName("Account.Affiliates.Fields.FriendlyUrlName")]
        [AllowHtml]
        public string FriendlyUrlName { get; set; }

        [NopResourceDisplayName("Account.Affiliates.Fields.BKash")]
        public string BKashNumber { get; set; }

        [NopResourceDisplayName("Account.Affiliates.Fields.Active")]
        public bool Active { get; set; }

        [NopResourceDisplayName("Account.Affiliates.Fields.Applied")]
        public bool Applied { get; set; }

        [NopResourceDisplayName("Account.Affiliates.Fields.Deleted")]
        public bool Deleted { get; set; }


        [NopResourceDisplayName("Account.Affiliates.Fields.FirstName")]
        [AllowHtml]
        public string FirstName { get; set; }

        [NopResourceDisplayName("Account.Affiliates.Fields.LastName")]
        [AllowHtml]
        public string LastName { get; set; }

        [NopResourceDisplayName("Account.Affiliates.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        [NopResourceDisplayName("Account.Affiliates.Fields.Company")]
        [AllowHtml]
        public string Company { get; set; }

        [NopResourceDisplayName("Account.Affiliates.Fields.Country")]
        public int? CountryId { get; set; }

        [NopResourceDisplayName("Account.Affiliates.Fields.Country")]
        [AllowHtml]
        public string CountryName { get; set; }

        [NopResourceDisplayName("Account.Affiliates.Fields.StateProvince")]
        public int? StateProvinceId { get; set; }

        [NopResourceDisplayName("Account.Affiliates.Fields.StateProvince")]
        [AllowHtml]
        public string StateProvinceName { get; set; }

        [NopResourceDisplayName("Account.Affiliates.Fields.City")]
        [AllowHtml]
        public string City { get; set; }

        [NopResourceDisplayName("Account.Affiliates.Fields.Address1")]
        [AllowHtml]
        public string Address1 { get; set; }

        [NopResourceDisplayName("Account.Affiliates.Fields.Address2")]
        [AllowHtml]
        public string Address2 { get; set; }

        [NopResourceDisplayName("Account.Affiliates.Fields.ZipPostalCode")]
        [AllowHtml]
        public string ZipPostalCode { get; set; }

        [NopResourceDisplayName("Account.Affiliates.Fields.PhoneNumber")]
        [AllowHtml]
        public string PhoneNumber { get; set; }

        [NopResourceDisplayName("Account.Affiliates.Fields.FaxNumber")]
        [AllowHtml]
        public string FaxNumber { get; set; }

        [NopResourceDisplayName("Admin.Address")]
        public string AddressHtml { get; set; }

        public IList<SelectListItem> AvailableCountries { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }
    }
}