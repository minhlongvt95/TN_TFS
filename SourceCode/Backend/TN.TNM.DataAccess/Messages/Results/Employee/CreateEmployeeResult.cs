using System;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class CreateEmployeeResult : BaseResult
    {
        public Guid EmployeeId { get; set; }
        public Guid ContactId { get; set; }
        public DataAccess.Models.Email.SendEmailEntityModel SendEmailEntityModel { get; set; }
    }
}
