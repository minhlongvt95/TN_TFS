using System.Linq;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Country;
using TN.TNM.DataAccess.Messages.Results.Admin.Country;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class CountryDAO : BaseDAO , ICountryDataAccess
    {
        public CountryDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }
        public GetAllCountryResult GetAllCountry(GetAllCountryParameter parameter)
        {
            var _listCountry = context.Country.Where(ct => true).ToList();
            return new GetAllCountryResult()
            {
                Message = "Success",
                Status = true,
                ListCountry = _listCountry
            };
        }
    }
}
