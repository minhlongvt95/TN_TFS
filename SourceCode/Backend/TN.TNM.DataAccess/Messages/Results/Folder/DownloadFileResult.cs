namespace TN.TNM.DataAccess.Messages.Results.Folder
{
    public class DownloadFileResult : BaseResult
    {
        public byte[] FileAsBase64 { get; set; }
        public string FileType { get; set; }
    }
}
