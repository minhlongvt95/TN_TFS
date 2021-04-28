using System;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class EmployeeInsuranceEntityModel
    {
        public Guid EmployeeInsuranceId { get; set; }
        public Guid? EmployeeId { get; set; }
        public decimal? SocialInsuranceSalary { get; set; }
        public decimal? SocialInsuranceSupportPercent { get; set; }
        public decimal? HealthInsuranceSupportPercent { get; set; }
        public decimal? UnemploymentinsuranceSupportPercent { get; set; }
        public decimal? SocialInsurancePercent { get; set; }
        public decimal? HealthInsurancePercent { get; set; }
        public decimal? UnemploymentinsurancePercent { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public Guid? UpdateById { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
