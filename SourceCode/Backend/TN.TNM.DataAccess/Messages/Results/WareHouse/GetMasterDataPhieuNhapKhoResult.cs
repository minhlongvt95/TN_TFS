using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Vendor;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class GetMasterDataPhieuNhapKhoResult : BaseResult
    {
        public List<VendorEntityModel> ListVendor { get; set; }
        public List<WareHouseEntityModel> ListWarehouse { get; set; }
        public List<CustomerEntityModel> ListCustomer { get; set; }
        public string EmployeeCodeName { get; set; }
    }
}
