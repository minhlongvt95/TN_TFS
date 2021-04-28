using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Employee
    {
        public Employee()
        {
            EmployeeAllowance = new HashSet<EmployeeAllowance>();
            EmployeeAssessment = new HashSet<EmployeeAssessment>();
            EmployeeInsurance = new HashSet<EmployeeInsurance>();
            EmployeeMonthySalary = new HashSet<EmployeeMonthySalary>();
            EmployeeRequest = new HashSet<EmployeeRequest>();
            EmployeeSalary = new HashSet<EmployeeSalary>();
            EmployeeTimesheet = new HashSet<EmployeeTimesheet>();
            Notifications = new HashSet<Notifications>();
            ProcurementRequestApprover = new HashSet<ProcurementRequest>();
            ProcurementRequestRequestEmployee = new HashSet<ProcurementRequest>();
            User = new HashSet<User>();
        }

        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime? StartedDate { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? PositionId { get; set; }
        public bool IsManager { get; set; }
        public DateTime? ProbationEndDate { get; set; }
        public DateTime? ProbationStartDate { get; set; }
        public DateTime? TrainingStartDate { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? ContractType { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? BranchId { get; set; }
        public bool? IsOrder { get; set; }
        public bool? IsCashier { get; set; }
        public bool IsTakeCare { get; set; }

        public Organization Organization { get; set; }
        public ICollection<EmployeeAllowance> EmployeeAllowance { get; set; }
        public ICollection<EmployeeAssessment> EmployeeAssessment { get; set; }
        public ICollection<EmployeeInsurance> EmployeeInsurance { get; set; }
        public ICollection<EmployeeMonthySalary> EmployeeMonthySalary { get; set; }
        public ICollection<EmployeeRequest> EmployeeRequest { get; set; }
        public ICollection<EmployeeSalary> EmployeeSalary { get; set; }
        public ICollection<EmployeeTimesheet> EmployeeTimesheet { get; set; }
        public ICollection<Notifications> Notifications { get; set; }
        public ICollection<ProcurementRequest> ProcurementRequestApprover { get; set; }
        public ICollection<ProcurementRequest> ProcurementRequestRequestEmployee { get; set; }
        public ICollection<User> User { get; set; }
    }
}
