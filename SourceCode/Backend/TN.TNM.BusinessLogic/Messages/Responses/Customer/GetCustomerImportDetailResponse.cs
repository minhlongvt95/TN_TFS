using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class GetCustomerImportDetailResponse: BaseResponse
    {
        public List<Models.Category.CategoryModel> ListCustomerGroup { get; set; }
        public List<string> ListCustomerCompanyCode { get; set; }
        public List<string> ListEmail { get; set; }
        public List<string> ListPhone { get; set; }
    }
}
