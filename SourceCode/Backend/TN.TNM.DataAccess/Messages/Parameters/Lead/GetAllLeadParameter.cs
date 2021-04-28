using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class GetAllLeadParameter : BaseParameter
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public List<Guid?> PotentialId { get; set; }
        public List<Guid?> StatusId { get; set; }
        public List<Guid?> InterestedGroupId { get; set; }
        public List<Guid?> PersonInChargeId { get; set; }
        public List<Guid?> LeadType { get; set; }
        public List<Guid?> ListSourceId { get; set; }
        public List<Guid?> ListAreaId { get; set; }
        public List<Guid?> ListCusGroupId { get; set; }
        public bool NoActivePic { get; set; }
        public string Sort { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool WaitingForApproval { get; set; }
    }
}
