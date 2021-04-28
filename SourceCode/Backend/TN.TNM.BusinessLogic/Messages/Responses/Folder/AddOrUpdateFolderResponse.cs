using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Folder;

namespace TN.TNM.BusinessLogic.Messages.Responses.Folder
{
    public class AddOrUpdateFolderResponse:BaseResponse
    {
        public List<FolderModel> ListFolder { get; set; }
    }
}
