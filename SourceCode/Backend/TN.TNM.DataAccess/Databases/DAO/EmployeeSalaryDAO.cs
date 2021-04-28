using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using OfficeOpenXml;
using TN.TNM.Common;
using TN.TNM.Common.Helper;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.Employee;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class EmployeeSalaryDAO : BaseDAO, IEmployeeSalaryDataAccess
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public EmployeeSalaryDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment _hostingEnvironment)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            this.hostingEnvironment = _hostingEnvironment;

        }

        public EmployeeTimeSheetImportResult EmployeeTimeSheetImport(EmployeeTimeSheetImportParameter parameter)
        {
            byte[] excelOutput = null;
            var newCommonId = Guid.NewGuid();
            if (parameter.FileList != null && parameter.FileList.Count > 0)
            {
                using (var stream = new MemoryStream())
                {
                    var lstEmployeeMonthySalaryExist = (from ems in context.EmployeeMonthySalary
                                                        join c in context.Category on ems.StatusId equals c.CategoryId
                                                        where c.CategoryCode == "Approved" && ems.Month == parameter.Month && ems.Year == parameter.Year
                                                        select ems).ToList();
                    if (lstEmployeeMonthySalaryExist.Count > 0)
                    {
                        return new EmployeeTimeSheetImportResult
                        {
                            ExcelFile = null,
                            Message = string.Format("Bảng lương {0}/{1} đã được Approved không thể thay đổi", parameter.Month, parameter.Year),
                            Status = false
                        };

                    }
                    else
                    {
                        //Xóa toàn bộ bảng lương cũ
                        lstEmployeeMonthySalaryExist = (from ems in context.EmployeeMonthySalary
                                                        where ems.Month == parameter.Month && ems.Year == parameter.Year
                                                        select ems).ToList();
                        context.EmployeeMonthySalary.RemoveRange(lstEmployeeMonthySalaryExist);
                        context.SaveChanges();

                        parameter.FileList[0].CopyTo(stream);
                        using (ExcelPackage package = new ExcelPackage(stream))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets["QLTTm"];
                            //Group cells by row
                            var rowcellgroups = worksheet.Cells["A:E"].GroupBy(c => c.Start.Row);
                            //Loại bỏ dòng tiêu đề
                            var groups = rowcellgroups.Skip(1);
                            //Group theo từng ngày
                            var cv = from item in groups
                                     group item by new
                                     {
                                         item.First().Value,
                                         Convert.ToDateTime(item.First(rc => rc.Start.Column == 4).Value).Date
                                     } into gcs
                                     select gcs;
                            //Dánh sách nhân viên
                            var listEmployee = context.Employee.Where(w => w.Active == true).Select(item => new { EmployeeId = item.EmployeeId, EmployeeCode = item.EmployeeCode }).ToList();
                            var listEmpInformation = (from emp in context.Employee
                                                      join pos in context.Position on emp.PositionId equals pos.PositionId
                                                      join org in context.Organization on emp.OrganizationId equals org.OrganizationId
                                                      select new { emp.EmployeeId, emp.EmployeeName, emp.PositionId, emp.EmployeeCode, pos.PositionName, org.OrganizationId, org.OrganizationName }
                                                    ).ToList();

                            var listEmployeeInsurance = context.EmployeeInsurance.GroupBy(g => g.EmployeeId).SelectMany(s => s.OrderByDescending(g => g.EffectiveDate).Take(1)).ToList();
                            var listEmployeeAllowance = context.EmployeeAllowance.GroupBy(g => g.EmployeeId).SelectMany(s => s.OrderByDescending(g => g.EffectiveDate).Take(1)).ToList();
                            var listEmployeeSalary = context.EmployeeSalary.GroupBy(g => g.EmployeeId).SelectMany(s => s.OrderByDescending(g => g.EffectiveDate).Take(1)).ToList();
                            var listEmployeeRequest = context.EmployeeRequest.ToList();
                            var listBankAccount = context.BankAccount.GroupBy(g => g.ObjectId).SelectMany(s => s.OrderByDescending(g => g.CreatedDate).Take(1)).ToList();
                            //Lấy ra giờ làm mỗi ngày cho từng nhân viên
                            var listEmployeeWorkHour = context.Contact.Where(w => w.ObjectType == "EMP").Select(item => new { EmployeeId = item.ObjectId, WorkHourOfEnd = item.WorkHourOfEnd, WorkHourOfStart = item.WorkHourOfStart }).DefaultIfEmpty().ToList();

                            //Lấy ra min max của từng ngày theo từng nhân viên
                            var maxMinGrouped = cv.Select(g => new Entities.EmployeeTimesheet
                            {
                                EmployeeId = listEmployee.Where(x => x.EmployeeCode == g.Key.Value.ToString()).Select(d => (Guid?)d.EmployeeId).DefaultIfEmpty().FirstOrDefault(),
                                TimesheetDate = Convert.ToDateTime(g.Key.Date),
                                CheckOut = Convert.ToDateTime(g.Select(o => o.First(rc => rc.Start.Column == 4)).Max(rc => rc.Value)).TimeOfDay,
                                CheckIn = Convert.ToDateTime(g.Select(o => o.First(rc => rc.Start.Column == 4)).Min(rc => rc.Value)).TimeOfDay,
                                CreateDate = DateTime.Now,
                                CreateById = parameter.UserId
                            }).ToList();
                            //Tính công cho mỗi nhân viên theo file excel
                            maxMinGrouped.ForEach(item =>
                            {
                                var EmployeeIdWorkHour = listEmployeeWorkHour.Where(w => w.EmployeeId == item.EmployeeId).Select(itemS => new { WorkHourOfStart = (TimeSpan?)itemS.WorkHourOfStart, WorkHourOfEnd = (TimeSpan?)itemS.WorkHourOfEnd }).DefaultIfEmpty().First();
                                if (EmployeeIdWorkHour != null)
                                {
                                    TimeSpan? WorkHourOfStart = EmployeeIdWorkHour.WorkHourOfStart;
                                    TimeSpan? WorkHourOfEnd = EmployeeIdWorkHour.WorkHourOfEnd;
                                    if (item.TimesheetDate.HasValue)
                                    {
                                        //Check không phải chủ nhật
                                        //if (item.TimesheetDate.Value.DayOfWeek != 0)
                                        //{
                                        //WorkHourOfEnd = listEmployeeWorkHour.Where(w => w.EmployeeId == item.EmployeeId).Select(itemS => itemS.WorkHourOfEnd).FirstOrDefault();
                                        if (WorkHourOfStart.HasValue && WorkHourOfEnd.HasValue)
                                        {
                                            if (TimeSpan.Compare(item.CheckIn.Value, WorkHourOfStart.Value) <= 0 && TimeSpan.Compare(item.CheckOut.Value, WorkHourOfEnd.Value) >= 0)
                                            {
                                                item.ActualWorkingDay = 1;
                                            }
                                            else
                                            {
                                                if (TimeSpan.Compare(item.CheckIn.Value, WorkHourOfStart.Value) > 0)
                                                {
                                                    TimeSpan span = item.CheckIn.Value - WorkHourOfStart.Value;
                                                    if (span.TotalMinutes > 30)
                                                    {
                                                        var myInClause = new string[] { "Late", "MCC" };
                                                        var empRequest = (from request in listEmployeeRequest
                                                                          join category in context.Category on request.TypeRequest equals category.CategoryId
                                                                          where request.OfferEmployeeId == item.EmployeeId && request.StartDate <= item.TimesheetDate && request.EnDate >= item.TimesheetDate
                                                                          && myInClause.Contains(category.CategoryCode)
                                                                          select request).FirstOrDefault();
                                                        if (empRequest != null)
                                                        {
                                                            //Sẽ cộng vào ActualWorkingDay sau khi kiểm tra toàn bộ trong tháng
                                                            item.ActualWorkingDay = 0;

                                                        }
                                                        else
                                                        {
                                                            item.ActualWorkingDay = (decimal)0.5;
                                                            item.ReductionAmount = (decimal)0.5;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        item.ActualWorkingDay = 1;
                                                    }
                                                }
                                            }
                                        }
                                        //}
                                    }
                                }
                            });

                            #region Tính lương cho người có trong TimeSheet
                            //lấy ra iD nhân viên có trong file excel
                            var listEmployeeGroup = (from item in maxMinGrouped
                                                     group item by new
                                                     {
                                                         item.EmployeeId
                                                     } into gcs
                                                     select new { EmployeeId = gcs.Key.EmployeeId }).Where(e => e.EmployeeId != null).ToList();
                            //join lay ra các thông tin liên  quan tới lương của mỗi nhân viên
                            var listEmployeeNoDuplicate = (from emp in listEmployeeGroup
                                                           join empif in listEmpInformation on emp.EmployeeId equals empif.EmployeeId into empLeft
                                                           from empif in empLeft.DefaultIfEmpty()
                                                           join empInsurance in listEmployeeInsurance on emp.EmployeeId equals empInsurance.EmployeeId into empInsuranceLeft
                                                           from empInsurance in empInsuranceLeft.DefaultIfEmpty()
                                                           join empAllowance in listEmployeeAllowance on emp.EmployeeId equals empAllowance.EmployeeId into empAllowanceLeft
                                                           from empAllowance in empAllowanceLeft.DefaultIfEmpty()
                                                           join empEmployeeSalary in listEmployeeSalary on emp.EmployeeId equals empEmployeeSalary.EmployeeId into empEmployeeSalaryLeft
                                                           from empEmployeeSalary in empEmployeeSalaryLeft.DefaultIfEmpty()
                                                           join empBankAccount in listBankAccount on emp.EmployeeId equals empBankAccount.ObjectId into empBankAccountLeft
                                                           from empBankAccount in empBankAccountLeft.DefaultIfEmpty()
                                                           select new
                                                           {
                                                               emp,
                                                               empif,
                                                               empInsurance,
                                                               empAllowance,
                                                               empEmployeeSalary,
                                                               empBankAccount,
                                                           }).ToList();

                            List<EmployeeMonthySalaryEntityModel> lstEmployeeMonthySalary = new List<EmployeeMonthySalaryEntityModel>();
                            List<EmployeeMonthySalaryEntityModel> lstEmployeeMonthySalaryUpdate = new List<EmployeeMonthySalaryEntityModel>();
                            List<Entities.EmployeeMonthySalary> lstEmployeeMonthySalaryEnities = new List<Entities.EmployeeMonthySalary>();
                            List<Entities.EmployeeMonthySalary> lstEmployeeMonthySalaryEnitiesUpdate = new List<Entities.EmployeeMonthySalary>();
                            var lstEmployeeMonthySalaryCheck = context.EmployeeMonthySalary.ToList();
                            listEmployeeNoDuplicate.ForEach(item =>
                            {
                                var employeeMonthySalaryObject = new Entities.EmployeeMonthySalary();
                                var empSalaryMonthObject = lstEmployeeMonthySalaryCheck.Where(w => w.EmployeeCode == item.empif.EmployeeCode && w.Month == parameter.Month && w.Year == parameter.Year).FirstOrDefault();
                                if (empSalaryMonthObject != null)
                                {
                                    try
                                    {
                                        //employeeMonthySalaryObject.EmployeeMonthySalaryId = empSalaryMonthObject.EmployeeMonthySalaryId;
                                        if (item.empif != null)
                                        {
                                            empSalaryMonthObject.EmployeeCode = item.empif.EmployeeCode;
                                            empSalaryMonthObject.EmployeeName = item.empif.EmployeeName;
                                            empSalaryMonthObject.EmployeePostionId = item.empif.PositionId;
                                            empSalaryMonthObject.PostionName = item.empif.PositionName;
                                            empSalaryMonthObject.EmployeeId = item.emp.EmployeeId;
                                            empSalaryMonthObject.EmployeeUnit = item.empif.OrganizationName;
                                            empSalaryMonthObject.EmployeeUnitId = item.empif.OrganizationId;
                                            empSalaryMonthObject.EmployeeBranch = item.empif.OrganizationName;
                                            empSalaryMonthObject.EmployeeBranchId = item.empif.OrganizationId;
                                        }
                                        if (item.empEmployeeSalary != null)
                                        {
                                            empSalaryMonthObject.BasedSalary = item.empEmployeeSalary.EmployeeSalaryBase;
                                        }
                                        empSalaryMonthObject.MonthlyWorkingDay = 0;
                                        empSalaryMonthObject.UnPaidLeaveDay = 0;
                                        empSalaryMonthObject.Overtime = 0;
                                        empSalaryMonthObject.ActualWorkingDay = 0;
                                        decimal? ActualWorkingDay = maxMinGrouped.Where(w => w.EmployeeId == item.emp.EmployeeId).Sum(s => s.ActualWorkingDay);
                                        //empSalaryMonthObject.ActualWorkingDay = maxMinGrouped.Where(w => w.EmployeeId == item.emp.EmployeeId).Sum(s => s.ActualWorkingDay);
                                        //kiem tra xem co nghi phep hay ko.Neu co thi cong vao ngay cong
                                        var empRequestList = (from request in listEmployeeRequest
                                                              join category in context.Category on request.TypeRequest equals category.CategoryId
                                                              where request.OfferEmployeeId == item.emp.EmployeeId && request.StartDate.Value.Month == parameter.Month && request.StartDate.Value.Year == parameter.Year
                                                              && category.CategoryCode == "NP"
                                                              select request).ToList();
                                        double numberNP = 0.0;
                                        if (empRequestList.Count > 0)
                                        {
                                            empRequestList.ForEach(empRequest =>
                                            {
                                                if (empRequest.EnDate.HasValue && empRequest.StartDate.HasValue)
                                                {
                                                    if (DateTime.Compare(empRequest.EnDate.Value, empRequest.StartDate.Value) == 0)
                                                    {
                                                        numberNP = 1;
                                                    }
                                                    else
                                                    {
                                                        numberNP = numberNP + (empRequest.EnDate.Value - empRequest.StartDate.Value).TotalDays;
                                                    }
                                                }
                                            });
                                        }

                                        employeeMonthySalaryObject.VacationDay = Convert.ToDecimal(numberNP);

                                        //Kiểm tra xem có (Late,MCC) ko.Nếu có thì cộng vào ngày công thực tế.
                                        double numberLateAndMCC = 0.0;
                                        var myInClause = new string[] { "Late", "MCC" };
                                        var empRequestLateAndMCCList = (from request in listEmployeeRequest
                                                                        join category in context.Category on request.TypeRequest equals category.CategoryId
                                                                        where request.OfferEmployeeId == item.emp.EmployeeId && request.StartDate.Value.Month == parameter.Month && request.StartDate.Value.Year == parameter.Year
                                                                        && myInClause.Contains(category.CategoryCode)
                                                                        select request).ToList();
                                        if (empRequestLateAndMCCList.Count > 0)
                                        {
                                            empRequestLateAndMCCList.ForEach(empRequest =>
                                            {
                                                if (empRequest.EnDate.HasValue && empRequest.StartDate.HasValue)
                                                {
                                                    numberLateAndMCC = numberLateAndMCC + (empRequest.EnDate.Value - empRequest.StartDate.Value).TotalDays;
                                                }
                                            });
                                        }
                                        ////////////////////////////////////////////////////////////
                                        empSalaryMonthObject.ActualWorkingDay = (ActualWorkingDay.HasValue ? ActualWorkingDay.Value : 0) + Convert.ToDecimal(numberNP) + Convert.ToDecimal(numberLateAndMCC);
                                        ///////////////////////////////
                                        empSalaryMonthObject.ActualOfSalary = (empSalaryMonthObject.ActualWorkingDay != null && empSalaryMonthObject.ActualWorkingDay != 0 && empSalaryMonthObject.BasedSalary != null && empSalaryMonthObject.BasedSalary != 0 && empSalaryMonthObject.MonthlyWorkingDay != 0) ?
                                        (empSalaryMonthObject.ActualWorkingDay * empSalaryMonthObject.BasedSalary) / empSalaryMonthObject.MonthlyWorkingDay : 0;
                                        empSalaryMonthObject.OvertimeOfSalary = 0;
                                        //các khoản phụ cấp
                                        if (item.empAllowance != null)
                                        {
                                            empSalaryMonthObject.FuelAllowance = item.empAllowance.FuelAllowance != null ? item.empAllowance.FuelAllowance : 0;
                                            empSalaryMonthObject.PhoneAllowance = item.empAllowance.PhoneAllowance != null ? item.empAllowance.PhoneAllowance : 0;
                                            empSalaryMonthObject.LunchAllowance = item.empAllowance.LunchAllowance != null ? item.empAllowance.LunchAllowance : 0;
                                            empSalaryMonthObject.OtherAllowance = item.empAllowance.OtherAllownce != null ? item.empAllowance.OtherAllownce : 0;
                                        }
                                        else
                                        {
                                            employeeMonthySalaryObject.FuelAllowance = 0;
                                            employeeMonthySalaryObject.PhoneAllowance = 0;
                                            employeeMonthySalaryObject.LunchAllowance = 0;
                                            employeeMonthySalaryObject.OtherAllowance = 0;
                                        }
                                        //các khoản bảo hiểm cty hỗ trợ
                                        if (item.empInsurance != null)
                                        {
                                            empSalaryMonthObject.SocialInsuranceSalary = item.empInsurance.SocialInsuranceSalary != null ? item.empInsurance.SocialInsuranceSalary : 0;
                                            empSalaryMonthObject.SocialInsuranceCompanyPaid = (item.empInsurance.SocialInsuranceSupportPercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.SocialInsuranceSupportPercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                            empSalaryMonthObject.HealthInsuranceCompanyPaid = (item.empInsurance.HealthInsuranceSupportPercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.HealthInsuranceSupportPercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                            empSalaryMonthObject.UnemploymentinsuranceCompanyPaid = (item.empInsurance.UnemploymentinsuranceSupportPercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.UnemploymentinsuranceSupportPercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                        }
                                        else
                                        {
                                            empSalaryMonthObject.SocialInsuranceSalary = 0;
                                            empSalaryMonthObject.SocialInsuranceCompanyPaid = 0;
                                            empSalaryMonthObject.HealthInsuranceCompanyPaid = 0;
                                            empSalaryMonthObject.UnemploymentinsuranceCompanyPaid = 0;
                                        }
                                        empSalaryMonthObject.TotalInsuranceCompanyPaid = (empSalaryMonthObject.SocialInsuranceCompanyPaid != null && empSalaryMonthObject.HealthInsuranceCompanyPaid != null && empSalaryMonthObject.UnemploymentinsuranceCompanyPaid != null) ? empSalaryMonthObject.SocialInsuranceCompanyPaid + empSalaryMonthObject.HealthInsuranceCompanyPaid + empSalaryMonthObject.UnemploymentinsuranceCompanyPaid : 0;
                                        //Các khoản lấy từ API CRM(giữ chân,lương tuyển sinh mới.............)
                                        empSalaryMonthObject.EnrollmentSalary = 0;
                                        empSalaryMonthObject.RetentionSalary = 0;
                                        //các mức tiền bảo hiểm cá nhân phải chịu
                                        if (item.empInsurance != null)
                                        {
                                            empSalaryMonthObject.SocialInsuranceEmployeePaid = (item.empInsurance.SocialInsurancePercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.SocialInsurancePercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                            empSalaryMonthObject.HealthInsuranceEmployeePaid = (item.empInsurance.HealthInsurancePercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.HealthInsurancePercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                            empSalaryMonthObject.UnemploymentinsuranceEmployeePaid = (item.empInsurance.UnemploymentinsurancePercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.UnemploymentinsurancePercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                        }
                                        else
                                        {
                                            empSalaryMonthObject.SocialInsuranceEmployeePaid = 0;
                                            empSalaryMonthObject.HealthInsuranceEmployeePaid = 0;
                                            empSalaryMonthObject.UnemploymentinsuranceEmployeePaid = 0;
                                        }
                                        empSalaryMonthObject.TotalInsuranceEmployeePaid = (empSalaryMonthObject.SocialInsuranceEmployeePaid != null && empSalaryMonthObject.HealthInsuranceEmployeePaid != null && empSalaryMonthObject.UnemploymentinsuranceEmployeePaid != null) ? empSalaryMonthObject.SocialInsuranceEmployeePaid + empSalaryMonthObject.HealthInsuranceEmployeePaid + empSalaryMonthObject.UnemploymentinsuranceEmployeePaid : 0;
                                        //tiền phạt
                                        empSalaryMonthObject.DesciplineAmount = 0;
                                        //
                                        empSalaryMonthObject.ReductionAmount = 0;
                                        empSalaryMonthObject.AdditionalAmount = 0;
                                        if (item.empBankAccount != null)
                                        {
                                            empSalaryMonthObject.BankAccountCode = !string.IsNullOrEmpty(item.empBankAccount.AccountNumber) ? item.empBankAccount.AccountNumber : string.Empty;
                                            empSalaryMonthObject.BankAccountName = !string.IsNullOrEmpty(item.empBankAccount.BankName) ? item.empBankAccount.BankName : string.Empty;
                                            empSalaryMonthObject.BranchOfBank = !string.IsNullOrEmpty(item.empBankAccount.BranchName) ? item.empBankAccount.BranchName : string.Empty;
                                        }
                                        //Tính tổng
                                        empSalaryMonthObject.TotalIncome = empSalaryMonthObject.ActualOfSalary + empSalaryMonthObject.OvertimeOfSalary + empSalaryMonthObject.FuelAllowance + empSalaryMonthObject.PhoneAllowance + empSalaryMonthObject.LunchAllowance + empSalaryMonthObject.OtherAllowance;
                                        empSalaryMonthObject.ActualPaid = empSalaryMonthObject.TotalIncome - (empSalaryMonthObject.TotalInsuranceEmployeePaid + empSalaryMonthObject.DesciplineAmount + empSalaryMonthObject.ReductionAmount) + empSalaryMonthObject.AdditionalAmount;
                                        empSalaryMonthObject.Month = parameter.Month;
                                        empSalaryMonthObject.Year = parameter.Year;
                                        empSalaryMonthObject.CreateById = parameter.UserId;
                                        empSalaryMonthObject.CreateDate = DateTime.Now;
                                        lstEmployeeMonthySalaryUpdate.Add(new EmployeeMonthySalaryEntityModel(empSalaryMonthObject));
                                        lstEmployeeMonthySalaryEnitiesUpdate.Add(empSalaryMonthObject);
                                    }
                                    catch (Exception ex)
                                    {

                                        throw;
                                    }
                                  
                                }
                                else
                                {
                                    try
                                    {
                                        if (item.empif != null)
                                        {
                                            employeeMonthySalaryObject.EmployeeCode = item.empif.EmployeeCode;
                                            employeeMonthySalaryObject.EmployeeName = item.empif.EmployeeName;
                                            employeeMonthySalaryObject.EmployeePostionId = item.empif.PositionId;
                                            employeeMonthySalaryObject.PostionName = item.empif.PositionName;
                                            employeeMonthySalaryObject.EmployeeId = item.emp.EmployeeId;
                                            employeeMonthySalaryObject.EmployeeUnit = item.empif.OrganizationName;
                                            employeeMonthySalaryObject.EmployeeBranch = item.empif.OrganizationName;
                                            employeeMonthySalaryObject.EmployeeUnitId = item.empif.OrganizationId;
                                            employeeMonthySalaryObject.EmployeeBranchId = item.empif.OrganizationId;
                                        }
                                        if (item.empEmployeeSalary != null)
                                        {
                                            employeeMonthySalaryObject.BasedSalary = item.empEmployeeSalary.EmployeeSalaryBase;
                                        }
                                        employeeMonthySalaryObject.MonthlyWorkingDay = 0;
                                        employeeMonthySalaryObject.UnPaidLeaveDay = 0;
                                        employeeMonthySalaryObject.Overtime = 0;
                                        decimal? ActualWorkingDay = maxMinGrouped.Where(w => w.EmployeeId == item.emp.EmployeeId).Sum(s => s.ActualWorkingDay);
                                        //employeeMonthySalaryObject.ActualWorkingDay = maxMinGrouped.Where(w => w.EmployeeId == item.emp.EmployeeId).Sum(s => s.ActualWorkingDay);
                                        //kiem tra xem co nghi phep hay ko.Neu co cong vao ngay cong
                                        var empRequestList = (from request in listEmployeeRequest
                                                              join category in context.Category on request.TypeRequest equals category.CategoryId
                                                              where request.OfferEmployeeId == item.emp.EmployeeId && request.StartDate.Value.Month == parameter.Month && request.StartDate.Value.Year == parameter.Year
                                                              && category.CategoryCode == "NP"
                                                              select request).ToList();
                                        double numberNP = 0.0;
                                        if (empRequestList.Count > 0)
                                        {
                                            empRequestList.ForEach(empRequest =>
                                            {
                                                if (empRequest.EnDate.HasValue && empRequest.StartDate.HasValue)
                                                {
                                                    if(DateTime.Compare(empRequest.EnDate.Value, empRequest.StartDate.Value) == 0)
                                                    {
                                                        numberNP = 1;
                                                    }
                                                    else
                                                    {
                                                        numberNP = numberNP + (empRequest.EnDate.Value - empRequest.StartDate.Value).TotalDays;
                                                    }
                                                }
                                            });
                                        }
                                        employeeMonthySalaryObject.VacationDay = Convert.ToDecimal(numberNP);
                                        //Kiểm tra xem có (Late,MCC) ko.Nếu có thì cộng vào ngày công thực tế.
                                        double numberLateAndMCC = 0.0;
                                        var myInClause = new string[] { "Late", "MCC" };
                                        var empRequestLateAndMCCList = (from request in listEmployeeRequest
                                                                        join category in context.Category on request.TypeRequest equals category.CategoryId
                                                                        where request.OfferEmployeeId == item.emp.EmployeeId && request.StartDate.Value.Month == parameter.Month && request.StartDate.Value.Year == parameter.Year
                                                                        && myInClause.Contains(category.CategoryCode)
                                                                        select request).ToList();
                                        if (empRequestLateAndMCCList.Count > 0)
                                        {
                                            empRequestLateAndMCCList.ForEach(empRequest =>
                                            {
                                                if (empRequest.EnDate.HasValue && empRequest.StartDate.HasValue)
                                                {
                                                    numberLateAndMCC = numberLateAndMCC + (empRequest.EnDate.Value - empRequest.StartDate.Value).TotalDays;
                                                }
                                            });
                                        }
                                        ////////////////////////////////
                                        employeeMonthySalaryObject.ActualWorkingDay = (ActualWorkingDay.HasValue ? ActualWorkingDay.Value : 0) + Convert.ToDecimal(numberNP) + Convert.ToDecimal(numberLateAndMCC);
                                        ///////////////////////////////
                                        employeeMonthySalaryObject.ActualOfSalary = (employeeMonthySalaryObject.ActualWorkingDay != null && employeeMonthySalaryObject.ActualWorkingDay != 0 && employeeMonthySalaryObject.BasedSalary != null && employeeMonthySalaryObject.BasedSalary != 0 && employeeMonthySalaryObject.MonthlyWorkingDay != 0) ?
                                        (employeeMonthySalaryObject.ActualWorkingDay * employeeMonthySalaryObject.BasedSalary) / employeeMonthySalaryObject.MonthlyWorkingDay : 0;
                                        employeeMonthySalaryObject.OvertimeOfSalary = 0;
                                        //các khoản phụ cấp
                                        if (item.empAllowance != null)
                                        {
                                            employeeMonthySalaryObject.FuelAllowance = item.empAllowance.FuelAllowance != null ? item.empAllowance.FuelAllowance : 0;
                                            employeeMonthySalaryObject.PhoneAllowance = item.empAllowance.PhoneAllowance != null ? item.empAllowance.PhoneAllowance : 0;
                                            employeeMonthySalaryObject.LunchAllowance = item.empAllowance.LunchAllowance != null ? item.empAllowance.LunchAllowance : 0;
                                            employeeMonthySalaryObject.OtherAllowance = item.empAllowance.OtherAllownce != null ? item.empAllowance.OtherAllownce : 0;
                                        }
                                        else
                                        {
                                            employeeMonthySalaryObject.FuelAllowance = 0;
                                            employeeMonthySalaryObject.PhoneAllowance = 0;
                                            employeeMonthySalaryObject.LunchAllowance = 0;
                                            employeeMonthySalaryObject.OtherAllowance = 0;
                                        }
                                        //các khoản bảo hiểm cty hỗ trợ
                                        if (item.empInsurance != null)
                                        {
                                            employeeMonthySalaryObject.SocialInsuranceSalary = item.empInsurance.SocialInsuranceSalary;
                                            employeeMonthySalaryObject.SocialInsuranceCompanyPaid = (item.empInsurance.SocialInsuranceSupportPercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.SocialInsuranceSupportPercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                            employeeMonthySalaryObject.HealthInsuranceCompanyPaid = (item.empInsurance.HealthInsuranceSupportPercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.HealthInsuranceSupportPercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                            employeeMonthySalaryObject.UnemploymentinsuranceCompanyPaid = (item.empInsurance.UnemploymentinsuranceSupportPercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.UnemploymentinsuranceSupportPercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                        }
                                        else
                                        {
                                            employeeMonthySalaryObject.SocialInsuranceSalary = 0;
                                            employeeMonthySalaryObject.SocialInsuranceCompanyPaid = 0;
                                            employeeMonthySalaryObject.HealthInsuranceCompanyPaid = 0;
                                            employeeMonthySalaryObject.UnemploymentinsuranceCompanyPaid = 0;
                                        }
                                        employeeMonthySalaryObject.TotalInsuranceCompanyPaid = (employeeMonthySalaryObject.SocialInsuranceCompanyPaid != null && employeeMonthySalaryObject.HealthInsuranceCompanyPaid != null && employeeMonthySalaryObject.UnemploymentinsuranceCompanyPaid != null) ? employeeMonthySalaryObject.SocialInsuranceCompanyPaid + employeeMonthySalaryObject.HealthInsuranceCompanyPaid + employeeMonthySalaryObject.UnemploymentinsuranceCompanyPaid : 0;
                                        //Các khoản lấy từ API CRM(giữ chân,lương tuyển sinh mới.............)
                                        employeeMonthySalaryObject.EnrollmentSalary = 0;
                                        employeeMonthySalaryObject.RetentionSalary = 0;
                                        //các mức tiền bảo hiểm cá nhân phải chịu
                                        if (item.empInsurance != null)
                                        {
                                            employeeMonthySalaryObject.SocialInsuranceEmployeePaid = (item.empInsurance.SocialInsurancePercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.SocialInsurancePercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                            employeeMonthySalaryObject.HealthInsuranceEmployeePaid = (item.empInsurance.HealthInsurancePercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.HealthInsurancePercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                            employeeMonthySalaryObject.UnemploymentinsuranceEmployeePaid = (item.empInsurance.UnemploymentinsurancePercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.UnemploymentinsurancePercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                        }
                                        else
                                        {
                                            employeeMonthySalaryObject.SocialInsuranceEmployeePaid = 0;
                                            employeeMonthySalaryObject.HealthInsuranceEmployeePaid = 0;
                                            employeeMonthySalaryObject.UnemploymentinsuranceEmployeePaid = 0;
                                        }
                                        employeeMonthySalaryObject.TotalInsuranceEmployeePaid = (employeeMonthySalaryObject.SocialInsuranceEmployeePaid != null && employeeMonthySalaryObject.HealthInsuranceEmployeePaid != null && employeeMonthySalaryObject.UnemploymentinsuranceEmployeePaid != null) ? employeeMonthySalaryObject.SocialInsuranceEmployeePaid + employeeMonthySalaryObject.HealthInsuranceEmployeePaid + employeeMonthySalaryObject.UnemploymentinsuranceEmployeePaid : 0;
                                        //tiền phạt
                                        employeeMonthySalaryObject.DesciplineAmount = 0;
                                        //
                                        employeeMonthySalaryObject.ReductionAmount = 0;
                                        employeeMonthySalaryObject.AdditionalAmount = 0;

                                        if (item.empBankAccount != null)
                                        {
                                            employeeMonthySalaryObject.BankAccountCode = !string.IsNullOrEmpty(item.empBankAccount.AccountNumber) ? item.empBankAccount.AccountNumber : string.Empty;
                                            employeeMonthySalaryObject.BankAccountName = !string.IsNullOrEmpty(item.empBankAccount.BankName) ? item.empBankAccount.BankName : string.Empty;
                                            employeeMonthySalaryObject.BranchOfBank = !string.IsNullOrEmpty(item.empBankAccount.BranchName) ? item.empBankAccount.BranchName : string.Empty;
                                        }
                                        //Tính tổng
                                        employeeMonthySalaryObject.TotalIncome = employeeMonthySalaryObject.ActualOfSalary + employeeMonthySalaryObject.OvertimeOfSalary + employeeMonthySalaryObject.FuelAllowance + employeeMonthySalaryObject.PhoneAllowance + employeeMonthySalaryObject.LunchAllowance + employeeMonthySalaryObject.OtherAllowance;
                                        employeeMonthySalaryObject.ActualPaid = employeeMonthySalaryObject.TotalIncome - (employeeMonthySalaryObject.TotalInsuranceEmployeePaid + employeeMonthySalaryObject.DesciplineAmount + employeeMonthySalaryObject.ReductionAmount) + employeeMonthySalaryObject.AdditionalAmount;
                                        employeeMonthySalaryObject.Month = parameter.Month;
                                        employeeMonthySalaryObject.Year = parameter.Year;
                                        employeeMonthySalaryObject.CreateById = parameter.UserId;
                                        employeeMonthySalaryObject.CreateDate = DateTime.Now;
                                        employeeMonthySalaryObject.Type = 0;
                                        employeeMonthySalaryObject.CommonId = newCommonId;
                                        var draftId = context.Category.FirstOrDefault(c => c.CategoryCode == "DR").CategoryId;
                                        employeeMonthySalaryObject.StatusId = draftId;
                                        lstEmployeeMonthySalary.Add(new EmployeeMonthySalaryEntityModel(employeeMonthySalaryObject));
                                        lstEmployeeMonthySalaryEnities.Add(employeeMonthySalaryObject);
                                    }
                                    catch (Exception ex)
                                    {

                                        throw;
                                    }
                                    
                                }
                            });
                            #endregion

                            #region Tính lương cho người đc miễn chấm công or ưu tiên
                            //Tính lương cho người đc miễn chấm công or Ưu tiên
                            var myInClausePostion = new string[] { "GV", "CTV", "TG" };

                            var listEmployeePostion = (from emp in context.Employee
                                                       join pos in context.Position on emp.PositionId equals pos.PositionId
                                                       where !myInClausePostion.Contains(pos.PositionCode)
                                                       select new { EmployeeId = emp.EmployeeId, EmployeeCode = emp.EmployeeCode }).ToList().Distinct();

                            var ListFreeTimeUnlimited = listEmployeePostion.Where(w1 => !listEmployeeGroup.Any(w2 => w2.EmployeeId == w1.EmployeeId));

                            var listEmployeeFreeTimeUnlimitedNoDuplicate = (from emp in ListFreeTimeUnlimited
                                                                            join empif in listEmpInformation on emp.EmployeeId equals empif.EmployeeId into empLeft
                                                                            from empif in empLeft.DefaultIfEmpty()
                                                                            join empInsurance in listEmployeeInsurance on emp.EmployeeId equals empInsurance.EmployeeId into empInsuranceLeft
                                                                            from empInsurance in empInsuranceLeft.DefaultIfEmpty()
                                                                            join empAllowance in listEmployeeAllowance on emp.EmployeeId equals empAllowance.EmployeeId into empAllowanceLeft
                                                                            from empAllowance in empAllowanceLeft.DefaultIfEmpty()
                                                                            join empEmployeeSalary in listEmployeeSalary on emp.EmployeeId equals empEmployeeSalary.EmployeeId into empEmployeeSalaryLeft
                                                                            from empEmployeeSalary in empEmployeeSalaryLeft.DefaultIfEmpty()
                                                                            join empBankAccount in listBankAccount on emp.EmployeeId equals empBankAccount.ObjectId into empBankAccountLeft
                                                                            from empBankAccount in empBankAccountLeft.DefaultIfEmpty()

                                                                            select new
                                                                            {
                                                                                emp,
                                                                                empif,
                                                                                empInsurance,
                                                                                empAllowance,
                                                                                empEmployeeSalary,
                                                                                empBankAccount,
                                                                            }).ToList();

                            //Tính T7,CN trong tháng
                            int days = DateTime.DaysInMonth(parameter.Year, parameter.Month);
                            int totalSaturdaysAndSundays = 0;
                            for (int i = 1; i < days; i++)
                            {
                                var day = new DateTime(parameter.Year, parameter.Month, i);
                                if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                                {
                                    totalSaturdaysAndSundays++;
                                }
                            }
                            ////
                            listEmployeeFreeTimeUnlimitedNoDuplicate.ForEach(item =>
                            {
                                var employeeMonthySalaryObject = new Entities.EmployeeMonthySalary();
                                var empSalaryMonthObject = lstEmployeeMonthySalaryCheck.Where(w => w.EmployeeCode == item.empif.EmployeeCode && w.Month == parameter.Month && w.Year == parameter.Year).FirstOrDefault();
                                if (empSalaryMonthObject != null)
                                {
                                    if (item.empif != null)
                                    {

                                        //employeeMonthySalaryObject.EmployeeMonthySalaryId = empSalaryMonthObject.EmployeeMonthySalaryId;
                                        empSalaryMonthObject.EmployeeCode = item.empif.EmployeeCode;
                                        empSalaryMonthObject.EmployeeName = item.empif.EmployeeName;
                                        empSalaryMonthObject.EmployeePostionId = item.empif.PositionId;
                                        empSalaryMonthObject.PostionName = item.empif.PositionName;
                                        empSalaryMonthObject.EmployeeId = item.emp.EmployeeId;
                                        empSalaryMonthObject.EmployeeUnit = item.empif.OrganizationName;
                                        empSalaryMonthObject.EmployeeBranch = item.empif.OrganizationName;
                                        empSalaryMonthObject.EmployeeUnitId = item.empif.OrganizationId;
                                        empSalaryMonthObject.EmployeeBranchId = item.empif.OrganizationId;
                                    }
                                    if (item.empEmployeeSalary != null)
                                    {
                                        empSalaryMonthObject.BasedSalary = item.empEmployeeSalary.EmployeeSalaryBase;
                                    }
                                    //Kiem tra duoc mien cham cong hay ko.
                                    if (item.empAllowance != null)
                                    {
                                        if (item.empAllowance.FreeTimeUnlimited.HasValue)
                                        {
                                            if (item.empAllowance.FreeTimeUnlimited.Value)
                                            {
                                                empSalaryMonthObject.MonthlyWorkingDay = days - totalSaturdaysAndSundays;
                                                empSalaryMonthObject.ActualWorkingDay = days - totalSaturdaysAndSundays;
                                            }
                                            else
                                            {
                                                empSalaryMonthObject.MonthlyWorkingDay = 0;
                                                empSalaryMonthObject.ActualWorkingDay = 0;
                                            }
                                        }
                                        else
                                        {
                                            empSalaryMonthObject.MonthlyWorkingDay = 0;
                                            empSalaryMonthObject.ActualWorkingDay = 0;
                                        }
                                    }
                                    else
                                    {
                                        empSalaryMonthObject.MonthlyWorkingDay = 0;
                                        empSalaryMonthObject.ActualWorkingDay = 0;
                                    }

                                    empSalaryMonthObject.UnPaidLeaveDay = 0;
                                    empSalaryMonthObject.Overtime = 0;
                                    employeeMonthySalaryObject.VacationDay = 0;

                                    empSalaryMonthObject.ActualOfSalary = (empSalaryMonthObject.ActualWorkingDay != null && empSalaryMonthObject.ActualWorkingDay != 0 && empSalaryMonthObject.BasedSalary != null && empSalaryMonthObject.BasedSalary != 0 && empSalaryMonthObject.MonthlyWorkingDay != 0) ?
                                    (empSalaryMonthObject.ActualWorkingDay * empSalaryMonthObject.BasedSalary) / empSalaryMonthObject.MonthlyWorkingDay : 0;
                                    empSalaryMonthObject.OvertimeOfSalary = 0;
                                    //các khoản phụ cấp
                                    if (item.empAllowance != null)
                                    {
                                        empSalaryMonthObject.FuelAllowance = item.empAllowance.FuelAllowance != null ? item.empAllowance.FuelAllowance : 0;
                                        empSalaryMonthObject.PhoneAllowance = item.empAllowance.PhoneAllowance != null ? item.empAllowance.PhoneAllowance : 0;
                                        empSalaryMonthObject.LunchAllowance = item.empAllowance.LunchAllowance != null ? item.empAllowance.LunchAllowance : 0;
                                        empSalaryMonthObject.OtherAllowance = item.empAllowance.OtherAllownce != null ? item.empAllowance.OtherAllownce : 0;
                                    }
                                    else
                                    {
                                        employeeMonthySalaryObject.FuelAllowance = 0;
                                        employeeMonthySalaryObject.PhoneAllowance = 0;
                                        employeeMonthySalaryObject.LunchAllowance = 0;
                                        employeeMonthySalaryObject.OtherAllowance = 0;
                                    }
                                    //các khoản bảo hiểm cty hỗ trợ
                                    if (item.empInsurance != null)
                                    {
                                        empSalaryMonthObject.SocialInsuranceSalary = item.empInsurance.SocialInsuranceSalary;
                                        empSalaryMonthObject.SocialInsuranceCompanyPaid = (item.empInsurance.SocialInsuranceSupportPercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.SocialInsuranceSupportPercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                        empSalaryMonthObject.HealthInsuranceCompanyPaid = (item.empInsurance.HealthInsuranceSupportPercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.HealthInsuranceSupportPercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                        empSalaryMonthObject.UnemploymentinsuranceCompanyPaid = (item.empInsurance.UnemploymentinsuranceSupportPercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.UnemploymentinsuranceSupportPercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                    }
                                    else
                                    {
                                        empSalaryMonthObject.SocialInsuranceSalary = 0;
                                        empSalaryMonthObject.SocialInsuranceCompanyPaid = 0;
                                        empSalaryMonthObject.HealthInsuranceCompanyPaid = 0;
                                        empSalaryMonthObject.UnemploymentinsuranceCompanyPaid = 0;
                                    }
                                    empSalaryMonthObject.TotalInsuranceCompanyPaid = (empSalaryMonthObject.SocialInsuranceCompanyPaid != null && empSalaryMonthObject.HealthInsuranceCompanyPaid != null && empSalaryMonthObject.UnemploymentinsuranceCompanyPaid != null) ? empSalaryMonthObject.SocialInsuranceCompanyPaid + empSalaryMonthObject.HealthInsuranceCompanyPaid + empSalaryMonthObject.UnemploymentinsuranceCompanyPaid : 0;
                                    //Các khoản lấy từ API CRM(giữ chân,lương tuyển sinh mới.............)
                                    empSalaryMonthObject.EnrollmentSalary = 0;
                                    empSalaryMonthObject.RetentionSalary = 0;
                                    //các mức tiền bảo hiểm cá nhân phải chịu
                                    if (item.empInsurance != null)
                                    {
                                        empSalaryMonthObject.SocialInsuranceEmployeePaid = (item.empInsurance.SocialInsurancePercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.SocialInsurancePercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                        empSalaryMonthObject.HealthInsuranceEmployeePaid = (item.empInsurance.HealthInsurancePercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.HealthInsurancePercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                        empSalaryMonthObject.UnemploymentinsuranceEmployeePaid = (item.empInsurance.UnemploymentinsurancePercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.UnemploymentinsurancePercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                    }
                                    else
                                    {
                                        empSalaryMonthObject.SocialInsuranceEmployeePaid = 0;
                                        empSalaryMonthObject.HealthInsuranceEmployeePaid = 0;
                                        empSalaryMonthObject.UnemploymentinsuranceEmployeePaid = 0;
                                    }

                                    empSalaryMonthObject.TotalInsuranceEmployeePaid = (empSalaryMonthObject.SocialInsuranceEmployeePaid != null && empSalaryMonthObject.HealthInsuranceEmployeePaid != null && empSalaryMonthObject.UnemploymentinsuranceEmployeePaid != null) ? empSalaryMonthObject.SocialInsuranceEmployeePaid + empSalaryMonthObject.HealthInsuranceEmployeePaid + empSalaryMonthObject.UnemploymentinsuranceEmployeePaid : 0;
                                    //tiền phạt
                                    empSalaryMonthObject.DesciplineAmount = 0;
                                    //
                                    empSalaryMonthObject.ReductionAmount = 0;
                                    empSalaryMonthObject.AdditionalAmount = 0;
                                    if (item.empBankAccount != null)
                                    {
                                        empSalaryMonthObject.BankAccountCode = !string.IsNullOrEmpty(item.empBankAccount.AccountNumber) ? item.empBankAccount.AccountNumber : string.Empty;
                                        empSalaryMonthObject.BankAccountName = !string.IsNullOrEmpty(item.empBankAccount.BankName) ? item.empBankAccount.BankName : string.Empty;
                                        empSalaryMonthObject.BranchOfBank = !string.IsNullOrEmpty(item.empBankAccount.BranchName) ? item.empBankAccount.BranchName : string.Empty;
                                    }
                                    //Tính tổng
                                    empSalaryMonthObject.TotalIncome = empSalaryMonthObject.ActualOfSalary + empSalaryMonthObject.OvertimeOfSalary + empSalaryMonthObject.FuelAllowance + empSalaryMonthObject.PhoneAllowance + empSalaryMonthObject.LunchAllowance + empSalaryMonthObject.OtherAllowance;
                                    empSalaryMonthObject.ActualPaid = empSalaryMonthObject.TotalIncome - (empSalaryMonthObject.TotalInsuranceEmployeePaid + empSalaryMonthObject.DesciplineAmount + empSalaryMonthObject.ReductionAmount) + empSalaryMonthObject.AdditionalAmount;
                                    empSalaryMonthObject.Month = parameter.Month;
                                    empSalaryMonthObject.Year = parameter.Year;
                                    empSalaryMonthObject.CreateById = parameter.UserId;
                                    empSalaryMonthObject.CreateDate = DateTime.Now;
                                    lstEmployeeMonthySalaryUpdate.Add(new EmployeeMonthySalaryEntityModel(empSalaryMonthObject));
                                    lstEmployeeMonthySalaryEnitiesUpdate.Add(empSalaryMonthObject);
                                }
                                else
                                {
                                    try
                                    {

                                        if (item.empif != null)
                                        {
                                            employeeMonthySalaryObject.EmployeeCode = item.empif.EmployeeCode;
                                            employeeMonthySalaryObject.EmployeeName = item.empif.EmployeeName;
                                            employeeMonthySalaryObject.EmployeePostionId = item.empif.PositionId;
                                            employeeMonthySalaryObject.PostionName = item.empif.PositionName;
                                            employeeMonthySalaryObject.EmployeeId = item.emp.EmployeeId;
                                            employeeMonthySalaryObject.EmployeeUnit = item.empif.OrganizationName;
                                            employeeMonthySalaryObject.EmployeeBranch = item.empif.OrganizationName;
                                            employeeMonthySalaryObject.EmployeeUnitId = item.empif.OrganizationId;
                                            employeeMonthySalaryObject.EmployeeBranchId = item.empif.OrganizationId;
                                        }
                                        if (item.empEmployeeSalary != null)
                                        {
                                            employeeMonthySalaryObject.BasedSalary = item.empEmployeeSalary.EmployeeSalaryBase;
                                        }
                                        employeeMonthySalaryObject.MonthlyWorkingDay = 0;
                                        employeeMonthySalaryObject.UnPaidLeaveDay = 0;
                                        employeeMonthySalaryObject.Overtime = 0;
                                        //kiem tra xem co nghi phep hay ko.Neu co cong vao ngay cong
                                        if (item.empAllowance != null)
                                        {
                                            if (item.empAllowance.FreeTimeUnlimited.HasValue)
                                            {
                                                if (item.empAllowance.FreeTimeUnlimited.Value)
                                                {
                                                    employeeMonthySalaryObject.MonthlyWorkingDay = days - totalSaturdaysAndSundays;
                                                    employeeMonthySalaryObject.ActualWorkingDay = days - totalSaturdaysAndSundays;
                                                }
                                                else
                                                {
                                                    employeeMonthySalaryObject.MonthlyWorkingDay = 0;
                                                    employeeMonthySalaryObject.ActualWorkingDay = 0;
                                                }
                                            }
                                            else
                                            {
                                                employeeMonthySalaryObject.MonthlyWorkingDay = 0;
                                                employeeMonthySalaryObject.ActualWorkingDay = 0;
                                            }
                                        }
                                        else
                                        {
                                            employeeMonthySalaryObject.MonthlyWorkingDay = 0;
                                            employeeMonthySalaryObject.ActualWorkingDay = 0;
                                        }
                                        employeeMonthySalaryObject.VacationDay = 0;
                                        employeeMonthySalaryObject.ActualOfSalary = (employeeMonthySalaryObject.ActualWorkingDay != null && employeeMonthySalaryObject.ActualWorkingDay != 0 && employeeMonthySalaryObject.BasedSalary != null && employeeMonthySalaryObject.BasedSalary != 0 && employeeMonthySalaryObject.MonthlyWorkingDay != 0) ?
                                        (employeeMonthySalaryObject.ActualWorkingDay * employeeMonthySalaryObject.BasedSalary) / employeeMonthySalaryObject.MonthlyWorkingDay : 0;
                                        employeeMonthySalaryObject.OvertimeOfSalary = 0;
                                        //các khoản phụ cấp
                                        if (item.empAllowance != null)
                                        {
                                            employeeMonthySalaryObject.FuelAllowance = item.empAllowance.FuelAllowance != null ? item.empAllowance.FuelAllowance : 0;
                                            employeeMonthySalaryObject.PhoneAllowance = item.empAllowance.PhoneAllowance != null ? item.empAllowance.PhoneAllowance : 0;
                                            employeeMonthySalaryObject.LunchAllowance = item.empAllowance.LunchAllowance != null ? item.empAllowance.LunchAllowance : 0;
                                            employeeMonthySalaryObject.OtherAllowance = item.empAllowance.OtherAllownce != null ? item.empAllowance.OtherAllownce : 0;
                                        }
                                        else
                                        {
                                            employeeMonthySalaryObject.FuelAllowance = 0;
                                            employeeMonthySalaryObject.PhoneAllowance = 0;
                                            employeeMonthySalaryObject.LunchAllowance = 0;
                                            employeeMonthySalaryObject.OtherAllowance = 0;
                                        }
                                        //các khoản bảo hiểm cty hỗ trợ
                                        if (item.empInsurance != null)
                                        {
                                            employeeMonthySalaryObject.SocialInsuranceSalary = item.empInsurance.SocialInsuranceSalary;
                                            employeeMonthySalaryObject.SocialInsuranceCompanyPaid = (item.empInsurance.SocialInsuranceSupportPercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.SocialInsuranceSupportPercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                            employeeMonthySalaryObject.HealthInsuranceCompanyPaid = (item.empInsurance.HealthInsuranceSupportPercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.HealthInsuranceSupportPercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                            employeeMonthySalaryObject.UnemploymentinsuranceCompanyPaid = (item.empInsurance.UnemploymentinsuranceSupportPercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.UnemploymentinsuranceSupportPercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                        }
                                        else
                                        {
                                            employeeMonthySalaryObject.SocialInsuranceSalary = 0;
                                            employeeMonthySalaryObject.SocialInsuranceCompanyPaid = 0;
                                            employeeMonthySalaryObject.HealthInsuranceCompanyPaid = 0;
                                            employeeMonthySalaryObject.UnemploymentinsuranceCompanyPaid = 0;
                                        }
                                        employeeMonthySalaryObject.TotalInsuranceCompanyPaid = (employeeMonthySalaryObject.SocialInsuranceCompanyPaid != null && employeeMonthySalaryObject.HealthInsuranceCompanyPaid != null && employeeMonthySalaryObject.UnemploymentinsuranceCompanyPaid != null) ? employeeMonthySalaryObject.SocialInsuranceCompanyPaid + employeeMonthySalaryObject.HealthInsuranceCompanyPaid + employeeMonthySalaryObject.UnemploymentinsuranceCompanyPaid : 0;
                                        //Các khoản lấy từ API CRM(giữ chân,lương tuyển sinh mới.............)
                                        employeeMonthySalaryObject.EnrollmentSalary = 0;
                                        employeeMonthySalaryObject.RetentionSalary = 0;
                                        //các mức tiền bảo hiểm cá nhân phải chịu
                                        if (item.empInsurance != null)
                                        {
                                            employeeMonthySalaryObject.SocialInsuranceEmployeePaid = (item.empInsurance.SocialInsurancePercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.SocialInsurancePercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                            employeeMonthySalaryObject.HealthInsuranceEmployeePaid = (item.empInsurance.HealthInsurancePercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.HealthInsurancePercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                            employeeMonthySalaryObject.UnemploymentinsuranceEmployeePaid = (item.empInsurance.UnemploymentinsurancePercent != null && item.empInsurance.SocialInsuranceSalary != null) ? (item.empInsurance.UnemploymentinsurancePercent * item.empInsurance.SocialInsuranceSalary) / 100 : 0;
                                        }
                                        else
                                        {
                                            employeeMonthySalaryObject.SocialInsuranceEmployeePaid = 0;
                                            employeeMonthySalaryObject.HealthInsuranceEmployeePaid = 0;
                                            employeeMonthySalaryObject.UnemploymentinsuranceEmployeePaid = 0;
                                        }

                                        employeeMonthySalaryObject.TotalInsuranceEmployeePaid = (employeeMonthySalaryObject.SocialInsuranceEmployeePaid != null && employeeMonthySalaryObject.HealthInsuranceEmployeePaid != null && employeeMonthySalaryObject.UnemploymentinsuranceEmployeePaid != null) ? employeeMonthySalaryObject.SocialInsuranceEmployeePaid + employeeMonthySalaryObject.HealthInsuranceEmployeePaid + employeeMonthySalaryObject.UnemploymentinsuranceEmployeePaid : 0;
                                        //tiền phạt
                                        employeeMonthySalaryObject.DesciplineAmount = 0;
                                        employeeMonthySalaryObject.ReductionAmount = 0;
                                        employeeMonthySalaryObject.AdditionalAmount = 0;
                                        if (item.empBankAccount != null)
                                        {
                                            employeeMonthySalaryObject.BankAccountCode = !string.IsNullOrEmpty(item.empBankAccount.AccountNumber) ? item.empBankAccount.AccountNumber : string.Empty;
                                            employeeMonthySalaryObject.BankAccountName = !string.IsNullOrEmpty(item.empBankAccount.BankName) ? item.empBankAccount.BankName : string.Empty;
                                            employeeMonthySalaryObject.BranchOfBank = !string.IsNullOrEmpty(item.empBankAccount.BranchName) ? item.empBankAccount.BranchName : string.Empty;
                                        }
                                        //Tính tổng
                                        employeeMonthySalaryObject.TotalIncome = employeeMonthySalaryObject.ActualOfSalary + employeeMonthySalaryObject.OvertimeOfSalary + employeeMonthySalaryObject.FuelAllowance + employeeMonthySalaryObject.PhoneAllowance + employeeMonthySalaryObject.LunchAllowance + employeeMonthySalaryObject.OtherAllowance;
                                        employeeMonthySalaryObject.ActualPaid = employeeMonthySalaryObject.TotalIncome - (employeeMonthySalaryObject.TotalInsuranceEmployeePaid + employeeMonthySalaryObject.DesciplineAmount + employeeMonthySalaryObject.ReductionAmount) + employeeMonthySalaryObject.AdditionalAmount;
                                        employeeMonthySalaryObject.Month = parameter.Month;
                                        employeeMonthySalaryObject.Year = parameter.Year;
                                        employeeMonthySalaryObject.CreateById = parameter.UserId;
                                        employeeMonthySalaryObject.CreateDate = DateTime.Now;
                                        employeeMonthySalaryObject.Type = 0;
                                        employeeMonthySalaryObject.CommonId = newCommonId;
                                        var draftId = context.Category.FirstOrDefault(c => c.CategoryCode == "DR").CategoryId;
                                        employeeMonthySalaryObject.StatusId = draftId;
                                        lstEmployeeMonthySalary.Add(new EmployeeMonthySalaryEntityModel(employeeMonthySalaryObject));
                                        lstEmployeeMonthySalaryEnities.Add(employeeMonthySalaryObject);
                                    }
                                    catch (Exception ex)
                                    {

                                        throw;
                                    }
                                }
                            });
                            #endregion

                            if (lstEmployeeMonthySalaryEnitiesUpdate.Count > 0)
                            {
                                context.EmployeeMonthySalary.UpdateRange(lstEmployeeMonthySalaryEnitiesUpdate);
                            }

                            if (lstEmployeeMonthySalaryEnities.Count > 0)
                            {
                                context.EmployeeMonthySalary.AddRange(lstEmployeeMonthySalaryEnities);
                            }

                            context.EmployeeTimesheet.AddRange(maxMinGrouped);
                            context.SaveChanges();
                            lstEmployeeMonthySalaryEnities.AddRange(lstEmployeeMonthySalaryEnitiesUpdate);

                            excelOutput = this.ExportEmployeeMonthySalary(lstEmployeeMonthySalaryEnities, parameter.Month, parameter.Year);
                            lstEmployeeMonthySalary.AddRange(lstEmployeeMonthySalaryUpdate);

                            return new EmployeeTimeSheetImportResult
                            {
                                ExcelFile = excelOutput,
                                lstEmployeeMonthySalary = lstEmployeeMonthySalary,
                                NameFile = string.Format("Thông tin các khoản nhập tay tháng {0}", parameter.Month),
                                Message = "Bảng chấm công đã được lưu vào hệ thống",
                                Status = true
                            };
                        }
                    }
                }
            }
            else
            {
                return new EmployeeTimeSheetImportResult
                {
                    ExcelFile = null,
                    lstEmployeeMonthySalary = null,
                    NameFile = null,
                    Message = "Chọn lại file chấm công",
                    Status = false
                };
            }

        }

        public EmployeeSalaryHandmadeResult EmployeeSalaryHandmadeImport(EmployeeSalaryHandmadeParameter parameter)
        {
            if (parameter.FileList != null && parameter.FileList.Count > 0)
            {
                using (var stream = new MemoryStream())
                {
                    parameter.FileList[0].CopyTo(stream);
                    using (ExcelPackage package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets["Customer"];
                        if (worksheet == null)
                        {
                            return new EmployeeSalaryHandmadeResult
                            {
                                lstEmployeeMonthySalary = null,
                                Message = "File excel không đúng theo template",
                                Status = false
                            };
                        }
                        //Group cells by row
                        var rowcellgroups = worksheet.Cells["A:M"].GroupBy(c => c.Start.Row);
                        //Loại bỏ 6 dòng tiêu đề
                        var groups = rowcellgroups.Skip(5);
                        //Group theo từng ngày
                        var cv = (from item in groups
                                  group item by new
                                  {
                                      item.First().Value,
                                  } into gcs
                                  select gcs).ToList();

                        var listCategoryTypeEmployeeAssessment = (from categoryT in context.CategoryType
                                                                  join category in context.Category on categoryT.CategoryTypeId equals category.CategoryTypeId
                                                                  where categoryT.CategoryTypeCode == "DVI"
                                                                  select category).ToList();
                        var listEmployee = context.Employee.Select(item => new { EmployeeId = item.EmployeeId, EmployeeCode = item.EmployeeCode }).ToList();

                        var listEmployeeAssessment = cv.Select(g => new EmployeeAssessment
                        {
                            Month = int.Parse(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "D")).First().Value.ToString()),
                            Year = int.Parse(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "E")).First().Value.ToString()),
                            EmployeeId = listEmployee.Where(x => x.EmployeeCode == g.Key.Value.ToString()).Select(d => (Guid?)d.EmployeeId).DefaultIfEmpty().FirstOrDefault().Value,
                            Type = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "M")).First() != null ? listCategoryTypeEmployeeAssessment.Where(w => w.CategoryCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "M")).First().Value.ToString()).Select(d => (Guid?)d.CategoryId).DefaultIfEmpty().FirstOrDefault() : null,
                            CreateById = parameter.UserId,
                            CreateDate = DateTime.Now
                        }).ToList();

                        var listEmpSalaryHandmade = cv.Select(g => new EmployeeMonthySalary
                        {
                            EmployeeCode = g.Key.Value.ToString(),
                            EmployeeName = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "B")).First().Value.ToString(),
                            PostionName = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "C")).First().Value.ToString(),
                            Month = int.Parse(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "D")).First().Value.ToString()),
                            Year = int.Parse(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "E")).First().Value.ToString()),
                            MonthlyWorkingDay = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "F")).First() != null ? int.Parse(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "F")).First().Value.ToString()) : 0,
                            Overtime = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "G")).First() != null ? Convert.ToDecimal(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "G")).First().Value.ToString()) : 0,
                            EnrollmentSalary = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "H")).First() != null ? Convert.ToDecimal(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "H")).First().Value.ToString()) : 0,
                            RetentionSalary = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First() != null ? Convert.ToDecimal(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "I")).First().Value.ToString()) : 0,
                            DesciplineAmount = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First() != null ? Convert.ToDecimal(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "J")).First().Value.ToString()) : 0,
                            ReductionAmount = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First() != null ? Convert.ToDecimal(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "K")).First().Value.ToString()) : 0,
                            AdditionalAmount = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First() != null ? Convert.ToDecimal(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "L")).First().Value.ToString()) : 0
                        }).ToList();

                        var lstEmployeeMonthySalary = context.EmployeeMonthySalary.ToList();
                        var listEmployeeAllowance = context.EmployeeAllowance.GroupBy(g => g.EmployeeId).SelectMany(s => s.OrderByDescending(g => g.EffectiveDate).Take(1)).ToList();
                        var lstEmpNoTimeSheet = new List<EmployeeMonthySalary>();
                        var lstEmpMonthySalaryUpdate = new List<EmployeeMonthySalary>();
                        var lstEmployeeMonthySalaryEntityModelResult = new List<EmployeeMonthySalaryEntityModel>();
                        var lstEmployeeMonthySalaryNotExist = new List<EmployeeMonthySalary>();
                        listEmpSalaryHandmade.ForEach(item =>
                        {
                            var employeeMonthySalaryObjectUpdate = new EmployeeMonthySalary();
                            var empSalaryMonthObject = lstEmployeeMonthySalary.Where(w => w.EmployeeCode == item.EmployeeCode && w.Month == item.Month && w.Year == item.Year).FirstOrDefault();
                            if (empSalaryMonthObject != null)
                            {
                                try
                                {
                                    employeeMonthySalaryObjectUpdate = empSalaryMonthObject;
                                    employeeMonthySalaryObjectUpdate.MonthlyWorkingDay = item.MonthlyWorkingDay == null ? 0 : item.MonthlyWorkingDay;
                                    employeeMonthySalaryObjectUpdate.UnPaidLeaveDay = employeeMonthySalaryObjectUpdate.MonthlyWorkingDay - employeeMonthySalaryObjectUpdate.ActualWorkingDay;
                                    employeeMonthySalaryObjectUpdate.Overtime = item.Overtime;

                                    employeeMonthySalaryObjectUpdate.EnrollmentSalary = item.EnrollmentSalary;
                                    employeeMonthySalaryObjectUpdate.RetentionSalary = item.RetentionSalary;

                                    employeeMonthySalaryObjectUpdate.DesciplineAmount = item.DesciplineAmount == null ? 0 : item.DesciplineAmount;
                                    employeeMonthySalaryObjectUpdate.ReductionAmount = item.ReductionAmount == null ? 0 : item.ReductionAmount;
                                    employeeMonthySalaryObjectUpdate.AdditionalAmount = item.AdditionalAmount == null ? 0 : item.AdditionalAmount;
                                    var empAllowance = listEmployeeAllowance.Where(w => w.EmployeeId == item.EmployeeId).FirstOrDefault();
                                    if (empAllowance != null)
                                    {
                                        employeeMonthySalaryObjectUpdate.LunchAllowance = (empAllowance.LunchAllowance != null && empAllowance.LunchAllowance != 0 && employeeMonthySalaryObjectUpdate.MonthlyWorkingDay != null && employeeMonthySalaryObjectUpdate.MonthlyWorkingDay != 0 && employeeMonthySalaryObjectUpdate.ActualWorkingDay != null && employeeMonthySalaryObjectUpdate.ActualWorkingDay != 0) ?
                                      Math.Round(((empAllowance.LunchAllowance * employeeMonthySalaryObjectUpdate.ActualWorkingDay) / employeeMonthySalaryObjectUpdate.MonthlyWorkingDay).Value, 0) : 0;
                                    }
                                    employeeMonthySalaryObjectUpdate.ActualOfSalary = (employeeMonthySalaryObjectUpdate.ActualWorkingDay != null && employeeMonthySalaryObjectUpdate.ActualWorkingDay != 0 && employeeMonthySalaryObjectUpdate.BasedSalary != null && employeeMonthySalaryObjectUpdate.BasedSalary != 0 && employeeMonthySalaryObjectUpdate.MonthlyWorkingDay != 0) ?
                                    Math.Round(((employeeMonthySalaryObjectUpdate.ActualWorkingDay * employeeMonthySalaryObjectUpdate.BasedSalary) / employeeMonthySalaryObjectUpdate.MonthlyWorkingDay).Value, 0) : 0;

                                    employeeMonthySalaryObjectUpdate.OvertimeOfSalary = (employeeMonthySalaryObjectUpdate.MonthlyWorkingDay != null && employeeMonthySalaryObjectUpdate.MonthlyWorkingDay != 0 && employeeMonthySalaryObjectUpdate.BasedSalary != null && employeeMonthySalaryObjectUpdate.BasedSalary != 0 && employeeMonthySalaryObjectUpdate.Overtime != null && employeeMonthySalaryObjectUpdate.Overtime != 0) ?
                                    Math.Round(((employeeMonthySalaryObjectUpdate.BasedSalary * employeeMonthySalaryObjectUpdate.Overtime) / employeeMonthySalaryObjectUpdate.MonthlyWorkingDay).Value, 0) : 0;

                                    employeeMonthySalaryObjectUpdate.TotalIncome = (employeeMonthySalaryObjectUpdate.ActualOfSalary == null ? 0 : employeeMonthySalaryObjectUpdate.ActualOfSalary) +
                                    (employeeMonthySalaryObjectUpdate.OvertimeOfSalary == null ? 0 : employeeMonthySalaryObjectUpdate.OvertimeOfSalary) +
                                    (employeeMonthySalaryObjectUpdate.FuelAllowance == null ? 0 : employeeMonthySalaryObjectUpdate.FuelAllowance) +
                                    (employeeMonthySalaryObjectUpdate.PhoneAllowance == null ? 0 : employeeMonthySalaryObjectUpdate.PhoneAllowance) +
                                    (employeeMonthySalaryObjectUpdate.LunchAllowance == null ? 0 : employeeMonthySalaryObjectUpdate.LunchAllowance) +
                                    (employeeMonthySalaryObjectUpdate.OtherAllowance == null ? 0 : employeeMonthySalaryObjectUpdate.OtherAllowance) +
                                    (employeeMonthySalaryObjectUpdate.EnrollmentSalary == null ? 0 : employeeMonthySalaryObjectUpdate.EnrollmentSalary) +
                                    (employeeMonthySalaryObjectUpdate.RetentionSalary == null ? 0 : employeeMonthySalaryObjectUpdate.RetentionSalary);

                                    employeeMonthySalaryObjectUpdate.ActualPaid = employeeMonthySalaryObjectUpdate.TotalIncome - (employeeMonthySalaryObjectUpdate.TotalInsuranceEmployeePaid + employeeMonthySalaryObjectUpdate.DesciplineAmount + employeeMonthySalaryObjectUpdate.ReductionAmount) + employeeMonthySalaryObjectUpdate.AdditionalAmount;
                                    lstEmpMonthySalaryUpdate.Add(employeeMonthySalaryObjectUpdate);
                                    lstEmployeeMonthySalaryEntityModelResult.Add(new EmployeeMonthySalaryEntityModel(employeeMonthySalaryObjectUpdate));

                                }
                                catch (Exception ex)
                                {

                                    throw;
                                }
                            }
                            else
                            {
                                lstEmployeeMonthySalaryNotExist.Add(item);
                            }
                        });
                        if (lstEmployeeMonthySalaryNotExist.Count > 0)
                        {
                            return new EmployeeSalaryHandmadeResult
                            {
                                lstEmployeeMonthySalary = null,
                                Message = string.Format("Chưa có dữ liệu chấm công của nhân viên.Cần import dữ liệu chấm công nhân viên của {0}/{1}", lstEmployeeMonthySalaryNotExist[0].Month, lstEmployeeMonthySalaryNotExist[0].Year),
                                Status = false
                            };

                        }
                        else
                        {

                            var lstEmployeeAssessmentObjectUpdate = new List<EmployeeAssessment>();
                            var lstemployeeAssessmentObjectInsert = new List<EmployeeAssessment>();
                            listEmployeeAssessment.ForEach(item =>
                            {
                                var employeeAssessmentObjectUpdate = new EmployeeAssessment();
                                var employeeAssessmentObject = context.EmployeeAssessment.Where(w => w.EmployeeId == item.EmployeeId && w.Month == item.Month && w.Year == item.Year).FirstOrDefault();
                                if (employeeAssessmentObject != null)
                                {
                                    employeeAssessmentObjectUpdate = employeeAssessmentObject;
                                    employeeAssessmentObjectUpdate.Type = item.Type;
                                    lstEmployeeAssessmentObjectUpdate.Add(employeeAssessmentObject);
                                }
                                else
                                {
                                    lstemployeeAssessmentObjectInsert.Add(item);
                                }
                            });

                            if (lstemployeeAssessmentObjectInsert.Count > 0)
                            {
                                context.EmployeeAssessment.AddRange(lstemployeeAssessmentObjectInsert);
                                context.SaveChanges();

                            }
                            if (lstEmployeeAssessmentObjectUpdate.Count > 0)
                            {
                                context.EmployeeAssessment.UpdateRange(lstEmployeeAssessmentObjectUpdate);
                                context.SaveChanges();

                            }
                            context.EmployeeMonthySalary.UpdateRange(lstEmpMonthySalaryUpdate);
                            context.SaveChanges();

                            return new EmployeeSalaryHandmadeResult
                            {
                                lstEmployeeMonthySalary = lstEmployeeMonthySalaryEntityModelResult,
                                Message = string.Format("Thông tin lương {0}/{1} đa được cập nhật", parameter.Month, parameter.Year),
                                Status = true
                            };

                        }
                    }

                }
            }
            else
            {
                return new EmployeeSalaryHandmadeResult
                {
                    lstEmployeeMonthySalary = null,
                    Message = "Chọn lại file nhập tay",
                    Status = false
                };
            }

        }

        private byte[] ExportEmployeeMonthySalary(List<Entities.EmployeeMonthySalary> lstEmployeeMonthySalary, int month, int year)
        {
            string rootFolder = hostingEnvironment.WebRootPath + "\\ExcelTemplate";
            string fileName = @"ExcelEmployeeMonthySalaryTemplate.xlsx";
            FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
            //MemoryStream output = new MemoryStream();
            byte[] data = null;
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                //worksheet.Name =string.Format("Bảng lương {0}/{1}",month,year);
                worksheet.InsertRow(8, lstEmployeeMonthySalary.Count);
                if (lstEmployeeMonthySalary.Count > 0)
                {

                    int rowIndex = 7;
                    int i = 0;
                    while (i < lstEmployeeMonthySalary.Count)
                    {
                        worksheet.Cells[rowIndex, 1].Value = lstEmployeeMonthySalary[i].EmployeeCode;
                        worksheet.Cells[rowIndex, 2].Value = lstEmployeeMonthySalary[i].EmployeeName;
                        worksheet.Cells[rowIndex, 3].Value = lstEmployeeMonthySalary[i].PostionName;
                        worksheet.Cells[rowIndex, 4].Value = lstEmployeeMonthySalary[i].Month;
                        worksheet.Cells[rowIndex, 5].Value = lstEmployeeMonthySalary[i].Year;
                        worksheet.Cells[rowIndex, 6].Value = lstEmployeeMonthySalary[i].MonthlyWorkingDay;
                        rowIndex++;
                        i++;
                    }
                    string newFilePath = Path.Combine(rootFolder, @"ExportedExcel.xlsx");
                    package.SaveAs(new FileInfo(newFilePath));
                    data = File.ReadAllBytes(newFilePath);
                    File.Delete(newFilePath);

                    //package.SaveAs(output);
                }
                return data;
            }

        }

        public GetEmployeeSalaryByEmpIdResult GetEmployeeSalaryByEmpId(GetEmployeeSalaryByEmpIdParameter parameter)
        {
            // Lay EmpSalary tu con text
            var empSalary = context.EmployeeSalary.Where(empslr => (empslr.EmployeeId == parameter.EmployeeId))
                                                                   .Select(s => new EmployeeSalaryEntityModel(s)).OrderByDescending(t => t.EffectiveDate.Value.Date).ToList();
            return new GetEmployeeSalaryByEmpIdResult()
            {
                ListEmployeeSalary = empSalary,
                Message = "Success",
                Status = true
            };
        }

        public CreateEmployeeSalaryResult CreateEmployeeSalary(CreateEmployeeSalaryParameter parameter)
        {
            if (parameter.EmployeeSalary.EmployeeSalaryBase < 0)
            {
                return new CreateEmployeeSalaryResult()
                {
                    Status = false,
                    Message = "Lương cơ bản không được âm"
                };
            }
            var _empSalary = context.EmployeeSalary.FirstOrDefault(empslr => empslr.EmployeeId == parameter.EmployeeSalary.EmployeeId && empslr.EffectiveDate.Value.Date == parameter.EmployeeSalary.EffectiveDate.Value.Date);
            if (_empSalary != null)
            {
                _empSalary.UpdateById = parameter.UserId;
                _empSalary.UpdateDate = DateTime.Now;
                _empSalary.EffectiveDate = parameter.EmployeeSalary.EffectiveDate;
                _empSalary.EmployeeSalaryBase = parameter.EmployeeSalary.EmployeeSalaryBase;
                _empSalary.ResponsibilitySalary = parameter.EmployeeSalary.ResponsibilitySalary;
                context.EmployeeSalary.Update(_empSalary);
            }
            else
            {
                parameter.EmployeeSalary.CreateDate = DateTime.Now;
                parameter.EmployeeSalary.CreateById = parameter.UserId;
                context.EmployeeSalary.Add(parameter.EmployeeSalary);
            }
            context.SaveChanges();
            return new CreateEmployeeSalaryResult()
            {
                Message = "Success",
                Status = true
            };
        }

        public DownloadEmployeeTimeSheetTemplateResult DownloadEmployeeTimeSheetTemplate(DownloadEmployeeTimeSheetTemplateParameter parameter)
        {
            string rootFolder = hostingEnvironment.WebRootPath + "\\ExcelTemplate";
            string fileName = @"TemplateTimeSheet.xlsx";
            FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
            //MemoryStream output = new MemoryStream();
            //using (ExcelPackage package = new ExcelPackage(file))
            //{
            //    package.SaveAs(output);
            //}
            string newFilePath = Path.Combine(rootFolder, @"TemplateTimeSheet.xlsx");
            byte[] data = File.ReadAllBytes(newFilePath);

            return new DownloadEmployeeTimeSheetTemplateResult
            {
                ExcelFile = data,
                NameFile = @"TemplateTimeSheet",
                Status = true
            };
        }

        public FindEmployeeMonthySalaryResult FindEmployeeMonthySalary(FindEmployeeMonthySalaryParameter parameter)
        {
            var listResult = (from item in context.EmployeeMonthySalary
                              where
                              (item.EmployeeName.Contains(parameter.EmployeeName.Trim()) || parameter.EmployeeName == null || parameter.EmployeeName.Trim() == "") &&
                              (item.EmployeeCode.Contains(parameter.EmployeeCode.Trim()) || parameter.EmployeeCode == null || parameter.EmployeeCode.Trim() == "") &&
                              (item.EmployeeUnit.Contains(parameter.EmployeeUnit.Trim()) || parameter.EmployeeUnit == null || parameter.EmployeeUnit.Trim() == "") &&
                              (item.EmployeeBranch.Contains(parameter.EmployeeBranch.Trim()) || parameter.EmployeeBranch == null || parameter.EmployeeBranch.Trim() == "") &&
                              (item.EmployeePostionId == parameter.EmployeePostionId || parameter.EmployeePostionId == null) &&
                              (parameter.lstEmployeeUnitId.Contains(item.EmployeeUnitId) || parameter.lstEmployeeUnitId.Count == 0) &&
                              (parameter.Month == null || item.Month == parameter.Month.Value) &&
                              (parameter.Year == null || item.Year == parameter.Year.Value) &&
                              (item.Type.Value == 0)
                              select new EmployeeMonthySalaryEntityModel(item)).ToList();

            var note = string.Empty;

            if (listResult.Count > 0)
            {
                note = context.FeatureNote.FirstOrDefault(f => f.FeatureId == listResult[0].CommonId)?.Note;
            }

            return new FindEmployeeMonthySalaryResult
            {
                lstEmployeeMonthySalary = listResult,
                Status = true,
                Notes = StringHelper.ConvertNoteToObject(note),
            };

        }

        public GetTeacherSalaryResult GetTeacherSalary(GetTeacherSalaryParameter parameter)
        {
            DateTime fTime;
            DateTime eTime;
            byte[] excelOutput = null;
            var newId = Guid.NewGuid();
            fTime = new DateTime(parameter.Year.Value, parameter.Month.Value, 1);
            eTime = fTime.AddMonths(1).AddDays(-1);
            string JsonService = SAPIService.GetTeacherSummary(fTime, eTime);
            var lstGetTeacherSummary = JsonConvert.DeserializeObject<List<GetTeacherSummary>>(JsonService);
            if (lstGetTeacherSummary.Count > 0)
            {
                //lấy ra danh sách giảng viên theo Email
                var listEmployeeWorkHour = context.Contact.Where(w => w.ObjectType == "EMP").Select(item => new { EmployeeId = item.ObjectId, Email = item.Email }).DefaultIfEmpty().ToList();
                var lstTimeSheetTeacher = new List<Entities.EmployeeTimesheet>();
                var lstTimeSheetTeacherUpdate = new List<Entities.EmployeeTimesheet>();
                var lstEmployeeMonthySalary = new List<Entities.EmployeeMonthySalary>();
                var lstEmployeeMonthySalaryUpdate = new List<Entities.EmployeeMonthySalary>();
                //lay DS timesheet de check duplicate
                var lstCheckExistTimeSheet = context.EmployeeTimesheet.Where(w => w.Month == parameter.Month && w.Year == parameter.Year && !string.IsNullOrEmpty(w.Center));
                var lstemployeeMonthySalary = context.EmployeeMonthySalary.Where(w => w.Month == parameter.Month && w.Year == parameter.Year && w.Type == 1);
                //////////////
                lstGetTeacherSummary.ForEach(item =>
                {
                    var empId = listEmployeeWorkHour.Where(w => w.Email.ToLower() == item.Email.ToLower()).FirstOrDefault();
                    if (empId != null)
                    {
                        var existTimeSheet = lstCheckExistTimeSheet.Where(w => w.Center.ToLower() == item.Center.ToLower() && w.EmployeeId == empId.EmployeeId).FirstOrDefault();
                        var itemEmpTimeSheet = new EmployeeTimesheet();
                        if (existTimeSheet == null)
                        {
                            itemEmpTimeSheet.ActualWorkingDay = item.GIODAY;
                            itemEmpTimeSheet.EmployeeId = empId.EmployeeId;
                            itemEmpTimeSheet.Month = parameter.Month;
                            itemEmpTimeSheet.Year = parameter.Year;
                            itemEmpTimeSheet.Center = item.Center;
                            itemEmpTimeSheet.CreateById = parameter.UserId;
                            itemEmpTimeSheet.CreateDate = DateTime.Now;
                            lstTimeSheetTeacher.Add(itemEmpTimeSheet);
                        }
                        else
                        {
                            itemEmpTimeSheet = existTimeSheet;
                            itemEmpTimeSheet.ActualWorkingDay = item.GIODAY;
                            itemEmpTimeSheet.UpdateById = parameter.UserId;
                            itemEmpTimeSheet.UpdateDate = DateTime.Now;
                            lstTimeSheetTeacherUpdate.Add(itemEmpTimeSheet);
                        }
                    }
                });
                //tạo mới trogn timesheet
                if (lstTimeSheetTeacher.Count > 0)
                {
                    context.EmployeeTimesheet.AddRange(lstTimeSheetTeacher);
                    context.SaveChanges();
                }
                //update lại timesheet nếu có
                if (lstTimeSheetTeacherUpdate.Count > 0)
                {
                    context.EmployeeTimesheet.UpdateRange(lstTimeSheetTeacherUpdate);
                    context.SaveChanges();
                }
                ///Gộp lại
                lstTimeSheetTeacher.AddRange(lstTimeSheetTeacherUpdate);
                var listEmpNoDuplicate = lstTimeSheetTeacher.Select(s => new { empId = s.EmployeeId }).Distinct().ToList();
                //lấy ra các trung tâm
                var listCenter = lstTimeSheetTeacher.Select(s => new { center = s.Center }).Distinct().ToList();
                //lấy ra thông tin giảng viên
                var listEmpInformation = (from emp in context.Employee
                                          join pos in context.Position on emp.PositionId equals pos.PositionId
                                          join org in context.Organization on emp.OrganizationId equals org.OrganizationId
                                          select new { emp.EmployeeId, emp.EmployeeName, emp.PositionId, emp.EmployeeCode, pos.PositionName, org.OrganizationName }
                                        ).ToList();

                List<dynamic> lstResult = new List<dynamic>();

                listEmpNoDuplicate.ForEach(item =>
                {
                    decimal SumGioDay = 0;
                    var employeeMonthySalaryObject = new EmployeeMonthySalary();
                    var checkExistEmployeeMonthySalary = lstemployeeMonthySalary.Where(w => w.EmployeeId == item.empId).FirstOrDefault();
                    if (checkExistEmployeeMonthySalary == null)
                    {
                        employeeMonthySalaryObject.EmployeeId = item.empId;
                        employeeMonthySalaryObject.Month = parameter.Month;
                        employeeMonthySalaryObject.Year = parameter.Year;
                        var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                        var empInf = listEmpInformation.Where(w => w.EmployeeId == item.empId).FirstOrDefault();
                        if (empInf != null)
                        {
                            var key = Guid.NewGuid();
                            sampleObject.Add("EmployeeMonthySalaryId", key);
                            employeeMonthySalaryObject.EmployeeMonthySalaryId = key;
                            sampleObject.Add("EmployeeCode", empInf.EmployeeCode);
                            employeeMonthySalaryObject.EmployeeCode = empInf.EmployeeCode;
                            sampleObject.Add("EmployeeName", empInf.EmployeeName);
                            employeeMonthySalaryObject.EmployeeName = empInf.EmployeeName;
                            employeeMonthySalaryObject.EmployeePostionId = empInf.PositionId;
                            listCenter.ForEach(itemC =>
                            {
                                var GIODAY = lstTimeSheetTeacher.Where(w => w.Center == itemC.center && w.EmployeeId == item.empId).Select(s => new { GIODAY = (decimal?)s.ActualWorkingDay }).DefaultIfEmpty().FirstOrDefault();
                                SumGioDay = SumGioDay + (GIODAY != null ? GIODAY.GIODAY.Value : 0);
                                sampleObject.Add(itemC.center, GIODAY != null ? GIODAY.GIODAY.Value : 0);
                            });
                            sampleObject.Add("ActualWorkingDay", SumGioDay);
                            employeeMonthySalaryObject.ActualWorkingDay = SumGioDay;
                            //sampleObject.Add("BasedSalary", null);
                            sampleObject.Add("BasedSalary", null);
                            sampleObject.Add("AdditionalAmount", null);
                            sampleObject.Add("ReductionAmount", null);
                            sampleObject.Add("ActualPaid", null);
                            var emailEmp = listEmployeeWorkHour.Where(w => w.EmployeeId == item.empId).Select(s => new { email = s.Email }).FirstOrDefault();
                            sampleObject.Add("Email", emailEmp != null ? emailEmp.email : null);
                            sampleObject.Add("Description", null);
                            employeeMonthySalaryObject.Email = emailEmp != null ? emailEmp.email : null;
                            employeeMonthySalaryObject.CreateById = parameter.UserId;
                            employeeMonthySalaryObject.CreateDate = DateTime.Now;
                            employeeMonthySalaryObject.Type = 1;
                            employeeMonthySalaryObject.CommonId = newId;
                            var draftId = context.Category.FirstOrDefault(c => c.CategoryCode == "DR").CategoryId;
                            employeeMonthySalaryObject.StatusId = draftId;
                            sampleObject.Add("CommonId", employeeMonthySalaryObject.CommonId);

                            lstResult.Add(sampleObject);
                            lstEmployeeMonthySalary.Add(employeeMonthySalaryObject);
                        }
                    }
                    else
                    {
                        employeeMonthySalaryObject = checkExistEmployeeMonthySalary;
                        var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                        sampleObject.Add("EmployeeMonthySalaryId", employeeMonthySalaryObject.EmployeeMonthySalaryId);
                        sampleObject.Add("CommonId", employeeMonthySalaryObject.CommonId);
                        sampleObject.Add("EmployeeCode", employeeMonthySalaryObject.EmployeeCode);
                        sampleObject.Add("EmployeeName", employeeMonthySalaryObject.EmployeeName);
                        listCenter.ForEach(itemC =>
                        {
                            var GIODAY = lstTimeSheetTeacher.Where(w => w.Center == itemC.center && w.EmployeeId == item.empId).Select(s => new { GIODAY = (decimal?)s.ActualWorkingDay }).DefaultIfEmpty().FirstOrDefault();
                            SumGioDay = SumGioDay + (GIODAY != null ? GIODAY.GIODAY.Value : 0);
                            sampleObject.Add(itemC.center, GIODAY != null ? GIODAY.GIODAY.Value : 0);
                        });
                        sampleObject.Add("ActualWorkingDay", SumGioDay);
                        employeeMonthySalaryObject.ActualWorkingDay = SumGioDay;
                        sampleObject.Add("BasedSalary", null);
                        sampleObject.Add("AdditionalAmount", null);
                        sampleObject.Add("ReductionAmount", null);
                        sampleObject.Add("ActualPaid", null);
                        sampleObject.Add("Email", employeeMonthySalaryObject.Email);
                        sampleObject.Add("Description", null);
                        employeeMonthySalaryObject.UpdateById = parameter.UserId;
                        employeeMonthySalaryObject.UpdateDate = DateTime.Now;
                        lstResult.Add(sampleObject);
                        lstEmployeeMonthySalaryUpdate.Add(employeeMonthySalaryObject);
                    }

                });

                //tạo mới trong bảng lương
                if (lstEmployeeMonthySalary.Count > 0)
                {
                    context.EmployeeMonthySalary.AddRange(lstEmployeeMonthySalary);
                    context.SaveChanges();
                }
                if (lstEmployeeMonthySalaryUpdate.Count > 0)
                {
                    context.EmployeeMonthySalary.UpdateRange(lstEmployeeMonthySalaryUpdate);
                    context.SaveChanges();
                }
                lstEmployeeMonthySalary.AddRange(lstEmployeeMonthySalaryUpdate);
                excelOutput = this.ExportTeacherMonthySalary(lstEmployeeMonthySalary, parameter.Month.Value, parameter.Year.Value);
                //Lấy ra danh sách các cột để hiển thị ngoài font-end
                List<KeyValuePair<string, string>> lstColumn = new List<KeyValuePair<string, string>>();
                if (lstResult.Count > 0)
                {
                    foreach (var property in (IDictionary<String, Object>)lstResult[0])
                    {
                        //lstColumn.Add(property.Key);
                        switch (property.Key)
                        {
                            case "EmployeeCode":
                                lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Mã nhân viên"));
                                break;
                            case "EmployeeName":
                                lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Tên nhân viên"));
                                break;
                            case "ActualWorkingDay":
                                lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Công thực tế"));
                                break;
                            case "BasedSalary":
                                lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Lương theo giờ"));
                                break;
                            case "AdditionalAmount":
                                lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Cộng thêm"));
                                break;
                            case "ReductionAmount":
                                lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Trừ"));
                                break;
                            case "ActualPaid":
                                lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Thực  lĩnh"));
                                break;
                            case "Description":
                                lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Diễn giải"));
                                break;
                            default:
                                lstColumn.Add(new KeyValuePair<string, string>(property.Key, property.Key));
                                break;
                        }
                    }
                }

                return new GetTeacherSalaryResult
                {
                    ExcelFile = excelOutput,
                    NameFile = string.Format("Thông tin các khoản nhập tay giảng viên tháng {0}", parameter.Month),
                    lstResult = lstResult,
                    lstColumn = lstColumn,
                    Message = "Success",
                    Status = true
                };

            }
            else
            {
                return new GetTeacherSalaryResult
                {
                    lstResult = null,
                    ExcelFile = null,
                    NameFile = null,
                    lstColumn = null,
                    Message = string.Format("Chưa có thông tin lương giảng viên tháng {0}/{1} trên API", parameter.Month, parameter.Year),
                    Status = false
                };
            }
        }

        private byte[] ExportTeacherMonthySalary(List<Entities.EmployeeMonthySalary> lstEmployeeMonthySalary, int month, int year)
        {
            string rootFolder = hostingEnvironment.WebRootPath + "\\ExcelTemplate";
            string fileName = @"ExcelTeacherMonthySalaryTemplate.xlsx";
            FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
            //MemoryStream output = new MemoryStream();
            byte[] data = null;
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                //worksheet.Name =string.Format("Bảng lương {0}/{1}",month,year);
                worksheet.InsertRow(8, lstEmployeeMonthySalary.Count);
                if (lstEmployeeMonthySalary.Count > 0)
                {

                    int rowIndex = 7;
                    int i = 0;
                    while (i < lstEmployeeMonthySalary.Count)
                    {
                        worksheet.Cells[rowIndex, 1].Value = lstEmployeeMonthySalary[i].EmployeeCode;
                        worksheet.Cells[rowIndex, 2].Value = lstEmployeeMonthySalary[i].EmployeeName;
                        //worksheet.Cells[rowIndex, 3].Value = lstEmployeeMonthySalary[i].PostionName;
                        worksheet.Cells[rowIndex, 3].Value = lstEmployeeMonthySalary[i].Month;
                        worksheet.Cells[rowIndex, 4].Value = lstEmployeeMonthySalary[i].Year;
                        rowIndex++;
                        i++;
                    }
                    string newFilePath = Path.Combine(rootFolder, @"ExportedExcelTeacher.xlsx");
                    package.SaveAs(new FileInfo(newFilePath));
                    data = File.ReadAllBytes(newFilePath);
                    File.Delete(newFilePath);

                    //package.SaveAs(output);
                }
                return data;
            }

        }

        public TeacherSalaryHandmadeResult TeacherSalaryHandmadeImport(TeacherSalaryHandmadeParameter parameter)
        {
            if (parameter.FileList != null && parameter.FileList.Count > 0)
            {
                using (var stream = new MemoryStream())
                {
                    parameter.FileList[0].CopyTo(stream);
                    using (ExcelPackage package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets["Customer"];
                        //Group cells by row
                        var rowcellgroups = worksheet.Cells["A:H"].GroupBy(c => c.Start.Row);
                        //Loại bỏ 6 dòng tiêu đề
                        var groups = rowcellgroups.Skip(5);
                        //Group theo từng ngày
                        var cv = (from item in groups
                                  group item by new
                                  {
                                      item.First().Value,
                                  } into gcs
                                  select gcs).ToList();

                        var listEmployee = context.Employee.Select(item => new { EmployeeId = item.EmployeeId, EmployeeCode = item.EmployeeCode, }).ToList();


                        var listEmpSalaryHandmade = cv.Select(g => new EmployeeMonthySalary
                        {
                            EmployeeCode = g.Key.Value.ToString(),
                            EmployeeName = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "B")).First().Value.ToString(),
                            Month = int.Parse(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "C")).First().Value.ToString()),
                            Year = int.Parse(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "D")).First().Value.ToString()),
                            BasedSalary = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "E")).First() != null ? Convert.ToDecimal(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "E")).First().Value.ToString()) : 0,
                            AdditionalAmount = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "F")).First() != null ? int.Parse(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "F")).First().Value.ToString()) : 0,
                            ReductionAmount = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "G")).First() != null ? Convert.ToDecimal(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "G")).First().Value.ToString()) : 0,
                            Description = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "H")).First() != null ? (g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "H")).First().Value.ToString()) : null,
                        }).ToList();

                        var lstEmployeeMonthySalary = context.EmployeeMonthySalary.Where(w => w.Month == parameter.Month && w.Year == parameter.Year).ToList();
                        var lstTimeSheetTeacher = context.EmployeeTimesheet.Where(w => w.Month == parameter.Month && w.Year == parameter.Year).ToList();
                        //Lay ra danh sach Emp ko duplicate
                        var listEmpNoDuplicate = lstTimeSheetTeacher.Select(s => new { empId = s.EmployeeId }).Distinct().ToList();
                        //lấy ra các trung tâm
                        var listCenter = lstTimeSheetTeacher.Select(s => new { center = s.Center }).Distinct().ToList();
                        ////////////////
                        var lstEmpMonthySalaryUpdate = new List<EmployeeMonthySalary>();
                        var lstEmployeeMonthySalaryNotExist = new List<EmployeeMonthySalary>();
                        ///////////////
                        listEmpSalaryHandmade.ForEach(item =>
                        {
                            var employeeMonthySalaryObjectUpdate = new EmployeeMonthySalary();
                            var empSalaryMonthObject = lstEmployeeMonthySalary.Where(w => w.EmployeeCode == item.EmployeeCode && w.Month == item.Month && w.Year == item.Year).FirstOrDefault();
                            if (empSalaryMonthObject != null)
                            {
                                employeeMonthySalaryObjectUpdate = empSalaryMonthObject;
                                employeeMonthySalaryObjectUpdate.BasedSalary = item.BasedSalary == null ? 0 : item.BasedSalary;
                                employeeMonthySalaryObjectUpdate.ReductionAmount = item.ReductionAmount == null ? 0 : item.ReductionAmount;
                                employeeMonthySalaryObjectUpdate.AdditionalAmount = item.AdditionalAmount == null ? 0 : item.AdditionalAmount;
                                employeeMonthySalaryObjectUpdate.Description = item.Description == null ? null : item.Description;

                                employeeMonthySalaryObjectUpdate.ActualPaid = (employeeMonthySalaryObjectUpdate.BasedSalary.Value * employeeMonthySalaryObjectUpdate.ActualWorkingDay.Value) + employeeMonthySalaryObjectUpdate.AdditionalAmount - employeeMonthySalaryObjectUpdate.ReductionAmount;

                                lstEmpMonthySalaryUpdate.Add(employeeMonthySalaryObjectUpdate);
                            }
                            else
                            {
                                lstEmployeeMonthySalaryNotExist.Add(item);
                            }
                        });
                        if (lstEmployeeMonthySalaryNotExist.Count > 0)
                        {
                            return new TeacherSalaryHandmadeResult
                            {
                                lstResult = null,
                                lstColumn = null,
                                Message = string.Format("Chưa lấy dữ liệu từ SIS.Cần lấy dữ liệu trước khi import file nhập tay của {0}/{1}", parameter.Month, parameter.Year),
                                Status = false
                            };
                        }
                        else
                        {
                            context.EmployeeMonthySalary.UpdateRange(lstEmpMonthySalaryUpdate);
                            context.SaveChanges();
                            List<dynamic> lstResult = new List<dynamic>();
                            listEmpNoDuplicate.ForEach(item =>
                            {
                                decimal SumGioDay = 0;
                                var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                                var empInf = lstEmpMonthySalaryUpdate.Where(w => w.EmployeeId == item.empId).FirstOrDefault();
                                if (empInf != null)
                                {
                                    sampleObject.Add("EmployeeMonthySalaryId", empInf.EmployeeMonthySalaryId);
                                    sampleObject.Add("EmployeeCode", empInf.EmployeeCode);
                                    sampleObject.Add("EmployeeName", empInf.EmployeeName);

                                    listCenter.ForEach(itemC =>
                                    {
                                        var GIODAY = lstTimeSheetTeacher.Where(w => w.Center == itemC.center && w.EmployeeId == item.empId).Select(s => new { GIODAY = (decimal?)s.ActualWorkingDay }).DefaultIfEmpty().FirstOrDefault();
                                        SumGioDay = SumGioDay + (GIODAY != null ? GIODAY.GIODAY.Value : 0);
                                        sampleObject.Add(itemC.center, GIODAY != null ? GIODAY.GIODAY.Value : 0);
                                    });
                                    sampleObject.Add("ActualWorkingDay", SumGioDay);
                                    sampleObject.Add("BasedSalary", empInf.BasedSalary);
                                    sampleObject.Add("AdditionalAmount", empInf.AdditionalAmount);
                                    sampleObject.Add("ReductionAmount", empInf.ReductionAmount);
                                    sampleObject.Add("ActualPaid", empInf.ActualPaid);
                                    sampleObject.Add("Email", empInf.Email);
                                    sampleObject.Add("Description", empInf.Description);
                                    lstResult.Add(sampleObject);
                                }
                            });

                            //Lấy ra danh sách các cột để hiển thị ngoài font-end
                            List<KeyValuePair<string, string>> lstColumn = new List<KeyValuePair<string, string>>();
                            if (lstResult.Count > 0)
                            {
                                foreach (var property in (IDictionary<String, Object>)lstResult[0])
                                {
                                    switch (property.Key)
                                    {
                                        case "EmployeeCode":
                                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Mã nhân viên"));
                                            break;
                                        case "EmployeeName":
                                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Tên nhân viên"));
                                            break;
                                        case "ActualWorkingDay":
                                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Công thực tế"));
                                            break;
                                        case "BasedSalary":
                                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Lương theo giờ"));
                                            break;
                                        case "AdditionalAmount":
                                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Cộng thêm"));
                                            break;
                                        case "ReductionAmount":
                                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Trừ"));
                                            break;
                                        case "ActualPaid":
                                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Thực  lĩnh"));
                                            break;
                                        case "Description":
                                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Diễn giải"));
                                            break;
                                        default:
                                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, property.Key));
                                            break;
                                    }
                                }
                            }
                            return new TeacherSalaryHandmadeResult
                            {
                                lstResult = lstResult,
                                lstColumn = lstColumn,
                                Message = "Bảng lương giảng viên đã được cập nhật lại",
                                Status = true
                            };
                        }

                    }

                }
            }
            else
            {
                return new TeacherSalaryHandmadeResult
                {
                    lstResult = null,
                    lstColumn = null,
                    Message = "Chọn lại thông tin file nhập tay",
                    Status = false
                };

            }
        }

        public FindTeacherMonthySalaryResult FindTeacherMonthySalary(FindTeacherMonthySalaryParameter parameter)
        {
            var listEmployeeMonthySalary = (from item in context.EmployeeMonthySalary
                                            where
                                            (parameter.EmployeeName == null || parameter.EmployeeName.Trim() == "" || item.EmployeeName.Contains(parameter.EmployeeName.Trim())) &&
                                            (parameter.EmployeeCode == null || parameter.EmployeeCode.Trim() == "" || item.EmployeeCode.Contains(parameter.EmployeeCode.Trim())) &&
                                            (parameter.EmployeePostionId == null || item.EmployeePostionId == parameter.EmployeePostionId) &&
                                            (parameter.Month == null || item.Month == parameter.Month.Value) &&
                                            (parameter.Year == null || item.Year == parameter.Year.Value) &&
                                            (item.Type.Value == 1)
                                            select new EmployeeMonthySalaryEntityModel(item)).ToList();

            var lstMonthAndYear = listEmployeeMonthySalary.Select(s => new { month = s.Month, year = s.Year }).ToList().Distinct();
            //Lấy ra danh sách các trung tâm có thể tồn tại trong tháng đó chi cho giảng viên
            var lstCenter = context.EmployeeTimesheet.Where(w => lstMonthAndYear.Select(s => s.month).Contains(w.Month.Value) && lstMonthAndYear.Select(s => s.year).Contains(w.Year.Value) && !string.IsNullOrEmpty(w.Center))
                .Select(s => new { center = s.Center }).ToList().Distinct();

            List<dynamic> lstResult = new List<dynamic>();

            listEmployeeMonthySalary.ForEach(empMonthySalary =>
            {

                var lstTimeSheetTeacher = context.EmployeeTimesheet.Where(w => w.Month == empMonthySalary.Month && w.Year == empMonthySalary.Year && w.EmployeeId == empMonthySalary.EmployeeId).ToList();

                decimal SumGioDay = 0;
                var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                var empInf = empMonthySalary;
                if (empInf != null)
                {

                    sampleObject.Add("EmployeeMonthySalaryId", empInf.EmployeeMonthySalaryId);
                    sampleObject.Add("EmployeeCode", empInf.EmployeeCode);
                    sampleObject.Add("EmployeeName", empInf.EmployeeName);
                    sampleObject.Add("CommonId", empInf.CommonId);

                    lstCenter.ToList().ForEach(itemC =>
                    {
                        var GIODAY = lstTimeSheetTeacher.Where(w => w.Center == itemC.center).Select(s => new { GIODAY = (decimal?)s.ActualWorkingDay }).DefaultIfEmpty().FirstOrDefault();
                        SumGioDay = SumGioDay + (GIODAY != null ? GIODAY.GIODAY.Value : 0);
                        sampleObject.Add(itemC.center, GIODAY != null ? GIODAY.GIODAY.Value : 0);
                    });
                    sampleObject.Add("ActualWorkingDay", SumGioDay);
                    sampleObject.Add("BasedSalary", empInf.BasedSalary);
                    sampleObject.Add("AdditionalAmount", empInf.AdditionalAmount);
                    sampleObject.Add("ReductionAmount", empInf.ReductionAmount);
                    sampleObject.Add("ActualPaid", empInf.ActualPaid);
                    sampleObject.Add("Email", empInf.Email);
                    sampleObject.Add("Description", empInf.Description);
                    lstResult.Add(sampleObject);
                }

            });

            //Lấy ra danh sách các cột để hiển thị ngoài font-end
            List<KeyValuePair<string, string>> lstColumn = new List<KeyValuePair<string, string>>();
            if (lstResult.Count > 0)
            {
                foreach (var property in (IDictionary<String, Object>)lstResult[0])
                {
                    switch (property.Key)
                    {
                        case "EmployeeCode":
                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Mã nhân viên"));
                            break;
                        case "EmployeeName":
                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Tên nhân viên"));
                            break;
                        case "ActualWorkingDay":
                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Công thực tế"));
                            break;
                        case "BasedSalary":
                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Lương theo giờ"));
                            break;
                        case "AdditionalAmount":
                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Cộng thêm"));
                            break;
                        case "ReductionAmount":
                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Trừ"));
                            break;
                        case "ActualPaid":
                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Thực  lĩnh"));
                            break;
                        case "Description":
                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Diễn giải"));
                            break;
                        default:
                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, property.Key));
                            break;
                    }
                }
            }

            var note = string.Empty;

            if (listEmployeeMonthySalary.Count > 0)
            {
                note = context.FeatureNote.FirstOrDefault(f => f.FeatureId == listEmployeeMonthySalary[0].CommonId)?.Note;
            }

            return new FindTeacherMonthySalaryResult
            {
                lstResult = lstResult,
                lstColumn = lstColumn,
                Message = "OK",
                Status = true,
                Notes = StringHelper.ConvertNoteToObject(note),
            };

        }

        public ExportAssistantResult ExportAssistant(ExportAssistantParameter parameter)
        {
            byte[] excelOutput = null;
            //Get danh sach tro giang
            var newId = Guid.NewGuid();

            var listEmpInformation = (from emp in context.Employee
                                      join pos in context.Position on emp.PositionId equals pos.PositionId
                                      where pos.PositionName == "Trợ giảng"
                                      join org in context.Organization on emp.OrganizationId equals org.OrganizationId
                                      select new { emp, pos.PositionName, org.OrganizationId, org.OrganizationName }
                        ).ToList();

            if (listEmpInformation.Count > 0)
            {
                var lstEmp = listEmpInformation.Select(s => s.emp).ToList();
                var lstCheckExistEmployeeMonthySalary = context.EmployeeMonthySalary.Where(w => w.Month == parameter.Month && w.Year == parameter.Year && w.Type == 2).ToList();

                if (lstEmp.Count > 0)
                {
                    List<EmployeeMonthySalaryEntityModel> lstEmployeeMonthySalary = new List<EmployeeMonthySalaryEntityModel>();
                    List<Entities.EmployeeMonthySalary> lstEmployeeMonthySalaryEnities = new List<Entities.EmployeeMonthySalary>();
                    List<Entities.EmployeeMonthySalary> lstEmployeeMonthySalaryEnitiesUpdate = new List<Entities.EmployeeMonthySalary>();
                    listEmpInformation.ForEach(item =>
                    {
                        var empSalaryMonthObject = lstCheckExistEmployeeMonthySalary.Where(w => w.EmployeeCode == item.emp.EmployeeCode).FirstOrDefault();
                        if (empSalaryMonthObject == null)
                        {
                            var employeeMonthySalaryObject = new Entities.EmployeeMonthySalary();
                            employeeMonthySalaryObject.EmployeeCode = item.emp.EmployeeCode;
                            employeeMonthySalaryObject.EmployeeName = item.emp.EmployeeName;
                            employeeMonthySalaryObject.EmployeePostionId = item.emp.PositionId;
                            employeeMonthySalaryObject.PostionName = item.PositionName;
                            employeeMonthySalaryObject.EmployeeId = item.emp.EmployeeId;
                            employeeMonthySalaryObject.EmployeeUnit = item.OrganizationName;
                            employeeMonthySalaryObject.EmployeeBranch = item.OrganizationName;
                            employeeMonthySalaryObject.EmployeeUnitId = item.OrganizationId;
                            employeeMonthySalaryObject.EmployeeBranchId = item.OrganizationId;
                            employeeMonthySalaryObject.Month = parameter.Month;
                            employeeMonthySalaryObject.Year = parameter.Year;
                            employeeMonthySalaryObject.CreateById = parameter.UserId;
                            employeeMonthySalaryObject.CreateDate = DateTime.Now;
                            employeeMonthySalaryObject.Type = 2;
                            employeeMonthySalaryObject.CommonId = newId;
                            var draftId = context.Category.FirstOrDefault(c => c.CategoryCode == "DR").CategoryId;
                            employeeMonthySalaryObject.StatusId = draftId;
                            lstEmployeeMonthySalary.Add(new EmployeeMonthySalaryEntityModel(employeeMonthySalaryObject));
                            lstEmployeeMonthySalaryEnities.Add(employeeMonthySalaryObject);
                        }
                        else
                        {
                            var employeeMonthySalaryObject = new Entities.EmployeeMonthySalary();
                            employeeMonthySalaryObject = empSalaryMonthObject;
                            lstEmployeeMonthySalary.Add(new EmployeeMonthySalaryEntityModel(employeeMonthySalaryObject));

                        }
                    });
                    if (lstEmployeeMonthySalaryEnities.Count > 0)
                    {
                        context.EmployeeMonthySalary.AddRange(lstEmployeeMonthySalaryEnities);
                        context.SaveChanges();
                    }

                    excelOutput = this.ExportListAssistant(lstEmployeeMonthySalary, parameter.Month, parameter.Year);
                    return new ExportAssistantResult
                    {
                        ExcelFile = excelOutput,
                        lstEmployeeMonthySalary = lstEmployeeMonthySalary,
                        NameFile = string.Format("Danh sách trợ giảng {0}/{1}", parameter.Month, parameter.Year),
                        Message = "OK",
                        Status = true
                    };
                }
                else
                {
                    return new ExportAssistantResult
                    {
                        ExcelFile = null,
                        NameFile = string.Empty,
                        Message = "Fail",
                        Status = false
                    };
                }
            }
            else
            {
                return new ExportAssistantResult
                {
                    ExcelFile = null,
                    NameFile = string.Empty,
                    Message = "Fail",
                    Status = false
                };
            }
        }

        private byte[] ExportListAssistant(List<EmployeeMonthySalaryEntityModel> lstAssistant, int month, int year)
        {
            string rootFolder = hostingEnvironment.WebRootPath + "\\ExcelTemplate";
            string fileName = @"ExcelAssistantList.xlsx";
            FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
            //MemoryStream output = new MemoryStream();
            byte[] data = null;
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                worksheet.InsertRow(8, lstAssistant.Count);
                if (lstAssistant.Count > 0)
                {

                    int rowIndex = 7;
                    int i = 0;
                    while (i < lstAssistant.Count)
                    {
                        worksheet.Cells[rowIndex, 1].Value = lstAssistant[i].EmployeeCode;
                        worksheet.Cells[rowIndex, 2].Value = lstAssistant[i].EmployeeName;
                        worksheet.Cells[rowIndex, 3].Value = month;
                        worksheet.Cells[rowIndex, 4].Value = year;
                        worksheet.Cells[rowIndex, 5].Value = lstAssistant[i].ActualWorkingDay;
                        worksheet.Cells[rowIndex, 6].Value = lstAssistant[i].BasedSalary;
                        worksheet.Cells[rowIndex, 7].Value = lstAssistant[i].AdditionalAmount;
                        worksheet.Cells[rowIndex, 8].Value = lstAssistant[i].ReductionAmount;
                        worksheet.Cells[rowIndex, 9].Value = lstAssistant[i].Description;
                        rowIndex++;
                        i++;
                    }
                    string newFilePath = Path.Combine(rootFolder, @"ExportedExcel.xlsx");
                    package.SaveAs(new FileInfo(newFilePath));
                    data = File.ReadAllBytes(newFilePath);
                    File.Delete(newFilePath);
                }
                return data;
            }

        }

        public AssistantSalaryHandmadeResult AssistantSalaryHandmadeImport(AssistantSalaryHandmadeParameter parameter)
        {
            if (parameter.FileList != null && parameter.FileList.Count > 0)
            {
                using (var stream = new MemoryStream())
                {
                    parameter.FileList[0].CopyTo(stream);
                    using (ExcelPackage package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets["Customer"];
                        //Group cells by row
                        var rowcellgroups = worksheet.Cells["A:I"].GroupBy(c => c.Start.Row);
                        //Loại bỏ 6 dòng tiêu đề
                        var groups = rowcellgroups.Skip(4);
                        //Group theo từng ngày
                        var cv = (from item in groups
                                  group item by new
                                  {
                                      item.First().Value,
                                  } into gcs
                                  select gcs).ToList();

                        var listEmployee = context.Employee.Select(item => new { EmployeeId = item.EmployeeId, EmployeeCode = item.EmployeeCode, }).ToList();


                        var listEmpSalaryHandmade = cv.Select(g => new EmployeeMonthySalary
                        {
                            EmployeeCode = g.Key.Value.ToString(),
                            EmployeeName = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "B")).First().Value.ToString(),
                            Month = int.Parse(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "C")).First().Value.ToString()),
                            Year = int.Parse(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "D")).First().Value.ToString()),
                            ActualWorkingDay = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "E")).First() != null ? Convert.ToDecimal(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "E")).First().Value.ToString()) : 0,
                            BasedSalary = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "F")).First() != null ? Convert.ToDecimal(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "F")).First().Value.ToString()) : 0,
                            AdditionalAmount = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "G")).First() != null ? int.Parse(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "G")).First().Value.ToString()) : 0,
                            ReductionAmount = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "H")).First() != null ? Convert.ToDecimal(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "H")).First().Value.ToString()) : 0,
                            Description = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First() != null ? (g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "I")).First().Value.ToString()) : null,
                        }).ToList();
                        var lstEmployeeMonthySalary = context.EmployeeMonthySalary.Where(w => w.Month == parameter.Month && w.Year == parameter.Year && w.Type == 2).ToList();
                        /////////////////////////
                        var lstEmpMonthySalaryUpdate = new List<EmployeeMonthySalary>();
                        List<EmployeeMonthySalaryEntityModel> lstEmployeeMonthySalaryEntityModel = new List<EmployeeMonthySalaryEntityModel>();
                        ///////////////
                        var lstEmployeeMonthySalaryNotExist = new List<EmployeeMonthySalary>();

                        listEmpSalaryHandmade.ForEach(item =>
                        {
                            var employeeMonthySalaryObjectUpdate = new EmployeeMonthySalary();
                            var empSalaryMonthObject = lstEmployeeMonthySalary.Where(w => w.EmployeeCode == item.EmployeeCode && w.Month == item.Month && w.Year == item.Year).FirstOrDefault();
                            if (empSalaryMonthObject != null)
                            {
                                employeeMonthySalaryObjectUpdate = empSalaryMonthObject;
                                employeeMonthySalaryObjectUpdate.ActualWorkingDay = item.ActualWorkingDay == null ? 0 : item.ActualWorkingDay;
                                employeeMonthySalaryObjectUpdate.BasedSalary = item.BasedSalary == null ? 0 : item.BasedSalary;
                                employeeMonthySalaryObjectUpdate.ReductionAmount = item.ReductionAmount == null ? 0 : item.ReductionAmount;
                                employeeMonthySalaryObjectUpdate.AdditionalAmount = item.AdditionalAmount == null ? 0 : item.AdditionalAmount;
                                employeeMonthySalaryObjectUpdate.Description = item.Description == null ? null : item.Description;

                                employeeMonthySalaryObjectUpdate.ActualPaid = (employeeMonthySalaryObjectUpdate.BasedSalary.Value * employeeMonthySalaryObjectUpdate.ActualWorkingDay.Value) + employeeMonthySalaryObjectUpdate.AdditionalAmount - employeeMonthySalaryObjectUpdate.ReductionAmount;

                                lstEmpMonthySalaryUpdate.Add(employeeMonthySalaryObjectUpdate);
                                lstEmployeeMonthySalaryEntityModel.Add(new EmployeeMonthySalaryEntityModel(employeeMonthySalaryObjectUpdate));
                            }
                            else
                            {
                                lstEmployeeMonthySalaryNotExist.Add(item);
                            }
                        });

                        if (lstEmployeeMonthySalaryNotExist.Count > 0)
                        {
                            return new AssistantSalaryHandmadeResult
                            {
                                lstEmployeeMonthySalary = null,
                                Message = string.Format("Chưa import danh sách trợ giảng.Cần import danh sách trợ giảng trước khi import file nhập tay {0}/{1}", parameter.Month, parameter.Year),
                                Status = false
                            };

                        }
                        else
                        {
                            context.EmployeeMonthySalary.UpdateRange(lstEmpMonthySalaryUpdate);
                            context.SaveChanges();

                            return new AssistantSalaryHandmadeResult
                            {
                                lstEmployeeMonthySalary = lstEmployeeMonthySalaryEntityModel,
                                Message = "Bảng lương trợ giảng đã được cập nhật lại",
                                Status = true
                            };
                        }
                    }

                }
            }
            else
            {
                return new AssistantSalaryHandmadeResult
                {
                    lstEmployeeMonthySalary = null,
                    Message = "Xem lại thông tin file nhập tay",
                    Status = false
                };

            }

        }

        public FindAssistantMonthySalaryResult FindAssistantMonthySalary(FindAssistantMonthySalaryParameter parameter)
        {
            var listResult = (from item in context.EmployeeMonthySalary
                              where
                              (item.EmployeeName.Contains(parameter.EmployeeName.Trim()) || parameter.EmployeeName == null || parameter.EmployeeName.Trim() == "") &&
                              (item.EmployeeCode.Contains(parameter.EmployeeCode.Trim()) || parameter.EmployeeCode == null || parameter.EmployeeCode.Trim() == "") &&
                              (item.EmployeeUnit.Contains(parameter.EmployeeUnit.Trim()) || parameter.EmployeeUnit == null || parameter.EmployeeUnit.Trim() == "") &&
                              (item.EmployeeBranch.Contains(parameter.EmployeeBranch.Trim()) || parameter.EmployeeBranch == null || parameter.EmployeeBranch.Trim() == "") &&
                              (item.EmployeePostionId == parameter.EmployeePostionId || parameter.EmployeePostionId == null) &&
                              (parameter.Month == null || item.Month == parameter.Month.Value) &&
                              (parameter.Year == null || item.Year == parameter.Year.Value) &&
                              (item.Type.Value == 2)
                              select new EmployeeMonthySalaryEntityModel(item)).ToList();

            var note = string.Empty;

            if (listResult.Count > 0)
            {
                note = context.FeatureNote.FirstOrDefault(f => f.FeatureId == listResult[0].CommonId)?.Note;
            }

            return new FindAssistantMonthySalaryResult
            {
                lstEmployeeMonthySalary = listResult,
                Status = true,
                Notes = StringHelper.ConvertNoteToObject(note),
            };

        }

        public ExportEmployeeSalaryResult ExportEmployeeSalary(ExportEmployeeSalaryParameter parameter)
        {
            string rootFolder = hostingEnvironment.WebRootPath + "\\ExcelTemplate";
            string fileName = @"ExcelExportEmployeeMonthySalaryTemplate.xlsx";
            FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
            byte[] data = null;
            var lstEmployeeMonthySalary = context.EmployeeMonthySalary.Where(w => parameter.lstEmpMonthySalary.Contains(w.EmployeeMonthySalaryId)).ToList();

            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                worksheet.InsertRow(8, lstEmployeeMonthySalary.Count);
                if (lstEmployeeMonthySalary.Count > 0)
                {

                    int rowIndex = 7;
                    int i = 0;
                    while (i < lstEmployeeMonthySalary.Count)
                    {
                        worksheet.Cells[rowIndex, 1].Value = lstEmployeeMonthySalary[i].EmployeeCode;
                        worksheet.Cells[rowIndex, 2].Value = lstEmployeeMonthySalary[i].EmployeeName;
                        worksheet.Cells[rowIndex, 3].Value = lstEmployeeMonthySalary[i].PostionName;
                        worksheet.Cells[rowIndex, 4].Value = lstEmployeeMonthySalary[i].EmployeeUnit;
                        worksheet.Cells[rowIndex, 5].Value = lstEmployeeMonthySalary[i].EmployeeBranch;
                        worksheet.Cells[rowIndex, 6].Value = lstEmployeeMonthySalary[i].BasedSalary;
                        worksheet.Cells[rowIndex, 7].Value = lstEmployeeMonthySalary[i].MonthlyWorkingDay;
                        worksheet.Cells[rowIndex, 8].Value = lstEmployeeMonthySalary[i].UnPaidLeaveDay;
                        worksheet.Cells[rowIndex, 9].Value = lstEmployeeMonthySalary[i].VacationDay;
                        worksheet.Cells[rowIndex, 10].Value = lstEmployeeMonthySalary[i].Overtime;
                        worksheet.Cells[rowIndex, 11].Value = lstEmployeeMonthySalary[i].ActualWorkingDay;
                        worksheet.Cells[rowIndex, 12].Value = lstEmployeeMonthySalary[i].ActualOfSalary;
                        worksheet.Cells[rowIndex, 13].Value = lstEmployeeMonthySalary[i].OvertimeOfSalary;
                        worksheet.Cells[rowIndex, 14].Value = lstEmployeeMonthySalary[i].EnrollmentSalary;
                        worksheet.Cells[rowIndex, 15].Value = lstEmployeeMonthySalary[i].RetentionSalary;
                        worksheet.Cells[rowIndex, 16].Value = lstEmployeeMonthySalary[i].FuelAllowance;
                        worksheet.Cells[rowIndex, 17].Value = lstEmployeeMonthySalary[i].PhoneAllowance;
                        worksheet.Cells[rowIndex, 18].Value = lstEmployeeMonthySalary[i].LunchAllowance;
                        worksheet.Cells[rowIndex, 19].Value = lstEmployeeMonthySalary[i].OtherAllowance;
                        worksheet.Cells[rowIndex, 20].Value = lstEmployeeMonthySalary[i].SocialInsuranceSalary;
                        //worksheet.Cells[rowIndex, 20].Value = lstEmployeeMonthySalary[i].SocialInsuranceCompanyPaid;
                        //worksheet.Cells[rowIndex, 21].Value = lstEmployeeMonthySalary[i].HealthInsuranceCompanyPaid;
                        //worksheet.Cells[rowIndex, 22].Value = lstEmployeeMonthySalary[i].UnemploymentinsuranceCompanyPaid;
                        worksheet.Cells[rowIndex, 21].Value = lstEmployeeMonthySalary[i].TotalInsuranceCompanyPaid;
                        //worksheet.Cells[rowIndex, 24].Value = lstEmployeeMonthySalary[i].SocialInsuranceEmployeePaid;
                        //worksheet.Cells[rowIndex, 25].Value = lstEmployeeMonthySalary[i].HealthInsuranceEmployeePaid;
                        //worksheet.Cells[rowIndex, 26].Value = lstEmployeeMonthySalary[i].UnemploymentinsuranceEmployeePaid;
                        worksheet.Cells[rowIndex, 22].Value = lstEmployeeMonthySalary[i].TotalInsuranceEmployeePaid;
                        worksheet.Cells[rowIndex, 23].Value = lstEmployeeMonthySalary[i].DesciplineAmount;
                        worksheet.Cells[rowIndex, 24].Value = lstEmployeeMonthySalary[i].ReductionAmount;
                        worksheet.Cells[rowIndex, 25].Value = lstEmployeeMonthySalary[i].AdditionalAmount;
                        worksheet.Cells[rowIndex, 26].Value = lstEmployeeMonthySalary[i].BankAccountCode;
                        worksheet.Cells[rowIndex, 27].Value = lstEmployeeMonthySalary[i].BankAccountName;
                        worksheet.Cells[rowIndex, 28].Value = lstEmployeeMonthySalary[i].BranchOfBank;
                        worksheet.Cells[rowIndex, 29].Value = lstEmployeeMonthySalary[i].TotalIncome;
                        worksheet.Cells[rowIndex, 30].Value = lstEmployeeMonthySalary[i].ActualPaid;
                        worksheet.Cells[rowIndex, 31].Value = lstEmployeeMonthySalary[i].Description;
                        rowIndex++;
                        i++;
                    }
                    string newFilePath = Path.Combine(rootFolder, @"ExportedExcel.xlsx");
                    package.SaveAs(new FileInfo(newFilePath));
                    data = File.ReadAllBytes(newFilePath);
                    File.Delete(newFilePath);
                    return new ExportEmployeeSalaryResult()
                    {
                        ExcelFile = data,
                        NameFile = "Thông tin lương nhân viên",
                        Message = "Success!",
                        Status = true
                    };
                }
                else
                {
                    return new ExportEmployeeSalaryResult()
                    {
                        ExcelFile = null,
                        NameFile = null,
                        Message = "Fail!",
                        Status = false
                    };

                }
            }

        }

        public ExportTeacherSalaryResult ExportTeacherSalary(ExportTeacherSalaryParameter parameter)
        {
            var listEmployeeMonthySalary = (from item in context.EmployeeMonthySalary
                                            where parameter.lstEmpMonthySalary.Contains(item.EmployeeMonthySalaryId)
                                            select new EmployeeMonthySalaryEntityModel(item)).ToList();

            var lstMonthAndYear = listEmployeeMonthySalary.Select(s => new { month = s.Month, year = s.Year }).ToList().Distinct();
            //Lấy ra danh sách các trung tâm có thể tồn tại trong tháng đó chi cho giảng viên
            var lstCenter = context.EmployeeTimesheet.Where(w => lstMonthAndYear.Select(s => s.month).Contains(w.Month.Value) && lstMonthAndYear.Select(s => s.year).Contains(w.Year.Value) && !string.IsNullOrEmpty(w.Center))
                .Select(s => new { center = s.Center }).ToList().Distinct();

            List<dynamic> lstResult = new List<dynamic>();

            listEmployeeMonthySalary.ForEach(empMonthySalary =>
            {

                var lstTimeSheetTeacher = context.EmployeeTimesheet.Where(w => w.Month == empMonthySalary.Month && w.Year == empMonthySalary.Year && w.EmployeeId == empMonthySalary.EmployeeId).ToList();

                decimal SumGioDay = 0;
                var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                var empInf = empMonthySalary;
                if (empInf != null)
                {
                    sampleObject.Add("EmployeeCode", empInf.EmployeeCode);
                    //sampleObject.Add("CommonId", empInf.CommonId);
                    sampleObject.Add("EmployeeName", empInf.EmployeeName);

                    lstCenter.ToList().ForEach(itemC =>
                    {
                        var GIODAY = lstTimeSheetTeacher.Where(w => w.Center == itemC.center).Select(s => new { GIODAY = (decimal?)s.ActualWorkingDay }).DefaultIfEmpty().FirstOrDefault();
                        SumGioDay = SumGioDay + (GIODAY != null ? GIODAY.GIODAY.Value : 0);
                        sampleObject.Add(itemC.center, GIODAY != null ? GIODAY.GIODAY.Value : 0);
                    });
                    sampleObject.Add("ActualWorkingDay", SumGioDay);
                    sampleObject.Add("BasedSalary", empInf.BasedSalary);
                    sampleObject.Add("AdditionalAmount", empInf.AdditionalAmount);
                    sampleObject.Add("ReductionAmount", empInf.ReductionAmount);
                    sampleObject.Add("ActualPaid", empInf.ActualPaid);
                    sampleObject.Add("Email", empInf.Email);
                    sampleObject.Add("Description", empInf.Description);
                    lstResult.Add(sampleObject);
                }

            });

            //Lấy ra danh sách các cột để hiển thị ngoài font-end
            List<KeyValuePair<string, string>> lstColumn = new List<KeyValuePair<string, string>>();
            if (lstResult.Count > 0)
            {
                foreach (var property in (IDictionary<String, Object>)lstResult[0])
                {
                    switch (property.Key)
                    {
                        case "EmployeeCode":
                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Mã nhân viên"));
                            break;
                        case "EmployeeName":
                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Tên nhân viên"));
                            break;
                        case "ActualWorkingDay":
                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Công thực tế"));
                            break;
                        case "BasedSalary":
                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Lương theo giờ"));
                            break;
                        case "AdditionalAmount":
                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Cộng thêm"));
                            break;
                        case "ReductionAmount":
                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Trừ"));
                            break;
                        case "ActualPaid":
                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Thực lĩnh"));
                            break;
                        case "Description":
                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, "Diễn giải"));
                            break;
                        default:
                            lstColumn.Add(new KeyValuePair<string, string>(property.Key, property.Key));
                            break;
                    }
                }

                string rootFolder = hostingEnvironment.WebRootPath + "\\ExcelTemplate";
                string fileName = @"ExcelExportTeacherMonthySalaryTemplate.xlsx";
                FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
                byte[] data = null;
                //var lstEmployeeMonthySalary = context.EmployeeMonthySalary.Where(w => parameter.lstEmpMonthySalary.Contains(w.EmployeeMonthySalaryId)).ToList();
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    worksheet.Cells[2, 3].Value = string.Format("BẢNG THÔNG TIN LƯƠNG GIẢNG VIÊN THÁNG {0}/{1}", lstMonthAndYear.Select(s => s.month).FirstOrDefault(), lstMonthAndYear.Select(s => s.year).FirstOrDefault());
                    //insert cột cho excell
                    worksheet.InsertRow(7, 1);
                    if (lstColumn.Count > 0)
                    {
                        int rowIndex = 6;
                        int i = 0;
                        while (i < lstColumn.Count)
                        {
                            var test = lstColumn[i].Value;
                            worksheet.Cells[rowIndex, i + 1].Value = lstColumn[i].Value;
                            i++;
                        }
                    }
                    ///Insert row
                    worksheet.InsertRow(8, lstResult.Count);
                    if (lstResult.Count > 0)
                    {

                        int rowIndex = 7;
                        int i = 0;
                        while (i < lstResult.Count)
                        {
                            for (int colIndex = 0; colIndex < lstColumn.Count; colIndex++)
                            {
                                var nameCol = lstColumn[colIndex].Key;
                                var item = (IDictionary<String, Object>)lstResult[i];
                                var io = item.Where(w => w.Key == nameCol).Select(s => s.Value).FirstOrDefault();
                                switch (nameCol)
                                {
                                    case "BasedSalary":
                                        worksheet.Cells[rowIndex, colIndex + 1].Style.Numberformat.Format = "#,##0.00";
                                        worksheet.Cells[rowIndex, colIndex + 1].Value = io;
                                        break;
                                    case "AdditionalAmount":
                                        worksheet.Cells[rowIndex, colIndex + 1].Style.Numberformat.Format = "#,##0.00";
                                        worksheet.Cells[rowIndex, colIndex + 1].Value = io;
                                        break;
                                    case "ReductionAmount":
                                        worksheet.Cells[rowIndex, colIndex + 1].Style.Numberformat.Format = "#,##0.00";
                                        worksheet.Cells[rowIndex, colIndex + 1].Value = io;
                                        break;
                                    case "ActualPaid":
                                        worksheet.Cells[rowIndex, colIndex + 1].Style.Numberformat.Format = "#,##0.00";
                                        worksheet.Cells[rowIndex, colIndex + 1].Value = io;
                                        break;
                                    default:
                                        worksheet.Cells[rowIndex, colIndex + 1].Value = io;
                                        break;
                                };
                            }
                            rowIndex++;
                            i++;
                        }
                        string newFilePath = Path.Combine(rootFolder, @"ExportedExcel.xlsx");
                        package.SaveAs(new FileInfo(newFilePath));
                        data = File.ReadAllBytes(newFilePath);
                        File.Delete(newFilePath);
                        return new ExportTeacherSalaryResult()
                        {
                            ExcelFile = data,
                            NameFile = "Thông tin lương giảng viên",
                            Message = "Success!",
                            Status = true
                        };
                    }
                    else
                    {
                        return new ExportTeacherSalaryResult()
                        {
                            ExcelFile = null,
                            NameFile = null,
                            Message = "Fail!",
                            Status = false
                        };
                    }
                }

            }
            else
            {
                return new ExportTeacherSalaryResult()
                {
                    ExcelFile = null,
                    NameFile = null,
                    Message = "Fail!",
                    Status = false
                };
            }
        }

        public ExportAssistantSalaryResult ExportAssistantSalary(ExportAssistantSalaryParameter parameter)
        {
            string rootFolder = hostingEnvironment.WebRootPath + "\\ExcelTemplate";
            string fileName = @"ExcelExportAssistantMonthySalaryTemplate.xlsx";
            FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
            byte[] data = null;
            var lstEmployeeMonthySalary = context.EmployeeMonthySalary.Where(w => parameter.lstEmpMonthySalary.Contains(w.EmployeeMonthySalaryId)).ToList();
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                worksheet.Cells[2, 3].Value = string.Format("Bảng Lương Tháng {0}", lstEmployeeMonthySalary[0].Month);

                worksheet.InsertRow(8, lstEmployeeMonthySalary.Count);
                if (lstEmployeeMonthySalary.Count > 0)
                {

                    int rowIndex = 7;
                    int i = 0;
                    while (i < lstEmployeeMonthySalary.Count)
                    {
                        worksheet.Cells[rowIndex, 1].Value = lstEmployeeMonthySalary[i].EmployeeCode;
                        worksheet.Cells[rowIndex, 2].Value = lstEmployeeMonthySalary[i].EmployeeName;
                        worksheet.Cells[rowIndex, 3].Value = lstEmployeeMonthySalary[i].PostionName;
                        worksheet.Cells[rowIndex, 4].Value = lstEmployeeMonthySalary[i].EmployeeUnit;

                        worksheet.Cells[rowIndex, 5].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[rowIndex, 5].Value = lstEmployeeMonthySalary[i].BasedSalary;

                        worksheet.Cells[rowIndex, 6].Value = lstEmployeeMonthySalary[i].ActualWorkingDay;

                        worksheet.Cells[rowIndex, 7].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[rowIndex, 7].Value = lstEmployeeMonthySalary[i].ReductionAmount;

                        worksheet.Cells[rowIndex, 8].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[rowIndex, 8].Value = lstEmployeeMonthySalary[i].AdditionalAmount;

                        worksheet.Cells[rowIndex, 9].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[rowIndex, 9].Value = lstEmployeeMonthySalary[i].ActualPaid;
                        //worksheet.Cells[rowIndex, 10].Value = lstEmployeeMonthySalary[i].Email;
                        worksheet.Cells[rowIndex, 10].Value = lstEmployeeMonthySalary[i].Description;
                        rowIndex++;
                        i++;
                    }
                    string newFilePath = Path.Combine(rootFolder, @"ExportedExcel.xlsx");
                    package.SaveAs(new FileInfo(newFilePath));
                    data = File.ReadAllBytes(newFilePath);
                    File.Delete(newFilePath);
                    return new ExportAssistantSalaryResult()
                    {
                        ExcelFile = data,
                        NameFile = "Thông tin lương trợ giảng",
                        Message = "Success!",
                        Status = true
                    };
                }
                else
                {
                    return new ExportAssistantSalaryResult()
                    {
                        ExcelFile = null,
                        NameFile = null,
                        Message = "Fail!",
                        Status = false
                    };
                }
            }
        }

        public GetEmployeeSalaryStatusResult GetEmployeeSalaryStatus(GetEmployeeSalaryStatusParameter parameter)
        {
            var empSalaryItemStatus = context.EmployeeMonthySalary.FirstOrDefault(ems => ems.CommonId == parameter.CommonId);
            Guid empSalaryItemStatusId = Guid.Empty;
            if (empSalaryItemStatus != null)
            {
                empSalaryItemStatusId = empSalaryItemStatus.StatusId.Value;
            }
            var waitingforApprove = context.Category.FirstOrDefault(c => c.CategoryCode == "WaitForAp");
            var approved = context.Category.FirstOrDefault(c => c.CategoryCode == "Approved");
            var rejected = context.Category.FirstOrDefault(c => c.CategoryCode == "Rejected");
            var featureprogress = context.FeatureWorkFlowProgress.FirstOrDefault(fwp => fwp.ApprovalObjectId == parameter.CommonId);

            var result = new GetEmployeeSalaryStatusResult()
            {
                Status = true,
                IsApproved = empSalaryItemStatusId == approved.CategoryId,
                IsInApprovalProgress = empSalaryItemStatusId == waitingforApprove.CategoryId,
                IsRejected = empSalaryItemStatusId == rejected.CategoryId,
                ApproverId = featureprogress != null ? featureprogress.ApproverPersonId : Guid.Empty,
                PositionId = featureprogress != null ? featureprogress.ApproverPositionId : Guid.Empty
            };

            result.StatusName = result.IsInApprovalProgress ? waitingforApprove.CategoryName :
                (result.IsApproved ? approved.CategoryName :
                (result.IsRejected ? rejected.CategoryName :
                (parameter.CommonId == Guid.Empty ? "Chưa có bảng lương" : "Chưa gửi phê duyệt")));
            return result;
        }

    }
    public class GetTeacherSummary
    {
        public string Email { get; set; }
        public string Center { get; set; }
        public int GIODAY { get; set; }
    }
}
