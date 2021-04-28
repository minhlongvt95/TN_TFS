using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class GetDanhSachSanPhamCuaPhieuRequest : BaseRequest<GetDanhSachSanPhamCuaPhieuParameter>
    {
        public List<Guid> ListObjectId { get; set; }
        public int ObjectType { get; set; }
        public Guid WarehouseId { get; set; }
        public Guid? InventoryReceivingVoucherId { get; set; }

        public override GetDanhSachSanPhamCuaPhieuParameter ToParameter()
        {
            return new GetDanhSachSanPhamCuaPhieuParameter()
            {
                UserId = UserId,
                ListObjectId = ListObjectId,
                ObjectType = ObjectType,
                WarehouseId = WarehouseId,
                InventoryReceivingVoucherId = InventoryReceivingVoucherId
            };
        }
    }
}
