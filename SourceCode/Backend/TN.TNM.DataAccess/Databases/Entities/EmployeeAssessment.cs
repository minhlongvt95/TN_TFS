using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class EmployeeAssessment
    {
        public Guid EmployeeAssessmentId { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public Guid? Type { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }
        public Guid? TenantId { get; set; }

        public Employee Employee { get; set; }
    }
}
