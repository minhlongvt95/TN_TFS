using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.CustomerCare
{
    public class GetMasterDataCustomerCareListResult : BaseResult
    {
        public List<CategoryEntityModel> ListStatus { get; set; }
        public List<CategoryEntityModel> ListFormCusCare { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        
    }
}
