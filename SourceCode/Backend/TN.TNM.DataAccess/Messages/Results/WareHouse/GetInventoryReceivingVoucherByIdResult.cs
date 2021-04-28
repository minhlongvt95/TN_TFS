using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Vendor;
using TN.TNM.DataAccess.Models.WareHouse;
//using Entities = TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class GetInventoryReceivingVoucherByIdResult:BaseResult
    {
        public InventoryReceivingVoucherModel inventoryReceivingVoucher { get; set; }
        public List<GetVendorOrderDetailByVenderOrderIdEntityModel> inventoryReceivingVoucherMapping { get; set; }
        public VendorEntityModel SelectVendor { get; set; }
        public CustomerEntityModel SelectCustomer { get; set; }
        public List<VendorOrder> listVendorOrder { get; set; }
        public List<CustomerOrder> listCustomerOrder { get; set; }
        public string SelectedNameCustomerOrderCode { get; set; }
        public string SelectedNameVendorOrderCode { get; set; }


    }
}
