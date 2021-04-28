using TN.TNM.BusinessLogic.Models.Folder;
using TN.TNM.DataAccess.Messages.Parameters.Folder;

namespace TN.TNM.BusinessLogic.Messages.Requests.Folder
{
    public class DeleteFolderRequest : BaseRequest<DeleteFolderParameter>
    {
        public FolderModel Folder { get; set; }

        public override DeleteFolderParameter ToParameter()
        {
            return new DeleteFolderParameter
            {
                Folder = Folder.ToEntityModel(),
                UserId = UserId
            };
        }
    }
}
