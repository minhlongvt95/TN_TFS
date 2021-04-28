using System;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetLeadByIdRequest : BaseRequest<GetLeadByIdParamater>
    {
        public Guid? LeadId { get; set; }
        public Guid? ContactId { get; set; }
        public Guid? CompanyId { get; set; }
        public string StatusCode { get; set; }
        public string PotentialCode { get; set; }
        public string InterestedCode { get; set; }
        public string PaymentCode { get; set; }
        public override GetLeadByIdParamater ToParameter() => new GetLeadByIdParamater
        {
            LeadId = LeadId,
            ContactId = ContactId,
            CompanyId = CompanyId,
            PotentialCode = PotentialCode,
            StatusCode = StatusCode,
            InterestedCode = InterestedCode,
            PaymentCode = PaymentCode,
            UserId = UserId
        };
    }
}

