using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class ApprovalOrRejectCustomerParameter : BaseParameter
    {
        public List<Guid> ListCustomerId { get; set; }
        public bool IsApproval { get; set; }
        public bool IsFreeCus { get; set; }
    }
}
