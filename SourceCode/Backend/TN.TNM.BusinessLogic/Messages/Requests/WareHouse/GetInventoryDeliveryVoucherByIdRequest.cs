using System;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class GetInventoryDeliveryVoucherByIdRequest : BaseRequest<GetInventoryDeliveryVoucherByIdParameter>
    {
        public Guid Id { get; set; }

        public override GetInventoryDeliveryVoucherByIdParameter ToParameter()
        {
            return new GetInventoryDeliveryVoucherByIdParameter
            {
                Id=this.Id
            };
        }
    }
}
