using TN.TNM.DataAccess.Messages.Parameters.Admin.Position;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Position
{
    public class GetAllPositionRequest : BaseRequest<GetAllPositionParameter>
    {
        public override GetAllPositionParameter ToParameter()
        {
            return new GetAllPositionParameter()
            {

            };
        }
    }
}
