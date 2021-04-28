using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class GetInventoryDeliveryVoucherByIdResult:BaseResult
    {
        public InventoryDeliveryVoucherEntityModel inventoryDeliveryVoucher { get; set; }
        public List<InventoryDeliveryVoucherMappingEntityModel> inventoryDeliveryVoucherMappingEntityModel { get; set; }
        //public VendorEntityModel SelectVendor { get; set; }
        //public CustomerEntityModel SelectCustomer { get; set; }
        //public List<VendorOrder> listVendorOrder { get; set; }
        //public List<CustomerOrder> listCustomerOrder { get; set; }

    }
}
