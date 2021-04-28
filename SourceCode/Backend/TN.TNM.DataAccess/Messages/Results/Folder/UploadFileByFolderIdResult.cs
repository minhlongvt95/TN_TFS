using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Results.Folder
{
    public class UploadFileByFolderIdResult : BaseResult
    {
        public List<FileInFolderEntityModel> ListFileInFolder { get; set; }
    }
}
