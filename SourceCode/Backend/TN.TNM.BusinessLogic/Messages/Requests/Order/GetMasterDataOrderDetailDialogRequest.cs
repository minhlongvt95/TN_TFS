using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class GetMasterDataOrderDetailDialogRequest : BaseRequest<GetMasterDataOrderDetailDialogParameter>
    {
        public override GetMasterDataOrderDetailDialogParameter ToParameter()
        {
            return new GetMasterDataOrderDetailDialogParameter()
            {
                UserId = UserId
            };
        }
    }
}
