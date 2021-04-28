using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetAllEmployeeRequest : BaseRequest<GetAllEmployeeParameter>
    {
        public override GetAllEmployeeParameter ToParameter()
        {
            return new GetAllEmployeeParameter()
            {
                
            };
        }
        
    }
}
