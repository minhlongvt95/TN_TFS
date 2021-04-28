using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class FileDAO : BaseDAO
    {
        public FileDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }
    }
}
