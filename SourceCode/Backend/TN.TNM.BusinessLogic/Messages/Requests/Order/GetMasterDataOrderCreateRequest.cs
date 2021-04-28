using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class GetMasterDataOrderCreateRequest : BaseRequest<GetMasterDataOrderCreateParameter>
    {
        public int CreateType { get; set; }
        public Guid? CreateObjectId { get; set; }
        public override GetMasterDataOrderCreateParameter ToParameter()
        {
            return new GetMasterDataOrderCreateParameter()
            {
                UserId = UserId,
                CreateType = CreateType,
                CreateObjectId = CreateObjectId
            };
        }
    }
}
