using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetEmployeeManagerRequest : BaseRequest<GetEmployeeManagerParameter>
    {
        public override GetEmployeeManagerParameter ToParameter()
        {
            return new GetEmployeeManagerParameter() {
                UserId = UserId
            };
        }
    }
}
