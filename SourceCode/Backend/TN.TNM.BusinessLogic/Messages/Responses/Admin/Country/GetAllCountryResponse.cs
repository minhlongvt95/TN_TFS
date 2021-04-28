using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Country
{
    public class GetAllCountryResponse : BaseResponse
    {
        public List<CountryModel> ListCountry { get; set; }
    }
}
