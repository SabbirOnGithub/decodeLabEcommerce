using FluentValidation.Attributes;
using Nop.Plugin.Widgets.BsAffiliate.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.BsAffiliate.Models
{
    [Validator(typeof(AddressValidator))]
    public partial class AddressModel : BaseNopEntityModel
    {
        public AddressModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AddressModel.FirstName")]
        [AllowHtml]
        public string FirstName { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AddressModel.LastName")]
        [AllowHtml]
        public string LastName { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AddressModel.Email")]
        [AllowHtml]
        public string Email { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AddressModel.Company")]
        [AllowHtml]
        public string Company { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AddressModel.Country")]
        public int? CountryId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AddressModel.Country")]
        [AllowHtml]
        public string CountryName { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AddressModel.StateProvince")]
        public int? StateProvinceId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AddressModel.StateProvince")]
        [AllowHtml]
        public string StateProvinceName { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AddressModel.City")]
        [AllowHtml]
        public string City { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AddressModel.Address1")]
        [AllowHtml]
        public string Address1 { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AddressModel.Address2")]
        [AllowHtml]
        public string Address2 { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AddressModel.ZipPostalCode")]
        [AllowHtml]
        public string ZipPostalCode { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AddressModel.PhoneNumber")]
        [AllowHtml]
        public string PhoneNumber { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AddressModel.FaxNumber")]
        [AllowHtml]
        public string FaxNumber { get; set; }

        [NopResourceDisplayName("Admin.Address")]
        public string AddressHtml { get; set; }

        public IList<SelectListItem> AvailableCountries { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }
    }
}