using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetTeacherSalaryResponse:BaseResponse
    {
        public List<dynamic> lstResult { get; set; }
        public List<KeyValuePair<string, string>> lstColumn { get; set; }
        public byte[] ExcelFile { get; set; }
        public string NameFile { get; set; }
    }
}
