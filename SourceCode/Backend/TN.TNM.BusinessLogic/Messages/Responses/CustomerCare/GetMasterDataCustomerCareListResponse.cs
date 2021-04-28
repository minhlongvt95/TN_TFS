using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.CustomerCare
{
    public class GetMasterDataCustomerCareListResponse : BaseResponse
    {
        public List<CategoryEntityModel> ListStatus { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<CategoryEntityModel> ListFormCusCare { get; set; }
    }
}
