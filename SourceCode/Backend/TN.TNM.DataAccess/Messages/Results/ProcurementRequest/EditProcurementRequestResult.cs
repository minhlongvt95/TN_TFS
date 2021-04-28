using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.ProcurementRequest
{
    public class EditProcurementRequestResult : BaseResult
    {
        public List<Models.Document.DocumentEntityModel> ListDocumentEntityModel { get; set; }
    }
}
