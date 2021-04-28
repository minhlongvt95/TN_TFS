using System;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class GetTop10WarehouseFromReceivingVoucherRequest : BaseRequest<GetTop10WarehouseFromReceivingVoucherParameter>
    {
        public override GetTop10WarehouseFromReceivingVoucherParameter ToParameter()
        {
            return new GetTop10WarehouseFromReceivingVoucherParameter
            {

            };
        }
    }
}
