using System;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class CreateEmployeeRequestResult : BaseResult
    {
        public Guid EmployeeRequestId { get; set; }
        public string EmployeeRequestCode { get; set; }
        public DataAccess.Models.Email.SendEmailEntityModel SendEmaiModel { get; set; }
    }
}
