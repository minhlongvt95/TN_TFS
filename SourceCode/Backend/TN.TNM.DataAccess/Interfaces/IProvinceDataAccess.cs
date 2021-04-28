using TN.TNM.DataAccess.Messages.Parameters.Admin.Province;
using TN.TNM.DataAccess.Messages.Results.Admin.Province;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IProvinceDataAccess
    {
        /// <summary>
        /// Get all Province
        /// </summary>
        /// <param name="parameter">parameter</param>
        /// <returns></returns>
        GetAllProvinceResult GetAllProvince(GetAllProvinceParameter parameter);
    }
}
