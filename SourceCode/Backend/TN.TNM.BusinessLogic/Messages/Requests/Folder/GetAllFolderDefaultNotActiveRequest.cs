using TN.TNM.DataAccess.Messages.Parameters.Folder;

namespace TN.TNM.BusinessLogic.Messages.Requests.Folder
{
    public class GetAllFolderDefaultNotActiveRequest : BaseRequest<GetAllFolderDefaultNotActiveParameter>
    {
        public override GetAllFolderDefaultNotActiveParameter ToParameter()
        {
            return new GetAllFolderDefaultNotActiveParameter()
            {
                UserId = UserId
            };
        }
    }
}
