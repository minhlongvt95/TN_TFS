using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.CashBook
{
    public class GetDataSearchCashBookResponse : BaseResponse
    {
        public List<EmployeeModel> ListEmployee { get; set; }
        public List<OrganizationModel> ListOrganization { get; set; }
    }
}
