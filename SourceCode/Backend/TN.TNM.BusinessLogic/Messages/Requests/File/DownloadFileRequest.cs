using TN.TNM.DataAccess.Messages.Parameters.File;

namespace TN.TNM.BusinessLogic.Messages.Requests.File
{
    public class DownloadFileRequest : BaseRequest<DownloadFileParameter>
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public override DownloadFileParameter ToParameter()
        {
            return new DownloadFileParameter()
            {
                FileName = FileName,
                FileUrl = FileUrl
            };
        }
    }
}
