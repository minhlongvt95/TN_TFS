using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class ApprovalOrRejectCustomerRequest : BaseRequest<ApprovalOrRejectCustomerParameter>
    {
        public List<Guid> ListCustomerId { get; set; }
        public bool IsApproval { get; set; }
        public bool IsFreeCus { get; set; }

        public override ApprovalOrRejectCustomerParameter ToParameter()
        {
            return new ApprovalOrRejectCustomerParameter
            {
                ListCustomerId = this.ListCustomerId,
                IsApproval = this.IsApproval,
                IsFreeCus = this.IsFreeCus,
                UserId = this.UserId,
            };
        }
    }
}
