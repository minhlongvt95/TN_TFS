using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Customer;

namespace TN.TNM.BusinessLogic.Messages.Responses.SaleBidding
{
    public class GetCustomerByEmployeeIdResponse : BaseResponse
    {
        public List<CustomerModel> ListCustomer { get; set; } // Lấy danh sách khách hàng định danh
    }
}
