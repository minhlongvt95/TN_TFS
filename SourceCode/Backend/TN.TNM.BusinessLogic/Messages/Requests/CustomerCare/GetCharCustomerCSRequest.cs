using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class GetCharCustomerCSRequest : BaseRequest<GetCharCustomerCSParameter>
    {

        public override GetCharCustomerCSParameter ToParameter()
        {
            return new GetCharCustomerCSParameter
            {
            };
        }
    }
}
