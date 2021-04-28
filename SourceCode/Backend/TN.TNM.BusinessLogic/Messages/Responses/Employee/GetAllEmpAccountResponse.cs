using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetAllEmpAccountResponse : BaseResponse
    {
        public List<string> EmpAccountList { get; set; }
    }
}
