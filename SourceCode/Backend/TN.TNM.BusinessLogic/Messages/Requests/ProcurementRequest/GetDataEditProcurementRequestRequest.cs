using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.ProcurementRequest;

namespace TN.TNM.BusinessLogic.Messages.Requests.ProcurementRequest
{
    public class GetDataEditProcurementRequestRequest : BaseRequest<GetDataEditProcurementRequestParameter>
    {
        public Guid ProcurementRequestId { get; set; }
        public bool? IsAprroval { get; set; }
        public string Description { get; set; }
        public List<ProcurementRequestItem> ListProcurementRequestItem { get; set; }

        public override GetDataEditProcurementRequestParameter ToParameter()
        {
            return new GetDataEditProcurementRequestParameter
            {
                ProcurementRequestId = ProcurementRequestId,
                IsAprroval = IsAprroval,
                Description = Description,
                //ListProcurementRequestItem = rs,
                ListProcurementRequestItem = ListProcurementRequestItem,
                UserId = UserId
            };
        }
    }
}
