using TN.TNM.BusinessLogic.Messages.Requests.Admin.Province;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Province;

namespace TN.TNM.BusinessLogic.Interfaces.Admin.Province
{
    public interface IProvince
    {
        /// <summary>
        /// GetAllProvince
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetAllProvinceResponse GetAllProvince(GetAllProvinceRequest request);
    }
}
