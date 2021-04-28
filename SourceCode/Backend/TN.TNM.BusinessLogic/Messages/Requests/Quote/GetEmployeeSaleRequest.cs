using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Quote;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class GetEmployeeSaleRequest : BaseRequest<GetEmployeeSaleParameter>
    {
        public List<EmployeeEntityModel> ListEmployeeByAccount { get; set; } //List người bán hàng
        public Guid EmployeeId { get; set; }
        public override GetEmployeeSaleParameter ToParameter()
        {
            return new GetEmployeeSaleParameter()
            {
                ListEmployeeByAccount = ListEmployeeByAccount,
                EmployeeId = EmployeeId,
                UserId = UserId
            };
        }
    }
}
