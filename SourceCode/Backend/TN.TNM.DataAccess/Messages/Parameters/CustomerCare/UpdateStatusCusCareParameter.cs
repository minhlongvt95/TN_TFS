using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.CustomerCare
{
    public class UpdateStatusCusCareParameter : BaseParameter
    {
        public Guid CustomerCareId { get; set; }
        public Guid StatusId { get; set; }
    }
}
