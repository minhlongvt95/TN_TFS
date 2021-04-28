using TN.TNM.DataAccess.Messages.Parameters.Admin.Company;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Company
{
    public class GetAllSystemParameterRequest : BaseRequest<GetAllSystemParameterParameter>
    {
        public override GetAllSystemParameterParameter ToParameter() => new GetAllSystemParameterParameter
        {
            UserId = UserId
        };
    }
}
