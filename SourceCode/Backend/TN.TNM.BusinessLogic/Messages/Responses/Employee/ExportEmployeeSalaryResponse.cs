namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class ExportEmployeeSalaryResponse:BaseResponse
    {
        public byte[] ExcelFile { get; set; }
        public string NameFile { get; set; }

    }
}
