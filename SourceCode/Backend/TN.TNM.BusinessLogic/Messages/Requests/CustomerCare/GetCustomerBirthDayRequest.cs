using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class GetCustomerBirthDayRequest : BaseRequest<GetCustomerBirthDayParameter>
    {

        public override GetCustomerBirthDayParameter ToParameter()
        {
            return new GetCustomerBirthDayParameter
            {
            };
        }
    }
}
