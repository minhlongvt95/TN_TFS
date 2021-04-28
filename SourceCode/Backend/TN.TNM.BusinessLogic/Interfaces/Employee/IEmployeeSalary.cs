using TN.TNM.BusinessLogic.Messages.Requests.Employee;
using TN.TNM.BusinessLogic.Messages.Responses.Employee;

namespace TN.TNM.BusinessLogic.Interfaces.Employee
{
    public interface IEmployeeSalary
    {
        EmployeeTimeSheetImportResponse EmployeeTimeSheetImport(EmployeeTimeSheetImportRequest request);
        GetEmployeeSalaryByEmpIdResponse GetEmployeeSalaryByEmpId(GetEmployeeSalaryByEmpIdRequest request);
        CreateEmployeeSalaryResponse CreateEmployeeSalary(CreateEmployeeSalaryRequest request);
        EmployeeSalaryHandmadeResponse EmployeeSalaryHandmadeImport(EmployeeSalaryHandmadeRequest request);
        DownloadEmployeeTimeSheetTemplateResponse DownloadEmployeeTimeSheetTemplate(DownloadEmployeeTimeSheetTemplateRequest request);
        FindEmployeeMonthySalaryResponse FindEmployeeMonthySalary(FindEmployeeMonthySalaryRequest request);
        GetTeacherSalaryResponse GetTeacherSalary(GetTeacherSalaryRequest request);
        TeacherSalaryHandmadeResponse TeacherSalaryHandmadeImport(TeacherSalaryHandmadeRequest request);
        FindTeacherMonthySalaryResponse FindTeacherMonthySalary(FindTeacherMonthySalaryRequest request);
        ExportAssistantResponse ExportAssistant(ExportAssistantRequest request);
        AssistantSalaryHandmadeResponse AssistantSalaryHandmadeImport(AssistantSalaryHandmadeRequest request);
        FindAssistantMonthySalaryResponse FindAssistantMonthySalary(FindAssistantMonthySalaryRequest request);
        ExportEmployeeSalaryResponse ExportEmployeeSalary(ExportEmployeeSalaryRequest request);
        ExportTeacherSalaryResponse ExportTeacherSalary(ExportTeacherSalaryRequest request);
        ExportAssistantSalaryResponse ExportAssistantSalary(ExportAssistantSalaryRequest request);
        GetEmployeeSalaryStatusResponse GetEmployeeSalaryStatus(GetEmployeeSalaryStatusRequest request);
    }

}
