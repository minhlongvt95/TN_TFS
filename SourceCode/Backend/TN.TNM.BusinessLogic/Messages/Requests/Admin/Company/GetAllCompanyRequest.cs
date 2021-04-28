using TN.TNM.BusinessLogic.Models.Company;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Company;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Company
{
    public class GetAllCompanyRequest : BaseRequest<GetAllCompanyParameter>
    {
        public CompanyModel Company { get; set; }
        public override GetAllCompanyParameter ToParameter() => new GetAllCompanyParameter
        {
            
        };
    }
}
