using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetAllEmpAccountRequest : BaseRequest<GetAllEmpAccountParameter>
    {
        public override GetAllEmpAccountParameter ToParameter()
        {
            return new GetAllEmpAccountParameter();
        }
    }
}
