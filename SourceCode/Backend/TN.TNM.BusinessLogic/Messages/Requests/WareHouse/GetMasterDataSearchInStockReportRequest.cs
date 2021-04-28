using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class GetMasterDataSearchInStockReportRequest : BaseRequest<GetMasterDataSearchInStockReportParameter>
    {
        public override GetMasterDataSearchInStockReportParameter ToParameter()
        {
            return new GetMasterDataSearchInStockReportParameter()
            {
                UserId = UserId
            };
        }
    }
}
