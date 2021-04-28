using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class GetEmployeeByPersonInChargeParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
        public Guid UserId { get; set; }
    }
}
