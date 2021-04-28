using System;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IAuditTraceDataAccess
    {
        void Trace(string actionName, string objectName, string description, Guid createById);
    }
}
