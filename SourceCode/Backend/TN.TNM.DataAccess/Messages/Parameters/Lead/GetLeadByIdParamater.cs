using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class GetLeadByIdParamater : BaseParameter
    {
        public Guid? LeadId { get; set; }
        public Guid? ContactId { get; set; }
        public Guid? CompanyId { get; set; }
        public string StatusCode { get; set; }
        public string PotentialCode { get; set; }
        public string InterestedCode { get; set; }
        public string PaymentCode { get; set; }
    }
}
