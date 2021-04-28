using System;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class CheckPhoneLeadResult: BaseResult
    {
        public Guid ContactId { get; set; }
        public Guid ObjectId { get; set; }
        public bool CheckPhone { get; set; }
        public string ObjectType { get; set; }
    }
}
