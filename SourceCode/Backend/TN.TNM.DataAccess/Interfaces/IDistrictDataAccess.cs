using TN.TNM.DataAccess.Messages.Parameters.Admin.District;
using TN.TNM.DataAccess.Messages.Results.Admin.District;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IDistrictDataAccess
    {
        GetAllDistrictByProvinceIdResult GetAllDistrictByProvinceId(GetAllDistrictByProvinceIdParameter parameter);
    }
}
