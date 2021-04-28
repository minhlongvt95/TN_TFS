using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.CustomerCare;

namespace TN.TNM.DataAccess.Messages.Results.CustomerCare
{
    public class GetCustomerCareByIdResult:BaseResult
    {
        public CustomerCareEntityModel CustomerCare { get; set; }
        public List<CustomerEntityModel> ListCustomer { get; set; }
        public List<CustomerCareFeedBackEntityModel> CustomerCareFeedBack { get; set; }
        public string QueryFilter { get; set; }
        public int TypeCutomer { get; set; }
    }
}
