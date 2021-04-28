using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Models.Employee
{
    public class EmployeeInsuranceModel : BaseModel<EmployeeInsurance> 
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

        public EmployeeInsuranceModel() { }

        public EmployeeInsuranceModel(EmployeeInsuranceEntityModel entity)
        {
            Mapper(entity, this);
        }

        public EmployeeInsuranceModel (EmployeeInsurance entity) : base(entity)
        {
        }

        public override EmployeeInsurance ToEntity()
        {
            var entity = new EmployeeInsurance();
            Mapper(this, entity);
            return entity;
        }
    }
}
