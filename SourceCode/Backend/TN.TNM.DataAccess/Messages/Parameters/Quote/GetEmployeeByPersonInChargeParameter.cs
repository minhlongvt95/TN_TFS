using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class GetEmployeeByPersonInChargeParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
    }
}
