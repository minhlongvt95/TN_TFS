using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class SuaPhieuNhapKhoParameter : BaseParameter
    {
        public InventoryReceivingVoucher InventoryReceivingVoucher { get; set; }
        public List<InventoryReceivingVoucherMapping> ListInventoryReceivingVoucherMapping { get; set; }
    }
}
