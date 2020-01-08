

using Nop.Web.Framework.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Nop.Web.Models.Customer
{
   public class CustomerWalletPayment : BaseNopModel
    {
        #region Properties
        public long SystemID { get; set; }
        public long ContactNo { get; set; }
        public string CustomerName { get; set; }
        public decimal TransferAmount { get; set; }
        //[Required(ErrorMessage = "Please enter valid contact number.")]
        public long TransferToContactNumber { get; set; }


        #endregion
    }
}
