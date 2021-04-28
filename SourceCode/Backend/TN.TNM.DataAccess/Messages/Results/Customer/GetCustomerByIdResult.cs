using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Lead;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetCustomerByIdResult : BaseResult
    {
        public List<CategoryEntityModel> ListStatusCustomerCare { get; set; }
        public List<CategoryEntityModel> ListCustomerGroup { get; set; }
        public List<CategoryEntityModel> ListCustomerStatus { get; set; }
        public List<CategoryEntityModel> ListBusinessType { get; set; }
        public List<CategoryEntityModel> ListBusinessSize { get; set; }
        public List<CategoryEntityModel> ListPaymentMethod { get; set; }
        public List<CategoryEntityModel> ListTypeOfBusiness { get; set; }
        public List<CategoryEntityModel> ListBusinessCareer { get; set; }
        public List<CategoryEntityModel> ListLocalTypeBusiness { get; set; }
        public List<CategoryEntityModel> ListCustomerPosition { get; set; }
        public List<CategoryEntityModel> ListMaritalStatus { get; set; }
        public List<EmployeeEntityModel> ListPersonInCharge { get; set; }
        public List<EmployeeEntityModel> ListCareStaff { get; set; }
        public List<ProvinceEntityModel> ListProvince { get; set; }
        public List<DistrictEntityModel> ListDistrict { get; set; }
        public List<WardEntityModel> ListWard { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
        public List<CustomerAdditionalInformation> ListCustomerAdditionalInformation { get; set; }
        public List<CustomerOtherContactModel> ListCusContact { get; set; }
        public List<dynamic> ListOrderOfCustomer { get; set; }
        public List<Databases.Entities.BankAccount> ListBankAccount { get; set; }
        public List<CustomerCareInforModel> ListCustomerCareInfor { get; set; }
        public CustomerMeetingInforModel CustomerMeetingInfor { get; set; }
        public List<EmployeeEntityModel> ListParticipants { get; set; }
        public List<string> CustomerCode { get; set; }
        public List<AreaEntityModel> ListArea { get; set; }
        public List<LeadEntityModel> ListCustomerLead { get; set; }


        public CustomerEntityModel Customer { get; set; }
        public ContactEntityModel Contact { get; set; }
        public List<Country> CountryList { get; set; }
        public bool isSendApproval { get; set; }
        public bool isApprovalNew { get; set; }
        public bool isApprovalDD { get; set; }
    }
}
