using System;
using System.Collections;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Lead;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class CreateLeadRequest : BaseRequest<CreateLeadParameter>
    {
        public LeadModel Lead { get; set; }
        public ContactModel Contact { get; set; }
        public bool IsCreateCompany { get; set; }
        public string CompanyName { get; set; }
        public List<Guid?> ListInterestedId { get; set; }
        public List<ContactModel> ListContact { get; set; }
        public List<DataAccess.Models.Lead.LeadDetailModel> ListLeadDetail { get; set; }

        public override CreateLeadParameter ToParameter()
        {
            List<DataAccess.Databases.Entities.Contact> ListContactDAO = new List<DataAccess.Databases.Entities.Contact>();
            ListContact?.ForEach(con =>
            {
                ListContactDAO.Add(con.ToEntity());
            });

            return new CreateLeadParameter()
            {
                Lead = Lead.ToEntity(),
                Contact = Contact.ToEntity(),
                IsCreateCompany = IsCreateCompany,
                CompanyName = CompanyName,
                ListInterestedId = ListInterestedId,
                ListContactDAO = ListContactDAO ?? new List<DataAccess.Databases.Entities.Contact>(),
                ListLeadDetail = ListLeadDetail ?? new List<DataAccess.Models.Lead.LeadDetailModel>(),
                UserId = UserId
            };
        }
    }

}
