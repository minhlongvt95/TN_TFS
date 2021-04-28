using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class GetDataEditVendorResponse: BaseResponse
    {
        public List<Models.Category.CategoryModel> ListVendorGroup { get; set; }
        public List<Models.Category.CategoryModel> ListPaymentMethod { get; set; }
        public List<string> ListVendorCode { get; set; }
        public List<Models.Admin.ProvinceModel> ListProvince { get; set; }
        public List<Models.Admin.DistrictModel> ListDistrict { get; set; }
        public List<Models.Admin.WardModel> ListWard { get; set; }
        public List<DataAccess.Models.Vendor.VendorOrderByMonthModel> ListVendorOrderByMonth { get; set; } //Tổng đặt SP/DV
        public List<DataAccess.Models.Vendor.VendorOrderByMonthModel> ListVendorOrderInProcessByMonth { get; set; } //tổng đơn hàng đang xử lý
        public List<DataAccess.Models.Vendor.VendorOrderByMonthModel> ListReceivableByMonth { get; set; } //Tổng công nợ theo tháng
    }
}
