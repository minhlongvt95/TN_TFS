using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetAllLeadRequest : BaseRequest<GetAllLeadParameter>
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

        public override GetAllLeadParameter ToParameter() => new GetAllLeadParameter
        {
            FirstName = FirstName,
            LastName = LastName,
            Phone = Phone,
            Email = Email,
            PotentialId = PotentialId,
            StatusId = StatusId,
            InterestedGroupId = InterestedGroupId,
            LeadType = LeadType,
            PersonInChargeId = PersonInChargeId,
            NoActivePic = NoActivePic,
            Sort = Sort,
            FromDate = FromDate,
            ToDate = ToDate,
            WaitingForApproval = WaitingForApproval,
            ListAreaId = ListAreaId,
            ListCusGroupId = ListCusGroupId,
            ListSourceId = ListSourceId,
            UserId = UserId
        };
    }
}
