using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class CreateEmployeeRequestResponse : BaseResponse
    {
        public Guid EmployeeRequestId { get; set; }
        public string EmployeeRequestCode { get; set; }
        public DataAccess.Models.Email.SendEmailEntityModel SendEmaiModel { get; set; }
    }
}
