using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.BusinessLogic.Models.BankAccount;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Lead;
using TN.TNM.BusinessLogic.Models.Note;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class GetCustomerByIdResponse : BaseResponse
    {
        public List<CategoryModel> ListStatusCustomerCare { get; set; }
        public List<CategoryModel> ListCustomerGroup { get; set; }
        public List<CategoryModel> ListCustomerStatus { get; set; }
        public List<CategoryModel> ListBusinessType { get; set; }
        public List<CategoryModel> ListBusinessSize { get; set; }
        public List<CategoryModel> ListPaymentMethod { get; set; }
        public List<CategoryModel> ListTypeOfBusiness { get; set; }
        public List<CategoryModel> ListBusinessCareer { get; set; }
        public List<CategoryModel> ListLocalTypeBusiness { get; set; }
        public List<CategoryModel> ListCustomerPosition { get; set; }
        public List<CategoryModel> ListMaritalStatus { get; set; }
        public List<EmployeeModel> ListPersonInCharge { get; set; }
        public List<EmployeeModel> ListCareStaff { get; set; }
        public List<AreaModel> ListArea { get; set; }
        public List<ProvinceModel> ListProvince { get; set; }
        public List<DistrictModel> ListDistrict { get; set; }
        public List<WardModel> ListWard { get; set; }
        public List<NoteModel> ListNote { get; set; }
        public List<CustomerAdditionalInformationModel> ListCustomerAdditionalInformation { get; set; }
        public List<CustomerOtherContactBusinessModel> ListCusContact { get; set; }
        public List<dynamic> ListOrderOfCustomer { get; set; }
        public List<BankAccountModel> ListBankAccount { get; set; }
        public List<CustomerCareInforBusinessModel> ListCustomerCareInfor { get; set; }
        public CustomerMeetingInforBusinessModel CustomerMeetingInfor { get; set; }
        public List<EmployeeModel> ListParticipants { get; set; }
        public List<string> CustomerCode { get; set; }
        public List<LeadModel> ListCustomerLead { get; set; }

       
        public CustomerModel Customer { get; set; }
        public ContactModel Contact { get; set; }
        public List<CountryModel> CountryList { get; set; }
        public bool isSendApproval { get; set; }
        public bool isApprovalNew { get; set; }
        public bool isApprovalDD { get; set; }
    }
}
