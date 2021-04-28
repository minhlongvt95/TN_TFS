using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.GeographicalArea;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class GetDataEditLeadResponse : BaseResponse
    {
        public Models.Lead.LeadModel LeadModel { get; set; }
        public Models.Contact.ContactModel LeadContactModel { get; set; }
        public List<Guid> ListLeadInterestedGroupMappingId { get; set; }
        public List<string> ListEmailLead { get; set; }
        public List<string> ListPhoneLead { get; set; }
        public List<DataAccess.Models.Lead.CheckDuplicateLeadWithCustomerEntityModel> ListCustomerContact { get; set; }
        public List<Models.Category.CategoryModel> ListInterestedGroup { get; set; }
        public List<Models.Category.CategoryModel> ListGender { get; set; }
        public List<Models.Category.CategoryModel> ListPaymentMethod { get; set; }
        public List<Models.Category.CategoryModel> ListPotential { get; set; }
        public List<Models.Category.CategoryModel> ListLeadType { get; set; }
        public List<Models.Category.CategoryModel> ListLeadStatus { get; set; }
        public List<Models.Category.CategoryModel> ListLeadGroup { get; set; }
        public List<GeographicalAreaEntityModel> ListArea { get; set; }
        public List<DataAccess.Models.Employee.EmployeeEntityModel> ListPersonalInChange { get; set; }
        public List<Models.Note.NoteModel> ListNote { get; set; }
        //NEW 
        public List<DataAccess.Models.CategoryEntityModel> ListBusinessType { get; set; }
        public List<DataAccess.Models.CategoryEntityModel> ListInvestFund { get; set; }
        public List<DataAccess.Models.CategoryEntityModel> ListProbability { get; set; }
        public List<DataAccess.Models.Lead.LeadReferenceCustomerModel> ListLeadReferenceCustomer { get; set; }
        public List<DataAccess.Models.Lead.LeadDetailModel> ListLeadDetail { get; set; }
        public List<Models.Contact.ContactModel> ListLeadContact { get; set; }
        public List<EmployeeModel> ListEmployee { get; set; }
        public bool CanDelete { get; set; }
        public bool CanCreateSaleBidding { get; set; }
        public int StatusSaleBiddingAndQuote { get; set; }
        public List<DataAccess.Models.Address.ProvinceEntityModel> ListProvince { get; set; }
        public List<DataAccess.Models.Quote.QuoteEntityModel> ListQuoteById { get; set; }

        public List<DataAccess.Models.Order.CustomerOrderEntityModel> ListOrder { get; set; }

        public List<DataAccess.Models.SaleBidding.SaleBiddingEntityModel> ListSaleBiddingById { get; set; }
        public List<DataAccess.Models.Document.LinkOfDocumentEntityModel> ListLinkOfDocument { get; set; }
        public List<DataAccess.Models.Folder.FileInFolderEntityModel> ListFile { get; set; }
        public List<CategoryEntityModel> ListStatusSupport { get; set; }
        public Guid? StatusSupportId { get; set; }
    }
}
