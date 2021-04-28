namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class ExportTeacherSalaryResult:BaseResult
    {
        public byte[] ExcelFile { get; set; }
        public string NameFile { get; set; }

    }
}
