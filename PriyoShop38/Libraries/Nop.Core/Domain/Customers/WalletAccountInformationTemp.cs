using System;
using System.ComponentModel.DataAnnotations;

namespace Nop.Core.Domain.Customers
{
    public class WalletAccountInformationTemp : BaseEntity
    {
        [Required(ErrorMessage = "Please enter valid contact number.")]
        public long ContactNo { get; set; }
        public int CustomerID { get; set; }
        public int CustomerType { get; set; }
        public DateTime OTPEntryDate { get; set; }
        public int OTPExpireTime { get; set; }
        [Required(ErrorMessage = "Please enter valid OTP.")]
        public int OTP { get; set; }
    }
}
