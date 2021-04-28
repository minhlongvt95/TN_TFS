using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class CheckEmailLeadResponse : BaseResponse
    {
        public Guid ContactId { get; set; }
        public Guid ObjectId { get; set; }
        public bool CheckEmail { get; set; }
        public string ObjectType { get; set; }
    }
}
