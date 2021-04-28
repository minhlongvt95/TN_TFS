using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetCustomerFromOrderCreateRequest : BaseRequest<GetCustomerFromOrderCreateParameter>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public List<Guid?> CustomerServiceLevelIdList { get; set; }
        public List<Guid?> CustomerGroupIdList { get; set; }
        public List<Guid?> PersonInChargeIdList { get; set; }
        public bool NoPic { get; set; }
        public bool IsBusinessCus { get; set; }
        public bool IsPersonalCus { get; set; }
        public bool IsHKDCus { get; set; }

        public override GetCustomerFromOrderCreateParameter ToParameter()
        {
            return new GetCustomerFromOrderCreateParameter()
            {
                Email = Email,
                FirstName = FirstName,
                IsBusinessCus = IsBusinessCus,
                IsPersonalCus = IsPersonalCus,
                IsHKDCus = IsHKDCus,
                LastName = LastName,
                NoPic = NoPic,
                PersonInChargeIdList = PersonInChargeIdList,
                Phone = Phone,
                UserId = UserId,
                CustomerServiceLevelIdList = CustomerServiceLevelIdList,
                CustomerGroupIdList = CustomerGroupIdList
            };
        }
    }
}
