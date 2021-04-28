using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class CheckPhoneLeadResponse : BaseResponse
    {
        public Guid ContactId { get; set; }
        public Guid ObjectId { get; set; }
        public bool CheckPhone { get; set; }
        public string ObjectType { get; set; }
    }
}
