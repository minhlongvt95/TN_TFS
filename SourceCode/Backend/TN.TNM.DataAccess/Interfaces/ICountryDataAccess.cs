using TN.TNM.DataAccess.Messages.Parameters.Admin.Country;
using TN.TNM.DataAccess.Messages.Results.Admin.Country;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface ICountryDataAccess
    {
        GetAllCountryResult GetAllCountry(GetAllCountryParameter parameter);
    }
}
