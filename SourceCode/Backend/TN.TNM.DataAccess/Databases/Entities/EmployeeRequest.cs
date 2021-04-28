using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class EmployeeRequest
    {
        public Guid EmployeeRequestId { get; set; }
        public string EmployeeRequestCode { get; set; }
        public Guid? CreateEmployeeId { get; set; }
        public string CreateEmployeeCode { get; set; }
        public Guid OfferEmployeeId { get; set; }
        public string OfferEmployeeCode { get; set; }
        public Guid? TypeRequest { get; set; }
        public Guid? StatusId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EnDate { get; set; }
        public Guid? StartTypeTime { get; set; }
        public Guid? EndTypeTime { get; set; }
        public DateTime? RequestDate { get; set; }
        public Guid? TypeReason { get; set; }
        public string Detail { get; set; }
        public Guid? ManagerId { get; set; }
        public Guid? ApproverId { get; set; }
        public string NotifyList { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }
        public Guid? TenantId { get; set; }
        public int? StepNumber { get; set; }

        public Employee OfferEmployee { get; set; }
    }
}
