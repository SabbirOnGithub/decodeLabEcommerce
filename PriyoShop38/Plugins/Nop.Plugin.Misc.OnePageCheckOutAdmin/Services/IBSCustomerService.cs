using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Customers;

namespace Nop.Plugin.Misc.OnePageCheckOutAdmin.Services
{
    public partial interface IBSCustomerService
    {
        IPagedList<Customer> GetAllCustomers(string keywords = null, int pageIndex = 0, int pageSize = int.MaxValue);

        IPagedList<Customer> GetAllCustomersByFirstNameAndLastName(string keywords = null, int pageIndex = 0,
            int pageSize = int.MaxValue);
    }
}
