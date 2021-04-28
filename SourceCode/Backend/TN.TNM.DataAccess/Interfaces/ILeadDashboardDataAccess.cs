using TN.TNM.DataAccess.Messages.Parameters.Lead;
using TN.TNM.DataAccess.Messages.Results.Lead;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface ILeadDashboardDataAccess
    {
        /// <summary>
        /// Lay danh sach top lead cho lead dashboard
        /// </summary>
        /// <param name="paramater"></param>
        /// <returns></returns>
        GetTopLeadResult GetTopLead(GetTopLeadParameter paramater);

        /// <summary>
        /// Lay ty le chuyen doi
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        GetConvertRateResult GetConvertRate(GetConvertRateParameter parameter);
        GetRequirementRateResult GetRequirementRate(GetRequirementRateParemeter parameter);
        GetPotentialRateResult GetPotentialRate(GetPotentialRateParameter parameter);
        GetDataLeadDashboardResult GetDataLeadDashboard(GetDataLeadDashboardParameter parameter);
    }
}
