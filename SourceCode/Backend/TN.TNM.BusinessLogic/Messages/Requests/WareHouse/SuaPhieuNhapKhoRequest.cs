using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class SuaPhieuNhapKhoRequest : BaseRequest<SuaPhieuNhapKhoParameter>
    {
        public InventoryReceivingVoucher InventoryReceivingVoucher { get; set; }
        public List<InventoryReceivingVoucherMapping> ListInventoryReceivingVoucherMapping { get; set; }

        public override SuaPhieuNhapKhoParameter ToParameter()
        {
            return new SuaPhieuNhapKhoParameter()
            {
                UserId = UserId,
                InventoryReceivingVoucher = InventoryReceivingVoucher,
                ListInventoryReceivingVoucherMapping = ListInventoryReceivingVoucherMapping
            };
        }
    }
}
