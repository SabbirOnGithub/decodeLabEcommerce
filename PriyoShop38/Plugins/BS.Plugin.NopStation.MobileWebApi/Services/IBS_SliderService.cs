using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using Nop.Core;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public partial interface IBS_SliderService
    {
        void DeleteSlider(int Id);

        void InsertSlider(BS_Slider item);

        void UpdateSlider(BS_Slider item);

        BS_Slider GetBsSliderImageById(int Id);

        IPagedList<BS_Slider> GetBSSliderImages(int? sdtid = null, DateTime? activeStartDate = null,
            DateTime? activeEndDate = null, int pageIndex = 0, int pageSize = int.MaxValue);

        List<BS_Slider> GetActiveBSSliderImages();
    }
}
