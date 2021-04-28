using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.BusinessLogic.Models.Category;
using CustomerServiceLevelModel = TN.TNM.BusinessLogic.Models.Customer.CustomerServiceLevelModel;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class GetListCustomerResponse: BaseResponse
    {
        public List<AreaModel> ListAreaModel { get; set; }
        public List<CategoryModel> ListSourceModel { get; set; }
        public List<CategoryModel> ListStatusCareModel { get; set; }
        public List<CategoryModel> ListCategoryModel { get; set; }
        public List<CustomerServiceLevelModel> ListCustomerServiceLevelModel { get; set; }
    }
}
