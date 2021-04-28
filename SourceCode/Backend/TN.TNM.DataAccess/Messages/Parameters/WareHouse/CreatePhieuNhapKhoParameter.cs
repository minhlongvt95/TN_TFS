using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class CreatePhieuNhapKhoParameter : BaseParameter
    {
        public InventoryReceivingVoucher InventoryReceivingVoucher { get; set; }
        public List<InventoryReceivingVoucherMapping> ListInventoryReceivingVoucherMapping { get; set; }
        public List<IFormFile> ListFile { get; set; }
    }
}
