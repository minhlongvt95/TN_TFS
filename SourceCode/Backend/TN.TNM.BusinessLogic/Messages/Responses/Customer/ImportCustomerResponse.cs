using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Customer;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class ImportCustomerResponse:BaseResponse
    {
        public List<CustomerModel> lstcontactCustomerDuplicate { get; set; }
        public List<ContactModel> lstcontactContactDuplicate { get; set; }
        public List<ContactModel> lstcontactContact_CON_Duplicate { get; set; }
        public bool isDupblicateInFile { get; set; }
    }
}
