using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetEmployeeWithNotificationPermisisonRequest : BaseRequest<GetEmployeeWithNotificationPermisisonParameter>
    {
        public override GetEmployeeWithNotificationPermisisonParameter ToParameter()
        {
            return new GetEmployeeWithNotificationPermisisonParameter() {
                UserId = UserId
            };
        }
    }
}
