using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Models.Employee
{
    public class EmployeeSalaryModel: BaseModel<EmployeeSalary>
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
        public EmployeeSalaryModel() { }

        public EmployeeSalaryModel(EmployeeSalaryEntityModel entity)
        {
            Mapper(entity, this);
        }

        public EmployeeSalaryModel(EmployeeSalary entity) : base(entity)
        {
        }

        public override EmployeeSalary ToEntity()
        {
            var entity = new EmployeeSalary();
            Mapper(this, entity);
            return entity;
        }
    }
}
