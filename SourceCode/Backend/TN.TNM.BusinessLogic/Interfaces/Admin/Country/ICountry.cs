using TN.TNM.BusinessLogic.Messages.Requests.Admin.Country;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Country;

namespace TN.TNM.BusinessLogic.Interfaces.Admin.Country
{
    public interface ICountry
    {
        GetAllCountryResponse GetAllCountry(GetAllCountryRequest request);
    }
}
