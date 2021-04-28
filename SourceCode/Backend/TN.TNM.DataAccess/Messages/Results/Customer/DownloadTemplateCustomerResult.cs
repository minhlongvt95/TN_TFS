namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class DownloadTemplateCustomerResult:BaseResult
    {
        public byte[] ExcelFile { get; set; }
        public string NameFile { get; set; }
    }
}
