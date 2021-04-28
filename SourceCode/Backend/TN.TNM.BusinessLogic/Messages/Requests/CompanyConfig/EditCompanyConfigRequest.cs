using TN.TNM.BusinessLogic.Models.Company;
using TN.TNM.DataAccess.Messages.Parameters.CompanyConfig;

namespace TN.TNM.BusinessLogic.Messages.Requests.CompanyConfig
{
    public class EditCompanyConfigRequest : BaseRequest<EditCompanyConfigParameter>
    {
        public CompanyConfigModel CompanyConfigurationObject { get; set; }

        public override EditCompanyConfigParameter ToParameter()
        {
            return new EditCompanyConfigParameter
            {
                CompanyConfigurationObject=this.CompanyConfigurationObject.ToEntity(),
                UserId=this.UserId
            };
        }
    }
}
