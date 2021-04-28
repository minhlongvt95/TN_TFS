using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Models.Employee
{
    public class EmployeeAllowanceModel : BaseModel<EmployeeAllowance>
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
        public EmployeeAllowanceModel() { }

        public EmployeeAllowanceModel(EmployeeAllowanceEntityModel entity)
        {
            Mapper(entity, this);
        }

        public EmployeeAllowanceModel(EmployeeAllowance entity) : base(entity)
        {
        }

        public override EmployeeAllowance ToEntity()
        {
            var entity = new EmployeeAllowance();
            Mapper(this, entity);
            return entity;
        }
    }
}
