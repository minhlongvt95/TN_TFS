using TN.TNM.DataAccess.Messages.Parameters.Admin.Position;
using TN.TNM.DataAccess.Messages.Results.Admin.Position;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IPositionDataAccess
    {
        GetAllPositionResult GetAllPosition(GetAllPositionParameter parameter);
    }
}
