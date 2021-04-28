using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetPotentialRateRequest: BaseRequest<GetPotentialRateParameter>
    {
        public int Count { get; set; }
        public string Type { get; set; }
        public override GetPotentialRateParameter ToParameter() => new GetPotentialRateParameter
        {
            UserId = UserId
        };
    }
    
}
