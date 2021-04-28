using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.ProcurementRequest
{
    public class CreateProcurementRequestParameter : BaseParameter
    {
        public Databases.Entities.ProcurementRequest ProcurementRequest { get; set; }
        public List<ProcurementRequestItem> ProcurementRequestItemList { get; set; }
        public List<IFormFile> FileList { get; set; }
    }
}
