using System;

namespace TN.TNM.DataAccess.Messages.Parameters.ProcurementRequest
{
    public class GetProcurementRequestByIdParameter : BaseParameter
    {
        public Guid ProcurementRequestId { get; set; }
    }
}
