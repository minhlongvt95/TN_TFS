using System.Linq;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.District;
using TN.TNM.DataAccess.Messages.Results.Admin.District;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class DistrictDAO : BaseDAO, IDistrictDataAccess
    {
        public DistrictDAO(IAuditTraceDataAccess _iAuditTrace, Databases.TNTN8Context context)
        {
            this.context = context;
            this.iAuditTrace = _iAuditTrace;
        }

        public GetAllDistrictByProvinceIdResult GetAllDistrictByProvinceId(
            GetAllDistrictByProvinceIdParameter parameter)
        {
            var provinceId = parameter.ProviceId;
            var listDistrict = context.District.Where(d => d.ProvinceId == provinceId).OrderBy(l => l.DistrictName).ToList();
            return new GetAllDistrictByProvinceIdResult()
            {
                ListDistrict = listDistrict,
                Status = true
            };
        }
    }
}
