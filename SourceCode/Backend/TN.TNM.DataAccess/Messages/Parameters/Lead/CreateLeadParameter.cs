using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class CreateLeadParameter : BaseParameter
    {
        public Databases.Entities.Lead Lead { get; set; }
        public Databases.Entities.Contact Contact { get; set; }
        public bool IsCreateCompany { get; set; }
        public string CompanyName { get; set; }
        public List<Guid?> ListInterestedId { get; set; }
        public List<Databases.Entities.Contact> ListContactDAO { get; set; }
        public List<DataAccess.Models.Lead.LeadDetailModel> ListLeadDetail { get; set; }
    }
}
