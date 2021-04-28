using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Lead;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.BusinessLogic.Models.Quote;
using TN.TNM.BusinessLogic.Models.SaleBidding;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class GetDataCreateUpdateQuoteResponse : BaseResponse
    {
        public List<CustomerModel> ListCustomerAll { get; set; }
        public List<CustomerModel> ListCustomer { get; set; }
        public List<CustomerModel> ListCustomerNew { get; set; }
        public List<LeadModel> ListLead { get; set; }
        public List<CategoryModel> ListPaymentMethod { get; set; }
        public List<EmployeeModel> ListEmployee { get; set; }
        public List<CategoryModel> ListQuoteStatus { get; set; }

        public QuoteModel Quote { get; set; }
        public List<QuoteDetailModel> ListQuoteDetail { get; set; }
        public List<QuoteCostDetailModel> ListQuoteCostDetail { get; set; }
        public List<QuoteDocumentModel> ListQuoteDocument { get; set; }
        public List<AdditionalInformationModel> ListAdditionalInformation { get; set; }
        public List<CategoryModel> ListAdditionalInformationTemplates { get; set; }
        public List<NoteModel> ListNote { get; set; }
        public List<SaleBiddingModel> ListSaleBidding { get; set; }
        public List<DataAccess.Models.CategoryEntityModel> ListInvestFund { get; set; }
        public bool IsAprovalQuote { get; set; }

        public List<ProductVendorMappingEntityModel> ListProductVendorMapping { get; set; }
        public List<EmployeeEntityModel> ListParticipant { get; set; }
        public List<Guid> ListParticipantId { get; set; }
    }
}
