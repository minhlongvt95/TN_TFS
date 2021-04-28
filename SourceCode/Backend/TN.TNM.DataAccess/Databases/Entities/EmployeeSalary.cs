using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class EmployeeSalary
    {
        public Guid EmployeeSalaryId { get; set; }
        public decimal? EmployeeSalaryBase { get; set; }
        public decimal? ResponsibilitySalary { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public Guid? EmployeeId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }
        public Guid? TenantId { get; set; }

        public Employee Employee { get; set; }
    }
}
