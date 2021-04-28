using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Quote;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class GetTop3PotentialCustomersResponse : BaseResponse
    {
        public List<GetTop3PotentialCustomerModel> QuoteList { get; set; }
    }
}
