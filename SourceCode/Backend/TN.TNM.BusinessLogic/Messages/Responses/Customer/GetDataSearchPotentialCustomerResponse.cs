using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.GeographicalArea;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class GetDataSearchPotentialCustomerResponse : BaseResponse
    {
        public List<CategoryModel> ListInvestFund { get; set; }
        public List<Models.Employee.EmployeeModel> ListEmployeeModel { get; set; }
        public List<CategoryEntityModel> ListCareState { get; set; }
        public List<CategoryEntityModel> ListCusGroup { get; set; }
        public List<GeographicalAreaEntityModel> ListArea { get; set; }
        public List<CategoryEntityModel> ListCusType { get; set; }
    }
}
