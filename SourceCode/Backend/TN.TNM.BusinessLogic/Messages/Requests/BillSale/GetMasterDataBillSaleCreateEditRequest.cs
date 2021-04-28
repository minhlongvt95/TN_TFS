using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.BillSale;

namespace TN.TNM.BusinessLogic.Messages.Requests.BillSale
{
    public class GetMasterDataBillSaleCreateEditRequest:BaseRequest<GetMasterDataBillSaleCreateEditParameter>
    {
        public bool IsCreate { get; set; }
        public Guid? ObjectId { get; set; }

        public override GetMasterDataBillSaleCreateEditParameter ToParameter()
        {
            return new GetMasterDataBillSaleCreateEditParameter()
            {
                IsCreate = IsCreate,
                ObjectId = ObjectId,
                UserId = UserId
            };
        }
    }
}
