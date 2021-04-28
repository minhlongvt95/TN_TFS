using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Country
{
    public class GetAllCountryResult : BaseResult
    {
        public List<Databases.Entities.Country> ListCountry { get; set; }
    }
}
