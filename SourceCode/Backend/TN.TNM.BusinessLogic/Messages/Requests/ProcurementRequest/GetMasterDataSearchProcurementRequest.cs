using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.ProcurementRequest;

namespace TN.TNM.BusinessLogic.Messages.Requests.ProcurementRequest
{
    public class GetMasterDataSearchProcurementRequest : BaseRequest<GetMasterDataSearchProcurementRequestParameter>
    {
        public override GetMasterDataSearchProcurementRequestParameter ToParameter()
        {
            return new GetMasterDataSearchProcurementRequestParameter
            {
                UserId = UserId,
            };
        }
    }
}
