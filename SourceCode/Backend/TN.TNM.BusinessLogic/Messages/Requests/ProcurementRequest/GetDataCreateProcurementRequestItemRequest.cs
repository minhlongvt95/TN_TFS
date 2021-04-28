using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.ProcurementRequest;

namespace TN.TNM.BusinessLogic.Messages.Requests.ProcurementRequest
{
    public class GetDataCreateProcurementRequestItemRequest: BaseRequest<GetDataCreateProcurementRequestItemParameter>
    {
        public override GetDataCreateProcurementRequestItemParameter ToParameter()
        {

            return new GetDataCreateProcurementRequestItemParameter()
            {
                UserId = UserId
            };
        }
    }
}
