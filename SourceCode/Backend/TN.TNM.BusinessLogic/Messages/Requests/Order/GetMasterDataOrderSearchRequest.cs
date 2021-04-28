using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class GetMasterDataOrderSearchRequest : BaseRequest<GetMasterDataOrderSearchParameter>
    {
        public override GetMasterDataOrderSearchParameter ToParameter()
        {
            return new GetMasterDataOrderSearchParameter()
            {
                UserId = UserId
            };
        }
    }
}
