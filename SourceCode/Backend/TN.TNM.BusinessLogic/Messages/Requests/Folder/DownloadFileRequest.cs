using System;
using TN.TNM.BusinessLogic.Models.Folder;
using TN.TNM.DataAccess.Messages.Parameters.Folder;

namespace TN.TNM.BusinessLogic.Messages.Requests.Folder
{
    public class DownloadFileRequest : BaseRequest<DownloadFileParameter>
    {
        public Guid FileInFolderId { get; set; }
        public override DownloadFileParameter ToParameter()
        {
            return new DownloadFileParameter()
            {
                FileInFolderId = FileInFolderId
            };
        }
    }
}
