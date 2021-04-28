using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class FindTeacherMonthySalaryResponse:BaseResponse
    {
        public List<dynamic> lstResult { get; set; }
        public List<KeyValuePair<string, string>> lstColumn { get; set; }
    }
}
