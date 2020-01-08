using Nop.Web.Framework.Mvc;

using System;
using System.ComponentModel.DataAnnotations;

namespace Nop.Web.Models.Customer
{
    public class WalletAccountView : BaseNopModel
    {
        public long SystemID { get; set; }

        [Required(ErrorMessage = "Please enter valid contact number.")]
        public long ContactNo { get; set; }
        public int CustomerID { get; set; }
        [Required(ErrorMessage = "Please enter valid OTP.")]
        public string OTP { get; set; }
        public int CustomerType { get; set; }
        public bool IsActive { get; set; }
        public DateTime EntryDate { get; set; }
    }
}
