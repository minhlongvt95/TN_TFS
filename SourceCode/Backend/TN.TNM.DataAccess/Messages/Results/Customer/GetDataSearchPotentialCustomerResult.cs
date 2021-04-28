using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.GeographicalArea;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetDataSearchPotentialCustomerResult: BaseResult
    {
        public List<CategoryEntityModel> ListInvestFund { get; set; }
        public List<CategoryEntityModel> ListCareState { get; set; }
        public List<EmployeeEntityModel> ListEmployeeEntityModel { get; set; }
        public List<CategoryEntityModel> ListCusGroup { get; set; }
        public List<GeographicalAreaEntityModel> ListArea { get; set; }
        public List<CategoryEntityModel> ListCusType { get; set; }
    }
}
