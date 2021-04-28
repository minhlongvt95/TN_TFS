using System;

using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class FilterCustomerInInventoryDeliveryVoucherRequest : BaseRequest<FilterCustomerInInventoryDeliveryVoucherParameter>
    {
        public override FilterCustomerInInventoryDeliveryVoucherParameter ToParameter()
        {
            return new FilterCustomerInInventoryDeliveryVoucherParameter
            {

            };
        }
    }
}
