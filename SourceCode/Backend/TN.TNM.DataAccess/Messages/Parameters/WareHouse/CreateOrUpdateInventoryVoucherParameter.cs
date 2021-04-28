using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class CreateOrUpdateInventoryVoucherParameter : BaseParameter
    {
        public InventoryReceivingVoucher inventoryReceivingVoucher { get; set; }
        public List<GetVendorOrderDetailByVenderOrderIdEntityModel> inventoryReceivingVoucherMapping { get; set; }
        public List<IFormFile> fileList { get; set; }
        public string noteContent { get; set; }

    }
}
