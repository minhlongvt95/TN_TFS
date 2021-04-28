using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.ProcurementRequest
{
    public class EditProcurementRequestResponse : BaseResponse
    {
        public List<DataAccess.Models.Document.DocumentEntityModel> ListDocumentEntityModel { get; set; }
    }
}
