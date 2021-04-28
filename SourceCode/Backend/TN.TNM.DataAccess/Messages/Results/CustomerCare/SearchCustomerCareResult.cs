using System.Collections.Generic;
using TN.TNM.DataAccess.Models.CustomerCare;

namespace TN.TNM.DataAccess.Messages.Results.CustomerCare
{
    public class SearchCustomerCareResult:BaseResult
    {
        public List<CustomerCareEntityModel> LstCustomerCare { get; set; }
    }
}
