using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
   public class GetRequirementRateRequest : BaseRequest<GetRequirementRateParemeter>
    {
        public int Count { get; set; }
        public string Type { get; set; }
        public override GetRequirementRateParemeter ToParameter() => new GetRequirementRateParemeter
        {
            UserId = UserId
        };
    }

}
