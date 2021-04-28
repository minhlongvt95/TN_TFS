using TN.TNM.DataAccess.Messages.Parameters.Admin.Company;
using TN.TNM.DataAccess.Messages.Parameters.CompanyConfig;
using TN.TNM.DataAccess.Messages.Results.Admin.Category;
using TN.TNM.DataAccess.Messages.Results.Admin.Company;
using TN.TNM.DataAccess.Messages.Results.CompanyConfig;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface ICompanyDataAccess
    {
        /// <summary>
        /// Get info from Company table
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        GetAllCompanyResult GetAllCompany(GetAllCompanyParameter parameter);
        /// <summary>
        /// GetCompanyConfig
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        GetCompanyConfigResults GetCompanyConfig(GetCompanyConfigParameter parameter);
        /// <summary>
        /// GetCompanyConfig
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        EditCompanyConfigResults EditCompanyConfig(EditCompanyConfigParameter parameter);
        GetAllSystemParameterResult GetAllSystemParameter(GetAllSystemParameterParameter parameter);
        ChangeSystemParameterResult ChangeSystemParameter(ChangeSystemParameterParameter parameter);
    }
}
