using TN.TNM.BusinessLogic.Messages.Requests.Admin.Ward;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Ward;

namespace TN.TNM.BusinessLogic.Interfaces.Admin.Ward
{
    public interface IWard
    {
        GetAllWardByDistrictIdResponse GetAllWardByDistrictId(GetAllWardByDistrictIdRequest request);
    }
}
