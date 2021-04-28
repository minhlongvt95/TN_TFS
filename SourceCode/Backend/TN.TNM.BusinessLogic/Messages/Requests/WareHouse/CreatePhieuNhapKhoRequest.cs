using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class CreatePhieuNhapKhoRequest : BaseRequest<CreatePhieuNhapKhoParameter>
    {
        public InventoryReceivingVoucher InventoryReceivingVoucher { get; set; }
        public List<InventoryReceivingVoucherMapping> ListInventoryReceivingVoucherMapping { get; set; }
        public List<IFormFile> ListFile { get; set; }

        public override CreatePhieuNhapKhoParameter ToParameter()
        {
            return new CreatePhieuNhapKhoParameter()
            {
                UserId = UserId,
                InventoryReceivingVoucher = InventoryReceivingVoucher,
                ListInventoryReceivingVoucherMapping = ListInventoryReceivingVoucherMapping,
                ListFile = ListFile
            };
        }
    }
}
