using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class GetDataImportPotentialCustomerResponse: BaseResponse
    {
        public List<DataAccess.Models.Employee.EmployeeEntityModel> ListPersonalInChange { get; set; }//nguoi phu trach
        public List<DataAccess.Models.CategoryEntityModel> ListInvestFund { get; set; }//Nguon tiem nang
        public List<string> ListEmail { get; set; }
        public List<string> ListPhone { get; set; }
    }
}
