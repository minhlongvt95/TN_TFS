using System.Collections.Generic;
using TN.TNM.DataAccess.Models.CustomerCare;

namespace TN.TNM.DataAccess.Messages.Results.CustomerCare
{
    public class GetCustomerCareActiveResult : BaseResult
    {
        public List<GetCustomerCareActiveEntityModel> ListCategoryCare { get; set; }
    }
}
