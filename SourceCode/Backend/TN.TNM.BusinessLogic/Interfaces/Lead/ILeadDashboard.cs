using TN.TNM.BusinessLogic.Messages.Requests.Lead;
using TN.TNM.BusinessLogic.Messages.Responses.Leads;

namespace TN.TNM.BusinessLogic.Interfaces.Lead
{
    /// <summary>
    /// TODO: Thuc hien cac chung nang lay du lieu lead dashboard
    /// 
    /// 1. GetTopLead: tim kiem lead theo cac tieu chi    
    /// 2. ...
    /// 
    /// Author: thanhhh@tringhiatech.vn
    /// Date: 25/06/2018
    /// </summary>
    public interface ILeadDashboard
    {
        /// <summary>
        /// TODO: Lay top lead theo cac tieu chi...
        /// ....
        ///  
        /// </summary>
        /// <param name="request">Chua cac parameter truyen vao</param>        
        /// <returns>Danh sach lead thoa man dieu kien</returns>
        GetTopLeadResponse GetTopLead(GetTopLeadRequest request);

        /// <summary>
        /// Lay ty le chuyen doi Lead
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetConvertRateResponse GetConvertRate(GetConvertRateRequest request);

        /// <summary>
        /// Lay Tỉ lệ lead theo nhu cầu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetRequirementRateResponse GetRequirementRate(GetRequirementRateRequest request);
        /// <summary>
        /// Lay Tỉ lệ lead theo mức độ tiềm năng
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetPotentialRateResponse GetPotentialRate(GetPotentialRateRequest request);
        GetDataLeadDashboardResponse GetDataLeadDashboard(GetDataLeadDashboardRequest request);

    }
}
