using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetPersonInChargeRequest : BaseRequest<GetPersonInChargeParameter>
    {
        public override GetPersonInChargeParameter ToParameter()
        {
            return new GetPersonInChargeParameter()
            {
                UserId = UserId,
            };
        }
    }
}
