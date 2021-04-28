using System;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetEmployeeRequestByIdResult : BaseResult
    {
        public EmployeeRequestEntityModel EmployeeRequest { get; set; }
        public bool IsInApprovalProgress { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public string StatusName { get; set; }
        public Guid? ApproverId { get; set; }
        public Guid? PositionId { get; set; }
    }
}
