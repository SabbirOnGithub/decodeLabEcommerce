using BS.Plugin.NopStation.MobileApp.Domain;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using Nop.Core;
using Nop.Core.Data;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileApp.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IRepository<Device> _deviceRepository;
        private readonly IEventPublisher _eventPublisher;

        public DeviceService(
            IRepository<Device> deviceRepository,
            IEventPublisher eventPublisher
            )
        {
            this._deviceRepository = deviceRepository;
            this._eventPublisher = eventPublisher;

        }



        public IPagedList<Device> GetAllDevice(int pageIndex, int pageSize)
        {
            var query = (from u in _deviceRepository.Table
                         orderby u.CustomerId
                         select u);
            var devices = new PagedList<Device>(query, pageIndex, pageSize);
            return devices; ;
        }

        public IList<Device> GetDevicesByCustomerId(int customerId)
        {
            var query = (from u in _deviceRepository.Table
                         orderby u.CreatedOnUtc descending
                         select u);
            var devices = query.ToList();
            return devices;
        }

        public Device InsertDevice(Device device)
        {
            if (device == null)
                throw new ArgumentNullException("device");

            _deviceRepository.Insert(device);
            //event device
            _eventPublisher.EntityInserted(device);

            return device; ;
        }

        public Device GetDeviceById(int id)
        {
            return _deviceRepository.GetById(id);
        }


        public Device GetDeviceByDeviceToken(string deviceToken)
        {

            Device device = null;
            var query = (from u in _deviceRepository.Table
                         where u.DeviceToken == deviceToken
                         select u);
            if (query.Any())
            {
                device = query.FirstOrDefault();
            }

            return device;
        }

        public void UpdateDevice(Device device)
        {

            if (device == null)
                throw new ArgumentNullException("device");
            _deviceRepository.Update(device);
            _eventPublisher.EntityUpdated(device);
        }

        public void DeleteDevice(Device device)
        {
            _deviceRepository.Delete(device);
            _eventPublisher.EntityDeleted(device);
        }
    }
}
