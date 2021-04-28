using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetAllEmpIdentityResponse : BaseResponse
    {
        public List<string> EmpIdentityList { get; set; }
    }
}
