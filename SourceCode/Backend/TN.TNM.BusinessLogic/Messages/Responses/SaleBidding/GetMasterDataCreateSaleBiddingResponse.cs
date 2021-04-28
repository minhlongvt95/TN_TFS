using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Lead;
using TN.TNM.BusinessLogic.Models.Product;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.BusinessLogic.Messages.Responses.SaleBidding
{
    public class GetMasterDataCreateSaleBiddingResponse : BaseResponse
    {
        public List<CustomerModel> ListCustomer { get; set; } // Lấy danh sách khách hàng định danh
        public LeadModel Lead { get; set; } // Lấy thông tin cơ hội
        public EmployeeModel Employee { get; set; } // Người phụ trách
        public List<EmployeeModel> ListPerson { get; set; } // Danh sách tất cả nhân viên phụ trách
        public List<EmployeeModel> ListEmployee { get; set; } // Danh sách tất cả nhân viên
        public List<LeadDetailModel> ListLeadDetail { get; set; } // Lấy list chi tiết cơ hội
        public List<CategoryModel> ListMoneyUnit { get; set; }
        public List<ProductModel> ListProduct { get; set; }
        public List<CategoryModel> ListTypeContact { get; set; }
    }
}
