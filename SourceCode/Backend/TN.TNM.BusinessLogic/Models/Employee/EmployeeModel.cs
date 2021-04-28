using System;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Models.Employee
{
    public class EmployeeModel : BaseModel<DataAccess.Databases.Entities.Employee>
    {
        public Guid? EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? StartedDate { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? PositionId { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public bool? IsManager { get; set; }
        public Guid? PermissionSetId { get; set; }
        public Guid? ContactId { get; set; }
        public Guid? ContractType { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public string Identity { get; set; }
        public string Username { get; set; }
        public string OrganizationName { get; set; }
        public string AvatarUrl { get; set; }
        public string PositionName { get; set; }
        public string LastName { get; set; }
        public string ContractName { get; set; }
        public DateTime? ProbationEndDate { get; set; }
        public DateTime? ProbationStartDate { get; set; }
        public DateTime? TrainingStartDate { get; set; }
        public bool? ActiveUser { get; set; }
        public Guid? RoleId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string EmployeeCodeName { get; set; }
        public bool IsTakeCare { get; set; }

        public EmployeeModel() { }

        public EmployeeModel(EmployeeEntityModel entity)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(entity, this);
        }

        public EmployeeModel(DataAccess.Databases.Entities.Employee entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }

        public override DataAccess.Databases.Entities.Employee ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.Employee();
            Mapper(this, entity);
            return entity;
        }

        public EmployeeEntityModel ToEntityModel()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new EmployeeEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}

