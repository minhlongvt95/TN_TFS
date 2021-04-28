using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Product;

namespace TN.TNM.BusinessLogic.Messages.Responses.Contract
{
    public class GetMasterDataSearchContractResponse : BaseResponse
    {
        public List<CategoryModel> ListStatus { get; set; }
        public List<CategoryModel> ListTypeContract { get; set; }
        public List<ProductModel> ListProduct { get; set; }
        public List<CustomerModel> ListCustomer { get; set; }
        public List<EmployeeModel> ListEmployee { get; set; }
    }
}
