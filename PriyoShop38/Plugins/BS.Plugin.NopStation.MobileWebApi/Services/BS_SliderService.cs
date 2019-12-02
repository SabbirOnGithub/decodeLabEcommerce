using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using Nop.Core;
using Nop.Core.Data;
using Nop.Services.Events;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
   public partial class BS_SliderService : IBS_SliderService
    {
        #region Field
        private readonly IRepository<BS_Slider> _bsSliderRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctr

        public BS_SliderService(IRepository<BS_Slider> bsSliderRepository, IEventPublisher eventPublisher)
        {
            this._bsSliderRepository = bsSliderRepository;
            this._eventPublisher = eventPublisher;

        }

        #endregion

        #region Methods

        public void DeleteSlider(int id)
        {
            var sliderImage = GetBsSliderImageById(id);

            _bsSliderRepository.Delete(sliderImage);

            _eventPublisher.EntityDeleted(sliderImage);
        }

        public void InsertSlider(BS_Slider item)
        {
            _bsSliderRepository.Insert(item);

            _eventPublisher.EntityInserted(item);
        }

        public BS_Slider GetBsSliderImageById(int id)
        {
            return _bsSliderRepository.GetById(id);
        }

        public IPagedList<BS_Slider> GetBSSliderImages(int? sdtid = null, DateTime? activeStartDate = null, 
            DateTime? activeEndDate = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _bsSliderRepository.Table;

            if (sdtid.HasValue)
                query = query.Where(x => x.SliderDomainTypeId == sdtid.Value);
            if (activeStartDate.HasValue)
                query = query.Where(x => x.SliderActiveStartDate >= activeEndDate);
            if (activeEndDate.HasValue)
                query = query.Where(x => x.SliderActiveEndDate <= activeEndDate);

            query = query.OrderBy(b => b.DisplayOrder);

            var bsSliderImages = new PagedList<BS_Slider>(query, pageIndex, pageSize);
            return bsSliderImages;
        }


        public List<BS_Slider> GetActiveBSSliderImages()
        {
            var currentDateTime = DateTime.UtcNow.AddHours(6);

            var bsSliderImages = _bsSliderRepository.Table.Where(x =>
                    (!x.SliderActiveStartDate.HasValue || x.SliderActiveStartDate <= currentDateTime) &&
                    (!x.SliderActiveEndDate.HasValue || x.SliderActiveEndDate >= currentDateTime))
                    .OrderBy(x => x.DisplayOrder)
                    .ToList();

            return bsSliderImages;
        }

        public void UpdateSlider(BS_Slider item)
        {
            _bsSliderRepository.Update(item);

            _eventPublisher.EntityUpdated(item);
        }

        #endregion
    }
}
