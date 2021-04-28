using System;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Parameters.Folder
{
    public class DownloadFileParameter : BaseParameter
    {
        public Guid FileInFolderId { get; set; }
    }
}
