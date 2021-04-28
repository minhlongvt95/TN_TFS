using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Folder;

namespace TN.TNM.BusinessLogic.Messages.Responses.Folder
{
    public class GetAllFolderActiveResponse:BaseResponse
    {
        public List<FolderModel> ListFolderActive { get; set; }
    }
}
