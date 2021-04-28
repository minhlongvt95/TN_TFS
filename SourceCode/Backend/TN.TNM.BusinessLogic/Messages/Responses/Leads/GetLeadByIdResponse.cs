using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Company;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Lead;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class GetLeadByIdResponse : BaseResponse
    {
        public LeadModel Lead { get; set; }
        public List<CompanyModel> ListCompany { get; set; }
        public CompanyModel Company { get; set; }
        public ContactModel Contact { get; set; }
        public List<EmployeeModel> Employee { get; set; }
        public List<CategoryModel> StatusCategory { get; set; }
        public List<CategoryModel> Potential { get; set; }
        public string InterestedGroupName { get; set; }
        public string Status_Code { get; set; }
        public string PotentialName { get; set; }
        public string Status_Name { get; set; }
        public string PIC_Name { get; set; }
        public string PositionName { get; set; }
        public Guid? ResponsibleName { get; set; }
        public List<CategoryModel> InterestedList { get; set; }
        public List<CategoryModel> PaymentMethod { get; set; }
        public string PaymentMethodName { get; set; }
        public string InterestedName { get; set; }

        public List<CategoryModel> Genders { get; set; }
        public string FullAddress { get; set; }
        public int CountLead { get; set; }
        public int StatusSaleBiddingAndQuote { get; set; }
    }

}
