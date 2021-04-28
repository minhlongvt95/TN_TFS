using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetEmployeeByPositionCodeRequest : BaseRequest<GetEmployeeByPositionCodeParameter>
    {
        public string PositionCode { get; set; }
        public override GetEmployeeByPositionCodeParameter ToParameter()
        {
            return new GetEmployeeByPositionCodeParameter() {
                UserId = UserId,
                PositionCode = PositionCode
            };
        }
    }
}
