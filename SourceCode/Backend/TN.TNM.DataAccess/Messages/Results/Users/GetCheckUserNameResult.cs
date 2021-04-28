using System;

namespace TN.TNM.DataAccess.Messages.Results.Users
{
    public class GetCheckUserNameResult : BaseResult
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
    }
}
