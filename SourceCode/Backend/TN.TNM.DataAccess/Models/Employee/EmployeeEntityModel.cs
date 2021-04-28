using System;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class EmployeeEntityModel
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
        public string Identity { get; set; }
        public string Username { get; set; }
        public string OrganizationName { get; set; }
        public string AvatarUrl { get; set; }
        public string PositionName { get; set; }
        public string LastName { get; set; }
        public string ContractName { get; set; }
        public Guid? ContractType { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public DateTime? ProbationEndDate { get; set; }
        public DateTime? ProbationStartDate { get; set; }
        public DateTime? TrainingStartDate { get; set; }
        public bool? ActiveUser { get; set; }
        public Guid? RoleId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string EmployeeCodeName { get; set; }
        public bool IsTakeCare { get; set; }

        public EmployeeEntityModel() { }

        public EmployeeEntityModel(Databases.Entities.Employee entity)
        {
            EmployeeId = entity.EmployeeId;
            EmployeeCode = entity.EmployeeCode;
            EmployeeName = entity.EmployeeName;
            Active = entity.Active;
            CreatedById = entity.CreatedById;
            CreatedDate = entity.CreatedDate;
            UpdatedById = entity.UpdatedById;
            UpdatedDate = entity.UpdatedDate;
            PositionId = entity.PositionId;
            OrganizationId = entity.OrganizationId;
        }
    }
}
