using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Lead;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class ImportLeadResponse:BaseResponse
    {
        public List<LeadModel> lstcontactLeadDuplicate { get; set; }
        public List<ContactModel> lstcontactContactDuplicate { get; set; }
        public List<ContactModel> lstcontactCustomerDuplicate { get; set; }
    }
}
