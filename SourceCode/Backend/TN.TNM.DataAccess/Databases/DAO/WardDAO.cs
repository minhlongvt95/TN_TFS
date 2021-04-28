using System.Linq;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Ward;
using TN.TNM.DataAccess.Messages.Results.Admin.Ward;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class WardDAO : BaseDAO, IWardDataAccess
    {
        public WardDAO(TNTN8Context _context, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _context;
            this.iAuditTrace = _iAuditTrace;
        }

        public GetAllWardByDistrictIdResult GetAllWardByDistrictId(GetAllWardByDistrictIdParameter parameter)
        {
            var districtId = parameter.DistrictId;
            var wardList = context.Ward.Where(w => w.DistrictId == districtId).OrderBy(l => l.WardName).ToList();
            return new GetAllWardByDistrictIdResult()
            {
                ListWard = wardList,
                Status = true
            };
        }
    }
}
