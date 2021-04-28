using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetAllEmployeeRequestRequest : BaseRequest<GetAllEmployeeRequestParameter>
    {
        public override GetAllEmployeeRequestParameter ToParameter()
        {
            return new GetAllEmployeeRequestParameter()
            {
                UserId = UserId
            };
        }
        
    }
}
