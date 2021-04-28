namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class DownloadTemplateProductResponse : BaseResponse
    {
        public byte[] TemplateExcel { get; set; }
        public string FileName { get; set; }
    }
}
