using System;
using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.Users
{
    public class GetUserProfileByEmailResponse : BaseResponse
    {
        public Guid UserId { get; set; }
        public Guid EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string AvatarUrl { get; set; }
        public Guid? PositionId { get; set; }
        public List<string> PermissionList { get; set; }
        public bool IsManager { get; set; }
    }
}
