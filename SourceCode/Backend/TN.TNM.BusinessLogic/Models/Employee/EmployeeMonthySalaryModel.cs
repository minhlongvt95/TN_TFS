using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Models.Employee
{
    public class EmployeeMonthySalaryModel : BaseModel<EmployeeMonthySalary>
    {
        public Guid EmployeeMonthySalaryId { get; set; }
        public Guid CommonId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public Guid? EmployeePostionId { get; set; }
        public string PostionName { get; set; }
        public Guid? EmployeeId { get; set; }
        public string EmployeeUnit { get; set; }
        public string EmployeeBranch { get; set; }
        public decimal? BasedSalary { get; set; }
        public int? MonthlyWorkingDay { get; set; }
        public decimal? UnPaidLeaveDay { get; set; }
        public decimal? VacationDay { get; set; }
        public decimal? Overtime { get; set; }
        public decimal? ActualWorkingDay { get; set; }
        public int? MonthOfSalary { get; set; }
        public decimal? ActualOfSalary { get; set; }
        public decimal? OvertimeOfSalary { get; set; }
        public decimal? EnrollmentSalary { get; set; }
        public decimal? RetentionSalary { get; set; }
        public decimal? LunchAllowance { get; set; }
        public decimal? MaternityAllowance { get; set; }
        public decimal? FuelAllowance { get; set; }
        public decimal? PhoneAllowance { get; set; }
        public decimal? OtherAllowance { get; set; }
        public decimal? SocialInsuranceSalary { get; set; }
        public decimal? SocialInsuranceEmployeePaid { get; set; }
        public decimal? HealthInsuranceEmployeePaid { get; set; }
        public decimal? TotalInsuranceEmployeePaid { get; set; }
        public decimal? UnemploymentinsuranceEmployeePaid { get; set; }
        public decimal? SocialInsuranceCompanyPaid { get; set; }
        public decimal? HealthInsuranceCompanyPaid { get; set; }
        public decimal? UnemploymentinsuranceCompanyPaid { get; set; }
        public decimal? TotalInsuranceCompanyPaid { get; set; }
        public decimal? DesciplineAmount { get; set; }
        public decimal? ReductionAmount { get; set; }
        public decimal? AdditionalAmount { get; set; }
        public string BankAccountCode { get; set; }
        public string BankAccountName { get; set; }
        public string BranchOfBank { get; set; }
        public decimal? TotalIncome { get; set; }
        public decimal? ActualPaid { get; set; }
        public string Description { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public string Email { get; set; }
        public Guid? StatusId { get; set; }
        public int? Type { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }

        public EmployeeMonthySalaryModel() { }

        public EmployeeMonthySalaryModel(EmployeeMonthySalaryEntityModel entity)
        {
            Mapper(entity, this);
        }

        public EmployeeMonthySalaryModel(EmployeeMonthySalary entity) : base(entity)
        {
        }

        public override EmployeeMonthySalary ToEntity()
        {
            var entity = new EmployeeMonthySalary();
            Mapper(this, entity);
            return entity;
        }
    }
}
