using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Folder;

namespace TN.TNM.BusinessLogic.Messages.Responses.Folder
{
    public class UploadFileResponse : BaseResponse
    {
        public List<FileInFolderModel> ListFileInFolder { get; set; }
    }
}
