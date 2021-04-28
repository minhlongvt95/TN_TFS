using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.Order;
using TN.TNM.BusinessLogic.Models.Vendor;
using TN.TNM.BusinessLogic.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class GetInventoryReceivingVoucherByIdResponse:BaseResponse
    {
        public InventoryReceivingVoucherSearchModel inventoryReceivingVoucher { get; set; }
        public List<GetVendorOrderDetailByVenderOrderIdModel> inventoryReceivingVoucherMapping { get; set; }
        public VendorModel SelectVendor { get; set; }
        public CustomerModel SelectCustomer { get; set; }
        public List<VendorOrderModel> listVendorOrder { get; set; }
        public List<CustomerOrderModel> listCustomerOrder { get; set; }

    }
}
