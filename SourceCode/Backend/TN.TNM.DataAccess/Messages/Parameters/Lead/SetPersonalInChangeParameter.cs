using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class SetPersonalInChangeParameter: BaseParameter
    {
        public Guid? CustomerId { get; set; }
    }
}
