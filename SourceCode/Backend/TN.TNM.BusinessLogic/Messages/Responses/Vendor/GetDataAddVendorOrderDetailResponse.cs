using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class GetDataAddVendorOrderDetailResponse: BaseResponse
    {
        public List<CategoryEntityModel> ListMoneyUnit { get; set; }
        public List<DataAccess.Models.Product.ProductEntityModel> ListProductByVendorId { get; set; }
    }
}
