using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetStatisticCustomerForDashboardRequest : BaseRequest<GetStatisticCustomerForDashboardParameter>
    {
        public string KeyName { get; set; }
        public override GetStatisticCustomerForDashboardParameter ToParameter()
        {
            return new GetStatisticCustomerForDashboardParameter()
            {
                KeyName = KeyName,
                UserId = UserId 
            };
        }
    }
}
