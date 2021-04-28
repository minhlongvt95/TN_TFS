using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class CreateEmployeeResponse : BaseResponse
    {
        public Guid EmployeeId { get; set; }
        public Guid ContactId { get; set; }
        public DataAccess.Models.Email.SendEmailEntityModel SendEmailEntityModel { get; set; }
    }
}
