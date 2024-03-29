using Nop.Core;
using Nop.Core.Domain.Vendors;
using System.Collections.Generic;

namespace Nop.Services.Vendors
{
    /// <summary>
    /// Vendor service interface
    /// </summary>
    public partial interface IVendorService
    {
        /// <summary>
        /// Gets a vendor by vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>Vendor</returns>
        Vendor GetVendorById(int vendorId);

        /// <summary>
        /// Delete a vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        void DeleteVendor(Vendor vendor);

        /// <summary>
        /// Gets all vendors
        /// </summary>
        /// <param name="name">Vendor name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Vendors</returns>
        IPagedList<Vendor> GetAllVendors(string name = "", 
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets all vendors
        /// </summary>
        /// <param name="name">Vendor name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="vendorStatus">A value that enables filtering based on activity of the vendors</param>
        /// <returns>Vendors</returns>
        IPagedList<Vendor> GetAllVendors(VendorStatus vendorStatus, string name = "",
            int pageIndex = 0, int pageSize = int.MaxValue);


        /// <summary>
        /// Inserts a vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        void InsertVendor(Vendor vendor);

        /// <summary>
        /// Updates the vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        void UpdateVendor(Vendor vendor);



        /// <summary>
        /// Gets a vendor note note
        /// </summary>
        /// <param name="vendorNoteId">The vendor note identifier</param>
        /// <returns>Vendor note</returns>
        VendorNote GetVendorNoteById(int vendorNoteId);

        /// <summary>
        /// Deletes a vendor note
        /// </summary>
        /// <param name="vendorNote">The vendor note</param>
        void DeleteVendorNote(VendorNote vendorNote);

        #region Payment method restriction
        void AddRestrictedPaymentMethod(VendorRestrictedPaymentMethod method);

        void DeleteRestrictedPaymentMethod(VendorRestrictedPaymentMethod method);

        IList<Vendor> GetVendorsByIds(int[] ids);

        #endregion

        #region Vendor History

        void InsertVendorHistory(VendorHistory vendorHistory);

        VendorHistory GetVendorHistoryById(int vendorHistoryId);

        IPagedList<VendorHistory> GetVendorHistoriesByVendorId(int vendorId, int pageIndex, int pageSize);

        void DeleteVendorHistory(VendorHistory vendorHistory);

        #endregion
    }
}