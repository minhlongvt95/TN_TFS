using System;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ProductionAndBusinessSalary
    {
        public Guid ProductionAndBusinessSalaryId { get; set; }
        public decimal? Salary { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? ApplyDate { get; set; }
        public Guid? EmployeeId { get; set; }
        public bool? IsNewest { get; set; }

        public Employee Employee { get; set; }
    }
}
