using System;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class CheckEmailLeadResult: BaseResult
    {
        public Guid ContactId { get; set; }
        public Guid ObjectId { get; set; }
        public bool CheckEmail { get; set; }
        public string ObjectType { get; set; }
    }
}
