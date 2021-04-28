using System.Collections.Generic;
using TN.TNM.DataAccess.Models.ProcurementRequest;

namespace TN.TNM.DataAccess.Messages.Results.ProcurementRequest
{
    public class SearchProcurementRequestResult : BaseResult
    {
        public List<ProcurementRequestEntityModel> ListProcurementRequest { get; set; }
    }
}
