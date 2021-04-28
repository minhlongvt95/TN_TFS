using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.Users
{
    public class GetCheckResetCodeUserResponse : BaseResponse
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
    }
}
