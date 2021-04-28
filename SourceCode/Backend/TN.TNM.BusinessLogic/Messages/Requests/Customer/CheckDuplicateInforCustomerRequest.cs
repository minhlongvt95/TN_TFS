using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class CheckDuplicateInforCustomerRequest : BaseRequest<CheckDuplicateInforCustomerParameter>
    {
        public Guid? CustomerId { get; set; }
        public int CheckType { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public override CheckDuplicateInforCustomerParameter ToParameter()
        {
            return new CheckDuplicateInforCustomerParameter()
            {
                UserId = UserId,
                CustomerId = CustomerId,
                Email = Email,
                Phone = Phone,
                CheckType = CheckType
            };

        }
    }
}
