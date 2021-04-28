using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.GeographicalArea;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetDataDetailPotentialCustomerResult: BaseResult
    {
        public List<EmployeeEntityModel> ListPersonalInChange { get; set; }//nguoi phu trach
        public List<CategoryEntityModel> ListInvestFund { get; set; }//Nguon tiem nang
        public Databases.Entities.Customer PotentialCustomerModel { get; set; }
        public Databases.Entities.Contact PotentialCustomerContactModel { get; set; }
        public List<Databases.Entities.Contact> ListContact { get; set; }
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
