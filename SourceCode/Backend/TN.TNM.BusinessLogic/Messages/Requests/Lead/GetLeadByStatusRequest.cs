using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetLeadByStatusRequest : BaseRequest<GetLeadByStatusParameter>
    {
        public string StatusName { get; set; }

        public override GetLeadByStatusParameter ToParameter()
        {
            return new GetLeadByStatusParameter()
            {
                StatusName = StatusName,
                UserId = UserId
            };
        }
    }
}
