using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Admin.Country;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Country;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Country;

namespace TN.TNM.Api.Controllers
{
    public class CountryController : Controller
    {
        private readonly ICountry _iCountry;
        public CountryController(ICountry iCountry)
        {
            this._iCountry = iCountry;
        }

        /// <summary>
        /// getAllCountry
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/country/getAllCountry")]
        [Authorize(Policy = "Member")]
        public GetAllCountryResponse GetAllCountry([FromBody]GetAllCountryRequest request)
        {
            return this._iCountry.GetAllCountry(request);
        }
    }
}