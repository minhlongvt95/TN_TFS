using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class EmployeeSalaryEntityModel
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
        public EmployeeSalaryEntityModel(EmployeeSalary employeeSalary)
        {
            EmployeeSalaryId = employeeSalary.EmployeeSalaryId;
            EmployeeSalaryBase = employeeSalary.EmployeeSalaryBase;
            ResponsibilitySalary = employeeSalary.ResponsibilitySalary;
            EffectiveDate = employeeSalary.EffectiveDate;
            EmployeeId = employeeSalary.EmployeeId;
            CreateById = employeeSalary.CreateById;
            CreateDate = employeeSalary.CreateDate;
            UpdateById = employeeSalary.UpdateById;
            UpdateDate = employeeSalary.UpdateDate;
        }
    }
}
