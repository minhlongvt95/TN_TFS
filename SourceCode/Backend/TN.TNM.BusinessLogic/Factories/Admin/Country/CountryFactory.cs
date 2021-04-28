using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Admin.Country;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Country;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Country;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Admin.Country
{
    public class CountryFactory : BaseFactory, ICountry
    {
        private ICountryDataAccess iCountryDataAccess;

        public CountryFactory(ICountryDataAccess _iCountryDataAccess, ILogger _logger)
        {
            this.iCountryDataAccess = _iCountryDataAccess;
            this.logger = _logger;
        }
        public GetAllCountryResponse GetAllCountry(GetAllCountryRequest request)
        {
            try
            {
                this.logger.LogInformation("Get All Country");
                var parameter = request.ToParameter();
                var result = iCountryDataAccess.GetAllCountry(parameter);
                var response = new GetAllCountryResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message == "Success" ? "Success" : "Truy xuat thong tin quoc tich that bai"
                };
                response.ListCountry = new List<CountryModel>();
                result.ListCountry.ForEach(countryEntity =>
                {
                    response.ListCountry.Add(new CountryModel(countryEntity));
                });
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new GetAllCountryResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.messages.exception"
                };
            }
        }
    }
}
