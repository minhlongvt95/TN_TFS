using Microsoft.Extensions.Logging;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class BaseDAO
    {
        protected IAuditTraceDataAccess iAuditTrace;
        protected TNTN8Context context;
        protected ILogger logger;

    }
}
