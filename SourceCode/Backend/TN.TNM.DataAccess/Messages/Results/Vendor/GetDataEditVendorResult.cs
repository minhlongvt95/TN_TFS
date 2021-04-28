using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class GetDataEditVendorResult: BaseResult
    {
        public List<Databases.Entities.Category> ListVendorGroup { get; set; }
        public List<Databases.Entities.Category> ListPaymentMethod { get; set; }
        public List<string> ListVendorCode { get; set; }
        public List<Models.Address.ProvinceEntityModel> ListProvince { get; set; }
        public List<Models.Address.DistrictEntityModel> ListDistrict { get; set; }
        public List<Models.Address.WardEntityModel> ListWard { get; set; }
        public List<Models.Vendor.VendorOrderByMonthModel> ListVendorOrderByMonth { get; set; } //Tổng đặt SP/DV
        public List<Models.Vendor.VendorOrderByMonthModel> ListVendorOrderInProcessByMonth { get; set; } //tổng đơn hàng đang xử lý
        public List<Models.Vendor.VendorOrderByMonthModel> ListReceivableByMonth { get; set;} //Tổng công nợ theo tháng
    }
}
