using System;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetEmployeeRequestByIdResponse : BaseResponse
    {
        public EmployeeRequestModel EmployeeRequest { get; set; }
        public bool IsInApprovalProgress { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public string StatusName { get; set; }
        public Guid? ApproverId { get; set; }
        public Guid? PositionId { get; set; }
    }
}
