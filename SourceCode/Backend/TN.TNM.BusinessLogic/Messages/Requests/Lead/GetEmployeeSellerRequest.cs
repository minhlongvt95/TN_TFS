using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetEmployeeSellerRequest : BaseRequest<GetEmployeeSellerParameter>
    {
        public List<EmployeeEntityModel> ListEmployeeByAccount { get; set; } //List người bán hàng
        public Guid EmployeeId { get; set; }
        public override GetEmployeeSellerParameter ToParameter()
        {
            return new GetEmployeeSellerParameter()
            {
                ListEmployeeByAccount = ListEmployeeByAccount,
                EmployeeId = EmployeeId,
                UserId = UserId
            };
        }
    }
}
