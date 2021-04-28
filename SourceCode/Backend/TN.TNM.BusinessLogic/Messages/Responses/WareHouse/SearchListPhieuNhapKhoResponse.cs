using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class SearchListPhieuNhapKhoResponse : BaseResponse
    {
        public List<InventoryReceivingVoucherEntityModel> ListPhieuNhapKho { get; set; }
    }
}
