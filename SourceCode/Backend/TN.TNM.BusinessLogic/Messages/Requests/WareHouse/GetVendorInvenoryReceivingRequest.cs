using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class GetVendorInvenoryReceivingRequest : BaseRequest<GetVendorInvenoryReceivingParameter>
    {
        public override GetVendorInvenoryReceivingParameter ToParameter()
        {
            return new GetVendorInvenoryReceivingParameter { };
        }
    }
}
