using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Lead;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.BusinessLogic.Models.Quote;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class GetEmployeeSaleResponse : BaseResponse
    {
        public List<EmployeeModel> ListEmployee { get; set; }
    }
}
