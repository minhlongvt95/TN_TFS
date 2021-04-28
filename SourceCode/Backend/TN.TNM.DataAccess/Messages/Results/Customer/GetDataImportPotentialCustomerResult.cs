using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetDataImportPotentialCustomerResult: BaseResult
    {
        public List<EmployeeEntityModel> ListPersonalInChange { get; set; }//nguoi phu trach
        public List<CategoryEntityModel> ListInvestFund { get; set; }//Nguon tiem nang
        public List<string> ListEmail { get; set; }
        public List<string> ListPhone { get; set; }
    }
}
