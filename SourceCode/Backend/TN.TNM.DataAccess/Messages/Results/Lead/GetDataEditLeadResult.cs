using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.GeographicalArea;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class GetDataEditLeadResult: BaseResult
    {
        public Databases.Entities.Lead LeadModel { get; set; }
        public Databases.Entities.Contact LeadContactModel { get; set; }
        public List<string> ListEmailLead { get; set; }
        public List<string> ListPhoneLead { get; set; }
        public List<CheckDuplicateLeadWithCustomerEntityModel> ListCustomerContact { get; set; }

        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<CategoryEntityModel> ListInterestedGroup { get; set; }
        public List<CategoryEntityModel> ListGender { get; set; }
        public List<CategoryEntityModel> ListPaymentMethod { get; set; }
        public List<CategoryEntityModel> ListPotential { get; set; }
        public List<CategoryEntityModel> ListLeadType { get; set; }
        public List<CategoryEntityModel> ListLeadStatus { get; set; }
        public List<CategoryEntityModel> ListLeadGroup { get; set; }
        public List<GeographicalAreaEntityModel> ListArea { get; set; }
        public List<Guid> ListLeadInterestedGroupMappingId { get; set; }
        public List<EmployeeEntityModel> ListPersonalInChange { get; set; }
        public List<Models.Note.NoteEntityModel> ListNote { get; set; }
        //NEW 
        public List<ProvinceEntityModel> ListProvince { get; set; }
        public List<CategoryEntityModel> ListBusinessType { get; set; }
        public List<CategoryEntityModel> ListInvestFund { get; set; }
        public List<CategoryEntityModel> ListProbability { get; set; }
        public List<LeadReferenceCustomerModel> ListLeadReferenceCustomer { get; set; }
        public List<LeadDetailModel> ListLeadDetail { get; set; }
        public List<Databases.Entities.Contact> ListLeadContact { get; set; }
        public bool CanDelete { get; set; }
        public bool CanCreateSaleBidding { get; set; }
        public int StatusSaleBiddingAndQuote { get; set; }

        public List<DataAccess.Models.Quote.QuoteEntityModel> ListQuoteById { get; set; }
        public List<DataAccess.Models.Order.CustomerOrderEntityModel> ListOrder { get; set; }
        public List<DataAccess.Models.SaleBidding.SaleBiddingEntityModel> ListSaleBiddingById { get; set; }
        public List<DataAccess.Models.Document.LinkOfDocumentEntityModel> ListLinkOfDocument { get; set; }
        public List<DataAccess.Models.Folder.FileInFolderEntityModel> ListFile { get; set; }
        public List<CategoryEntityModel> ListStatusSupport { get; set; }
        public Guid? StatusSupportId { get; set; }
    }
}
