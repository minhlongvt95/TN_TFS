using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.ProcurementRequest
{
    public class GetDataEditProcurementRequestParameter: BaseParameter
    {
        public Guid ProcurementRequestId { get; set; }
        public bool? IsAprroval { get; set; }
        public string Description { get; set; }
        public List<ProcurementRequestItem> ListProcurementRequestItem { get; set; }
    }
}
