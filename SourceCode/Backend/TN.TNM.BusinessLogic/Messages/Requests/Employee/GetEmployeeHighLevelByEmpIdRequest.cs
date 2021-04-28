using System;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetEmployeeHighLevelByEmpIdRequest : BaseRequest<GetEmployeeHighLevelByEmpIdParameter>
    {
        public Guid EmployeeId { get; set; }
        public string ModuleCode { get; set; }
        public override GetEmployeeHighLevelByEmpIdParameter ToParameter()
        {
            return new GetEmployeeHighLevelByEmpIdParameter
            {
                EmployeeId = EmployeeId,
                ModuleCode = ModuleCode,
                UserId = UserId
            };
        }
    }
}
