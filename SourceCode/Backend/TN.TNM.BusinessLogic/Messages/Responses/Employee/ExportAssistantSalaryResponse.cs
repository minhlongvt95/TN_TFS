namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class ExportAssistantSalaryResponse:BaseResponse
    {
        public byte[] ExcelFile { get; set; }
        public string NameFile { get; set; }

    }
}
