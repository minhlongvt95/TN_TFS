using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.BillSale;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Results.BillSale
{
    public class GetOrderByOrderIdResult:BaseResult
    {
        public OrderBillEntityModel Order { get; set; }
        public List<BillSaleCostEntityModel> ListCost { get; set; }
        public List<BillSaleDetailEntityModel> ListBillSaleDetail { get; set; }
        public List<InventoryDeliveryVoucherEntityModel> ListInventoryDeliveryVoucher { get; set; }
    }
}
