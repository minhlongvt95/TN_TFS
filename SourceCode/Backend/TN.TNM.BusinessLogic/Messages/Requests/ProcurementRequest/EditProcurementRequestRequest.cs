using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TN.TNM.BusinessLogic.Models.ProcurementRequest;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.ProcurementRequest;

namespace TN.TNM.BusinessLogic.Messages.Requests.ProcurementRequest
{
    public class EditProcurementRequestRequest : BaseRequest<EditProcurementRequestParameter>
    { 
        public ProcurementRequestModel ProcurementRequest { get; set; }
        public List<string> ListProcurementRequestItem { get; set; }
        //public List<Guid> ListItemToDelete { get; set; }
        public List<IFormFile> FileList { get; set; }
        public List<Guid> ListDocumentId { get; set; }
        //public List<string> lstDocument { get; set; }
        public override EditProcurementRequestParameter ToParameter()
        {
            var rs = new List<ProcurementRequestItem>();
            if (ListProcurementRequestItem != null)
            {
                ListProcurementRequestItem.ForEach(doc =>
                {
                    rs.Add(JsonConvert.DeserializeObject<ProcurementRequestItem>(doc));
                });
            }
            return new EditProcurementRequestParameter()
            {
                //ListItemToDelete = ListItemToDelete == null ? new List<Guid>() : ListItemToDelete,
                UserId = UserId,
                ProcurementRequest = ProcurementRequest.ToEntity(),
                ListProcurementRequestItem = rs,
                FileList = FileList,
                ListDocumentId = ListDocumentId != null ? ListDocumentId : new List<Guid>()
                //lstDocument = lstDocument == null ? new List<string>() : lstDocument
            };
        }
    }
}
