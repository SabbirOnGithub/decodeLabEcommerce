using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Services.Orders;
using Nop.Services.Payments;

namespace Nop.Plugin.Misc.OnePageCheckOutAdmin.Services
{
    public interface IOrderProcessingServiceOnePageCheckOut
    {
        PlaceOrderResult PlaceOrder(ProcessPaymentRequest processPaymentRequest,Customer customer);

    }
}
