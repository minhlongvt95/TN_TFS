using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.CustomerCare;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class UpdateCustomerCareRequest : BaseRequest<UpdateCustomerCareParameter>
    {
        public CustomerCareModel CustomerCare { get; set; }
        public List<Guid> CustomerId { get; set; }
        public string QueryFilter { get; set; }

        public override UpdateCustomerCareParameter ToParameter()
        {
            return new UpdateCustomerCareParameter
            {
                CustomerCare=this.CustomerCare.ToEntity(),
                CustomerId=this.CustomerId,
                QueryFilter=this.QueryFilter,
                UserId = this.UserId
            };
        }
    }
}
