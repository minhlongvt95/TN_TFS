using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetTopLeadRequest : BaseRequest<GetTopLeadParameter>
    {
        public int Count { get; set; }
        public string StatusCode { get; set; }
        public override GetTopLeadParameter ToParameter() => new GetTopLeadParameter
        {
            Count = this.Count,
            StatusCode = this.StatusCode,
            UserId = UserId
        };
    }
}
