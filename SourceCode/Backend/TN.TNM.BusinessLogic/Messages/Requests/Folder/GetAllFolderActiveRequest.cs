using TN.TNM.DataAccess.Messages.Parameters.Folder;

namespace TN.TNM.BusinessLogic.Messages.Requests.Folder
{
    public class GetAllFolderActiveRequest : BaseRequest<GetAllFolderActiveParameter>
    {
        public override GetAllFolderActiveParameter ToParameter()
        {
            return new GetAllFolderActiveParameter()
            {
                UserId = UserId
            };
        }
    }
}
