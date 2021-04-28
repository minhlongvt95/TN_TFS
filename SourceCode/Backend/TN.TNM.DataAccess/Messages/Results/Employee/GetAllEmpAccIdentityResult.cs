using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetAllEmpAccIdentityResult : BaseResult
    {
        public List<string> ListAccEmployee { get; set; }
    }
}
