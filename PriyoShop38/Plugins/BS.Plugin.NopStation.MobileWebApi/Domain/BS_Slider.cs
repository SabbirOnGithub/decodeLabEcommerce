using System;
using Nop.Core;

namespace BS.Plugin.NopStation.MobileWebApi.Domain
{
    public partial class BS_Slider : BaseEntity 
    {
        public int PictureId { get; set; }

        public DateTime? SliderActiveStartDate { get; set; }

        public DateTime? SliderActiveEndDate { get; set; }

        public int SliderDomainTypeId { get; set; }

        public int DomainId { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsProduct { get; set; }

        public int ProdOrCatId { get; set; }

        public SliderDomainType SliderDomainType
        {
            get
            {
                return (SliderDomainType)SliderDomainTypeId;
            }
            set
            {
                this.SliderDomainTypeId = (int)value;
            }
        }
    }
}
