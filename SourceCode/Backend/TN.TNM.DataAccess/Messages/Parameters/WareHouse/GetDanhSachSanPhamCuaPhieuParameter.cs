using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class GetDanhSachSanPhamCuaPhieuParameter : BaseParameter
    {
        public List<Guid> ListObjectId { get; set; }
        public int ObjectType { get; set; }
        public Guid WarehouseId { get; set; }
        public Guid? InventoryReceivingVoucherId { get; set; }
    }
}
