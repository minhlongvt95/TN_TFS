using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.Users
{
    public class GetCheckUserNameResponse : BaseResponse
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
    }
}
