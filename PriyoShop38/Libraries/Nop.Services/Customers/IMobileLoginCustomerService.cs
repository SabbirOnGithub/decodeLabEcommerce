using Nop.Core;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Customers
{
    public partial interface IMobileLoginCustomerService
    {
        CustomerLoginResults ValidateCustomerByMobileNumber(string mobileNumber);

        MobileLoginCustomer GetMobileLoginCustomerById(int id);

        MobileLoginCustomer GetMobileLoginCustomerByCustomerId(int customerId);

        MobileLoginCustomer GetMobileLoginCustomerByMobileNumber(string mobileNumber);

        Customer GetCustomerByMobileNumber(string mobileNumber);

        void InsertMobileLoginCustomer(MobileLoginCustomer mobileLoginCustomer);

        void UpdateMobileLoginCustomer(MobileLoginCustomer mobileLoginCustomer);

        void DeleteMobileLoginCustomer(MobileLoginCustomer googleProductRecord);

        IPagedList<MobileLoginCustomer> GetAllMobileLoginCustomers(string searchEmail, string SearchName,
            string searchMobileNumber, int pageIndex = 0, int pageSize = int.MaxValue);
    }
}
