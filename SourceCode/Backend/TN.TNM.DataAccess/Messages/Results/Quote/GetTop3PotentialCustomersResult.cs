using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.DataAccess.Messages.Results.Quote
{
    public class GetTop3PotentialCustomersResult : BaseResult
    {
        public List<GetTop3PotentialCustomersModel> PotentialCustomersList { get; set; }
    }
}

