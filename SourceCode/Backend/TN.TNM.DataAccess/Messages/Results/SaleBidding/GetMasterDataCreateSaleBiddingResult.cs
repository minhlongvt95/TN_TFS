using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Lead;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Results.SaleBidding
{
    public class GetMasterDataCreateSaleBiddingResult : BaseResult
    {
        public List<CustomerEntityModel> ListCustomer { get; set; } // Lấy danh sách khách hàng định danh
        public LeadEntityModel Lead { get; set; } // Lấy thông tin cơ hội
        public EmployeeEntityModel Employee { get; set; } // Người phụ trách
        public List<EmployeeEntityModel> ListPerson { get; set; } // Danh sách tất cả nhân viên phụ trách
        public List<EmployeeEntityModel> ListEmployee { get; set; } // Danh sách tất cả nhân viên
        public List<LeadDetailModel> ListLeadDetail { get; set; } // Lấy list chi tiết cơ hô
        public List<CategoryEntityModel> ListMoneyUnit { get; set; }
        public List<ProductEntityModel> ListProduct { get; set; }
        public ContactEntityModel Contact { get; set; }
        public List<CategoryEntityModel> ListTypeContact { get; set; }
    }
}
