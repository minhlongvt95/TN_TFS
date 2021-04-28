using TN.TNM.DataAccess.Messages.Parameters.Admin.CustomerServiceLevel;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.CustomerServiceLevel
{
    public class GetConfigCustomerServiceLevelRequest : BaseRequest<GetConfigCustomerServiceLevelParameter>
    {
        public override GetConfigCustomerServiceLevelParameter ToParameter()
        {
            return new GetConfigCustomerServiceLevelParameter
            {
            };
        }
    }
}
