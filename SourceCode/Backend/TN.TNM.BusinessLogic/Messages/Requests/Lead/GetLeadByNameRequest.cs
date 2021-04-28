using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetLeadByNameRequest : BaseRequest<GetLeadByNameParameter>
    {
        public string Name { get; set; }
        public override GetLeadByNameParameter ToParameter()
        {
            return new GetLeadByNameParameter()
            {
                Name = Name,
                UserId = UserId
            };
        }
    }
}
