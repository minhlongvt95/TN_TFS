namespace TN.TNM.BusinessLogic.Messages.Responses.File
{
    public class DownloadFileResponse : BaseResponse
    {
        public byte[] FileAsBase64 { get; set; }
        public string FileType { get; set; }
    }
}
