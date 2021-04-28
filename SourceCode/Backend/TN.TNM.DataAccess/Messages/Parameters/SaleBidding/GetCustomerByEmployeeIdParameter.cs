using System;

namespace TN.TNM.DataAccess.Messages.Parameters.SaleBidding
{
    public class GetCustomerByEmployeeIdParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
    }
}
