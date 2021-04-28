namespace TN.TNM.DataAccess.Messages.Results.Quote
{
    public class DownloadTemplateProductResult : BaseResult
    {
        public byte[] TemplateExcel { get; set; }
        public string FileName { get; set; }
    }
}
