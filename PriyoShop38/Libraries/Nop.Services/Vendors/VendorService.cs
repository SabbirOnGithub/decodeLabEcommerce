using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Vendors;
using Nop.Services.Events;

namespace Nop.Services.Vendors
{
    /// <summary>
    /// Vendor service
    /// </summary>
    public partial class VendorService : IVendorService
    {
        #region Fields

        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<VendorNote> _vendorNoteRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<VendorRestrictedPaymentMethod> _vendorRPMRepository;
        private readonly IRepository<VendorHistory> _vendorHistoryRepository;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="vendorRepository">Vendor repository</param>
        /// <param name="vendorNoteRepository">Vendor note repository</param>
        /// <param name="eventPublisher">Event published</param>
        public VendorService(IRepository<Vendor> vendorRepository,
            IRepository<VendorNote> vendorNoteRepository,
            IEventPublisher eventPublisher,
            IRepository<VendorRestrictedPaymentMethod> vendorRPMRepository,
            IRepository<VendorHistory> vendorHistoryRepository)
        {
            this._vendorRepository = vendorRepository;
            this._vendorNoteRepository = vendorNoteRepository;
            this._eventPublisher = eventPublisher;
            this._vendorRPMRepository = vendorRPMRepository;
            this._vendorHistoryRepository = vendorHistoryRepository;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Gets a vendor by vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>Vendor</returns>
        public virtual Vendor GetVendorById(int vendorId)
        {
            if (vendorId == 0)
                return null;

            return _vendorRepository.GetById(vendorId);
        }

        /// <summary>
        /// Delete a vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        public virtual void DeleteVendor(Vendor vendor)
        {
            if (vendor == null)
                throw new ArgumentNullException("vendor");

            vendor.Deleted = true;
            UpdateVendor(vendor);
        }

        /// <summary>
        /// Gets all vendors
        /// </summary>
        /// <param name="name">Vendor name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Vendors</returns>
        public virtual IPagedList<Vendor> GetAllVendors(string name = "",
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _vendorRepository.Table;
            if (!String.IsNullOrWhiteSpace(name))
                query = query.Where(v => v.Name.Contains(name));
            if (!showHidden)
                query = query.Where(v => v.Active);

            query = query.Where(v => !v.Deleted);
            query = query.OrderBy(v => v.DisplayOrder).ThenBy(v => v.Name);

            var vendors = new PagedList<Vendor>(query, pageIndex, pageSize);
            return vendors;
        }

        /// <summary>
        /// Gets all vendors
        /// </summary>
        /// <param name="name">Vendor name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="vendorStatus">A value that enables filtering based on activity of the vendors</param>
        /// <returns>Vendors</returns>
        public virtual IPagedList<Vendor> GetAllVendors(VendorStatus vendorStatus, string name = "",
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _vendorRepository.Table;
            if (!String.IsNullOrWhiteSpace(name))
                query = query.Where(v => v.Name.Contains(name));
                        
            if (vendorStatus == VendorStatus.Active)
                query = query.Where(v => v.Active);
            else if (vendorStatus == VendorStatus.Inactive)
                query = query.Where(v => !v.Active);
            
            query = query.Where(v => !v.Deleted);
            query = query.OrderBy(v => v.DisplayOrder).ThenBy(v => v.Name);

            var vendors = new PagedList<Vendor>(query, pageIndex, pageSize);
            return vendors;
        }

        /// <summary>
        /// Inserts a vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        public virtual void InsertVendor(Vendor vendor)
        {
            if (vendor == null)
                throw new ArgumentNullException("vendor");

            _vendorRepository.Insert(vendor);

            //event notification
            _eventPublisher.EntityInserted(vendor);
        }

        /// <summary>
        /// Updates the vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        public virtual void UpdateVendor(Vendor vendor)
        {
            if (vendor == null)
                throw new ArgumentNullException("vendor");

            _vendorRepository.Update(vendor);

            //event notification
            _eventPublisher.EntityUpdated(vendor);
        }



        /// <summary>
        /// Gets a vendor note note
        /// </summary>
        /// <param name="vendorNoteId">The vendor note identifier</param>
        /// <returns>Vendor note</returns>
        public virtual VendorNote GetVendorNoteById(int vendorNoteId)
        {
            if (vendorNoteId == 0)
                return null;

            return _vendorNoteRepository.GetById(vendorNoteId);
        }

        /// <summary>
        /// Deletes a vendor note
        /// </summary>
        /// <param name="vendorNote">The vendor note</param>
        public virtual void DeleteVendorNote(VendorNote vendorNote)
        {
            if (vendorNote == null)
                throw new ArgumentNullException("vendorNote");

            _vendorNoteRepository.Delete(vendorNote);

            //event notification
            _eventPublisher.EntityDeleted(vendorNote);
        }

        #region Payment method restriction

        public void AddRestrictedPaymentMethod(VendorRestrictedPaymentMethod method)
        {
            _vendorRPMRepository.Insert(method);
        }

        public void DeleteRestrictedPaymentMethod(VendorRestrictedPaymentMethod method)
        {
            _vendorRPMRepository.Delete(method);
        }

        public IList<Vendor> GetVendorsByIds(int[] ids)
        {
            if (ids == null || !ids.Any())
                return new List<Vendor>();

            var query = _vendorRepository.Table.Where(x => ids.Contains(x.Id));
            return query.ToList();
        }

        #endregion

        #region Vendor History

        /// <summary>
        /// Inserts a history record for a 
        /// </summary>
        /// <param name="vendorHistory">Vendor history</param>
        public virtual void InsertVendorHistory(VendorHistory vendorHistory)
        {
            if (vendorHistory == null)
                throw new ArgumentNullException("vendorHistory");

            _vendorHistoryRepository.Insert(vendorHistory);

            //event notification
            _eventPublisher.EntityInserted(vendorHistory);
        }

        /// <summary>
        /// Gets an vendor history
        /// </summary>
        /// <param name="vendorHistoryId">The vendor history identifier</param>
        /// <returns>Vendor History</returns>
        public virtual VendorHistory GetVendorHistoryById(int vendorHistoryId)
        {
            if (vendorHistoryId == 0)
                return null;

            return _vendorHistoryRepository.GetById(vendorHistoryId);
        }

        /// <summary>
        /// Gets an vendor history
        /// </summary>
        /// <param name="vendorId"> Denotes the id of the vendor history is to retrieved</param>
        /// <param name="pageIndex"> Denoted the page-index</param>
        /// <param name="pageSize"> Denoted the page-size</param>
        /// <returns>Vendor History</returns>
        public virtual IPagedList<VendorHistory> GetVendorHistoriesByVendorId(int vendorId, int pageIndex,
            int pageSize)
        {
            var query = _vendorHistoryRepository.Table;
            query = query.Where(ph => ph.VendorId == vendorId);
            query = query.OrderByDescending(ph => ph.CreatedOnUtc);

            return new PagedList<VendorHistory>(query, pageIndex, pageSize);
        }


        /// <summary>
        /// Deletes a prodcut history
        /// </summary>
        /// <param name="vendorHistory">The vendor history</param>
        public virtual void DeleteVendorHistory(VendorHistory vendorHistory)
        {
            if (vendorHistory == null)
                throw new ArgumentNullException("vendorHistory");

            _vendorHistoryRepository.Delete(vendorHistory);

            //event notification
            _eventPublisher.EntityDeleted(vendorHistory);
        }

        #endregion

        #endregion
    }

    public enum VendorStatus
    {
        All = 0,
        Active = 1,
        Inactive = 2
    }
}