using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Lead;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class EditLeadByIdRequest : BaseRequest<EditLeadByIdParameter>
    {
        public LeadModel Lead { get; set; }
        public ContactModel Contact { get; set; }
        public List<Guid?> ListInterestedId { get; set; }
        public List<ContactModel> ListContact { get; set; }
        public List<DataAccess.Models.Lead.LeadDetailModel> ListLeadDetail { get; set; }
        public List<Guid?> ListDocumentIdNeedRemove { get; set; }
        public List<DataAccess.Models.Document.LinkOfDocumentEntityModel> ListLinkOfDocument { get; set; }

        public override EditLeadByIdParameter ToParameter()
        {
            List<DataAccess.Databases.Entities.Contact> ListContactDAO = new List<DataAccess.Databases.Entities.Contact>();
            ListContact?.ForEach(con =>
            {
                ListContactDAO.Add(con.ToEntity());
            });

            return new EditLeadByIdParameter
            {
                Lead = Lead.ToEntity(),
                Contact = Contact.ToEntity(),
                ListContactDAO = ListContactDAO ?? new List<DataAccess.Databases.Entities.Contact>(),
                ListInterestedId = ListInterestedId ?? new List<Guid?>(),
                ListLeadDetail = ListLeadDetail ?? new List<DataAccess.Models.Lead.LeadDetailModel>(),
                ListDocumentIdNeedRemove = ListDocumentIdNeedRemove ?? new List<Guid?>(),
                ListLinkOfDocument = ListLinkOfDocument ?? new List<DataAccess.Models.Document.LinkOfDocumentEntityModel>(),
                UserId = UserId
            };
        }
    }
}
