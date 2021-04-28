using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class GetDataSearchVendorResponse: BaseResponse
    {
        public List<Models.Category.CategoryModel> ListVendorGroup { get; set; }
    }
}
