using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.ProcurementRequest;

namespace TN.TNM.BusinessLogic.Messages.Requests.ProcurementRequest
{
    public class GetDataCreateProcurementRequestRequest: BaseRequest<GetDataCreateProcurementRequestParameter>
    {
        public override GetDataCreateProcurementRequestParameter ToParameter()
        {
       
            return new GetDataCreateProcurementRequestParameter()
            {
                UserId = UserId
            };
        }
    }
}
