using System.Collections.Generic;
using Nop.Core;
using Nop.Plugin.Widgets.MobileLogin.Domain;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;

namespace Nop.Plugin.Widgets.MobileLogin.Services
{
    public partial interface IMobileLoginService
    {
        CustomerLoginResults ValidateCustomerByMobileNumber(string mobileNumber);
        void Delete(Domain.MobileLoginCustomer googleProductRecord);
        IList<Domain.MobileLoginCustomer> GetAll();
        Domain.MobileLoginCustomer GetById(int googleProductRecordId);
        Domain.MobileLoginCustomer GetByCustomerId(int productId);
        Domain.MobileLoginCustomer GetByMobileNumber(string mobileNumber);
        Customer GetCustomerByMobileNumber(string mobileNumber);
        void Insert(Domain.MobileLoginCustomer googleProductRecord);
        void Update(Domain.MobileLoginCustomer mobileLoginCustomer);
        CustomerRegistrationResult RegisterCustomer(CustomerRegistrationRequest request);
        IPagedList<Customer> getAllMobileLoginCustomers(string searchEmail, string SearchName,
            string searchMobileNumber, int pageIndex = 0, int pageSize = int.MaxValue);
    }
}
