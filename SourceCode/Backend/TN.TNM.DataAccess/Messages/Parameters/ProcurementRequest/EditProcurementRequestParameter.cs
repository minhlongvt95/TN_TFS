using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.ProcurementRequest
{
    public class EditProcurementRequestParameter : BaseParameter
    {
        public Databases.Entities.ProcurementRequest ProcurementRequest { get; set; }
        public List<ProcurementRequestItem> ListProcurementRequestItem { get; set; }
        public List<IFormFile> FileList { get; set; }
        public List<Guid> ListDocumentId { get; set; }

        //public List<Guid> ListItemToDelete { get; set; }
        //public List<string> lstDocument { get; set; }
    }
}
