using TN.TNM.DataAccess.Messages.Parameters.Admin.Ward;
using TN.TNM.DataAccess.Messages.Results.Admin.Ward;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IWardDataAccess
    {
        GetAllWardByDistrictIdResult GetAllWardByDistrictId(GetAllWardByDistrictIdParameter parameter);
    }
}
