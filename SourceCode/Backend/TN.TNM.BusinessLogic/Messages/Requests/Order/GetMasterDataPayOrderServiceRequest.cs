using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class GetMasterDataPayOrderServiceRequest : BaseRequest<GetMasterDataPayOrderServiceParameter>
    {
        public override GetMasterDataPayOrderServiceParameter ToParameter()
        {
            return new GetMasterDataPayOrderServiceParameter()
            {
                UserId = UserId
            };
        }
    }
}
