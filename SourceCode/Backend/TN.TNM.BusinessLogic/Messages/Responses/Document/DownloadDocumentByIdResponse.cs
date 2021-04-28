namespace TN.TNM.BusinessLogic.Messages.Responses.Document
{
    public class DownloadDocumentByIdResponse:BaseResponse
    {
        public byte[] ExcelFile { get; set; }
        public string NameFile { get; set; }

    }
}
