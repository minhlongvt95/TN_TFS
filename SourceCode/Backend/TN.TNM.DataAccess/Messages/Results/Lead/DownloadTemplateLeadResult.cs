namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class DownloadTemplateLeadResult:BaseResult
    {
        public byte[] ExcelFile { get; set; }
        public string NameFile { get; set; }
    }
}
