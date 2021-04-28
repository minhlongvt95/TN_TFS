using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class EditPersonInChargeParameter : BaseParameter
    {
        public List<Guid> ListLeadId { get; set; }
        public Guid EmployeeId { get; set; }
    }
}
