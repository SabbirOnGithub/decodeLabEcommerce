using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Vendors;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Models.Common;
using Nop.Services.Seo;

namespace BS.Plugin.NopStation.MobileWebApi.Extensions
{
    public static class MappingExtensions
    {
        public static void PrepareModelApi(this AddressModel model,
             Address address, bool excludeProperties,
             AddressSettings addressSettings,
             ILocalizationService localizationService = null,
             IStateProvinceService stateProvinceService = null,
             IAddressAttributeService addressAttributeService = null,
             IAddressAttributeParser addressAttributeParser = null,
             IAddressAttributeFormatter addressAttributeFormatter = null,
             Func<IList<Country>> loadCountries = null,
             bool prePopulateWithCustomerFields = false,
             Customer customer = null)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (addressSettings == null)
                throw new ArgumentNullException("addressSettings");

            if (!excludeProperties && address != null)
            {
                model.Id = address.Id;
                model.FirstName = address.FirstName;
                model.LastName = address.LastName;
                model.Email = address.Email;
                model.Company = address.Company;
                model.CountryId = address.CountryId;
                model.CountryName = address.Country != null
                    ? address.Country.GetLocalized(x => x.Name)
                    : null;
                model.StateProvinceId = address.StateProvinceId??0;
                if (localizationService != null)
                {
                    model.StateProvinceName = address.StateProvince != null
                    ? address.StateProvince.GetLocalized(x => x.Name)
                    : localizationService.GetResource("Address.OtherNonUS"); 
                }
                else
                {
                   model.StateProvinceName = address.StateProvince != null
                    ? address.StateProvince.GetLocalized(x => x.Name)
                    : null; 
                }
                
                model.City = address.City;
                model.Address1 = address.Address1;
                model.Address2 = address.Address2;
                model.ZipPostalCode = address.ZipPostalCode;
                model.PhoneNumber = address.PhoneNumber;
                model.FaxNumber = address.FaxNumber;
            }

