using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class GetSoGTCuaSanPhamTheoKhoRequest : BaseRequest<GetSoGTCuaSanPhamTheoKhoParameter>
    {
        public Guid ProductId { get; set; }
        public Guid WarehouseId { get; set; }
        public decimal QuantityRequest { get; set; }

        public override GetSoGTCuaSanPhamTheoKhoParameter ToParameter()
        {
            return new GetSoGTCuaSanPhamTheoKhoParameter()
            {
                UserId = UserId,
                ProductId = ProductId,
                WarehouseId = WarehouseId,
                QuantityRequest = QuantityRequest
            };
        }
    }
}
