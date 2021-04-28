namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class DownloadTemplateCustomerResponse:BaseResponse
    {
        public byte[] ExcelFile { get; set; }
        public string NameFile { get; set; }
    }
}
