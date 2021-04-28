using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.BillSale;
using TN.TNM.BusinessLogic.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Responses.BillSale
{
    public class GetOrderByOrderIdResponse:BaseResponse
    {
        public OrderBillModel Order { get; set; }
        public List<BillSaleCostModel> ListCost { get; set; }
        public List<BillSaleDetailModel> ListBillSaleDetail { get; set; }
        public List<InventoryDeliveryVoucherModel> ListInventoryDeliveryVoucher { get; set; }
    }
}
