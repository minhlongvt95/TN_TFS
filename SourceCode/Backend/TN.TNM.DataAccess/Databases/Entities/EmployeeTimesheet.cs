using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class EmployeeTimesheet
    {
        public Guid EmployeeTimesheetId { get; set; }
        public Guid? EmployeeId { get; set; }
        public DateTime? TimesheetDate { get; set; }
        public TimeSpan? CheckIn { get; set; }
        public TimeSpan? CheckOut { get; set; }
        public decimal? ActualWorkingDay { get; set; }
        public decimal? Overtime { get; set; }
        public decimal? ReductionAmount { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public string Center { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }
        public Guid? TenantId { get; set; }

        public Employee Employee { get; set; }
    }
}
