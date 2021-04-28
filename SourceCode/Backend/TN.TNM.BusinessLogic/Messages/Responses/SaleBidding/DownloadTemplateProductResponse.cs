namespace TN.TNM.BusinessLogic.Messages.Responses.SaleBidding
{
    public class DownloadTemplateProductResponse : BaseResponse
    {
        public byte[] TemplateExcel { get; set; }
        public string FileName { get; set; }
    }
}
