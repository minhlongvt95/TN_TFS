using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class GetDetailPhieuNhapKhoRequest : BaseRequest<GetDetailPhieuNhapKhoParameter>
    {
        public Guid InventoryReceivingVoucherId { get; set; }

        public override GetDetailPhieuNhapKhoParameter ToParameter()
        {
            return new GetDetailPhieuNhapKhoParameter()
            {
                UserId = UserId,
                InventoryReceivingVoucherId = InventoryReceivingVoucherId
            };
        }
    }
}
