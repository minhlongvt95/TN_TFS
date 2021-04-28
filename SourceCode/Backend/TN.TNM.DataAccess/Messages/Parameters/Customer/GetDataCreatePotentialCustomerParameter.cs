using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class GetDataCreatePotentialCustomerParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
    }
}
