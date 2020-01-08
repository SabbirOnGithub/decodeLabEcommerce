using System;
using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Customer
{
    public partial class CustomerDownloadableProductsModel : BaseResponse
    {
        public CustomerDownloadableProductsModel()
        {
            Items = new List<DownloadableProductsModel>();
        }

        public IList<DownloadableProductsModel> Items { get; set; }

        #region Nested classes
        public partial class DownloadableProductsModel : BaseNopModel
        {
            public Guid OrderItemGuid { get; set; }

            public int OrderId { get; set; }

            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string ProductSeName { get; set; }
            public string ProductAttributes { get; set; }
            //start modify for download
            public bool IsDownloadable { get; set; } 
            public string DownloadUrl { get; set; }
            public bool IsLicenseDownloadable { get; set; }
            public string LicenseDownloadUrl { get; set; }
            //end mdify for download
            public int DownloadId { get; set; }
            public int LicenseId { get; set; }

            public DateTime CreatedOn { get; set; }
        }
        #endregion
    }

    public partial class UserAgreementModel : BaseNopModel
    {
        public Guid OrderItemGuid { get; set; }
        public string UserAgreementText { get; set; }
    }
}