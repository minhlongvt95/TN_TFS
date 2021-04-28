using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetAllEmpAccountResult : BaseResult
    {
        public List<string> EmpAccountList { get; set; }
    }
}
