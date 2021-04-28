using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Folder;

namespace TN.TNM.BusinessLogic.Messages.Responses.Folder
{
    public class GetAllFolderDefaultNotActiveResponse : BaseResponse
    {
        public List<FolderModel> ListFolderDefault { get; set; }
    }
}
