using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.CustomerCare;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class CreateCustomerCareRequest : BaseRequest<CreateCustomerCareParameter>
    {
        public CustomerCareModel CustomerCare { get; set; }
        public List<Guid> CustomerId { get; set; }
        public List<string> ListTypeCustomer { get; set; }
        public string QueryFilter { get; set; }

        public override CreateCustomerCareParameter ToParameter()
        {
            return new CreateCustomerCareParameter
            {
                CustomerCare = CustomerCare.ToEntity(),
                CustomerId = CustomerId,
                ListTypeCustomer = ListTypeCustomer,
                QueryFilter = QueryFilter,
                UserId = UserId
            };
        }
    }
}
