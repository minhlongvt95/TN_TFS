using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class EmployeeMonthySalaryEntityModel
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

        public EmployeeMonthySalaryEntityModel(EmployeeMonthySalary entity)
        {
            EmployeeMonthySalaryId = entity.EmployeeMonthySalaryId;
            CommonId = entity.CommonId;
            EmployeeCode = entity.EmployeeCode;
            EmployeeName = entity.EmployeeName;
            EmployeePostionId = entity.EmployeePostionId;
            PostionName = entity.PostionName;
            EmployeeId = entity.EmployeeId;
            EmployeeUnit = entity.EmployeeUnit;
            EmployeeBranch = entity.EmployeeBranch;
            BasedSalary = entity.BasedSalary;
            MonthlyWorkingDay = entity.MonthlyWorkingDay;
            UnPaidLeaveDay = entity.UnPaidLeaveDay;
            Overtime = entity.Overtime;
            ActualWorkingDay = entity.ActualWorkingDay;
            MonthOfSalary = entity.MonthOfSalary;
            ActualOfSalary = entity.ActualOfSalary;
            OvertimeOfSalary = entity.OvertimeOfSalary;
            EnrollmentSalary = entity.EnrollmentSalary;
            RetentionSalary = entity.RetentionSalary;
            LunchAllowance = entity.LunchAllowance;
            MaternityAllowance = entity.MaternityAllowance;
            FuelAllowance = entity.FuelAllowance;
            PhoneAllowance = entity.PhoneAllowance;
            OtherAllowance = entity.OtherAllowance;
            VacationDay = entity.VacationDay;
            SocialInsuranceSalary = entity.SocialInsuranceSalary;
            SocialInsuranceEmployeePaid = entity.SocialInsuranceEmployeePaid;
            HealthInsuranceEmployeePaid = entity.HealthInsuranceEmployeePaid;
            TotalInsuranceEmployeePaid = entity.TotalInsuranceEmployeePaid;
            UnemploymentinsuranceEmployeePaid = entity.UnemploymentinsuranceEmployeePaid;
            SocialInsuranceCompanyPaid = entity.SocialInsuranceCompanyPaid;
            HealthInsuranceCompanyPaid = entity.HealthInsuranceCompanyPaid;
            UnemploymentinsuranceCompanyPaid = entity.UnemploymentinsuranceCompanyPaid;
            TotalInsuranceCompanyPaid = entity.TotalInsuranceCompanyPaid;
            DesciplineAmount = entity.DesciplineAmount;
            ReductionAmount = entity.ReductionAmount;
            AdditionalAmount = entity.AdditionalAmount;
            BankAccountCode = entity.BankAccountCode;
            BankAccountName = entity.BankAccountName;
            BranchOfBank = entity.BranchOfBank;
            TotalIncome = entity.TotalIncome;
            ActualPaid = entity.ActualPaid;
            Description = entity.Description;
            Month = entity.Month;
            Year = entity.Year;
            StatusId = entity.StatusId;
            Type = entity.Type;
            Email = entity.Email;
            CreateDate = entity.CreateDate;
            CreateById = entity.CreateById;
            UpdateDate = entity.UpdateDate;
            UpdateById = entity.UpdateById;
        }
    }
}
