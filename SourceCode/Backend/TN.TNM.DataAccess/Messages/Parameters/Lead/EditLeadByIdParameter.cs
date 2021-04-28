using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class EditLeadByIdParameter : BaseParameter
    {
        public Databases.Entities.Lead Lead { get; set; }
        public Databases.Entities.Contact Contact { get; set; }
        public List<Guid?> ListInterestedId { get; set; }
        public List<Databases.Entities.Contact> ListContactDAO { get; set; }
        public List<DataAccess.Models.Lead.LeadDetailModel> ListLeadDetail { get; set; }
        public List<DataAccess.Models.Document.LinkOfDocumentEntityModel> ListLinkOfDocument { get; set; }
        public List<Guid?> ListDocumentIdNeedRemove { get; set; }
        public Boolean IsGetNoti { get; set; }
    }
}
