using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class EmployeeAllowanceEntityModel
    {
        public Guid EmployeeAllowanceId { get; set; }
        public decimal? LunchAllowance { get; set; }
        public decimal? MaternityAllowance { get; set; }
        public decimal? FuelAllowance { get; set; }
        public decimal? PhoneAllowance { get; set; }
        public decimal? OtherAllownce { get; set; }
        public Guid? EmployeeId { get; set; }
        public bool? FreeTimeUnlimited { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }

        public EmployeeAllowanceEntityModel (EmployeeAllowance employeeAllowance)
        {
            EmployeeAllowanceId = employeeAllowance.EmployeeAllowanceId;
            LunchAllowance = employeeAllowance.LunchAllowance;
            MaternityAllowance = employeeAllowance.MaternityAllowance;
            FuelAllowance = employeeAllowance.FuelAllowance;
            PhoneAllowance = employeeAllowance.PhoneAllowance;
            OtherAllownce = employeeAllowance.OtherAllownce;
            FreeTimeUnlimited = employeeAllowance.FreeTimeUnlimited;
            EffectiveDate = employeeAllowance.EffectiveDate;
            EmployeeId = employeeAllowance.EmployeeId;
            CreateById = employeeAllowance.CreateById;
            CreateDate = employeeAllowance.CreateDate;
            UpdateById = employeeAllowance.UpdateById;
            UpdateDate = employeeAllowance.UpdateDate;
        }
    }
}
