using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class GetCustomerFromOrderCreateParameter : BaseParameter
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
    }
}
