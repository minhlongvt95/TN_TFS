namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class DownloadTemplateLeadResponse:BaseResponse
    {
        public byte[] ExcelFile { get; set; }
        public string NameFile { get; set; }

    }
}
