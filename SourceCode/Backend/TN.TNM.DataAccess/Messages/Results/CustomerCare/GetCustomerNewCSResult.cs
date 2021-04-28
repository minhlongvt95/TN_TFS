using System.Collections.Generic;
using TN.TNM.DataAccess.Models.CustomerCare;

namespace TN.TNM.DataAccess.Messages.Results.CustomerCare
{
    public class GetCustomerNewCSResult : BaseResult
    {
        public List<GetCustomerNewCSEntityModel> ListCustomerNewOrder { get; set; }
    }
}
