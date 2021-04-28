using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.DataAccess.Models.GeographicalArea;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class GetDataDetailPotentialCustomerResponse: BaseResponse
    {
        public List<EmployeeModel> ListPersonalInChange { get; set; }
        public List<CategoryModel> ListInvestFund { get; set; }//Nguon tiem nang
        public Models.Customer.CustomerModel PotentialCustomerModel { get; set; }
        public Models.Contact.ContactModel PotentialCustomerContactModel { get; set; }
        public List<Models.Contact.ContactModel> ListContact { get; set; }
        public List<DataAccess.Models.Folder.FileInFolderEntityModel> ListFileByPotentialCustomer { get; set; }
        public List<DataAccess.Models.Document.LinkOfDocumentEntityModel> ListLinkOfDocument { get; set; }
        public List<DataAccess.Models.Product.ProductEntityModel> ListProduct { get; set; }
        public List<DataAccess.Models.Customer.PotentialCustomerProductEntityModel> ListPotentialCustomerProduct { get; set; }
        public List<DataAccess.Models.Quote.QuoteEntityModel> ListQuoteByPotentialCustomer { get; set; }
        public List<DataAccess.Models.Lead.LeadEntityModel> ListLeadByPotentialCustomer { get; set; }
        public List<DataAccess.Models.Customer.CustomerCareInforModel> ListCustomerCareInfor { get; set; }
        public DataAccess.Models.Customer.CustomerMeetingInforModel CustomerMeetingInfor { get; set; }
        public List<DataAccess.Models.Employee.EmployeeEntityModel> ListParticipants { get; set; }
        public List<DataAccess.Models.Note.NoteEntityModel> ListNote { get; set; }
        public List<GeographicalAreaEntityModel> ListArea { get; set; }
        public List<CategoryEntityModel> ListCusGroup { get; set; } //Nhóm khách hàng
        public List<CategoryEntityModel> ListStatusSupport { get; set; } //Trạng thái phụ
        public Guid? StatusSupportId { get; set; }
        public string StatusCustomerCode { get; set; }
        public List<EmployeeEntityModel> ListEmpTakeCare { get; set; }
    }
}
