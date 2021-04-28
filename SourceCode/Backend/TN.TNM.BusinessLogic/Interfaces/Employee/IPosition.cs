using TN.TNM.BusinessLogic.Messages.Requests.Admin.Position;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Position;

namespace TN.TNM.BusinessLogic.Interfaces.Employee
{
    public interface IPosition
    {
        GetAllPositionResponse GetAllPosition(GetAllPositionRequest request);
    }
}
