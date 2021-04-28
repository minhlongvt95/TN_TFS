using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Results.Folder
{
    public class UploadFileResult : BaseResult
    {
        public List<FileInFolderEntityModel> ListFileInFolder { get; set; }
    }
}
