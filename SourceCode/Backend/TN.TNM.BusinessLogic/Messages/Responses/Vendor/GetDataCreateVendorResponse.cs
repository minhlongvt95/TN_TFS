using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class GetDataCreateVendorResponse : BaseResponse
    {
        public List<Models.Category.CategoryModel> ListVendorGroup { get; set; }
        public List<string> ListVendorCode { get; set; }
        public List<Models.Admin.ProvinceModel> ListProvince { get; set; }
        public List<Models.Admin.DistrictModel> ListDistrict { get; set; }
        public List<Models.Admin.WardModel> ListWard { get; set; }
    }
}
