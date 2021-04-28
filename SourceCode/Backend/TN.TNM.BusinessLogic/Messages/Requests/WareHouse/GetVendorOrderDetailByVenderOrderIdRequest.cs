using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class GetVendorOrderDetailByVenderOrderIdRequest : BaseRequest<GetVendorOrderDetailByVenderOrderIdParameter>
    {
        public int TypeWarehouseVocher { get; set; }
        public List<Guid> ListVendorOrderId { get; set; }
        public override GetVendorOrderDetailByVenderOrderIdParameter ToParameter()
        {
            return new GetVendorOrderDetailByVenderOrderIdParameter()
            {
                UserId = UserId,
                ListVendorOrderId = ListVendorOrderId,
                TypeWarehouseVocher= TypeWarehouseVocher
            };
        }
    }
}
