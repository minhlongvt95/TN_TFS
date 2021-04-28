using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.Employee;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IEmployeeSalaryDataAccess
    {
        EmployeeTimeSheetImportResult EmployeeTimeSheetImport(EmployeeTimeSheetImportParameter parameter);
        GetEmployeeSalaryByEmpIdResult GetEmployeeSalaryByEmpId(GetEmployeeSalaryByEmpIdParameter parameter);
        CreateEmployeeSalaryResult CreateEmployeeSalary(CreateEmployeeSalaryParameter parameter);
        EmployeeSalaryHandmadeResult EmployeeSalaryHandmadeImport(EmployeeSalaryHandmadeParameter parameter);
        DownloadEmployeeTimeSheetTemplateResult DownloadEmployeeTimeSheetTemplate(DownloadEmployeeTimeSheetTemplateParameter parameter);
        FindEmployeeMonthySalaryResult FindEmployeeMonthySalary(FindEmployeeMonthySalaryParameter parameter);
        GetTeacherSalaryResult GetTeacherSalary(GetTeacherSalaryParameter parameter);
        TeacherSalaryHandmadeResult TeacherSalaryHandmadeImport(TeacherSalaryHandmadeParameter parameter);
        FindTeacherMonthySalaryResult FindTeacherMonthySalary(FindTeacherMonthySalaryParameter parameter);
        ExportAssistantResult ExportAssistant(ExportAssistantParameter parameter);
        AssistantSalaryHandmadeResult AssistantSalaryHandmadeImport(AssistantSalaryHandmadeParameter parameter);
        FindAssistantMonthySalaryResult FindAssistantMonthySalary(FindAssistantMonthySalaryParameter parameter);
        ExportEmployeeSalaryResult ExportEmployeeSalary(ExportEmployeeSalaryParameter parameter);
        ExportTeacherSalaryResult ExportTeacherSalary(ExportTeacherSalaryParameter parameter);
        ExportAssistantSalaryResult ExportAssistantSalary(ExportAssistantSalaryParameter parameter);
        GetEmployeeSalaryStatusResult GetEmployeeSalaryStatus(GetEmployeeSalaryStatusParameter parameter);
    }
}
