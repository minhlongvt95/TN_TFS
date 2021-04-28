using TN.TNM.DataAccess.Messages.Parameters.Admin.Organization;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Organization
{
    public class GetFinancialindependenceOrgRequest : BaseRequest<GetFinancialindependenceOrgParameter>
    {
        public override GetFinancialindependenceOrgParameter ToParameter()
        {
            return new GetFinancialindependenceOrgParameter() {
                UserId = UserId
            };
        }
    }
}
