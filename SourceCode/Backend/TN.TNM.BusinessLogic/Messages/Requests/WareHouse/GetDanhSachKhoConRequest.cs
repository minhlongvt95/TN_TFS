using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class GetDanhSachKhoConRequest : BaseRequest<GetDanhSachKhoConParameter>
    {
        public Guid WarehouseId { get; set; }

        public override GetDanhSachKhoConParameter ToParameter()
        {
            return new GetDanhSachKhoConParameter()
            {
                UserId = UserId,
                WarehouseId = WarehouseId
            };
        }
    }
}
