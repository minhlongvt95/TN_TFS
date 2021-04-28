using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Results.Contract
{
    public class GetMasterDataSearchContractResult : BaseResult
    {
        public List<CategoryEntityModel> ListStatus { get; set; }
        public List<ProductEntityModel> ListProduct { get; set; }
        public List<CustomerEntityModel> ListCutomer { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<CategoryEntityModel> ListTypeContract { get; set; }
    }
}
