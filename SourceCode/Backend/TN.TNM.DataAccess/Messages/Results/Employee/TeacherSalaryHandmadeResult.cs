using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class TeacherSalaryHandmadeResult:BaseResult
    {
        public List<dynamic> lstResult { get; set; }
        public List<KeyValuePair<string, string>> lstColumn { get; set; }
    }
}