            if (address == null && prePopulateWithCustomerFields)
            {
                if (customer == null)
                    throw new Exception("Customer cannot be null when prepopulating an address");
                model.Email = customer.Email;
                model.FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName);
                model.LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName);
                model.Company = customer.GetAttribute<string>(SystemCustomerAttributeNames.Company);
                model.Address1 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress);
                model.Address2 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress2);
                model.ZipPostalCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.ZipPostalCode);
                model.City = customer.GetAttribute<string>(SystemCustomerAttributeNames.City);
                model.PhoneNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                model.FaxNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Fax);
            }

            //countries and states
            if (addressSettings.CountryEnabled && loadCountries != null)
            {
                if (localizationService == null)
                    throw new ArgumentNullException("localizationService");

                model.AvailableCountries.Add(new SelectListItem { Text = localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                foreach (var c in loadCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = c.GetLocalized(x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (addressSettings.StateProvinceEnabled)
                {
                    //states
                    if (stateProvinceService == null)
                        throw new ArgumentNullException("stateProvinceService");

                    var states = stateProvinceService
                        .GetStateProvincesByCountryId(model.CountryId.HasValue ? model.CountryId.Value : 0)
                        .ToList();
                    if (states.Count > 0)
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = localizationService.GetResource("Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem
                            {
                                Text = s.GetLocalized(x => x.Name),
                                Value = s.Id.ToString(),
                                Selected = (s.Id == model.StateProvinceId)
                            });
                        }
                    }
                    else
                    {
                        bool anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);
                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = localizationService.GetResource(anyCountrySelected ? "Address.OtherNonUS" : "Address.SelectState"),
                            Value = "0"
                        });
                    }
                }
            }

            //form fields
            model.CompanyEnabled = addressSettings.CompanyEnabled;
            model.CompanyRequired = addressSettings.CompanyRequired;
            model.StreetAddressEnabled = addressSettings.StreetAddressEnabled;
            model.StreetAddressRequired = addressSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = addressSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = addressSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = addressSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = addressSettings.ZipPostalCodeRequired;
            model.CityEnabled = addressSettings.CityEnabled;
            model.CityRequired = addressSettings.CityRequired;
            model.CountryEnabled = addressSettings.CountryEnabled;
            model.StateProvinceEnabled = addressSettings.StateProvinceEnabled;
            model.PhoneEnabled = addressSettings.PhoneEnabled;
            model.PhoneRequired = addressSettings.PhoneRequired;
            model.FaxEnabled = addressSettings.FaxEnabled;
            model.FaxRequired = addressSettings.FaxRequired;

            //customer attribute services
            if (addressAttributeService != null && addressAttributeParser != null)
            {
                PrepareCustomAddressAttributes(model, address, addressAttributeService, addressAttributeParser);
            }
            if (addressAttributeFormatter != null && address != null)
            {
                model.FormattedCustomAddressAttributes = addressAttributeFormatter.FormatAttributes(address.CustomAttributes);
            }
        }

        private static void PrepareCustomAddressAttributes(this AddressModel model,
           Address address,
           IAddressAttributeService addressAttributeService,
           IAddressAttributeParser addressAttributeParser)
        {
            if (addressAttributeService == null)
                throw new ArgumentNullException("addressAttributeService");

            if (addressAttributeParser == null)
                throw new ArgumentNullException("addressAttributeParser");

            var attributes = addressAttributeService.GetAllAddressAttributes();
            foreach (var attribute in attributes)
            {
                var attributeModel = new AddressAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.GetLocalized(x => x.Name),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = addressAttributeService.GetAddressAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new AddressAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.GetLocalized(x => x.Name),
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }

                //set already selected attributes
                var selectedAddressAttributes = address != null ? address.CustomAttributes : null;
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                        {
                            if (!String.IsNullOrEmpty(selectedAddressAttributes))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = addressAttributeParser.ParseAddressAttributeValues(selectedAddressAttributes);
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
                            if (!String.IsNullOrEmpty(selectedAddressAttributes))
                            {
                                var enteredText = addressAttributeParser.ParseValues(selectedAddressAttributes, attribute.Id);
                                if (enteredText.Count > 0)
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    default:
                        //not supported attribute control types
                        break;
                }

                model.CustomAddressAttributes.Add(attributeModel);
            }
        }

        public static CategoryModelApi ToModel(this Category entity)
        {
            if (entity == null)
                return null;

            var model = new CategoryModelApi
            {
                Id = entity.Id,
                Name = entity.GetLocalized(x => x.Name),
                Description = entity.GetLocalized(x => x.Description)
            };
            return model;
        }

        public static CategoryResponseModel ToResponseModel(this Category entity)
        {
            if (entity == null)
                return null;

            var model = new CategoryResponseModel
            {
                Id = entity.Id,
                Name = entity.GetLocalized(x => x.Name),
                Description = entity.GetLocalized(x => x.Description),
                SeName = entity.GetSeName(),
            };
            return model;
        }

        public static ManuFactureModelApi ToModel(this Manufacturer entity)
        {
            if (entity == null)
                return null;

            var model = new ManuFactureModelApi
            {
                Id = entity.Id,
                Name = entity.GetLocalized(x => x.Name),
                Description = entity.GetLocalized(x => x.Description)
            };
            return model;
        }

        public static ManufacturerResponseModel ToResponseModel(this Manufacturer entity)
        {
            if (entity == null)
                return null;

            var model = new ManufacturerResponseModel
            {
                Id = entity.Id,
                Name = entity.GetLocalized(x => x.Name),
                Description = entity.GetLocalized(x => x.Description)
            };

            return model;
        }

        public static TagModelApi ToModel(this ProductTag entity)
        {
            if (entity == null)
                return null;
            var model = new TagModelApi()
            {
                Id = entity.Id,
                Name = entity.GetLocalized(x => x.Name)
            };
            return model;
        }

        public static VendorModelApi ToModel(this Vendor entity,VendorSettings vendorSettings)
        {
            if (entity == null)
                return null;
            var model = new VendorModelApi()
            {
                Id = entity.Id,
                Name = entity.GetLocalized(x => x.Name),
                Description = entity.GetLocalized(x => x.Description),
                AllowCustomersToContactVendors = vendorSettings.AllowCustomersToContactVendors
            };
            return model;
        }
        public static CategoryDetailProductResponseModel ToModel(this CategoryModelApi entity,PriceRange price)
        {
            if (entity == null)
                return null;

            var model = new CategoryDetailProductResponseModel
            {
                Name = entity.Name,
                Products = entity.Products,
                TotalPages = entity.PagingFilteringContext.TotalPages,
                NotFilteredItems = entity.PagingFilteringContext.SpecificationFilter.NotFilteredItems,
                AlreadyFilteredItems = entity.PagingFilteringContext.SpecificationFilter.AlreadyFilteredItems,
                FilterItems = entity.PagingFilteringContext.SpecificationFilter.FilterItems,
                PriceRange = price,
                AvailableSortOptions = entity.PagingFilteringContext.AvailableSortOptions
                
            };
            return model;
        }

        public static CategoryDetailProductResponseModel ToModel(this FreeDeliveryModelApi entity, PriceRange price)
        {
            if (entity == null)
                return null;

            var model = new CategoryDetailProductResponseModel
            {
                Name = entity.Name,
                Products = entity.Products,
                TotalPages = entity.PagingFilteringContext.TotalPages,
                NotFilteredItems = entity.PagingFilteringContext.SpecificationFilter.NotFilteredItems,
                AlreadyFilteredItems = entity.PagingFilteringContext.SpecificationFilter.AlreadyFilteredItems,
                FilterItems = entity.PagingFilteringContext.SpecificationFilter.FilterItems,
                PriceRange = price,
                AvailableSortOptions = entity.PagingFilteringContext.AvailableSortOptions
            };
            return model;
        }

        public static ManufactureDetailProductResponseModel ToModel(this ManuFactureModelApi entity, PriceRange price)
        {
            if (entity == null)
                return null;

            var model = new ManufactureDetailProductResponseModel
            {
                Name = entity.Name,
                Products = entity.Products,
                TotalPages = entity.PagingFilteringContext.TotalPages,
                NotFilteredItems = entity.PagingFilteringContext.SpecificationFilter.NotFilteredItems,
                AlreadyFilteredItems = entity.PagingFilteringContext.SpecificationFilter.AlreadyFilteredItems,
                PriceRange = price,
                AvailableSortOptions = entity.PagingFilteringContext.AvailableSortOptions

            };
            return model;
        }

        public static VendorResponseModel ToResponseModel(this VendorModelApi entity)
        {
            if (entity == null) 
                return null;

            var model = new VendorResponseModel
            {
                Name = entity.Name,
                Products = entity.Products
            };
            return model;
        }

        public static ProductTagDetailResponseModel ToModel(this TagModelApi entity)
        {
            if (entity == null)
                return null;

            var model = new ProductTagDetailResponseModel
            {
                Name = entity.Name,
                Products = entity.Products,
                TotalPages = entity.PagingFilteringContext.TotalPages,
                NotFilteredItems = entity.PagingFilteringContext.SpecificationFilter.NotFilteredItems,
                AlreadyFilteredItems = entity.PagingFilteringContext.SpecificationFilter.AlreadyFilteredItems,
                AvailableSortOptions = entity.PagingFilteringContext.AvailableSortOptions

            };
            return model;
        }

        public static IList<TDestination> MapTo<TSource, TDestination>(this IList<TSource> source)
        {
            //Mapper.CreateMap<TSource, TDestination>();
            return AutoMapperConfiguration.Mapper.Map<IList<TSource>, IList<TDestination>>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            //Mapper.CreateMap<TSource, TDestination>();

            return AutoMapperConfiguration.Mapper.Map<TSource, TDestination>(source);
        }


        public static NameValueCollection ToNameValueCollection(this List<KeyValueApi> formValues)
        {
            var form = new NameValueCollection();
            foreach (var values in formValues)
            {
                form.Add(values.Key, values.Value);
            }
            return form;
        }

        public static Address AddressFromToModel(this NameValueCollection form, string prefix)
        {
            prefix = prefix + ".";
            var address = new Address()
            {
                FirstName = form[prefix + "FirstName"],
                LastName = form[prefix + "LastName"],
                Email = form[prefix + "Email"],
                Address1 = form[prefix + "Address1"],
                Address2 = form[prefix + "Address2"],
                Company = form[prefix + "Company"],
                ZipPostalCode = form[prefix + "ZipPostalCode"],
                City = form[prefix + "City"],
                PhoneNumber = form[prefix + "PhoneNumber"],
                FaxNumber = form[prefix + "FaxNumber"],

            };
            int cId = 0;
            int.TryParse(form[prefix + "CountryId"], out cId);
            if (cId > 0)
            {
                address.CountryId = cId;
            }
            int sId = 0;
            int.TryParse(form[prefix + "StateProvinceId"], out sId);
            if (sId > 0)
            {
                address.StateProvinceId = sId;
            }
            return address;
        }

        public static Address ToEntity(this AddressModel model, bool trimFields = true)
        {
            if (model == null)
                return null;

            var entity = new Address();
            return ToEntity(model, entity, trimFields);
        }
        public static Address ToEntity(this AddressModel model, Address destination, bool trimFields = true)
        {
            if (model == null)
                return destination;

            if (trimFields)
            {
                if (model.FirstName != null)
                    model.FirstName = model.FirstName.Trim();
                if (model.LastName != null)
                    model.LastName = model.LastName.Trim();
                if (model.Email != null)
                    model.Email = model.Email.Trim();
                if (model.Company != null)
                    model.Company = model.Company.Trim();
                if (model.City != null)
                    model.City = model.City.Trim();
                if (model.Address1 != null)
                    model.Address1 = model.Address1.Trim();
                if (model.Address2 != null)
                    model.Address2 = model.Address2.Trim();
                if (model.ZipPostalCode != null)
                    model.ZipPostalCode = model.ZipPostalCode.Trim();
                if (model.PhoneNumber != null)
                    model.PhoneNumber = model.PhoneNumber.Trim();
                if (model.FaxNumber != null)
                    model.FaxNumber = model.FaxNumber.Trim();
            }
            destination.Id = model.Id;
            destination.FirstName = model.FirstName;
            destination.LastName = model.LastName;
            destination.Email = model.Email;
            destination.Company = model.Company;
            destination.CountryId = model.CountryId;
            destination.StateProvinceId = model.StateProvinceId;
            destination.City = model.City;
            destination.Address1 = model.Address1;
            destination.Address2 = model.Address2;
            destination.ZipPostalCode = model.ZipPostalCode;
            destination.PhoneNumber = model.PhoneNumber;
            destination.FaxNumber = model.FaxNumber;

            return destination;
        }
    }
}
