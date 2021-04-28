using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ProcurementRequest
    {
        public ProcurementRequest()
        {
            ProcurementRequestItem = new HashSet<ProcurementRequestItem>();
        }

        public Guid ProcurementRequestId { get; set; }
        public string ProcurementCode { get; set; }
        public string ProcurementContent { get; set; }
        public Guid? RequestEmployeeId { get; set; }
        public string EmployeePhone { get; set; }
        public Guid? Unit { get; set; }
        public Guid? ApproverId { get; set; }
        public Guid? ApproverPostion { get; set; }
        public string Explain { get; set; }
        public Guid? StatusId { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? OrderId { get; set; }
        public bool? Active { get; set; }

        public Employee Approver { get; set; }
        public Employee RequestEmployee { get; set; }
        public ICollection<ProcurementRequestItem> ProcurementRequestItem { get; set; }
    }
}
