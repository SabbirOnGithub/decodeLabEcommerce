using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using BS.Plugin.NopStation.MobileWebApi.Validator.Slider;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Slider
{
    [Validator(typeof(SliderImageValidator))]
    public class SliderImageModel : BaseNopEntityModel
    {
        public SliderImageModel()
        {
            AvailableSliderDomainTypes = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.SliderImage.Fields.Picture")]
        [UIHint("Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Admin.SliderImage.Fields.Picture")]
        public string PictureUrl { get; set; }

        [NopResourceDisplayName("Admin.SliderImage.Fields.SliderActiveStartDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? SliderActiveStartDate { get; set; }

        [NopResourceDisplayName("Admin.SliderImage.Fields.SliderActiveEndDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? SliderActiveEndDate { get; set; }

        [NopResourceDisplayName("Admin.SliderImage.Fields.SliderDomainType")]
        public int SliderDomainTypeId { get; set; }

        [NopResourceDisplayName("Admin.SliderImage.Fields.SliderDomainType")]
        public string SliderDomainTypeStr { get; set; }

        [NopResourceDisplayName("Admin.SliderImage.Fields.DomainId")]
        public int DomainId { get; set; }

        [NopResourceDisplayName("Admin.SliderImage.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<SelectListItem> AvailableSliderDomainTypes { get; set; }
    }
}
