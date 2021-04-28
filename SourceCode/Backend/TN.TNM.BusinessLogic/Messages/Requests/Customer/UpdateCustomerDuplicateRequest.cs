using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class UpdateCustomerDuplicateRequest : BaseRequest<UpdateCustomerDuplicateParameter>
    {
        public List<CustomerModel> lstcontactCustomerDuplicate { get; set; }
        public List<ContactModel> lstcontactContactDuplicate { get; set; }
        public List<ContactModel> lstcontactContact_CON_Duplicate { get; set; }

        public override UpdateCustomerDuplicateParameter ToParameter()
        {
            var parameter = new UpdateCustomerDuplicateParameter
            {
                lstcontactContactDuplicate = new List<DataAccess.Databases.Entities.Contact>(),
                lstcontactContact_CON_Duplicate = new List<DataAccess.Databases.Entities.Contact>(),
                lstcontactCustomerDuplicate = new List<DataAccess.Databases.Entities.Customer>(),
                UserId = this.UserId
            };
            if (this.lstcontactContactDuplicate.Count > 0)
            {
                this.lstcontactContactDuplicate.ForEach(item =>
                {
                    parameter.lstcontactContactDuplicate.Add(item.ToEntity());
                });
            }
            if (this.lstcontactContact_CON_Duplicate.Count > 0)
            {
                this.lstcontactContact_CON_Duplicate.ForEach(item =>
                {
                    parameter.lstcontactContact_CON_Duplicate.Add(item.ToEntity());
                });
            }
            if (this.lstcontactCustomerDuplicate.Count > 0)
            {
                this.lstcontactCustomerDuplicate.ForEach(item =>
                {
                    parameter.lstcontactCustomerDuplicate.Add(item.ToEntity());
                });
            }
            return parameter;
        }
    }
}
