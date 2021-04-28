using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetEmployeeSalaryStatusResponse : BaseResponse
    {
        public bool IsInApprovalProgress { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public string StatusName { get; set; }
        public Guid? ApproverId { get; set; }
        public Guid? PositionId { get; set; }
    }
}
