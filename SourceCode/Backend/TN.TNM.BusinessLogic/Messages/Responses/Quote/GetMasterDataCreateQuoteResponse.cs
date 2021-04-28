using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Lead;
using TN.TNM.DataAccess.Models.SaleBidding;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class GetMasterDataCreateQuoteResponse : BaseResponse
    {
        public List<CategoryEntityModel> ListInvestFund { get; set; }
        public List<CategoryEntityModel> ListAdditionalInformationTemplates { get; set; }
        public List<CategoryEntityModel> ListPaymentMethod { get; set; }
        public List<CategoryEntityModel> ListQuoteStatus { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<CustomerEntityModel> ListCustomer { get; set; }
        public List<CustomerEntityModel> ListCustomerNew { get; set; }
        public List<LeadEntityModel> ListAllLead { get; set; }
        public List<SaleBiddingEntityModel> ListAllSaleBidding { get; set; }
        public List<EmployeeEntityModel> ListParticipant { get; set; }
    }
}
