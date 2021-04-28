using TN.TNM.DataAccess.Messages.Parameters.Admin.Country;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Country
{
    public class GetAllCountryRequest : BaseRequest<GetAllCountryParameter>
    {
        public override GetAllCountryParameter ToParameter()
        {
            return new GetAllCountryParameter()
            {
                UserId = UserId
            };
        }
    }
}
