using System;

namespace TN.TNM.DataAccess.Messages.Results.Users
{
    public class GetCheckResetCodeUserResult : BaseResult
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
    }
}
