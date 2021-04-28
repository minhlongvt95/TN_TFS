using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Lead;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class UpdateLeadDuplicateRequest : BaseRequest<UpdateLeadDuplicateParameter>
    {
        public List<LeadModel> lstcontactLeadDuplicate { get; set; }
        public List<ContactModel> lstcontactContactDuplicate { get; set; }

        public override UpdateLeadDuplicateParameter ToParameter()
        {
            var parameter= new UpdateLeadDuplicateParameter
            {
                UserId = this.UserId,
                lstcontactLeadDuplicate = new List<DataAccess.Databases.Entities.Lead>(),
                lstcontactContactDuplicate = new List<DataAccess.Databases.Entities.Contact>(),
            };

            if (this.lstcontactLeadDuplicate.Count > 0)
            {
                this.lstcontactLeadDuplicate.ForEach(item =>
                {
                    parameter.lstcontactLeadDuplicate.Add(item.ToEntity());
                });
            }
            if (this.lstcontactContactDuplicate.Count > 0)
            {
                this.lstcontactContactDuplicate.ForEach(item =>
                {
                    parameter.lstcontactContactDuplicate.Add(item.ToEntity());
                });
            }
            return parameter;
        }
    }
}
