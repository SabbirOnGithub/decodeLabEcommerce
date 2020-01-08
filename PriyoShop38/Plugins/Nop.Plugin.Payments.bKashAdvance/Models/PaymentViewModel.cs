using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.bKashAdvance.Models
{
    public class PaymentViewModel
    {
        public int OrderId { get; set; }
        public decimal OrderTotal { get; set; }
        public string Intent { get; set; }
        public bool IsSandBox { get; set; }
    }
}
