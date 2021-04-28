using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.SaleBidding;

namespace TN.TNM.DataAccess.Messages.Results.SaleBidding
{
    public class GetMasterDataSaleBiddingDetailResult : BaseResult
    {
        public SaleBiddingEntityModel SaleBidding { get; set; }
        public List<SaleBiddingDetailEntityModel> ListSaleBiddingDetail { get; set; }
        public List<Guid?> ListEmployeeMapping { get; set; } // Danh sách tất cả nhân viên tham gia
        public List<EmployeeEntityModel> ListEmployee { get; set; } // Danh sách tất cả nhân viên
        public List<CategoryEntityModel> ListMoneyUnit { get; set; }
        public List<CustomerEntityModel> ListCustomer { get; set; } // Lấy danh sách khách hàng định danh
        public List<NoteEntityModel> ListNote { get; set; }
        public List<CategoryEntityModel> ListStatus { get; set; }
        public List<ProductEntityModel> ListProduct { get; set; }
        public ContactEntityModel Contact { get; set; }
        public List<CustomerCareInforModel> ListCustomerCareInfor { get; set; }
        public CustomerMeetingInforModel CustomerMeetingInfor { get; set; }
        public List<EmployeeEntityModel> ListPerson { get; set; } // Danh sách tất cả nhân viên phụ trách
        public List<CategoryEntityModel> ListTypeContact { get; set; }
        public bool IsApproved { get; set; }
        public bool isEdit { get; set; }
        public bool isEmployeeSupport { get; set; }
        public bool isLoginEmployeeJoin { get; set; }
    }
}
