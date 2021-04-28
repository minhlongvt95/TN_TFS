using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Lead;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.Quote;
using TN.TNM.DataAccess.Models.SaleBidding;

namespace TN.TNM.DataAccess.Messages.Results.Quote
{
    public class GetDataCreateUpdateQuoteResult : BaseResult
    {
        public List<CustomerEntityModel> ListCustomerAll { get; set; }
        public List<CustomerEntityModel> ListCustomer { get; set; }
        public List<CustomerEntityModel> ListCustomerNew { get; set; }
        public List<LeadEntityModel> ListLead { get; set; }
        public List<CategoryEntityModel> ListPaymentMethod { get; set; }
        public List<CategoryEntityModel> ListAdditionalInformationTemplates { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; } //List người phụ trách
        public List<CategoryEntityModel> ListQuoteStatus { get; set; }

        public QuoteEntityModel Quote { get; set; }
        public List<QuoteDetailEntityModel> ListQuoteDetail { get; set; }
        public List<QuoteCostDetailEntityModel> ListQuoteCostDetail { get; set; }
        public List<QuoteDocumentEntityModel> ListQuoteDocument { get; set; }
        public List<AdditionalInformationEntityModel> ListAdditionalInformation { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
        public List<SaleBiddingEntityModel> ListSaleBidding { get; set; }

        public List<CategoryEntityModel> ListInvestFund { get; set; }
        public bool IsAprovalQuote { get; set; }

        public List<ProductVendorMappingEntityModel> ListProductVendorMapping { get; set; }
        public List<EmployeeEntityModel> ListParticipant { get; set; }
        public List<Guid> ListParticipantId { get; set; }
    }
}
