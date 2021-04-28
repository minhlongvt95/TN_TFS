using TN.TNM.BusinessLogic.Messages.Requests.Admin.Company;
using TN.TNM.BusinessLogic.Messages.Requests.CompanyConfig;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Company;
using TN.TNM.BusinessLogic.Messages.Responses.CompanyConfig;

namespace TN.TNM.BusinessLogic.Interfaces.Admin.Company
{
    public interface ICompany
    {
        /// <summary>
        /// Get info from Company table
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns></returns>
        GetAllCompanyResponse GetAllCompany(GetAllCompanyRequest request);
        /// <summary>
        /// GetCompanyConfig
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetCompanyConfigResponse GetCompanyConfig(GetCompanyConfigRequest request);
        /// <summary>
        /// EditCompanyConfig
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        EditCompanyConfigResponse EditCompanyConfig(EditCompanyConfigRequest request);
        GetAllSystemParameterResponse GetAllSystemParameter(GetAllSystemParameterRequest request);
        ChangeSystemParameterResponse ChangeSystemParameter(ChangeSystemParameterRequest request);
    }
}
