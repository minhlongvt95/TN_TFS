using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class CheckAdminLoginRequest : BaseRequest<CheckAdminLoginParameter>
    {
        public override CheckAdminLoginParameter ToParameter()
        {
            return new CheckAdminLoginParameter()
            {
                UserId = UserId
            };
        }
    }
}
