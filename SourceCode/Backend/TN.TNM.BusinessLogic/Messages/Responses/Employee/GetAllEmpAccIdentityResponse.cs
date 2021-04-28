using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetAllEmpAccIdentityResponse : BaseResponse
    {
        public List<string> ListAccEmployee { get; set; }
    }
}
