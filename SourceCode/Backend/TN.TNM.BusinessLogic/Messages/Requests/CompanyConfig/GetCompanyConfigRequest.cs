using TN.TNM.DataAccess.Messages.Parameters.CompanyConfig;

namespace TN.TNM.BusinessLogic.Messages.Requests.CompanyConfig
{
    public class GetCompanyConfigRequest : BaseRequest<GetCompanyConfigParameter>
    {
        public override GetCompanyConfigParameter ToParameter()
        {
            return new GetCompanyConfigParameter
            {
            };
        }
    }
}
