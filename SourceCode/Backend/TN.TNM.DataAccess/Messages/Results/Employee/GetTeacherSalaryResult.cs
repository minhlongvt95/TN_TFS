using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetTeacherSalaryResult : BaseResult
    {
        public List<dynamic> lstResult { get; set; }
        public List<KeyValuePair<string, string>> lstColumn { get; set; }
        public byte[] ExcelFile { get; set; }
        public string NameFile { get; set; }
    }
}
