using TN.TNM.BusinessLogic.Models.Folder;
using TN.TNM.DataAccess.Messages.Parameters.Folder;

namespace TN.TNM.BusinessLogic.Messages.Requests.Folder
{
    public class CreateFolderRequest : BaseRequest<CreateFolderParameter>
    {
        public FolderModel FolderParent { get; set; }
        public string FolderName { get; set; }

        public override CreateFolderParameter ToParameter()
        {
            return new CreateFolderParameter()
            {
                FolderName = FolderName,
                FolderParent = FolderParent.ToEntityModel(),
                UserId = UserId
            };
        }
    }
}
