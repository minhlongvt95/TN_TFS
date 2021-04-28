using System;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class EmployeeAssessmentEntityModel
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
    }
}
