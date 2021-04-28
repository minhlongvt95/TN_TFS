using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class DeleteListCustomerAdditionalRequest : BaseRequest<DeleteListCustomerAdditionalParameter>
    {
        public List<Guid> ListCusAddInfId { get; set; }
        public Guid CustomerId { get; set; }
        public override DeleteListCustomerAdditionalParameter ToParameter()
        {
            return new DeleteListCustomerAdditionalParameter()
            {
                UserId = UserId,
                ListCusAddInfId = ListCusAddInfId,
                CustomerId = CustomerId
            };
        }
    }
}
