using System;

namespace TN.TNM.BusinessLogic.Models.Admin
{
    public class AuthModel
    {
        public string Token { get; set; }
        public DateTime LoginTime { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid? PositionId { get; set; }
    }
}
