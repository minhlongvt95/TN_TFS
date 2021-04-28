using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class AuditTraceDAO: BaseDAO,IAuditTraceDataAccess
    {
        public AuditTraceDAO(TNTN8Context _content)
        {
            this.context = _content;
        }

        public void Trace(string actionName, string objectName, string description, Guid createById)
        {
            //var trace = new AuditTrace
            //{                
            //    ActionName = actionName,
            //    ObjectName = objectName,
            //    CreatedById = createById,
            //    CreatedDate = DateTime.Now,
            //    Description = description
            //};
            //this.context.Add(trace);
            //this.context.SaveChanges();
        }
    }
}
