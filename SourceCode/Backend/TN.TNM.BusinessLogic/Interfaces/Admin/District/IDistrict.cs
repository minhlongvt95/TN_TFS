using TN.TNM.BusinessLogic.Messages.Requests.Admin.District;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.District;

namespace TN.TNM.BusinessLogic.Interfaces.Admin.District
{
    public interface IDistrict
    {
        GetAllDistrictByProvinceIdResponse GetAllDistrictByProvinceId(GetAllDistrictByProvinceIdRequest request);
    }
}
