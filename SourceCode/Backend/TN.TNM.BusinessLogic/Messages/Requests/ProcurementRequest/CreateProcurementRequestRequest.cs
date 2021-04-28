using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TN.TNM.BusinessLogic.Models.ProcurementRequest;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.ProcurementRequest;

namespace TN.TNM.BusinessLogic.Messages.Requests.ProcurementRequest
{
    public class CreateProcurementRequestRequest : BaseRequest<CreateProcurementRequestParameter>
    {
        public ProcurementRequestModel ProcurementRequest { get; set; }
        public List<string> ProcurementRequestItemList { get; set; }
        public List<IFormFile> FileList { get; set; }
        public override CreateProcurementRequestParameter ToParameter()
        {
            var lst = new List<ProcurementRequestItem>();
            if (ProcurementRequestItemList != null)
            {
                ProcurementRequestItemList.ForEach(item =>
                {
                    lst.Add(JsonConvert.DeserializeObject<ProcurementRequestItemModel>(item).ToEntity());
                });
            }

            return new CreateProcurementRequestParameter() {
                FileList = FileList,
                UserId = UserId,
                ProcurementRequest = ProcurementRequest.ToEntity(),
                ProcurementRequestItemList = lst
            };
        }
    }
}
