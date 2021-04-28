using System.Linq;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Position;
using TN.TNM.DataAccess.Messages.Results.Admin.Position;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class PositionDAO : BaseDAO, IPositionDataAccess
    {
        public PositionDAO(Databases.TNTN8Context context, IAuditTraceDataAccess iAuditTrace)
        {
            this.context = context;
            this.iAuditTrace = iAuditTrace;
        }

        public GetAllPositionResult GetAllPosition(GetAllPositionParameter parameter)
        {
            var listPosition = context.Position.OrderBy(p => p.PositionName).ToList();
            return new GetAllPositionResult()
            {
                Status = true,
                ListPosition = listPosition
            };
        }
    }
}
