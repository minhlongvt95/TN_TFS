using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.BillSale;

namespace TN.TNM.BusinessLogic.Messages.Requests.BillSale
{
    public class GetMasterBillOfSaleRequest : BaseRequest<GetMasterBillOfSaleParameter>
    {

        public override GetMasterBillOfSaleParameter ToParameter()
        {
            return new GetMasterBillOfSaleParameter()
            {
                UserId =UserId
            };
        }
    }
}
