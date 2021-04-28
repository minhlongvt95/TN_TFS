using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetAllEmpIdentityResult : BaseResult
    {
        public List<string> EmpIdentityList { get; set; }
    }
}
