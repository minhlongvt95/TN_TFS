namespace TN.TNM.DataAccess.Messages.Results.Document
{
    public class DownloadDocumentByIdResult:BaseResult
    {
        public byte[] ExcelFile { get; set; }
        public string NameFile { get; set; }
    }
}
