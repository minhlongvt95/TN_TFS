using TN.TNM.BusinessLogic.Models.Folder;

namespace TN.TNM.BusinessLogic.Messages.Responses.Folder
{
    public class CreateFolderResponse : BaseResponse
    {
        public FolderModel Folder { get; set; }
    }
}
