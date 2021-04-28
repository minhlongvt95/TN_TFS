using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.BusinessLogic.Models.Product;
using TN.TNM.BusinessLogic.Models.SaleBidding;

namespace TN.TNM.BusinessLogic.Messages.Responses.SaleBidding
{
    public class GetMasterDataSaleBiddingDetailResponse : BaseResponse
    {
        public SaleBiddingModel SaleBidding { get; set; }
        public List<SaleBiddingDetailModel> ListSaleBiddingDetail { get; set; }
        public List<Guid?> ListEmployeeMapping { get; set; }
        public List<EmployeeModel> ListEmployee { get; set; }
        public List<CategoryModel> ListMoneyUnit { get; set; }
        public List<EmployeeModel> ListPerson { get; set; } // Danh sách tất cả nhân viên phụ trách
        public List<CustomerModel> ListCustomer { get; set; } // Lấy danh sách khách hàng định danh
        public List<NoteModel> ListNote { get; set; }
        public List<CategoryModel> ListStatus { get; set; }
        public List<ProductModel> ListProduct { get; set; }
        public ContactModel Contact { get; set; }
        public List<CustomerCareInforBusinessModel> ListCustomerCareInfor { get; set; }
        public CustomerMeetingInforBusinessModel CustomerMeetingInfor { get; set; }
        public List<CategoryModel> ListTypeContact { get; set; }
        public bool IsApproved { get; set; }
        public bool isEdit { get; set; }
        public bool isEmployeeSupport { get; set; }
        public bool isLoginEmployeeJoin { get; set; }
    }
}
