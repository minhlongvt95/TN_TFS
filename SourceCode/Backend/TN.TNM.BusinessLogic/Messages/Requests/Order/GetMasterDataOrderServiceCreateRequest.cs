using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class GetMasterDataOrderServiceCreateRequest : BaseRequest<GetMasterDataOrderServiceCreateParameter>
    {
        public override GetMasterDataOrderServiceCreateParameter ToParameter()
        {
            return new GetMasterDataOrderServiceCreateParameter()
            {
                UserId = UserId
            };
        }
    }
}
