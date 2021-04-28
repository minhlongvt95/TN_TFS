using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Models.Employee
{
    public class EmployeeAssessmentModel:BaseModel<EmployeeAssessment>
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
        public EmployeeAssessmentModel(EmployeeAssessmentEntityModel entity)
        {
            Mapper(entity, this);
        }

        public EmployeeAssessmentModel(EmployeeAssessment entity) : base(entity)
        {
        }

        public override EmployeeAssessment ToEntity()
        {
            var entity = new EmployeeAssessment();
            Mapper(this, entity);
            return entity;
        }
    }
}
